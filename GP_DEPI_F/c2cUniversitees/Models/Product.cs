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

        // لتصنيف المنتج (مثل: أدوات تخرج، كتب)
        public string Category { get; set; }

        // اسم ملف الصورة المخزن
        public string ImagePath { get; set; }

        [Display(Name = "حالة البيع")]
        public bool IsSold { get; set; } = false; // القيمة الافتراضية: لم يُبع بعد

        public DateTime PostedDate { get; set; } = DateTime.Now;

        // **علاقة مفتاح خارجي (Foreign Key) مع البائع:**

        // 1. المفتاح الخارجي الذي يشير إلى ID البائع (تغير من string إلى int)
        [Required]
        public int SellerId { get; set; }

        // 2. خاصية الملاحة (Navigation Property)
        [ForeignKey("SellerId")]
        public User Seller { get; set; } // يجب أن يشير إلى نموذج User الجديد
    }
}
