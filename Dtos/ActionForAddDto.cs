using System;

namespace IssueLog.API.Dtos
{
    public class ActionForAddDto
    {

        public string CreatedById { get; set; }
        public int IssueId { get; set; }
        public string ActionNotes { get; set; }
        public string ResponsibleId { get; set; }
        public DateTime? DueDate { get; set; }
        public string ActionStatus { get; set; }
        public string SubProjectId { get; set; }

    }
}