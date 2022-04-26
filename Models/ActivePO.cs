using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("active_po",Schema="supply_chain")]
    public class ActivePO
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("project")]
        public string ProjectNo { get; set; }
        
        [Column("po_num")]
        public string PoNo { get; set; }
        [Column("system_eta")]
        public DateTime? SystemEta { get; set; }
        [Column("buyer_eta")]
        public DateTime? BuyerEta { get; set; }
        [Column("ec_flag")]
        public string IssueNo { get; set; }
        [Column("itemno")]
        public string PartNo { get; set; }
        [Column("status")]
        public string Status { get; set; }

    }
}