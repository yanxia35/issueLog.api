using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("v_ec_parts_eta", Schema = "issue_log")]
    public class EcPart
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
        [Column("description")]
        public string Description {get;set;}
        [Column("project_no")]
        public string ProjectNo { get; set; }
        [Column("po_number")]
        public string PoNumber {get;set;}
        public Issue Issue { get; set; }
        [Column("issue_id")]
        public int IssueId { get; set; }

        [Column("system_eta")]
        public DateTime? SystemEta {get; set;}

        [Column("buyer_eta")]
        public DateTime? BuyerEta {get; set;}

        [Column("po_eta")]
        public DateTime? PoEta {get; set;}


    }
}