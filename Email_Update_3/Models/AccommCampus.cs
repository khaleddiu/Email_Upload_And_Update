using System;
using System.Collections.Generic;

#nullable disable

namespace Email_Update_3.Models
{
    public partial class AccommCampus
    {
        public AccommCampus()
        {
            SmisStudents = new HashSet<SmisStudent>();
        }

        public string CampusNo { get; set; }
        public string CampusName { get; set; }
        public string CampusLocation { get; set; }
        public string CampusPhones { get; set; }
        public string PhotoPath { get; set; }
        public DateTime? InSat { get; set; }
        public DateTime? InSun { get; set; }
        public DateTime? InMon { get; set; }
        public DateTime? InTue { get; set; }
        public DateTime? InWed { get; set; }
        public DateTime? InThu { get; set; }
        public DateTime? InFri { get; set; }
        public DateTime? OutSat { get; set; }
        public DateTime? OutSun { get; set; }
        public DateTime? OutMon { get; set; }
        public DateTime? OutTue { get; set; }
        public DateTime? OutWed { get; set; }
        public DateTime? OutThu { get; set; }
        public DateTime? OutFri { get; set; }
        public string Weekend { get; set; }
        public string Blocked { get; set; }
        public string Active { get; set; }
        public string GridView { get; set; }
        public string ListView { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string CampusCode { get; set; }

        public virtual ICollection<SmisStudent> SmisStudents { get; set; }
    }
}
