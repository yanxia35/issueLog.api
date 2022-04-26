using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
        Task<bool> SendEmail(Issue issue, List<string> recipientList, string subject);
        Task<bool> SendEmail(Issue issue, string subject);
        Task<bool> SendEmail_line(Issue issue, string subject);
    }
}