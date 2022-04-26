using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("mbom_parts", Schema = "issue_log")]
    public class MbomPart
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("part_no")]
        public string PartNo { get; set; }

        [Column("quantity")]
        public double Quantity { get; set; }
        [Column("release_date")]
        public DateTime? ReleaseDate { get; set; }
        [Column("supplier_name")]
        public string SupplierName { get; set; }
        // [Column("description")]
        // public string Description {get;set;}
        [Column("project_no")]
        public string ProjectNo { get; set; }

        [Column("lead_time")]
        public int LeadTime { get; set; }
        
        
    }
}