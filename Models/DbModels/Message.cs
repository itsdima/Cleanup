using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class Message : BaseEntity
    {
        public string Content{get;set;}
        [ForeignKey("User")]
        public int RecipientUserId{get;set;}
        [ForeignKey("User")]
        public int SenderUserId{get;set;}
    }
}