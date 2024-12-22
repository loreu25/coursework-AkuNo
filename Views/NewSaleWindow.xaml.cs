using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class NewSaleWindow : Window
    {
        private readonly StoreContext _context;
        private Product _selectedProduct;
        private int _quantity;

        public Sale Sale { get; private set; }

        public NewSaleWindow(StoreContext context)
        {
            InitializeComponent();
            _context = context;
            LoadProducts();
        }

        private void LoadProducts()
        {
            dgProducts.ItemsSource = _context.Products.Where(p => p.StockQuantity > 0).ToList();
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = dgProducts.SelectedItem as Product;
            UpdateTotal();
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtQuantity.Text, out int quantity))
            {
                _quantity = quantity;
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            if (_selectedProduct != null && _quantity > 0)
            {
                decimal total = _selectedProduct.Price * _quantity;
                txtTotal.Text = $"Итого: {total} руб.";
            }
            else
            {
                txtTotal.Text = "Итого: 0 руб.";
            }
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            if (quantity > _selectedProduct.StockQuantity)
            {
                MessageBox.Show("Недостаточно товара на складе");
                return;
            }

            Sale = new Sale
            {
                ProductId = _selectedProduct.Id,
                Product = _selectedProduct,
                Quantity = quantity,
                TotalPrice = _selectedProduct.Price * quantity,
                SaleDate = DateTime.Now
            };

            _selectedProduct.StockQuantity -= quantity;

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
