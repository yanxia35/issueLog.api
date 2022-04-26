using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
  [Table("actions",Schema ="issue_log")]
  public class IssueAction
  {
    [Column("id")]

    public int Id { get; set; }

    public virtual Employee CreatedBy { get; set; }

    [Column("created_by_id")]
    public string CreatedById { get; set; }

    [Column("created_on")]
    public DateTime CreatedOn { get; set; }

    [Column("action_notes")]
    public string ActionNotes { get; set; }
    
    public virtual Employee Responsible { get; set; }

    [Column("responsible_id")]
    public string ResponsibleId { get; set; }

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    [Column("action_status")]
    public string ActionStatus { get; set; }

    [Column("action_closed_date")]
    public DateTime? ActionClosedDate { get; set; }

    [Column("action_active_date")]
    public DateTime? ActionActiveDate { get; set; }

    [Column("due_date_entered_date")]
    public DateTime? DueDateEnteredDate { get; set; }

    public Issue Issue { get; set; }
    [ForeignKey("Issue")]

    [Column("issue_id")]
    public int IssueId { get; set; }

    [Column("sub_project_id")]
    public string SubProjectId { get; set; }

  }
}