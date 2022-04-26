using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("v_projectsdetail")]
    public class Project
    {
        [Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("mfg_date")]
        public DateTime? MfgDate { get; set; }
        

        [Column("sys_eng")]
        public string SysEng { get; set; }

        [Column("dde_eng")]
        public string DdeEng { get; set; }

        [Column("elec_eng")]
        public string ElecEng { get; set; }

        [Column("soft_eng")]
        public string SoftEng { get; set; }

        [Column("sit_eng")]
        public string SitEng { get; set; }

        [Column("css_eng")]
        public string CssEng { get; set; }
        
        [Column("pal_eng")]
        public string PalEng { get; set; }


    }
}