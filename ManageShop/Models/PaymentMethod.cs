using System;
using System.Collections.Generic;

namespace ManageShop.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Orders = new HashSet<Order>();
        }

        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
