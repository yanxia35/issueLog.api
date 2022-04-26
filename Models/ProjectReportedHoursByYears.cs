using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("v_project_reported_hours_by_years")]
    public class ProjectReportedHoursByYears
    {
        [Column("project_id")]
        public string ProjectId { get; set; }
        [Column("task_id")]
        public string TaskId { get; set; }
        [Column("hours")]
        public double hours { get; set; }
        [Column("year")]
        public int Year {get;set;}
    }
}