using c2cUniversitees.Models;
using c2cUniversitees.Models.Data;
using c2cUniversitees.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace c2cUniversitees.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AccountController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

       
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.CollegeList = c2cUniversitees.Utilities.CollegeData.CollegeNames;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "الرجاء إدخال البريد وكلمة المرور.";
                return View();
            }

            string hashedPassword = HashPassword(password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), 
            new Claim(ClaimTypes.Name, user.Username), 
            new Claim("College", user.CollegeName) 
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);

                return RedirectToAction("Index", "Products");
            }

            ViewBag.Error = "البريد الإلكتروني أو كلمة المرور غير صحيحة.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username,Email,Password,CollegeName")] User newUser)
        {

            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "هذا البريد الإلكتروني مُسجل بالفعل.");
                    ViewBag.CollegeList = c2cUniversitees.Utilities.CollegeData.CollegeNames;
                    return View(newUser);
                }

                newUser.Password = HashPassword(newUser.Password);

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }

            ViewBag.CollegeList = c2cUniversitees.Utilities.CollegeData.CollegeNames;
            return View(newUser);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "هذا البريد الإلكتروني غير مسجل في نظامنا. الرجاء التأكد من البريد المدخل.";
                return View();
            }


            string resetToken = Guid.NewGuid().ToString("N");
            user.ResetToken = resetToken;
            user.TokenExpiry = DateTime.Now.AddMinutes(30);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            string resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = resetToken },
                protocol: HttpContext.Request.Scheme);

            string subject = "🔑 طلب إعادة تعيين كلمة المرور لمنصة سوق الجامعة";
            string body = $@"
                <html><body style='font-family: Arial, sans-serif; text-align: right; direction: rtl;'>
                    <h2>طلب إعادة تعيين كلمة المرور</h2>
                    <p>وصلنا طلب لإعادة تعيين كلمة المرور لحسابك. الرجاء النقر على الرابط التالي:</p>
                    <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                        انقر هنا لإعادة تعيين كلمة المرور
                    </a>
                    <p style='margin-top: 20px;'>الرابط صالح لمدة 30 دقيقة. إذا لم تطلب هذا، يمكنك تجاهل هذه الرسالة.</p>
                </body></html>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, subject, body);
                ViewBag.Message = "تم إرسال رابط استعادة كلمة المرور إلى بريدك الإلكتروني بنجاح.";
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.Error = $"حدث خطأ أثناء إرسال البريد. الرجاء المحاولة مرة أخرى لاحقاً. الخطأ: {ex.Message}";
            }

            return View();
        }

        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "الرمز المميز غير صالح أو مفقود.";
                return View("ForgotPassword");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == model.Token);

            if (user == null || user.TokenExpiry < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "الرمز غير صالح، منتهي الصلاحية، أو غير موجود.");
                return View(model);
            }

            user.Password = HashPassword(model.Password);
            user.ResetToken = null;
            user.TokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "تم إعادة تعيين كلمة المرور بنجاح. يمكنك الآن تسجيل الدخول.";
            return RedirectToAction(nameof(Login));
        }
    }
}