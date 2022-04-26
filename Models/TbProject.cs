using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("projects")]
    public class TbProject
    {
        [Column("id")]
        [Key]
        public string ProjectId { get; set; }
        [Column("product")]
        public string Product { get; set; }

        [Column("mech_complexity")]
        public string MechComplexity { get; set; }
        [Column("elec_complexity")]
        public string ElecComplexity { get; set; }
        [Column("social_complexity")]
        public string SocialComplexity { get; set; }
        [Column("status")]
        public string Status { get; set; }
        
        
    }
}