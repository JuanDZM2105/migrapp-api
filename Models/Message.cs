using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace migrapp_api.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public int? SenderId { get; set; }

        public int? ReceiverId { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsRead { get; set; }

        public User? Sender { get; set; }

        public User? Receiver { get; set; }

    }
}
