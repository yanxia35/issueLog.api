using System.ComponentModel.DataAnnotations;

namespace IssueLog.API.Dtos
{
    public class UserForChangePasswordDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 2, ErrorMessage = "You must specify password between 2 to 12 characters")]
        public string NewPassword { get; set; }
    }
}