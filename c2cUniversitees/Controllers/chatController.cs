using Microsoft.AspNetCore.Mvc;
using c2cUniversitees.Models;
using c2cUniversitees.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace c2cUniversitees.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int? receiverId)
        {
            var myId = HttpContext.Session.GetInt32("UserId");
            if (myId == null) return RedirectToAction("Login", "Account");

            if (receiverId == null) return RedirectToAction("MyChats");

            
            var receiverUser = await _context.Users.FindAsync(receiverId);
            ViewBag.ReceiverName = receiverUser != null ? receiverUser.Username : "مستخدم"; 

           
            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverId == myId && m.SenderId == receiverId && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

          
            var messages = await _context.Messages
                .Where(m => (m.SenderId == myId && m.ReceiverId == receiverId)
                         || (m.SenderId == receiverId && m.ReceiverId == myId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            ViewBag.ReceiverId = receiverId;
            return View(messages);
        }


        public async Task<IActionResult> MyChats()
        {
            var myId = HttpContext.Session.GetInt32("UserId");
            if (myId == null) return RedirectToAction("Login", "Account");

           
            var chats = await _context.Messages
                .Where(m => m.SenderId == myId || m.ReceiverId == myId)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            var chatList = chats
                .Select(m => m.SenderId == myId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToList();

            ViewBag.CurrentUserId = myId;
            return View(chats);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(string messageContent, int receiverId)
        {
            var myId = HttpContext.Session.GetInt32("UserId");
            if (myId != null && !string.IsNullOrEmpty(messageContent))
            {
                var newMessage = new Message
                {
                    Content = messageContent,
                    SenderId = myId.Value,
                    ReceiverId = receiverId,
                    Timestamp = DateTime.Now,
                    IsRead = false 
                };

                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", new { receiverId = receiverId });
        }
    }
}