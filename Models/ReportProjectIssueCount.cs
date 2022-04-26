using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  IssueLog.API.Models
{
  [Table("v_report_project_issue_count",Schema="issue_log")]
    public class ReportProjectIssueCount
    {
      [Key]
      [Column("id")]
      public string projectId { get; set; }
      [Column("number_of_issues")]
      public int NumberOfIssues { get; set; }
      [Column("mfg_date")]
      public DateTime? MfgDate { get; set; }
      [Column("project_type")]
      public string ProjectType { get; set; }
      [Column("mech_complexity")]
      public string MechComplexity { get; set; }
      [Column("elec_complexity")]
      public string ElecComplexity { get; set; }
      [Column("social_complexity")]
      public string SocialComplexity { get; set; }

    }
}
