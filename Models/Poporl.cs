using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IssueLog.API.Models
{
    [Table("poporl", Schema ="supply_chain")]
    public class Poporl
    {
        [Key]
        [Column("porhseq")]
        public double Porhseq { get; set; }
        [Column("contract")]
        public string ProjectNo { get; set; }
        [Column("itemno")]
        public string PartNo { get; set; }
        [Column("oeonumber")]
        public string IssueNo { get; set; }
        
        [Column("exparrival")]
        public double Eta { get; set; }
        
    }
}