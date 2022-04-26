using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("deliverables")]
    public class Deliverable
    {
        [Column("id"), Key]
        public int Id { get; set; }


        [Column("project_id")]
        public string ProjectId { get; set; }

        [Column("type")]
        public string Type { get; set; }
        [Column("completion_date")]
        public DateTime? CompletionDate { get; set; }
        [Column("start_date")]

        public DateTime? StartDate { get; set; }
        [Column("end_date")]
        public DateTime? EndDate { get; set; }



    }
}