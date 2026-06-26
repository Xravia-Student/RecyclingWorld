using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecyclingWorld.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string description { get; set; }
        [Required]
        public string Grade { get; set; }
        [Range(0, 10000000)]
        [Display(Name = "Price Per Kg")]
        public decimal PricePerKg { get; set; }

        [Range(0, 10000000)]
        [Display(Name = "Price Per Kg (500kg+)")]
        public decimal PricePerKg500 { get; set; }

        [Range(0, 10000000)]
        [Display(Name = "Price Per Kg (1000kg+)")]
        public decimal PricePerKg1000 { get; set; }

        public string ImageUrl { get; set; }

        // Foreign key to Category
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public decimal GetPriceForQuantity(double quantityKg)
        {
            if (quantityKg >= 1000 && PricePerKg1000 > 0)
                return PricePerKg1000;
            if (quantityKg >= 500 && PricePerKg500 > 0)
                return PricePerKg500;
            return PricePerKg;
        }

    }

}
