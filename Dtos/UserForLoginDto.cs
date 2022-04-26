using System.ComponentModel.DataAnnotations;

namespace IssueLog.API.Dtos
{
    public class UserForLoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        
    }
}