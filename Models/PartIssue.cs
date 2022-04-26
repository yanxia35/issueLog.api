using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("part_issue", Schema ="issue_log")]
    public class PartIssue
    {
        [Column("Id")]
        public int Id { get; set; }
        [Column("part_no")]
        public string PartNo { get; set; }
        [Column("is_hard_flag")]
        public bool IsHardFlag { get; set; }
        [Column("is_resolved")]
        public bool IsResolved { get; set; }
        public Employee Employee {get;set;}
        [ForeignKey("Employee")]

        [Column("resolved_by")]
        public string ResolvedBy { get; set; }
        [Column("resolved_date")]
        public DateTime? ResolvedDate { get; set; }
        public Issue Issue { get; set; }
        [Column("issue_id")]
        public int IssueId { get; set; }

    }
}