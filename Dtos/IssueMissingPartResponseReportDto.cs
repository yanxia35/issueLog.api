using System;
using IssueLog.API.Models;

namespace IssueLog.API.Dtos
{
    public class IssueMissingPartResponseReportDto
    {
        public string IssueNo { get; set; }

        public DateTime? CreatedOn { get; set; }
        
        public string ProjectNo { get; set; }

        public virtual Employee IssueOwner { get; set; }

        public DateTime? FirstPartOrderDate { get; set; }
        
    }
}