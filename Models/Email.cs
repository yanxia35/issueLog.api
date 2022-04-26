using System.Collections.Generic;

namespace IssueLog.API.Models
{
    public class Email
    {
        public Email()
        {
            this.ToRecipientList = new List<string>();
            this.CCRecipientList = new List<string>();
        }        
        public List<string> ToRecipientList { get; set; }
        public List<string> CCRecipientList { get; set; }
        public string Subject { get; set; }
        public string mailBody { get; set; }
        
    }
}