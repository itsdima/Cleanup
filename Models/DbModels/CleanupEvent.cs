using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class CleanupEvent : BaseEntity
    {
        [Key]
        public int CleanupId {get;set;}
        public double Latitude{get;set;}
        public double Longitude{get;set;}
        public string DescriptionOfArea{get;set;}
        public string DescriptionOfTrash{get;set;}
        public int Value{get;set;}
        public bool Pending{get;set;}
        [NotMapped]
        public int CreatedByUserId{get;set;}
        // [ForeignKey("User")]
        public int UserId { get; set; }
        public User User {get;set;}
        public List<Image> TrashImages{get;set;}
        public CleanupEvent()
        {
            TrashImages = new List<Image>();
        }
    }
}