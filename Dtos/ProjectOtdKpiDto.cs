using System;

namespace IssueLog.API.Dtos
{
    public class ProjectOtdKpiDto
    {
        public string ProjectNo { get; set; }
        public DateTime? KbDueDate { get; set; }
        public DateTime? KbCompletionDate { get; set; }
        public DateTime? AdDueDate { get; set; }
        public DateTime? AdCompletionDate { get; set; }

        public DateTime? EngDueDate { get; set; }
        public DateTime? EdDueDate { get; set; }
        public DateTime? EdCompletionDate { get; set; }
        public DateTime? HmDate { get; set; }
        public DateTime? HeDueDate { get; set; }
        public DateTime? HeCompletionDate { get; set; }

        public bool IsLine {get;set;}
        
    }
}