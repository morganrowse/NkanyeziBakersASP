using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Order
    {
        public int ID { get; set; }
        public float DeliveryPrice { get; set; }
        public DateTime DeliveryDate { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public String Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
