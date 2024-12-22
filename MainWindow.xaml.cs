using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Views;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StoreContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new StoreContext();
            try
            {
                _context.Database.EnsureCreated();
                LoadProducts();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _context.Products.ToList();
                lvProducts.ItemsSource = products;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product
            {
                Name = "Новый товар",
                Description = "Описание",
                Price = 0,
                StockQuantity = 0
            };

            var editWindow = new EditProductWindow(product);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    _context.Products.Add(product);
                    _context.SaveChanges();
                    LoadProducts();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", 
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = lvProducts.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для редактирования");
                return;
            }

            var editWindow = new EditProductWindow(selectedProduct);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    _context.SaveChanges();
                    LoadProducts();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", 
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = lvProducts.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для удаления");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот товар?", 
                                       "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Products.Remove(selectedProduct);
                    _context.SaveChanges();
                    LoadProducts();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", 
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnNewSale_Click(object sender, RoutedEventArgs e)
        {
            var saleWindow = new NewSaleWindow(_context);
            if (saleWindow.ShowDialog() == true)
            {
                try
                {
                    _context.Sales.Add(saleWindow.Sale);
                    _context.SaveChanges();
                    LoadProducts();
                    MessageBox.Show($"Продажа оформлена на сумму {saleWindow.Sale.TotalPrice} руб.", "Успех");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при оформлении продажи: {ex.Message}", 
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}