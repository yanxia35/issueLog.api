

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("project_schedule")]
    public class ProjectSchedule
    {
        [Column("id")]
        public string Id { get; set; }
        

        [Column("is_line")]
        public bool IsLine { get; set; }
        
        
        [Column("eng_due_date")]
        public DateTime? EngDueDate { get; set; }

        [Column("hm_date")]
        public DateTime? HmDate { get; set; }

        [Column("kb_start_date")]
        public DateTime? KbStartDate { get; set; }

        [Column("kb_end_date")]
        public DateTime? KbEndDate { get; set; }

        [Column("kit_start_date")]
        public DateTime? KitStartDate { get; set; }

        [Column("kit_end_date")]
        public DateTime? KitEndDate { get; set; }

        [Column("facility")]
        public string Facility { get; set; }

        [Column("mfg_start_date")]
        public DateTime? MfgStartDate { get; set; }

        [Column("mfg_end_date")]
        public DateTime? MfgEndDate { get; set; }

        [Column("sit_start_date")]
        public DateTime? SitStartDate { get; set; }
        
        [Column("sit_end_date")]
        public DateTime? SitEndDate { get; set; }
        
        [Column("shipping_date")]
        public DateTime? ShippingDate { get; set; }

        [Column("tst_eng")]
        public string TestEng { get; set; }
        [Column("rd_due_date")]
        public DateTime? RdDueDate { get; set; }
        [Column("ce_due_date")]
        public DateTime? CeDueDate { get; set; }
        [Column("he_due_date")]
        public DateTime? HeDueDate { get; set; }
        [Column("is_sub_proj")]
        public bool? IsSubProj { get; set; }
        [Column("status")]
        public string Status { get; set; }
        
    }
}