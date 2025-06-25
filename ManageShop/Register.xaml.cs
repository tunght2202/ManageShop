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
using System.Windows.Shapes;

namespace ManageShop
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private ManageShopDBContext _context;

        public Register()
        {
            InitializeComponent();
            _context = new ManageShopDBContext();
        }


        // Handle register button click event
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string employeeName = EmployeeNameTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            bool isMale = MaleRadioButton.IsChecked == true;
            int? roleId = 2;

            if (string.IsNullOrEmpty(employeeName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || roleId == null)
            {
                ErrorMessageTextBlock.Text = "Please fill in all fields.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Check if the username already exists
            var existingEmployee = _context.Employees
                .FirstOrDefault(e => e.Username == username);

            if (existingEmployee != null)
            {
                ErrorMessageTextBlock.Text = "Username already exists.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Create a new employee
            var newEmployee = new Employee
            {
                EmployeeName = employeeName,
                Username = username,
                Password = password,
                Gender = isMale,
                RoleId = (int)roleId
            };

            // Add the new employee to the database
            _context.Employees.Add(newEmployee);
            _context.SaveChanges();

            MessageBox.Show("Employee registered successfully.");
            this.Close(); // Close the register window
        }
    }
}
