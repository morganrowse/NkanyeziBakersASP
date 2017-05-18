using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Payment
    {
        public int ID { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string Method { get; set; }
        public float Amount { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
