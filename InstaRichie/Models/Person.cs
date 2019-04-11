using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using System.ComponentModel.DataAnnotations;

namespace StartFinance.Models
{
    class Person
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        [Required]
        public string FirstName { get; set; }
        [NotNull]
        [Required]
        public string LastName { get; set; }
        [NotNull]
        [Required]
        public string DOB { get; set; }
        [NotNull]
        [Required]
        public string Gender { get; set; }
        [NotNull]
        [Required]
        public string Email { get; set; }
        [NotNull]
        [Required]
        public int Phone { get; set; }
    }
}
