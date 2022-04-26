
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IssueLog.API.Models
{
    [Table("sub_projects", Schema = "issue_log")]
    public class SubProject
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("main_project_id")]
        public string MainProjectId { get; set; }
        [Column("sub_project_id")]
        public string SubProjectId { get; set; }
        [Column("status")]
        public string Status { get; set; }
    }
}