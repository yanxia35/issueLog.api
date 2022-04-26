using System.Collections.Generic;

namespace IssueLog.API.Models
{
    public class ActionOwnerCondition
    {
        public ActionOwnerCondition(){}        

        public string template {get; set;}
        public Issue issue {get;set;}
        
    }
}