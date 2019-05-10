using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    public class Appointments
    {
        [PrimaryKey, AutoIncrement]
        public int AppointmentID { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        [NotNull]
        public string DateOfAppointment { get; set; }

        [NotNull]
        public String TimeOfAppointmant { get; set; }
    }
}
