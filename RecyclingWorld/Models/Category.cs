using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecyclingWorld.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary key

        [Required]
        [MaxLength(60)]
        public string Name { get; set; }

        [Range(1, 100)]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}
//settting up the model for the category, with the properties of Id, Name and DisplayOrder. The Id is the primary key, Name is required and has a maximum length of 60 characters, and DisplayOrder is an integer that must be between 1 and 100.