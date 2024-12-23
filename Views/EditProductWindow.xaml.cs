using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class EditProductWindow : Window
    {
        private readonly StoreContext _context;
        private Product _product;
        private bool _isNewProduct;
        private bool _isLoading;
        private string _selectedImagePath;

        public EditProductWindow(Product product, StoreContext context)
        {
            InitializeComponent();
            _context = context;
            _isLoading = true;

            // Если продукт не передан, создаем новый
            if (product == null)
            {
                _isNewProduct = true;
                _product = new FoodProduct
                {
                    ExpirationDate = DateTime.Now,
                    ProductType = "Food" // Устанавливаем тип продукта
                };
                Title = "Добавление товара";
                cmbProductType.SelectedIndex = 0;
            }
            else
            {
                _isNewProduct = false;
                _product = product;
                Title = "Редактирование товара";
                
                // Устанавливаем тип продукта в комбобоксе
                cmbProductType.SelectedIndex = _product is FoodProduct ? 0 : 1;
            }

            // Загружаем категории
            var categories = _context.Categories.ToList();
            cmbCategory.ItemsSource = categories;
            cmbCategory.DisplayMemberPath = "Name";
            cmbCategory.SelectedValuePath = "Id";

            if (!_isNewProduct)
            {
                // Заполняем поля существующими данными
                txtName.Text = _product.Name;
                txtDescription.Text = _product.Description;
                txtPrice.Text = _product.Price.ToString();
                txtQuantity.Text = _product.StockQuantity.ToString();
                cmbCategory.SelectedValue = _product.CategoryId;

                if (_product.ImageData != null)
                {
                    _selectedImagePath = _product.ImagePath;
                    txtImagePath.Text = _product.ImagePath;
                }

                if (_product is FoodProduct foodProduct)
                {
                    dpExpirationDate.SelectedDate = foodProduct.ExpirationDate;
                    chkRefrigeration.IsChecked = foodProduct.RequiresRefrigeration;
                    txtStorageConditions.Text = foodProduct.StorageConditions;
                    txtNutritionalValue.Text = foodProduct.NutritionalValue;
                }
                else if (_product is NonFoodProduct nonFoodProduct)
                {
                    txtBrand.Text = nonFoodProduct.Brand;
                    txtManufacturer.Text = nonFoodProduct.Manufacturer;
                    txtCountryOfOrigin.Text = nonFoodProduct.CountryOfOrigin;
                    txtWarrantyPeriod.Text = nonFoodProduct.WarrantyPeriod;
                }
            }

            UpdateProductTypeFields();
            _isLoading = false;
        }

        private void UpdateProductTypeFields()
        {
            if (_product is FoodProduct)
            {
                // Показываем поля для продовольственных товаров
                foodGroup.Visibility = Visibility.Visible;
                nonFoodGroup.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Показываем поля для непродовольственных товаров
                foodGroup.Visibility = Visibility.Collapsed;
                nonFoodGroup.Visibility = Visibility.Visible;
            }
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                txtImagePath.Text = _selectedImagePath;
            }
        }

        private void cmbProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;

            var selectedType = cmbProductType.SelectedItem as ComboBoxItem;
            if (selectedType == null) return;

            var newProductType = selectedType.Content.ToString();
            if (_product != null && 
                ((_product is FoodProduct && newProductType == "Продовольственный") ||
                 (_product is NonFoodProduct && newProductType == "Непродовольственный")))
            {
                return; // Тип не изменился
            }

            var result = MessageBox.Show(
                "При изменении типа товара некоторые специфические свойства могут быть потеряны. Продолжить?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                // Сохраняем общие свойства
                var name = _product?.Name ?? "";
                var description = _product?.Description ?? "";
                var price = _product?.Price ?? 0;
                var stockQuantity = _product?.StockQuantity ?? 0;
                var categoryId = _product?.CategoryId ?? 0;
                var imageData = _product?.ImageData;
                var imagePath = _product?.ImagePath;

                if (newProductType == "Продовольственный")
                {
                    _product = new FoodProduct
                    {
                        Id = _product?.Id ?? 0,
                        Name = name,
                        Description = description,
                        Price = price,
                        StockQuantity = stockQuantity,
                        CategoryId = categoryId,
                        ImageData = imageData,
                        ImagePath = imagePath,
                        ProductType = "Food",
                        ExpirationDate = DateTime.Now,
                        RequiresRefrigeration = false,
                        StorageConditions = "",
                        NutritionalValue = ""
                    };
                }
                else
                {
                    _product = new NonFoodProduct
                    {
                        Id = _product?.Id ?? 0,
                        Name = name,
                        Description = description,
                        Price = price,
                        StockQuantity = stockQuantity,
                        CategoryId = categoryId,
                        ImageData = imageData,
                        ImagePath = imagePath,
                        ProductType = "NonFood",
                        Brand = "",
                        Manufacturer = "",
                        CountryOfOrigin = "",
                        WarrantyPeriod = ""
                    };
                }

                UpdateProductTypeFields();
            }
            else
            {
                _isLoading = true;
                cmbProductType.SelectedItem = e.RemovedItems[0];
                _isLoading = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Пожалуйста, введите название товара");
                return;
            }

            decimal price;
            if (!decimal.TryParse(txtPrice.Text.Replace('.', ','), out price))
            {
                MessageBox.Show("Пожалуйста, введите корректную цену");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity))
            {
                MessageBox.Show("Пожалуйста, введите корректное количество");
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите категорию");
                return;
            }

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Обновляем общие свойства
                        _product.Name = txtName.Text;
                        _product.Description = txtDescription.Text;
                        _product.Price = price;
                        _product.StockQuantity = quantity;
                        _product.CategoryId = (int)cmbCategory.SelectedValue;

                        // Обновляем специфические свойства
                        if (_product is FoodProduct foodProduct)
                        {
                            foodProduct.ExpirationDate = dpExpirationDate.SelectedDate ?? DateTime.Now;
                            foodProduct.RequiresRefrigeration = chkRefrigeration.IsChecked ?? false;
                            foodProduct.StorageConditions = txtStorageConditions.Text;
                            foodProduct.NutritionalValue = txtNutritionalValue.Text;
                        }
                        else if (_product is NonFoodProduct nonFoodProduct)
                        {
                            nonFoodProduct.Brand = txtBrand.Text;
                            nonFoodProduct.Manufacturer = txtManufacturer.Text;
                            nonFoodProduct.CountryOfOrigin = txtCountryOfOrigin.Text;
                            nonFoodProduct.WarrantyPeriod = txtWarrantyPeriod.Text;
                        }

                        // Обновляем изображение
                        if (!string.IsNullOrEmpty(_selectedImagePath))
                        {
                            _product.ImageData = File.ReadAllBytes(_selectedImagePath);
                            _product.ImagePath = _selectedImagePath;
                        }

                        // Отключаем отслеживание всех сущностей этого типа
                        foreach (var entry in _context.ChangeTracker.Entries<Product>())
                        {
                            entry.State = EntityState.Detached;
                        }

                        if (_isNewProduct)
                        {
                            // Для нового продукта просто добавляем его
                            _context.Products.Add(_product);
                        }
                        else
                        {
                            // Для существующего продукта проверяем его наличие в базе
                            var existingProduct = _context.Products
                                .AsNoTracking()
                                .FirstOrDefault(p => p.Id == _product.Id);

                            if (existingProduct == null)
                            {
                                throw new Exception("Товар не найден в базе данных");
                            }

                            // Присоединяем и помечаем как измененный
                            _context.Products.Attach(_product);
                            _context.Entry(_product).State = EntityState.Modified;
                        }

                        _context.SaveChanges();
                        transaction.Commit();
                        DialogResult = true;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Ошибка при сохранении товара: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? $"\n\nПодробности: {ex.InnerException.Message}" : "";
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}{innerException}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
