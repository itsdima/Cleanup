using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class Message : BaseEntity
    {
        [Key]
        public int MessageId {get;set;}
        public string Content{get;set;}
        [ForeignKey("Recipient")]
        public int RecipientId{get;set;}
        public User Recipient{get;set;}
        [ForeignKey("Sender")]
        public int SenderId{get;set;}
        public User Sender{get;set;}
    }
}