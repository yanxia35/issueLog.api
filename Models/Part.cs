using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.API.Models
{
    [Table("PART_CATALOG", Schema="part_catalog")]
    public class Part
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("PART_NO")]
        public string PartNo { get; set; }
        [Column("MFG_INFO")]
        public string MfgInfo { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column ("LEADTIME")]
        public int? Leadtime { get; set; }
        
        
        
    }
}