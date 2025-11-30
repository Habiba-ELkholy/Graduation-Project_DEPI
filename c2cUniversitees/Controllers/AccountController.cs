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

        // ===============================================
        // A. تسجيل الدخول (Login)


        // ===============================================
        // داخل AccountController.cs

        // ===============================================
        // A. تسجيل الدخول (Login)
        // ===============================================

        // دالة GET: لعرض نموذج تسجيل الدخول
        [HttpGet]
        public IActionResult Login()
        {
            // ببساطة ترجع الواجهة (View) باسم "Login.cshtml"
            return View();
        }

        // ... يتبعها دالة Login (POST) التي قمنا بتعديلها ...

        // ===============================================
        // B. إنشاء حساب جديد (Register)
        // ===============================================

        // دالة GET: لعرض نموذج التسجيل
        [HttpGet]
        public IActionResult Register()
        {
            // يجب تمرير قائمة الكليات إلى الـ View
            ViewBag.CollegeList = c2cUniversitees.Utilities.CollegeData.CollegeNames;
            return View();
        }

        // ... يتبعها دالة Register (POST) التي قمنا بتعديلها ...

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            // 1. التحقق من المدخلات الفارغة
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "الرجاء إدخال البريد وكلمة المرور.";
                return View();
            }

            // 2. تشفير كلمة المرور المُدخلة للبحث في قاعدة البيانات
            string hashedPassword = HashPassword(password);

            // 3. البحث عن المستخدم بالإيميل وكلمة المرور المُشفرة
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);

            // 4. التحقق من نجاح تسجيل الدخول
            if (user != null)
            {
                // ==========================================================
                // الخطوة 4.1: المصادقة الرسمية باستخدام Claims و Cookies
                // ==========================================================

                // إنشاء المطالبات (Claims) لتمثيل هوية المستخدم
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // الـ ID الأساسي للمستخدم
            new Claim(ClaimTypes.Name, user.Username), // اسم المستخدم
            new Claim("College", user.CollegeName) // المطالبة الخاصة بالكلية
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // يمكن تحديد خصائص مثل isPersistent = true; (تذكرني)
                    IsPersistent = false
                };

                // تسجيل الدخول الرسمي (يجب أن يتطابق مع المخطط المحدد في Program.cs)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, // استخدام المخطط الافتراضي لـ Cookies
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // ==========================================================
                // الخطوة 4.2: تخزين بيانات سريعة في الجلسة (اختياري لكن مفيد)
                // ==========================================================
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);

                // 5. التوجيه عند النجاح
                return RedirectToAction("Index", "Products");
            }

            // 6. إذا فشلت المصادقة
            ViewBag.Error = "البريد الإلكتروني أو كلمة المرور غير صحيحة.";
            return View();
        }

        // ===============================================
        // B. إنشاء حساب جديد (Register)
        // ===============================================

  

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ملاحظة: يجب تعديل هذه الدالة لتطبيق قواعد النطاق الجامعي (التي ناقشناها سابقاً)
        public async Task<IActionResult> Register([Bind("Username,Email,Password,CollegeName")] User newUser)
        {
            // ... (منطق التحقق من النطاق الجامعي هنا) ...

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

        // ===============================================
        // C. تسجيل الخروج (Logout)
        // ===============================================

        public async Task<IActionResult> Logout()
        {
            // 1. إنهاء جلسة المصادقة الرسمية (إزالة الكوكيز)
            // نستخدم اسم المخطط الافتراضي لضمان التوافق مع الإعدادات في Program.cs
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 2. إزالة بيانات الجلسة (Sessions) لضمان التنظيف الكامل
            HttpContext.Session.Clear();

            // 3. التوجيه إلى الصفحة الرئيسية
            return RedirectToAction("Index", "Home");
        }

        // ===============================================
        // D. نسيان كلمة المرور (Forgot Password)
        // ===============================================

        // 1. عرض نموذج نسيان كلمة المرور (GET) - توقيع وحيد
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // 2. معالجة طلب نسيان كلمة المرور (POST) - توقيع وحيد
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

            // **البريد موجود: استكمال عملية الإرسال**

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

        // ===============================================
        // E. إعادة تعيين كلمة المرور (Reset Password)
        // ===============================================

        // عرض نموذج إعادة تعيين كلمة المرور (GET)
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "الرمز المميز غير صالح أو مفقود.";
                return View("ForgotPassword");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        // معالجة طلب تحديث كلمة المرور (POST)
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