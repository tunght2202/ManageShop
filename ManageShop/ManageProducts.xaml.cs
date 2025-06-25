using ManageShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace ManageShop
{
    /// <summary>
    /// Interaction logic for ManageProducts.xaml
    /// </summary>
    public partial class ManageProducts : Window
    {
        private List<Product> products;
        private List<Category> categories;
        private ManageShopDBContext context;
        public ManageProducts()
        {
            InitializeComponent();
            context = new ManageShopDBContext();
            LoadProducts();
            LoadCategories();
            LoadCategories2();
            LoadProductsByCategory();
        }

        private void LoadProducts()
        {
            products = context.Products.ToList();
            productsListView.ItemsSource = products;
        }

        private void LoadCategories2()
        {
            categories = context.Categories.ToList();
            productCategoryCombox.ItemsSource = categories;
            productCategoryCombox.DisplayMemberPath = "CategoryName";
            productCategoryCombox.SelectedValuePath = "CategoryId";
        }

        private void LoadCategories()
        {
            var categories = context.Categories.ToList();

            // Tạo một mục đại diện cho "All"
            var allCategory = new Category { CategoryId = 0, CategoryName = "All" };

            // Tạo danh sách kết hợp với "All" và danh sách danh mục
            var combinedCategories = new List<Category> { allCategory };
            combinedCategories.AddRange(categories);

            CategoryComboBox.ItemsSource = combinedCategories;
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            CategoryComboBox.SelectedValuePath = "CategoryId";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();

            // Filter products based on search text
            var filteredProducts = context.Products
                .Where(p => p.ProductName.ToLower().Contains(searchText))
                .ToList();

            productsListView.ItemsSource = filteredProducts;
        }

        private void LoadProductsByCategory(int? categoryId = null)
        {
            IQueryable<Product> query = context.Products
                .Include(p => p.Category); // Chỉ lấy sản phẩm có số lượng tồn kho > 0

            // Nếu có categoryId, lọc theo danh mục
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Cập nhật danh sách sản phẩm
            productsListView.ItemsSource = query.ToList();
        }


        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategoryId = (int?)CategoryComboBox.SelectedValue;
            LoadProductsByCategory(selectedCategoryId);
        }

        private void ProductsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productsListView.SelectedItem is Product selectedProduct)
            {
                productNameTextBox.Text = selectedProduct.ProductName;
                productCategoryCombox.SelectedValue = selectedProduct.CategoryId;
                productPriceTextBox.Text = selectedProduct.Price.ToString("F2");
                productStockQuantityTextBox.Text = selectedProduct.StockQuantity.ToString();
                productCompanyTextBox.Text = selectedProduct.Company;
                productCountryTextBox.Text = selectedProduct.Country;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(productNameTextBox.Text) ||
                productCategoryCombox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(productPriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(productStockQuantityTextBox.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra định dạng dữ liệu
            if (!decimal.TryParse(productPriceTextBox.Text, out var price))
            {
                MessageBox.Show("Invalid price format.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(productStockQuantityTextBox.Text, out var stockQuantity))
            {
                MessageBox.Show("Invalid stock quantity format.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newProduct = new Product
            {
                ProductName = productNameTextBox.Text,
                CategoryId = (int)productCategoryCombox.SelectedValue,
                Price = price,
                StockQuantity = stockQuantity,
                Company = productCompanyTextBox.Text,
                Country = productCountryTextBox.Text
            };

            try
            {
                context.Products.Add(newProduct);
                context.SaveChanges();

                MessageBox.Show("Product has been successfully added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProducts();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (productsListView.SelectedItem is Product selectedProduct)
            {
                if (string.IsNullOrWhiteSpace(productNameTextBox.Text) ||
                    productCategoryCombox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(productPriceTextBox.Text) ||
                    string.IsNullOrWhiteSpace(productStockQuantityTextBox.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra định dạng dữ liệu
                if (!decimal.TryParse(productPriceTextBox.Text, out var price))
                {
                    MessageBox.Show("Invalid price format.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(productStockQuantityTextBox.Text, out var stockQuantity))
                {
                    MessageBox.Show("Invalid stock quantity format.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Cập nhật thông tin sản phẩm
                selectedProduct.ProductName = productNameTextBox.Text;
                selectedProduct.CategoryId = (int)productCategoryCombox.SelectedValue;
                selectedProduct.Price = price;
                selectedProduct.StockQuantity = stockQuantity;
                selectedProduct.Company = productCompanyTextBox.Text;
                selectedProduct.Country = productCountryTextBox.Text;

                try
                {
                    context.Products.Update(selectedProduct);
                    context.SaveChanges();

                    MessageBox.Show("Product has been successfully updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadProducts();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating the product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a product to update.");
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (productsListView.SelectedItem is Product selectedProduct)
            {

                var result = MessageBox.Show(
                    $"Are you sure you want to delete the product '{selectedProduct.ProductName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    context.Products.Remove(selectedProduct);
                    context.SaveChanges();

                    MessageBox.Show("Product has been successfully deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadProducts();
                    ClearFields();
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            productNameTextBox.Text = string.Empty;
            productPriceTextBox.Text = string.Empty;
            productStockQuantityTextBox.Text = string.Empty;
            productCompanyTextBox.Text = string.Empty;
            productCountryTextBox.Text = string.Empty;
            productCategoryCombox.SelectedIndex = -1;
            productsListView.SelectedItem = null;
        }
    }
}
