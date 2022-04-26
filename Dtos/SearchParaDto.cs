namespace IssueLog.API.Dtos
{
    public class SearchParaDto
    {
        public string Originator { get; set; }
        public string IssueStatus { get; set; }
        public string IssueOwner { get; set; }
        public string ActionOwner { get; set; }
        public string IssueNotes { get; set; }
        public string ProjectNo { get; set; }
        public string IssueNo { get; set; }
        public int IsMissingParts { get; set; }
        public int RootCauseProcess { get; set; }
        public string OwnerDepartment { get; set; }
        public string ActionStatus { get; set; }
        public int IsReady { get; set; }
        public int IsActionLate { get; set; }
        public int IsMissingDueDate { get; set; }
        public int ActionDueDateMoreThan { get; set; }
        public string ActionOwnerDept { get; set; }
    }
}