using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IssueLog.API.Models
{
  [Table("issue_schedule",Schema ="issue_log")]
  public class IssueSchedule
  {
    [Key]
    [Column("issue_id")]
    public int IssueId { get; set; }

    [ForeignKey("IssueId")]
    public Issue Issue { get; set; }
    

    [Column("created_on")]
    public DateTime CreatedOn { get; set; }

    
    [Column("issue_owner_id")]
    public string IssueOwnerId { get; set; }
    
    [ForeignKey("IssueOwnerId")]
    public Employee Responsible { get; set; }
    
    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("action_ids")]
    public int[] ActionIds {get; set;}

    [Column("responded_at")]
    public DateTime? RespondedAt { get; set; }

   [Column("late")]
    public Boolean Late { get; set; }

    [Column("reminder_sent")]
    public Boolean ReminderSent { get; set; }

    [Column("issue_status")]
    public string IssueStatus { get; set; } 

    [Column("is_line_project")]
    public Boolean IsLine { get; set; }

  }
}