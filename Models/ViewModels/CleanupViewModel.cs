using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class CleanupViewModel : BaseEntity
    {
        [Required(ErrorMessage="Address required")]
        [Display(Name="Address: ")]
        [MinLength(10)]
        public string Address{get;set;}
        [Required(ErrorMessage="Description required")]
        [Display(Name="Describe what to look for: ")]
        [MinLength(10, ErrorMessage="Description must be at least 10 characters long")]
        public string DescriptionOfArea{get;set;}
        [Required(ErrorMessage="Description required")]
        [Display(Name="Description of litter: ")]
        [MinLength(10, ErrorMessage="Description must be at least 10 characters long")]
        public string DescriptionOfTrash{get;set;}
    }
}