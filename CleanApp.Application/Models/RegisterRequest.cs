using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Application.Models
{
    public class RegisterRequest
    {
        [Required]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string  PhoneNo { get; set; }
    }
}
