using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("sub_project_status", Schema = "issue_log")]
    public class SubProjectStatus
    {
        [Key][Column("id")]
        public int Id { get; set; }
        [Column("issue_id")]
        public int IssueId { get; set; }
        public Issue Issue { get; set; }
        [Column("sub_project_id")]
        public string SubProjectId { get; set; }
        [Column("status")]
        public string Status { get; set; }

    }
}