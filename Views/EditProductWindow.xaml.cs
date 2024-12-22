using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class EditProductWindow : Window
    {
        private readonly Product _product;
        private readonly StoreContext _context;
        private string _selectedImagePath;

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            _product = product;
            _context = new StoreContext();
            LoadCategories();
            DataContext = _product;

            if (_product.ImageData != null)
            {
                try
                {
                    var image = new System.Windows.Media.Imaging.BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(_product.ImageData);
                    image.EndInit();
                    imgProduct.Source = image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                }
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _context.Categories.OrderBy(c => c.Name).ToList();
                cmbCategory.ItemsSource = categories;

                if (_product.CategoryId == 0 && categories.Any())
                {
                    _product.CategoryId = categories.First().Id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}");
            }
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif|Все файлы|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _selectedImagePath = openFileDialog.FileName;
                    _product.ImagePath = Path.GetFileName(_selectedImagePath);
                    _product.ImageData = File.ReadAllBytes(_selectedImagePath);

                    var image = new System.Windows.Media.Imaging.BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(_product.ImageData);
                    image.EndInit();
                    imgProduct.Source = image;

                    txtImagePath.Text = _product.ImagePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_product.Name))
            {
                MessageBox.Show("Введите название товара");
                return;
            }

            if (_product.Price <= 0)
            {
                MessageBox.Show("Цена должна быть больше нуля");
                return;
            }

            if (_product.StockQuantity < 0)
            {
                MessageBox.Show("Количество не может быть отрицательным");
                return;
            }

            if (_product.CategoryId == 0)
            {
                MessageBox.Show("Выберите категорию товара");
                return;
            }

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
