using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class Image : BaseEntity
    {
        public string FileName{get;set;}
        [ForeignKey("CleanupEvent")]
        public int CleanupId{get;set;}
    }
}