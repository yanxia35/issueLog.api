using System;

namespace IssueLog.API.Dtos
{
    public class EcEtaDto
    {
        public string ProjectNo { get; set; }
        public string PartNo { get; set; }
        public string IssueNo { get; set; }
        public DateTime? Eta { get; set; }
        public string Status { get; set; }
        
        
    }
}