namespace IssueLog.API.Dtos {
    public class PartToFlag {
        public string PartNo { get; set; }
        public string IssueNo { get; set; }
        public int IssueId { get; set; }
        public bool IsHardFlag { get; set; }
    }
}