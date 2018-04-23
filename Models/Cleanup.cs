using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class Cleanup : BaseEntity
    {
        public double Latitude{get;set;}
        public double Longitude{get;set;}
        public string DescriptionOfArea{get;set;}
        public string DescriptionOfTrash{get;set;}
        public int Value{get;set;}
        public bool Pending{get;set;}
        [ForeignKey("User")]
        public int CreatedByUserId{get;set;}
        public List<Image> TrashImages{get;set;}
        public Cleanup()
        {
            TrashImages = new List<Image>();
        }
    }
}