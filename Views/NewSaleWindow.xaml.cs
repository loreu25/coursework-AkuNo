using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class NewSaleWindow : Window
    {
        private readonly StoreContext _context;
        private ObservableCollection<CartItem> _cartItems;

        public NewSaleWindow(StoreContext context)
        {
            InitializeComponent();
            _context = context;
            _cartItems = new ObservableCollection<CartItem>();
            dgCart.ItemsSource = _cartItems;
            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = _context.Products
                .Where(p => p.StockQuantity > 0)
                .AsNoTracking()
                .ToList();
            dgProducts.ItemsSource = products;
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = dgProducts.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            if (quantity > selectedProduct.StockQuantity)
            {
                MessageBox.Show($"Недостаточно товара на складе. Доступно: {selectedProduct.StockQuantity}");
                return;
            }

            var existingItem = _cartItems.FirstOrDefault(i => i.Product.Id == selectedProduct.Id);
            if (existingItem != null)
            {
                if (existingItem.Quantity + quantity > selectedProduct.StockQuantity)
                {
                    MessageBox.Show($"Недостаточно товара на складе. Доступно: {selectedProduct.StockQuantity}");
                    return;
                }
                existingItem.Quantity += quantity;
                existingItem.UpdateTotal();
            }
            else
            {
                var cartItem = new CartItem
                {
                    Product = selectedProduct,
                    Quantity = quantity
                };
                cartItem.UpdateTotal();
                _cartItems.Add(cartItem);
            }

            UpdateTotal();
            txtQuantity.Text = "";
        }

        private void btnRemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CartItem cartItem)
            {
                _cartItems.Remove(cartItem);
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            decimal total = _cartItems.Sum(item => item.Total);
            txtTotal.Text = $"Итого: {total:N2} ₽";
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            try
            {
                var now = DateTime.Now;
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in _cartItems)
                        {
                            if (item.Product == null || item.Product.Id == 0)
                            {
                                MessageBox.Show("Ошибка: некорректные данные товара");
                                return;
                            }

                            var product = _context.Products.Find(item.Product.Id);
                            if (product == null)
                            {
                                MessageBox.Show($"Товар '{item.Product.Name}' не найден в базе данных");
                                return;
                            }

                            if (product.StockQuantity < item.Quantity)
                            {
                                MessageBox.Show($"Недостаточно товара '{product.Name}' на складе. Доступно: {product.StockQuantity}");
                                return;
                            }

                            product.StockQuantity -= item.Quantity;

                            var sale = new Sale
                            {
                                ProductId = product.Id,
                                Quantity = item.Quantity,
                                TotalPrice = item.Total,
                                Date = now
                            };
                            _context.Sales.Add(sale);
                        }

                        _context.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show($"Продажа успешно оформлена на сумму {_cartItems.Sum(i => i.Total):N2} ₽", "Успех");
                        DialogResult = true;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Ошибка при оформлении продажи: {message}\n\nStack trace: {ex.StackTrace}", "Ошибка");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class CartItem : INotifyPropertyChanged
    {
        private Product _product;
        private int _quantity;
        private decimal _total;

        public Product Product
        {
            get => _product;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Product cannot be null");
                _product = value;
                UpdateTotal();
                OnPropertyChanged(nameof(Product));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Quantity must be greater than zero", nameof(value));
                _quantity = value;
                UpdateTotal();
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public decimal Total
        {
            get => _total;
            private set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }

        public void UpdateTotal()
        {
            if (Product == null)
                throw new InvalidOperationException("Cannot calculate total: Product is null");
            Total = Product.Price * Quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
