using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("action_owner")]
    public class ActionOwner
    {
        [Column("group_id")]
        [Key]
        public int GroupId { get; set; }
        
        [Column("employee_id")]
        public string EmployeeId { get; set; }
        
        [Column("department")]
        public string Department { get; set; }

        [Column("facility")]
        public string Facility { get; set; }

        [Column("is_line")]
        public bool IsLine { get; set; }

        [Column("type")]
        public string Type { get; set; }
    }
}