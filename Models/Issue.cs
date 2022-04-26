using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("issues", Schema = "issue_log")]
    public class Issue
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("issue_no")]
        public string IssueNo { get; set; }

        [Column("created_on")]
        public DateTime CreatedOn { get; set; }

        public virtual Employee Originator { get; set; }

        [Column("originator_id")]
        public string OriginatorId { get; set; }

        [Column("project_no")]
        public string ProjectNo { get; set; }

        [Column("project")]
        public string Project { get; set; }

        [Column("issue_status")]
        public string IssueStatus { get; set; }

        [Column("issue_notes")]
        public string IssueNotes { get; set; }

        [Column("issue_description")]
        public string IssueDescription { get; set; }
        public virtual Employee IssueOwner { get; set; }

        [Column("issue_owner_id")]

        public string IssueOwnerId { get; set; }

        [Column("follow_up_date")]
        public DateTime? FollowUpDate { get; set; }
        public virtual Process IssueFoundProcess { get; set; }

        [Column("issue_found_process_id")]
        public int IssueFoundProcessId { get; set; }
        public virtual Process RootCauseProcess { get; set; }

        [Column("root_cause_process_id")]
        public int RootCauseProcessId { get; set; }
        public virtual FailureMode FailureMode { get; set; }

        [Column("failure_mode_id")]
        public int FailureModeId { get; set; }

        [Column("issue_closed_date")]
        public DateTime? IssueClosedDate { get; set; }
        [Column("is_missing_parts")]
        public bool IsMissingParts { get; set; }
        [Column("is_ready")]
        public bool IsReady { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("software_category")]
        public string SoftwareCategory { get; set; }
        public List<IssueAction> IssueActions { get; set; }
        public List<PartIssue> PartIssues { get; set; }

        public List<Subscriber> Subscribers { get; set; }
        public List<EcPart> EcParts { get; set; }

        public List<IssueFile> IssueFiles { get; set; }

        public IEnumerable<SubProjectStatus> SubProjectStatuses { get; set; }

    }
}