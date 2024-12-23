using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class NonFoodProduct : Product
    {
        public string Brand { get; set; }
        public string Manufacturer { get; set; }
        public string CountryOfOrigin { get; set; }
        public string WarrantyPeriod { get; set; }
    }
}
