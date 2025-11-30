using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace c2cUniversitees.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; } 

        [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 

        [Required(ErrorMessage = "الرجاء تحديد الكلية.")]
        [Display(Name = "الكلية")]
        public string CollegeName { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
        public string? ResetToken { get; set; } 
        public DateTime? TokenExpiry { get; set; } 
    }
}
