using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class EditProductWindow : Window
    {
        public Product Product { get; private set; }
        private string _selectedImagePath;

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            Product = product;
            LoadProduct();
        }

        private void LoadProduct()
        {
            txtName.Text = Product.Name;
            txtDescription.Text = Product.Description;
            txtPrice.Text = Product.Price.ToString();
            txtQuantity.Text = Product.StockQuantity.ToString();

            if (Product.ImageData != null && Product.ImageData.Length > 0)
            {
                using (var ms = new MemoryStream(Product.ImageData))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    imgProduct.Source = image;
                }
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
                _selectedImagePath = openFileDialog.FileName;
                imgProduct.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity))
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            Product.Name = txtName.Text;
            Product.Description = txtDescription.Text;
            Product.Price = price;
            Product.StockQuantity = quantity;

            if (!string.IsNullOrEmpty(_selectedImagePath))
            {
                Product.ImageData = File.ReadAllBytes(_selectedImagePath);
                Product.ImagePath = _selectedImagePath;
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
