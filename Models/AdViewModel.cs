using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplication5.Models
{
    public class AdViewModel
    {
        [Required, MinLength(3), MaxLength(15)]
        public string Title { get; set; }

        [Required, MinLength(3), MaxLength(50)]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [MinLength(3), MaxLength(10)]
        public string Color { get; set; }

        public string Date { get; set; }

        [Required(ErrorMessage ="Select location on map")]
        public string Location { get; set; }

        [DisplayName("Image")]
        public HttpPostedFileBase ImagePath { get; set; }

        public string Type { get; set; }

        public string Organization { get; set; }

        
        public int Reward { get; set; }

        public string ImageName { get; set; }

        public string City { get; set; }

        public int Id { get; set; }

        public string PostedBy { get; set; }
    }
}