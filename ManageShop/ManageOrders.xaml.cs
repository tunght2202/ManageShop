using ManageShop.Models;
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
    /// Interaction logic for ManageOrders.xaml
    /// </summary>
    public partial class ManageOrders : Window
    {
        private ManageShopDBContext _context;

        public ManageOrders()
        {
            InitializeComponent();
            _context = new ManageShopDBContext();
            LoadStaffMembers();
            LoadOrders();
        }

        private void LoadStaffMembers()
        {
            var staffMembers = _context.Employees
                .Where(e => e.RoleId == 2) // Filter staff members
                .ToList();

            // Add "All" option
            staffMembers.Insert(0, new Employee
            {
                EmployeeId = 0,
                Username = "All"
            });

            StaffComboBox.ItemsSource = staffMembers;
            StaffComboBox.DisplayMemberPath = "Username";
            StaffComboBox.SelectedValuePath = "EmployeeId";
        }

        private void LoadOrders(int? employeeId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var ordersQuery = _context.Orders.AsQueryable();

            if (employeeId.HasValue && employeeId.Value > 0)
            {
                ordersQuery = ordersQuery.Where(o => o.EmployeeId == employeeId.Value);
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.SalesDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.SalesDate <= endDate.Value);
            }

            var orders = ordersQuery
                .Select(o => new
                {
                    o.OrderId,
                    SalesDate = o.SalesDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    Customer = o.Customer.CustomerName,
                    Employee = o.Employee.Username,
                    o.TotalAmount,
                    PaymentMethod = o.PaymentMethod.PaymentMethodName
                })
                .ToList();

            ordersListView.ItemsSource = orders;
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue)
            {
                // Kiểm tra xem ngày chọn có lớn hơn ngày hôm nay không
                if (StartDatePicker.SelectedDate.Value.Date > DateTime.Now.Date)
                {
                    MessageBox.Show("Start date cannot be greater than today.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    StartDatePicker.SelectedDate = null;
                    return;
                }
            }

            if (EndDatePicker.SelectedDate.HasValue && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
            {
                MessageBox.Show("End date cannot be earlier than start date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                EndDatePicker.SelectedDate = null;
                return;
            }

            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;
            int? employeeId = (int?)StaffComboBox.SelectedValue;

            LoadOrders(employeeId, startDate, endDate);
        }


        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EndDatePicker.SelectedDate.HasValue)
            {
                // Kiểm tra xem ngày chọn có lớn hơn ngày hôm nay không
                if (EndDatePicker.SelectedDate.Value.Date > DateTime.Now.Date)
                {
                    MessageBox.Show("End date cannot be greater than today.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EndDatePicker.SelectedDate = null;
                    return;
                }
            }

            if (EndDatePicker.SelectedDate.HasValue && StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
            {
                MessageBox.Show("End date cannot be earlier than start date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                EndDatePicker.SelectedDate = null;
                return;
            }

            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;
            int? employeeId = (int?)StaffComboBox.SelectedValue;

            LoadOrders(employeeId, startDate, endDate);
        }



        private void StaffComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;
            int? employeeId = (int?)StaffComboBox.SelectedValue;

            LoadOrders(employeeId, startDate, endDate);
        }

        private void OrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra xem có item nào được chọn không
            if (ordersListView.SelectedItem != null)
            {
                // Lấy OrderId từ item được chọn
                var selectedOrder = ordersListView.SelectedItem as dynamic;
                int orderId = selectedOrder.OrderId;

                // Tìm đơn hàng chi tiết từ cơ sở dữ liệu
                var orderDetails = _context.Orders
                    .Where(o => o.OrderId == orderId)
                    .Select(o => new
                    {
                        o.OrderId,
                        CustomerName = o.Customer.CustomerName,
                        EmployeeName = o.Employee.Username,
                        o.TotalAmount,
                        PaymentMethod = o.PaymentMethod.PaymentMethodName,
                        Items = o.OrderDetails.Select(od => new
                        {
                            ProductName = od.Product.ProductName,
                            od.Quantity,
                            od.UnitPrice,
                            od.Amount
                        }).ToList()
                    })
                    .SingleOrDefault();

                if (orderDetails != null)
                {
                    // Hiển thị thông tin chi tiết đơn hàng
                    OrderIdTextBlock.Text = $"Order ID: {orderDetails.OrderId}";
                    CustomerNameTextBlock.Text = $"Customer: {orderDetails.CustomerName}";
                    EmployeeNameTextBlock.Text = $"Employee: {orderDetails.EmployeeName}";
                    TotalAmountTextBlock.Text = $"Total Amount: {orderDetails.TotalAmount:C}";
                    PaymentMethodTextBlock.Text = $"Payment Method: {orderDetails.PaymentMethod}";

                    // Hiển thị danh sách mặt hàng trong đơn hàng
                    var items = new StringBuilder();
                    foreach (var item in orderDetails.Items)
                    {
                        items.AppendLine($"{item.ProductName} - Quantity: {item.Quantity}, Unit Price: {item.UnitPrice:C}, Amount: {item.Amount:C}");
                    }
                    OrderItemsTextBlock.Text = items.ToString();
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy đơn hàng
                    MessageBox.Show("Order details not found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
