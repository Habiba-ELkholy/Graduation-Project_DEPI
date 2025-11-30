using c2cUniversitees.Models;
using c2cUniversitees.Models.Data;
using c2cUniversitees.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Hosting; 
using System.Security.Claims; 

namespace c2cUniversitees.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Index(string college)
        {
            var products = _context.Products.Include(p => p.Seller).AsQueryable();

            if (!string.IsNullOrEmpty(college))
            {
                products = products.Where(p => p.Seller.CollegeName == college);
            }
            
            var myId = HttpContext.Session.GetInt32("UserId");
            if (myId.HasValue)
            {
                var myWishlistIds = await _context.WishlistItems
      .Where(w => w.UserId == myId)
      .Select(w => w.ProductId)
      .ToListAsync(); 

                ViewBag.WishlistIds = myWishlistIds;
            }

            var colleges = new List<string> { "هندسة", "طب", "حاسبات", "تجارة", "أخرى / غير محدد" };
            ViewBag.FilterOptions = colleges.Select(c => new Tuple<string, bool>(c, c == college)).ToList();

            return View(await products.ToListAsync());
        }

        [Authorize]
        public IActionResult Create()
        {
            
            ViewBag.Categories = new List<string> { "هندسة", "فنون", "علوم", "آثار", "أدوات تخرج", "كتب", "أثاث", "إلكترونيات", "أخرى" };
            return View();
        }

        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Price,Category,ImageFile")] ProductViewModel model)
        {
          
            var categories = new List<string> { "هندسة", "فنون", "علوم", "آثار", "أدوات تخرج", "كتب", "أثاث", "إلكترونيات", "أخرى" };

            
            int? sellerId = HttpContext.Session.GetInt32("UserId");
            if (!sellerId.HasValue)
            {
                
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
               
                string uniqueFileName = null;
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                
                var product = new Product
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    Category = model.Category,
                    ImagePath = uniqueFileName,
                    SellerId = sellerId.Value
                };

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }

           
            ViewBag.Categories = categories;
            return View(model);
        }

        
        [HttpPost, ActionName("Sold")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoldConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (product == null)
            {
                return NotFound();
            }

            
            if (!currentUserId.HasValue || product.SellerId != currentUserId.Value)
            {
                return Unauthorized(); 
            }

            product.IsSold = true;
            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}