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
        [ForeignKey("User")]
        public int RecipientUserId{get;set;}
        // public User RecipientUser{get;set;}
        [ForeignKey("User")]
        public int SenderUserId{get;set;}
        // public User SenderUser{get;set;}
    }
}