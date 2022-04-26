using System.Collections.Generic;

namespace IssueLog.API.Models
{
    public class ActionTemplateResult
    {
        public ActionTemplateResult(){}        

        public List<Employee> actionOwner {get; set;}
        public string actionTemplate {get;set;}
        public string message{get;set;}
        
    }
}