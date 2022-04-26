namespace IssueLog.API.Dtos
{
    public class ProjectComplexityDto
    {
        public string ProjectId { get; set; }
        public string Product {get;set;}
        public string MechComplexity { get; set; }
        public string ElecComplexity { get; set; }
        public string SocialComplexity { get; set; }
        public string Status {get;set;}
    }
}