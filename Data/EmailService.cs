using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public class EmailService : IEmailService
    {

        public Task<bool> SendEmail(Email email)
        {
            email.ToRecipientList = ValidateEmailRecipient(email.ToRecipientList);
            email.CCRecipientList = ValidateEmailRecipient(email.CCRecipientList); 
            if (email.ToRecipientList.Count() <= 0)
            {
                System.Diagnostics.Debug.WriteLine("No Recipient");
                return Task.FromResult(false);
            }
            if (email.CCRecipientList == null)
            {
                email.CCRecipientList = new List<string>();
            }
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = "mail.avl.com";
            // client.EnableSsl = true;
            // client.Credentials = new System.Net.NetworkCredential("notifications@greenlightinnovation.com","");
            // client.Timeout = 5000;
            // client.UseDefaultCredentials = true;
                MailMessage message = new MailMessage();
            message.From = new MailAddress("notifications@greenlightinnovation.com");
            foreach (string toRecipient in email.ToRecipientList)
            {
                message.To.Add(new MailAddress(toRecipient));
            }
            if (email.CCRecipientList.Count() > 0)
            {
                foreach (string ccRecipient in email.CCRecipientList)
                {
                    message.CC.Add(new MailAddress(ccRecipient));
                }
            }
            message.Subject = email.Subject;
            message.Body = email.mailBody;
            //string userState = "test messgae";
            try
            {
                object task = new object();
                client.SendAsync(message, task);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Task.FromResult(false); ;
            }
            System.Diagnostics.Debug.WriteLine("Email sent");
            return Task.FromResult(true); ;
        }
        private List<string> ValidateEmailRecipient(List<string> emailRecipients)
        {
            List<string> result = new List<string>();

            if (emailRecipients != null)
            {
                if (emailRecipients.Count > 0)
                {
                    foreach (string recipient in emailRecipients)
                    {
                        if (recipient != null && recipient != "")
                        {
                            result.Add(recipient);
                        }
                    }
                }
            }

            return result;
        }
        public async Task<bool> SendEmail(Issue issue, List<string> recipientList, string subject)
        {
            Email email = new Email();
            email.ToRecipientList = recipientList;
            email.Subject =  subject+ " " + issue.Project ;
            email.mailBody = "Issue No: " + issue.IssueNo + "\nhttp://issuelog.greenlightinnovation.com/issue/" +issue.IssueNo
                + "\nProject No: " + issue.ProjectNo + " " + issue.Project
                +"\nIssue Created On: " + issue.CreatedOn.ToString("yyyy/MM/dd")
                + "; \nCreated by: " + issue.Originator.FirstName + " " + issue.Originator.LastName + 
                "\nIssue Notes: \n" + issue.IssueNotes;
            var isEmailed = await SendEmail(email);
            return isEmailed;
        }
        public async Task<bool> SendEmail(Issue issue, string subject)
        {

            List<string> ToRecipientList = new List<string>();
            ToRecipientList.Add(issue.IssueOwner.Email);

            var isEmailed = await SendEmail(issue, ToRecipientList, subject);
            return isEmailed;
        }

         
        /**
         * Function sends a email to issue owner to notify the issue is line ready
         * 
         * @para issue: issue created or modified
         * @para subject: the subject of the email
         */
        public async Task<bool> SendEmail_line(Issue issue, string subject)
        {
            Email email = new Email();
            List<string> recipientList = new List<string>();
            recipientList.Add(issue.IssueOwner.Email);
            email.ToRecipientList = recipientList;
            email.Subject =  subject+ " " + issue.Project ;
            email.mailBody = "Urgent new issue for GI Express Project, Please respond within 2 hours.\n" + 
            "Issue No: " + issue.IssueNo + "\nhttp://issuelog.greenlightinnovation.com/issue/" +issue.IssueNo;
 
            var isEmailed = await SendEmail(email);
            return isEmailed;
        }
    }
}