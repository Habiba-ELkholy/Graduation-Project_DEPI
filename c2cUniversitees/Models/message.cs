using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c2cUniversitees.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        
        public int SenderId { get; set; }

       
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int? ProductId { get; set; }

        public bool IsRead { get; set; } = false; 

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; } 
    }
}