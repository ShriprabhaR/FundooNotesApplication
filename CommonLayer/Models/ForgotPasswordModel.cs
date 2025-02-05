using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer.Models
{
    public class ForgotPasswordModel
    {
        public int UserId {  get; set; }
        [EmailAddress(ErrorMessage ="Validate your Address")]
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
