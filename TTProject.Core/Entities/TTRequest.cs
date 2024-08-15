using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTProject.Core.Entities
{
    public enum Status
    {
        Pending, Approved, Updated, Rejected
    }

    public class TTRequest
    {

        [Key]
        public long RequestId { get; set; }
        public Status status { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string comment { get; set; }

        public string? note { get; set; }


        [ForeignKey(nameof(Employee))]
        public long? EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [ForeignKey(nameof(ProjectManager))]
        public long? ProjectManagerId { get; set; }

        public ProjectManager? ProjectManager { get; set; }


    }
}
