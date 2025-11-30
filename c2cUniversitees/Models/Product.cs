using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c2cUniversitees.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "الرجاء إدخال عنوان المنتج.")]
        [Display(Name = "عنوان المنتج")]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "الوصف التفصيلي")]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string Category { get; set; }

        public string ImagePath { get; set; }

        [Display(Name = "حالة البيع")]
        public bool IsSold { get; set; } = false; 

        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Required]
        public int SellerId { get; set; }

        [ForeignKey("SellerId")]
        public User Seller { get; set; } 
    }
}
