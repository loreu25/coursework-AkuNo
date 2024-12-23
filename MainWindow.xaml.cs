using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Views;
using System.Linq;

namespace WpfApp1
{
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _context.Products
                    .Include(p => p.Category)
                    .OrderBy(p => p.Category.Name)
                    .ThenBy(p => p.Name)
                    .ToList();
                lvProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditProductWindow(null, _context);
            if (window.ShowDialog() == true)
            {
                LoadProducts();
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

            var window = new EditProductWindow(selectedProduct, _context);
            if (window.ShowDialog() == true)
            {
                LoadProducts();
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

            var result = MessageBox.Show("Вы действительно хотите удалить этот товар?",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Products.Remove(selectedProduct);
                    _context.SaveChanges();
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnNewSale_Click(object sender, RoutedEventArgs e)
        {
            var window = new NewSaleWindow(_context);
            if (window.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _context.Dispose();
        }
    }
}