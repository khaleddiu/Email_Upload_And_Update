using System;
using System.Collections.Generic;

#nullable disable

namespace Email_Update_3.Models
{
    public partial class SmisProgram
    {
        public string ProgramId { get; set; }
        public string ProgramName { get; set; }
        public short? NoOfProjects { get; set; }
        public short? Credits { get; set; }
        public string Duration { get; set; }
        public string DepartmentId { get; set; }
        public int? PaymentScheme { get; set; }
        public string ProgShortName { get; set; }
        public string ProgramType { get; set; }
        public string Blocked { get; set; }
        public string Active { get; set; }
        public string GridView { get; set; }
        public string ListView { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public int? NoOfSemesterPerYear { get; set; }
        //public string ApprovedNeedAssoHead { get; set; }
        //public string ApprovedNeedHead { get; set; }
        //public string ApprovedNeedDean { get; set; }
    }
}
