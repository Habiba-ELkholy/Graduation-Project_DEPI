using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace c2cUniversitees.Models.Data
{
    public class ApplicationDbContext : DbContext
    {
    
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

           
            public DbSet<Product> Products { get; set; }

        
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
    }
}
