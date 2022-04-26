using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("ec_parts", Schema = "issue_log")]
    public class TEcPart
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("part_no")]
        public string partNo { get; set; }

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
        public Issue Issue { get; set; }
        [Column("issue_id")]
        public int IssueId { get; set; }

    }
}