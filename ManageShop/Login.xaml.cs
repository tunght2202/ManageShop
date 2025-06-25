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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            using (var context = new ManageShopDBContext())
            {
                var employee = context.Employees
                    .Where(emp => emp.Username == username && emp.Password == password)
                    .FirstOrDefault();

                if (employee != null)
                {
                    UserSession.EmployeeId = employee.EmployeeId; // Lưu EmployeeId vào UserSession

                    if (employee.RoleId == 1)
                    {
                        var dashboard = new Dashboard();
                        dashboard.Show();
                        this.Close(); // Đóng cửa sổ đăng nhập
                    }
                    else if (employee.RoleId == 2)
                    {
                        var sales = new Sales();
                        sales.Show();
                        this.Close(); // Đóng cửa sổ đăng nhập
                    }
                }
                else
                {
                    errorMessageTextBlock.Text = "Invalid username or password";
                    errorMessageTextBlock.Visibility = Visibility.Visible;
                }
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();    
            registerWindow.Show();
        }
    }
}
