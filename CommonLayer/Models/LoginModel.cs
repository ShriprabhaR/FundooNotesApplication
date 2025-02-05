using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Validate your email")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
