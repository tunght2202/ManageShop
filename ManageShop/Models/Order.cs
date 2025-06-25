using System;
using System.Collections.Generic;

namespace ManageShop.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            SoldOrders = new HashSet<SoldOrder>();
        }

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime SalesDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentMethodId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<SoldOrder> SoldOrders { get; set; }
    }
}
