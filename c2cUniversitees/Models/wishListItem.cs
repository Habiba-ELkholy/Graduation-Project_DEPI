using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c2cUniversitees.Models
{
    public class WishlistItem
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ProductId { get; set; } 

        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}