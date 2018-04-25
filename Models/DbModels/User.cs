using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class User : BaseEntity
    {
        public int UserId {get;set;}
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string UserName{get;set;}
        public string Email{get;set;}
        public string Password{get;set;}
        public int UserLevel{get;set;}
        public string ProfilePic{get;set;}
        public int Score{get;set;}
        public int Token{get;set;}
        [ForeignKey("AttendingCleanup")]
        public int? AttendingCleanupId{get;set;}
        public CleanupEvent AttendingCleanup{get;set;}
        [InverseProperty("User")]
        public List<CleanupEvent> CreatedCleanups{get;set;}
        [InverseProperty("SenderUser")]
        public List<Message> MessagesSent{get;set;}
        [InverseProperty("RecipientUser")]
        public List<Message> MessagesReceived{get;set;}
        public User()
        {
            CreatedCleanups = new List<CleanupEvent>();
            MessagesSent = new List<Message>();
            MessagesReceived = new List<Message>();
        }
    }
}