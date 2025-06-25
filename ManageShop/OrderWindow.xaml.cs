using ManageShop.Models;
using System.Windows;
using System.Windows.Controls;

namespace ManageShop
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public delegate void OrderPlacedEventHandler();
        public event OrderPlacedEventHandler OrderPlaced;

        private ManageShopDBContext _context;
        public OrderWindow()
        {
            InitializeComponent();
            _context = new ManageShopDBContext();
            LoadPaymentMethods();
        }

        private void LoadPaymentMethods()
        {
            using (var context = new ManageShopDBContext())
            {
                var paymentMethods = context.PaymentMethods.ToList();
                PaymentMethodComboBox.ItemsSource = paymentMethods;
                PaymentMethodComboBox.DisplayMemberPath = "PaymentMethodName";
                PaymentMethodComboBox.SelectedValuePath = "PaymentMethodId";
            }
        }

        public void SetCartDetails(List<OrderDetail> cartItems, string totalAmount)
        {
            // Set cart details to ListView
            CartListView.ItemsSource = cartItems;
            TotalAmountTextBlock.Text = totalAmount;
        }


        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            string customerName = CustomerNameTextBox.Text;
            string customerPhone = CustomerPhoneTextBox.Text;
            string customerAddress = CustomerAddressTextBox.Text;
            bool isMale = MaleRadioButton.IsChecked == true;
            int paymentMethodId = (int)PaymentMethodComboBox.SelectedValue;

            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(customerPhone) || string.IsNullOrEmpty(customerAddress) || PaymentMethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            // Kiểm tra xem khách hàng đã tồn tại chưa
            Customer customer = _context.Customers
                .FirstOrDefault(c => c.PhoneNumber == customerPhone);

            if (customer == null)
            {
                // Nếu khách hàng chưa tồn tại, tạo mới
                customer = new Customer
                {
                    CustomerName = customerName,
                    PhoneNumber = customerPhone,
                    Address = customerAddress,
                    Gender = isMale
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            else
            {
                // Cập nhật thông tin khách hàng nếu cần
                customer.CustomerName = customerName;
                customer.Address = customerAddress;
                customer.Gender = isMale;
                _context.Customers.Update(customer);
                _context.SaveChanges();
            }

            int customerId = customer.CustomerId;

            var order = new Order
            {
                CustomerId = customerId,
                EmployeeId = (int)UserSession.EmployeeId,
                SalesDate = DateTime.Now,
                TotalAmount = decimal.Parse(TotalAmountTextBlock.Text),
                PaymentMethodId = paymentMethodId
            };

            foreach (var item in CartListView.Items)
            {
                var orderDetail = (OrderDetail)item;
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = orderDetail.ProductId,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                    Amount = orderDetail.Amount
                });

                // Cập nhật số lượng tồn kho của sản phẩm
                var product = _context.Products
                    .FirstOrDefault(p => p.ProductId == orderDetail.ProductId);

                if (product != null)
                {
                    product.StockQuantity -= orderDetail.Quantity;
                }
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            var soldOrder = new SoldOrder
            {
                OrderId = order.OrderId,
                EmployeeId = (int)UserSession.EmployeeId,
                SaleDate = DateTime.Now
            };

            _context.SoldOrders.Add(soldOrder);
            _context.SaveChanges();

            MessageBox.Show("Order placed successfully.");

            OrderPlaced?.Invoke();

            this.Close();
        }

        private void CustomerPhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string phoneNumber = CustomerPhoneTextBox.Text;

            // Kiểm tra số điện thoại có chứa chỉ số và không vượt quá 10 ký tự
            if (phoneNumber.Any(ch => !char.IsDigit(ch)) || phoneNumber.Length > 10)
            {
                MessageBox.Show("Phone number must be numeric and no more than 10 characters.");
                // Xóa ký tự không hợp lệ
                CustomerPhoneTextBox.Text = new string(phoneNumber.Where(char.IsDigit).ToArray());
                CustomerPhoneTextBox.SelectionStart = CustomerPhoneTextBox.Text.Length;
                return;
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                // Nếu số điện thoại trống, xóa thông tin khách hàng
                ClearCustomerInfo();
                return;
            }

            using (var context = new ManageShopDBContext())
            {
                var customer = context.Customers
                    .Where(c => c.PhoneNumber == phoneNumber)
                    .FirstOrDefault();

                if (customer != null)
                {
                    // Nếu khách hàng tồn tại, điền thông tin vào các ô
                    CustomerNameTextBox.Text = customer.CustomerName;
                    CustomerAddressTextBox.Text = customer.Address;
                    MaleRadioButton.IsChecked = customer.Gender; // Giả sử Gender = true là nam
                    FemaleRadioButton.IsChecked = !customer.Gender; // Giả sử Gender = false là nữ
                }
                else
                {
                    // Nếu khách hàng không tồn tại, xóa thông tin khách hàng
                    ClearCustomerInfo();
                }
            }
        }

        private void ClearCustomerInfo()
        {
            CustomerNameTextBox.Text = string.Empty;
            CustomerAddressTextBox.Text = string.Empty;
            MaleRadioButton.IsChecked = false;
            FemaleRadioButton.IsChecked = false;
        }
    }
}
