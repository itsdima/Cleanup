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
        [ForeignKey("RecipientUser")]
        public int RecipientUserId{get;set;}
        public User RecipientUser{get;set;}
        [ForeignKey("SenderUser")]
        public int SenderUserId{get;set;}
        public User SenderUser{get;set;}
    }
}