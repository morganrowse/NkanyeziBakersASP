using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bakery.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Product
    {
        public int ID { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public String Description { get; set; }
        public float Price { get; set; }
        public string Photo { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
