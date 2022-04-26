using System;

namespace IssueLog.API.Dtos
{
    public class PartReleaseOtoDto
    {
        public string ProjectNo { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string PartNo { get; set; }

        public double Qty { get; set; }
        public DateTime? MfgStartDate { get; set; }

        public bool IsLine { get; set; }
        
        public string Supplier { get; set; }

        public int LeadTime { get; set; }
        
        
        
        
    }
}