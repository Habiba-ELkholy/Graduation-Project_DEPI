using Microsoft.AspNetCore.Mvc;
using c2cUniversitees.Models;
using c2cUniversitees.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace c2cUniversitees.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            var myId = HttpContext.Session.GetInt32("UserId");
            if (myId == null) return RedirectToAction("Login", "Account");


            var items = await _context.WishlistItems
                .Include(w => w.Product) 
                .Where(w => w.UserId == myId)
                .ToListAsync();

            return View(items);
        }

       
        public async Task<IActionResult> Toggle(int productId)
        {
            var myId = HttpContext.Session.GetInt32("UserId");
            if (myId == null) return RedirectToAction("Login", "Account");

         
            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == myId && w.ProductId == productId);

            if (existingItem != null)
            {
               
                _context.WishlistItems.Remove(existingItem);
            }
            else
            {
              
                var newItem = new WishlistItem
                {
                    UserId = myId.Value,
                    ProductId = productId
                };
                _context.WishlistItems.Add(newItem);
            }

            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}