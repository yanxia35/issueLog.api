using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("subscribers", Schema="issue_log")]
    public class Subscriber
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public Employee Employee { get; set; }

        [Column("employee_id"), Required]
        public string EmployeeId { get; set; }

        public Issue Issue { get; set; }
        [Column("issue_id")]
        public int IssueId { get; set; }

    }

}