using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace c2cUniversitees.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; } // المفتاح الأساسي من نوع int

        [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } // يجب تشفيرها في التطبيق الفعلي

        // خاصية الكلية للفلترة
        [Required(ErrorMessage = "الرجاء تحديد الكلية.")]
        [Display(Name = "الكلية")]
        public string CollegeName { get; set; }

        // خاصية للمنتجات التي يمتلكها البائع
        public List<Product> Products { get; set; } = new List<Product>();
        public string? ResetToken { get; set; } // الرمز المميز لاستعادة كلمة المرور
        public DateTime? TokenExpiry { get; set; } // وقت انتهاء صلاحية الرمز
    }
}
