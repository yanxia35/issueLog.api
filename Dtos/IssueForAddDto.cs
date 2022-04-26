namespace IssueLog.API.Dtos
{
    public class IssueForAddDto
    {
        public string ProjectNo { get; set; }
        public string Project { get; set; }
        public string IssueOwnerId { get; set; }
        public string IssueDescription { get; set; }
        public string IssueNotes { get; set; }
        public int IssueFoundProcessId { get; set; }
        public string OriginatorId { get; set; }
        public string IssueStatus { get; set; }
        public bool IsMissingParts { get; set; }
    }
}