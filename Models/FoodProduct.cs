using System;

namespace WpfApp1.Models
{
    public class FoodProduct : Product
    {
        public DateTime ExpirationDate { get; set; }
        public bool RequiresRefrigeration { get; set; }
        public string StorageConditions { get; set; }
        public string NutritionalValue { get; set; }
    }
}
