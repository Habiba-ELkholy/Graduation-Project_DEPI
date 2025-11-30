using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace c2cUniversitees.Models
{
    public class ProductViewModel
    {
        
        [Required(ErrorMessage = "العنوان مطلوب."), StringLength(100)]
        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "السعر مطلوب.")]
        [Range(0.01, 1000000, ErrorMessage = "يجب أن يكون السعر قيمة موجبة وأكبر من الصفر.")]
        public decimal Price { get; set; }

        public string Category { get; set; }

        
        [Required(ErrorMessage = "الرجاء رفع صورة للمنتج.")]
        [Display(Name = "صورة المنتج")]
        public IFormFile ImageFile { get; set; }
    }
}
