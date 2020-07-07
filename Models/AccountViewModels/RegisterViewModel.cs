using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication5.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        public int id { get; set; }


        [Required(ErrorMessage ="*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Minimum 8 characters"), MaxLength(15), MinLength(8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Enter a password")]
        [Compare("Password", ErrorMessage ="Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [MaxLength(17, ErrorMessage ="Phone number too long"), MinLength(5, ErrorMessage ="Phone number too short")]
        public string Phone { get; set; }

        public string Role { get; set; }


        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string Organization { get; set; }
    }
}