using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueLog.ModelsSage
{
    [Table("poporl", Schema="issue_log")]
    public partial class PoporlPG
    {
        [Key]
        public decimal Porhseq { get; set; }
        public string Oeonumber { get; set; }
        public string Itemno { get; set; }       
        public decimal Oqordered { get; set; }
        public decimal Oqreceived { get; set; }
        public decimal Oqcanceled { get; set; }
        public string Contact { get; set; }
        public string Ponumber {get; set;}
        
    }
}
