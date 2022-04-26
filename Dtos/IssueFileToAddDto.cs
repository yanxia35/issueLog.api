using Microsoft.AspNetCore.Http;

namespace IssueLog.API.Dtos
{
    public class IssueFileToAddDto
    {
        public int IssueId { get; set; }
        public IFormFile File {get; set;}
    }
}