using ManageShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ManageShop
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : Window
    {
        private ManageShopDBContext _context;
        public Sales()
        {
            InitializeComponent();
            _context = new ManageShopDBContext();
            LoadCategories();
            LoadProductsByCategory(); // Tải sản phẩm không có lọc mặc định khi bắt đầu

            // Khởi tạo giỏ hàng
            CartListView.ItemsSource = new List<OrderDetail>();
        }
        private void LoadCategories()
        {
            var categories = _context.Categories.ToList();

            // Tạo một mục đại diện cho "All"
            var allCategory = new Category { CategoryId = 0, CategoryName = "All" };

            // Tạo danh sách kết hợp với "All" và danh sách danh mục
            var combinedCategories = new List<Category> { allCategory };
            combinedCategories.AddRange(categories);

            CategoryComboBox.ItemsSource = combinedCategories;
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            CategoryComboBox.SelectedValuePath = "CategoryId";
        }

        private void LoadProductsByCategory(int? categoryId = null)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity > 0); // Chỉ lấy sản phẩm có số lượng tồn kho > 0

            // Nếu có categoryId, lọc theo danh mục
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Cập nhật danh sách sản phẩm
            ProductListView.ItemsSource = query.ToList();
        }

        private void LoadProductsBySearchName(string searchName = null)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity > 0); // Chỉ lấy sản phẩm có số lượng tồn kho > 0

            // Nếu có tên tìm kiếm, lọc theo tên
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(p => EF.Functions.Like(p.ProductName, $"%{searchName}%"));
            }

            // Cập nhật danh sách sản phẩm
            ProductListView.ItemsSource = query.ToList();
        }


        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategoryId = (int?)CategoryComboBox.SelectedValue;
            LoadProductsByCategory(selectedCategoryId);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchName = SearchTextBox.Text;
            LoadProductsBySearchName(searchName);
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy sản phẩm được chọn từ ProductListView
            var selectedProduct = ProductListView.SelectedItem as Product;

            // Kiểm tra nếu có sản phẩm được chọn
            if (selectedProduct != null)
            {
                // Cập nhật dữ liệu vào các TextBox
                ProductNameTextBox.Text = selectedProduct.ProductName;
                QuantityTextBox.Text = "1"; // Đặt giá trị mặc định là 1

                // Nếu cần thêm các thông tin khác, bạn có thể cập nhật tại đây
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy sản phẩm và số lượng từ giao diện
            var selectedProduct = ProductListView.SelectedItem as Product;
            if (selectedProduct == null)
            {
                MessageBox.Show("Please select a product.");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            // Tính tổng tiền cho sản phẩm dựa trên số lượng và giá
            decimal unitPrice = selectedProduct.Price;
            decimal amount = quantity * unitPrice;

            // Tạo OrderDetail mới
            var newOrderDetail = new OrderDetail
            {
                ProductId = selectedProduct.ProductId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Amount = amount,
                Product = selectedProduct // Gán sản phẩm vào để lấy tên sản phẩm
            };

            // Thêm hoặc cập nhật OrderDetail trong giỏ hàng
            AddOrUpdateOrderDetailInCart(newOrderDetail);

            // Cập nhật giỏ hàng và tổng số tiền
            UpdateCart();
        }

        private void AddOrUpdateOrderDetailInCart(OrderDetail newOrderDetail)
        {
            // Lấy danh sách giỏ hàng hiện tại
            var cartItems = CartListView.ItemsSource as List<OrderDetail>;
            if (cartItems == null)
            {
                cartItems = new List<OrderDetail>();
            }

            // Tìm OrderDetail tương ứng trong giỏ hàng
            var existingOrderDetail = cartItems.FirstOrDefault(item => item.ProductId == newOrderDetail.ProductId);

            if (existingOrderDetail != null)
            {
                // Nếu sản phẩm đã tồn tại, cập nhật số lượng và số tiền
                existingOrderDetail.Quantity += newOrderDetail.Quantity;
                existingOrderDetail.Amount = existingOrderDetail.Quantity * existingOrderDetail.UnitPrice;
            }
            else
            {
                // Nếu sản phẩm chưa tồn tại, thêm mới vào giỏ hàng
                cartItems.Add(newOrderDetail);
            }

            // Cập nhật ItemsSource cho ListView
            CartListView.ItemsSource = null;
            CartListView.ItemsSource = cartItems;
        }


        private void UpdateCart()
        {
            // Cập nhật tổng số tiền trong giỏ hàng
            var cartItems = CartListView.ItemsSource as List<OrderDetail>;
            if (cartItems != null)
            {
                decimal totalAmount = cartItems.Sum(item => item.Amount);
                TotalAmountTextBlock.Text = totalAmount.ToString("F2");
            }
        }

        private void CartListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy item được chọn từ CartListView
            var selectedOrderDetail = CartListView.SelectedItem as OrderDetail;

            // Kiểm tra nếu có item được chọn
            if (selectedOrderDetail != null)
            {
                // Cập nhật các TextBox với thông tin của sản phẩm được chọn
                ProductNameTextBox.Text = selectedOrderDetail.Product.ProductName;
                QuantityTextBox.Text = selectedOrderDetail.Quantity.ToString();

                // Nếu cần cập nhật thêm các thông tin khác, bạn có thể thực hiện tại đây
            }
        }

        private void EditSelectedProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy item được chọn từ CartListView
            var selectedOrderDetail = CartListView.SelectedItem as OrderDetail;
            if (selectedOrderDetail == null)
            {
                MessageBox.Show("Please select an item from the cart.");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int newQuantity) || newQuantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            // Cập nhật số lượng và số tiền của sản phẩm đã chọn
            selectedOrderDetail.Quantity = newQuantity;
            selectedOrderDetail.Amount = newQuantity * selectedOrderDetail.UnitPrice;

            // Cập nhật danh sách giỏ hàng
            // Việc set null sau đó set lại ItemsSource là không cần thiết
            // Thay vào đó, cần phải sử dụng CollectionView để refresh view
            var cartItems = CartListView.ItemsSource as List<OrderDetail>;
            if (cartItems != null)
            {
                // Tìm và cập nhật item đã chọn trong danh sách giỏ hàng
                var itemIndex = cartItems.IndexOf(selectedOrderDetail);
                if (itemIndex >= 0)
                {
                    cartItems[itemIndex] = selectedOrderDetail;
                }

                // Cập nhật danh sách giỏ hàng
                CartListView.ItemsSource = null;
                CartListView.ItemsSource = cartItems;
            }

            // Cập nhật tổng số tiền trong giỏ hàng
            UpdateCart();
        }

        private void DeleteSelectedProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy item được chọn từ CartListView
            var selectedOrderDetail = CartListView.SelectedItem as OrderDetail;
            if (selectedOrderDetail == null)
            {
                MessageBox.Show("Please select an item from the cart.");
                return;
            }

            // Xóa item khỏi giỏ hàng
            var cartItems = CartListView.ItemsSource as List<OrderDetail>;
            if (cartItems != null)
            {
                cartItems.Remove(selectedOrderDetail);
                CartListView.ItemsSource = null;
                CartListView.ItemsSource = cartItems;

                // Cập nhật tổng số tiền
                UpdateCart();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of OrderWindow
            var orderWindow = new OrderWindow();

            // Subscribe to the OrderPlaced event
            orderWindow.OrderPlaced += OnOrderPlaced;

            // Pass cart details and total amount to OrderWindow
            var cartItems = CartListView.ItemsSource as List<OrderDetail>;
            var totalAmount = TotalAmountTextBlock.Text;

            // Pass data to OrderWindow
            orderWindow.SetCartDetails(cartItems, totalAmount);

            // Open the OrderWindow
            orderWindow.Show();
        }

        private void OnOrderPlaced()
        {
            // Xóa giỏ hàng
            CartListView.ItemsSource = null;
            TotalAmountTextBlock.Text = "0.00";
        }
    }
}
