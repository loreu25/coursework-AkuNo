using System.Linq;
using System.Windows;
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
            _context.Database.EnsureCreated();
            LoadProducts();
        }

        private void LoadProducts()
        {
            dgProducts.ItemsSource = _context.Products.ToList();
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
                _context.Products.Add(product);
                _context.SaveChanges();
                LoadProducts();
            }
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = dgProducts.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для редактирования");
                return;
            }

            var editWindow = new EditProductWindow(selectedProduct);
            if (editWindow.ShowDialog() == true)
            {
                _context.SaveChanges();
                LoadProducts();
            }
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = dgProducts.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для удаления");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот товар?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _context.Products.Remove(selectedProduct);
                _context.SaveChanges();
                LoadProducts();
            }
        }

        private void btnNewSale_Click(object sender, RoutedEventArgs e)
        {
            var saleWindow = new NewSaleWindow(_context);
            if (saleWindow.ShowDialog() == true)
            {
                _context.Sales.Add(saleWindow.Sale);
                _context.SaveChanges();
                LoadProducts();
                MessageBox.Show($"Продажа оформлена на сумму {saleWindow.Sale.TotalPrice} руб.", "Успех");
            }
        }
    }
}