using System;
using System.Collections.Generic;

namespace ManageShop.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Orders = new HashSet<Order>();
            SoldOrders = new HashSet<SoldOrder>();
        }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public bool Gender { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<SoldOrder> SoldOrders { get; set; }
    }
}
