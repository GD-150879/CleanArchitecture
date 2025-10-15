using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CleanApp.Domain.Entities
{
    public class Emp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        // Foreign key to Company
        public int CompanyId { get; set; }

        // Navigation property (many-to-one)
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
