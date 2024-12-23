using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new HashSet<Product>();
        }
    }
}
