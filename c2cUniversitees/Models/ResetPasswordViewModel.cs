using System.ComponentModel.DataAnnotations;

namespace c2cUniversitees.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } 

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة.")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين.")]
        public string ConfirmPassword { get; set; }
    }
}
