using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Mandatory field")]
        [StringLength(20)]
        [RegularExpression(@"^[A-Z][a-z]{2,}$", ErrorMessage = "First letter should be in UpperCase other letters should LowerCase.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        [EmailAddress]
        [Required(ErrorMessage ="Mandatory field")]
        public string Email { get; set; }
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
