using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Application.Models
{
    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public ErrorResponse? Error { get; set; }
        public SuccessResponse? Success { get; set; }
    }

}
