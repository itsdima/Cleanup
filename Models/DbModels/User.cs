using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class User : BaseEntity
    {
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string UserName{get;set;}
        public string Email{get;set;}
        public string Password{get;set;}
        public int UserLevel{get;set;}
        public string ProfilePic{get;set;}
        public int Score{get;set;}
        public int Token{get;set;}
        [ForeignKey("Cleanup")]
        public int? AttendingCleanupId{get;set;}
        public List<CleanupEvent> CreatedCleanups{get;set;}
        public List<Message> MessagesSent{get;set;}
        public List<Message> MessagesReceived{get;set;}
        public User()
        {
            CreatedCleanups = new List<CleanupEvent>();
            MessagesSent = new List<Message>();
            MessagesReceived = new List<Message>();
        }
    }
}