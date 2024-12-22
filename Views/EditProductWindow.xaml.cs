using System;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class EditProductWindow : Window
    {
        public Product Product { get; private set; }

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
