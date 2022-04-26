using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IssueLog.API.Data
{
    public class ActionRepository : IActionRepository
    {
        private readonly DataContext _context;
        private readonly PacsContext _pacsContext;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private List<string> emailException = new List<string>();
        private IDictionary<string,string> actionTemplateDict = new Dictionary<string, string>();
        private IDictionary<string,string> StatusToDeptDict = new Dictionary<string, string>();

        public ActionRepository(DataContext context, PacsContext pacsContext, IEmailService emailService,IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _pacsContext = pacsContext;
            _config = configuration;
            emailException = _config.GetSection("EmailException").Get<List<string>>();
            actionTemplateDict = _config.GetSection("ActionTemplate").Get<Dictionary<string, string>>();
            StatusToDeptDict = _config.GetSection("StatusToDept").Get<Dictionary<string, string>>();
        }

        // add action
        public async Task<IssueAction> AddAction(IssueAction newAction)
        {
            newAction = ValidateNewAction(newAction);
            if (newAction.ActionStatus == "active")
            {
                newAction.ActionActiveDate = DateTime.Now;
            }
            var actionSaved = await _context.IssueActions.AddAsync(newAction);
            await _context.SaveChangesAsync();
            newAction = await _context.IssueActions.Where(x => x.Id == newAction.Id).Include(x => x.Responsible).Include(x => x.Issue).FirstOrDefaultAsync();

            if(newAction != null)
            {
                var IssueInSchedule = await _context.IssueSchedule.Where(x => x.IssueId == newAction.Issue.Id).FirstOrDefaultAsync(); //update IssueSchedule Table when action changes
                if(IssueInSchedule != null)
                {
                    if(IssueInSchedule.RespondedAt == null && newAction.ActionStatus == "active")
                    {
                        IssueInSchedule.RespondedAt = DateTime.Now;
                    }

                    List<int> actionList = IssueInSchedule.ActionIds.ToList();
                    if(!actionList.Contains(newAction.Id))
                    {
                        actionList.Add(newAction.Id);
                    }
                    var actionArr = actionList.ToArray();
                    IssueInSchedule.ActionIds = actionArr;
                    _context.Entry(IssueInSchedule).CurrentValues.SetValues(IssueInSchedule);
                    await _context.SaveChangesAsync();
                } 
            }

            if (newAction.ResponsibleId != null && newAction.ActionStatus == "active") 
            {
                Email email = new Email();
                email.ToRecipientList.Add(newAction.Responsible.Email);
                email.Subject = "New Action Created" + " " + newAction.Issue.Project;
                email.mailBody = "Issue No: " + newAction.Issue.IssueNo +
                "\nhttp://issuelog.greenlightinnovation.com/issue/" + newAction.Issue.IssueNo
                + "\nProject No: " + newAction.Issue.ProjectNo + " " + newAction.Issue.Project
                + "\n" + "Action Notes:\n" + newAction.ActionNotes;
                await _emailService.SendEmail(email);
            }
            return newAction;
        }
        public async Task<IssueAction> UploadAction(IssueAction newAction)
        {
            var actionSaved = await _context.IssueActions.AddAsync(newAction);
            await _context.SaveChangesAsync();
            return newAction;
        }
        private async Task<Issue> GetIssueById(int id)
        {
            var issue = await _context.Issues.Where(x => x.Id == id).Include(x => x.Originator)
              .Include(x => x.IssueOwner)
              .Include(x => x.IssueActions).ThenInclude(x => x.CreatedBy)
              .Include(x => x.IssueActions).ThenInclude(x => x.Responsible).FirstOrDefaultAsync();
            return issue;
        }
        private IssueAction ValidateNewAction(IssueAction newAction)
        {
            newAction.Id = 0;
            newAction.CreatedOn = DateTime.Now;
            // newAction.ActionStatus = "draft";
            return newAction;
        }

        public async Task<IssueAction> GetAction(int id)
        {
            var action = await _context.IssueActions.Where(x => x.Id == id).Include(x => x.Responsible).Include(x => x.CreatedBy).Include(x => x.Issue).FirstOrDefaultAsync();
            return action;
        }

        public async Task<IssueAction> SaveAction(IssueAction action)
        {
            bool requireEmail = false;
            bool requreSendSubscriber = false;
            var actionInDb = await _context.IssueActions.Where(x => x.Id == action.Id).Include(x => x.Responsible).Include(x => x.Issue).FirstOrDefaultAsync();
            if (actionInDb == null)
            {
                return null;
            }
            if (actionInDb.ActionStatus != "closed" && action.ActionStatus == "closed")
            {
                action.ActionClosedDate = DateTime.Now;
                if(actionInDb.ActionActiveDate ==null){
                    action.ActionActiveDate = DateTime.Now;
                }
                requreSendSubscriber = true;
                if (actionInDb.ResponsibleId != null)
                {

                    if (actionInDb.Responsible.GroupId == "MAT")
                    {
                        actionInDb.Issue.IsReady = true;
                    }
                }
            }

            if (actionInDb.ActionStatus != "active" && action.ActionStatus == "active")
            {
                action.ActionActiveDate = DateTime.Now;
                requreSendSubscriber = true;
                requireEmail = true;
            }
            if (actionInDb.ResponsibleId != action.ResponsibleId && action.ActionStatus == "active")
            {
                requireEmail = true;
            }
            if (actionInDb.DueDate == null && action.DueDate != null)
            {
                action.DueDateEnteredDate = DateTime.Now;
            }

            _context.Entry(actionInDb).CurrentValues.SetValues(action);
            await _context.SaveChangesAsync();
            var actionToReturn = await GetAction(action.Id);

            if(actionToReturn != null)
            {
                var IssueInSchedule = await _context.IssueSchedule.Where(x => x.IssueId == actionToReturn.Issue.Id).FirstOrDefaultAsync(); //update IssueSchedule Table when action changes
                if(IssueInSchedule != null)
                {
                    if(IssueInSchedule.RespondedAt == null && actionToReturn.ActionStatus == "active")
                    {
                        IssueInSchedule.RespondedAt = DateTime.Now;
                    }

                    List<int> actionList = IssueInSchedule.ActionIds.ToList();
                    if(!actionList.Contains(actionToReturn.Id))
                    {
                        actionList.Add(actionToReturn.Id);
                    }
                    var actionArr = actionList.ToArray();
                    IssueInSchedule.ActionIds = actionArr;
                    _context.Entry(IssueInSchedule).CurrentValues.SetValues(IssueInSchedule);
                    await _context.SaveChangesAsync();
                } 
            }

            if (requireEmail && action.ResponsibleId != null && !emailException.Contains(actionToReturn.Responsible.Email))
            {
                var actionIssue = await _context.Issues.Where(x => x.Id == actionToReturn.IssueId).FirstOrDefaultAsync(); //get the issue that action corresponding to
                var projectSchedule = await _pacsContext.ProjectSchedules.Where(x => x.Id == actionIssue.Project).FirstOrDefaultAsync(); // get the project schedule the issue corresponding to
                 if(projectSchedule != null && projectSchedule.IsLine)
                 {
                     Email email = new Email();
                     email.ToRecipientList.Add(actionToReturn.Responsible.Email);
                     email.Subject = "Urgent New Action Created for GI Express Project" + " " + actionToReturn.Issue.Project;                            //send URGENT email
                     email.mailBody = "Urgent new action created for GI Express project, Please respond within 2 hours." 
                     + "\nIssue No: " + actionToReturn.Issue.IssueNo 
                     + "\nhttp://issuelog.greenlightinnovation.com/issue/" + actionToReturn.Issue.IssueNo
                     + "\nProject No: " + actionToReturn.Issue.ProjectNo + " " + actionToReturn.Issue.Project
                     + "\n" + "Action Notes:\n" + actionToReturn.ActionNotes;
                     await _emailService.SendEmail(email);
                 }
                 else
                 {
                     Email email = new Email();
                     email.ToRecipientList.Add(actionToReturn.Responsible.Email);
                     email.Subject = "New Action Created" + " " + actionToReturn.Issue.Project;                            //send regular email
                     email.mailBody = "Issue No: " + actionToReturn.Issue.IssueNo +
                     "\nhttp://issuelog.greenlightinnovation.com/issue/" + actionToReturn.Issue.IssueNo
                     + "\nProject No: " + actionToReturn.Issue.ProjectNo + " " + actionToReturn.Issue.Project
                     + "\n" + "Action Notes:\n" + actionToReturn.ActionNotes;
                     await _emailService.SendEmail(email);
                 } 
            }
            if (requreSendSubscriber)
            {
                var subscribers = await _context.Subscribers.Where(x => x.IssueId == actionInDb.IssueId).Include(e => e.Employee).ToListAsync();
                if (subscribers.Count > 0)
                {
                    List<string> toRecipientList = new List<string>();
                    foreach (var item in subscribers)
                    {
                        toRecipientList.Add(item.Employee.Email);
                    }
                    var issue = await GetIssueById(actionInDb.IssueId);
                    if (toRecipientList.Count > 0)
                    {
                        await _emailService.SendEmail(issue, toRecipientList, "Subscriber - Action Status Change Notification");
                    }
                }
            }
            return actionToReturn;
        }

        public async Task<List<IssueAction>> GetActions(int issueId)
        {
            var actions = await _context.IssueActions.Where(x => x.IssueId == issueId).ToListAsync();
            return actions;
        }

        public async Task<List<IssueAction>> GetAllActions()
        {
            var actions = await _context.IssueActions.Include(x => x.Responsible).OrderByDescending(x => x.IssueId).ToListAsync();
            return actions;
        }

        public async Task<int> DeleteAction(IssueAction action)
        {
            var actionInDb = await _context.IssueActions.Where(x => x.Id == action.Id).Include(x => x.Issue).FirstOrDefaultAsync();
            if (actionInDb == null)
            {
                return 0;
            }

            var IssueInSchedule = await _context.IssueSchedule.Where(x => x.IssueId == actionInDb.Issue.Id).FirstOrDefaultAsync(); // update IssueSchedule table when delete action.
                if(IssueInSchedule != null)
                {
                    List<int> actionList = IssueInSchedule.ActionIds.ToList();
                    if(actionList.Contains(action.Id))
                    {
                        actionList.Remove(action.Id);
                    }
                    var actionArr = actionList.ToArray();
                    IssueInSchedule.ActionIds = actionArr;
                    _context.Entry(IssueInSchedule).CurrentValues.SetValues(IssueInSchedule);
                    await _context.SaveChangesAsync();
                } 

            _context.IssueActions.Remove(actionInDb);
            await _context.SaveChangesAsync();
            return 1;
        }

        public async Task<List<IssueAction>> GetActionsByMonth(int month)
        {
            var actions = await _context.IssueActions.Where(x=>x.CreatedOn >= DateTime.Today.AddMonths(-month)).Include(x=>x.Issue).Include(x=>x.Responsible).OrderBy(x=>x.CreatedOn).ToListAsync();
            return actions;
        }
        /**
        * The funciton call the corresponding template function to find the action owner
        */
        public async Task<ActionTemplateResult> GetActionOwner (ActionOwnerCondition condition)
        {   
            ActionTemplateResult result = new ActionTemplateResult();
            String template = condition.template;
            Issue issue = condition.issue;

            switch(template)
            {
             case "MaterialChange":
                result = await MaterialTemplate(issue);
                break;
            
            case "MechChange":
                result = await MechChangeTemplate(issue);
                break;

            case "ElecChange":
                result = await ElecChangeTemplate(issue);
                break;

            case "TestEng":
                result = await TestEngTemplate(issue);
                break;
            
            case "CSS":
                result = await CSSTemplate(issue);
                break;

            default:
                result = null;
                break;
            }

            return result;
        }

        private async Task<ActionTemplateResult> MaterialTemplate(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
            string actionTemplate = "";

            if(actionTemplateDict.ContainsKey("MaterialChange")){
                actionTemplate = actionTemplateDict["MaterialChange"];
            }

            queryCondition.Department = "MAT";
            queryCondition.Facility = null;
            queryCondition.IsLine = false;
            queryCondition.Type = null;

            Task<List<ActionOwner>> actionowners = SearchActionOwnerTabe(queryCondition);
            
            foreach( ActionOwner owner in actionowners.Result){
                Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                if(employee != null)
                {
                    employees.Add(employee);
                }
            }

            result.actionOwner = employees;
            result.actionTemplate = actionTemplate;
            result.message = "success";
            
            return result;
        }

        private async Task<ActionTemplateResult> MechChangeTemplate(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
            string actionTemplate = "";

            if(actionTemplateDict.ContainsKey("MechChange")){
                actionTemplate = actionTemplateDict["MechChange"];
            }

            ProjectSchedule projSch = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync();

            if(projSch == null || projSch.Status ==null || StatusToDeptDict.ContainsKey(projSch.Status)==false){ result.message = "projsch/status Not in DB";}
            else
            {
                if(StatusToDeptDict[projSch.Status] == "SIT")
                {
                    Employee employee = await _context.Employees.Where(x => x.Id == projSch.TestEng).FirstOrDefaultAsync();
                    if(employee == null){ result.message = "TSTENG not found in DB";}
                    else{
                        employees.Add(employee);
                        result.actionOwner = employees;
                        result.actionTemplate = actionTemplate;
                        result.message = "success";
                    }
                }else
                {
                    DateTime? mfg_end = projSch.MfgEndDate;
                    DateTime? sit_start = projSch.SitStartDate;

                    if(StatusToDeptDict[projSch.Status] == "MAT" || StatusToDeptDict[projSch.Status] == "MFG" )
                    {
                        if(mfg_end != null && sit_start != null)
                        {
                            if(DateTime.Now >= mfg_end && DateTime.Now <= sit_start)
                            {
                                queryCondition.Department = "SIT";
                                queryCondition.IsLine = projSch.IsLine;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = null;
                            }else
                            {
                                queryCondition.Department = "MFG";
                                queryCondition.IsLine = projSch.IsLine;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = "Mechanical";
                            }
                        }else
                        {
                            queryCondition.Department = "MFG";
                            queryCondition.IsLine = projSch.IsLine;
                            queryCondition.Facility = projSch.Facility;
                            queryCondition.Type = "Mechanical";
                        }
                    }

                    if(StatusToDeptDict[projSch.Status] == "CSS")
                    {
                        queryCondition.Department = "CSS";
                        queryCondition.Facility = null;
                        queryCondition.IsLine = false;
                        queryCondition.Type = null;
                    }

                    Task<List<ActionOwner>> actionowners = SearchActionOwnerTabe(queryCondition);

                    if(actionowners.Result.Count() == 0){result.message = "Find nothing in ActionOwner Table";}
                    else
                    {
                        foreach( ActionOwner owner in actionowners.Result)
                        {
                            Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                            employees.Add(employee);
                        }

                        result.actionOwner = employees;
                        result.actionTemplate = actionTemplate;
                        result.message = "success";
                    }
                }
            }
            return result;
        }

        private async Task<ActionTemplateResult> ElecChangeTemplate(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
            string actionTemplate = "";

            if(actionTemplateDict.ContainsKey("ElecChange")){
                actionTemplate = actionTemplateDict["ElecChange"];
            }

            ProjectSchedule projSch = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync();

            if(projSch == null || projSch.Status ==null || StatusToDeptDict.ContainsKey(projSch.Status)==false){ result.message = "projsch/status Not in DB";}
            else
            {
                if(StatusToDeptDict[projSch.Status] == "SIT")
                {
                    Employee employee = await _context.Employees.Where(x => x.Id == projSch.TestEng).FirstOrDefaultAsync();
                    if(employee == null){ result.message = "TSTENG not found in DB";}
                    else{
                        employees.Add(employee);
                        result.actionOwner = employees;
                        result.actionTemplate = actionTemplate;
                        result.message = "success";
                    }
                }else
                {
                    DateTime? mfg_end = projSch.MfgEndDate;
                    DateTime? sit_start = projSch.SitStartDate;

                    if(StatusToDeptDict[projSch.Status] == "MAT" || StatusToDeptDict[projSch.Status] == "MFG" )
                    {
                        if(mfg_end != null && sit_start != null)
                        {
                            if(DateTime.Now >= mfg_end && DateTime.Now <= sit_start)
                            {
                                queryCondition.Department = "SIT";
                                queryCondition.IsLine = projSch.IsLine;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = null;
                            }else
                            {
                                queryCondition.Department = "MFG";
                                queryCondition.IsLine = projSch.IsLine;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = "Electrical";
                            }
                        }else
                        {
                            queryCondition.Department = "MFG";
                            queryCondition.IsLine = projSch.IsLine;
                            queryCondition.Facility = projSch.Facility;
                            queryCondition.Type = "Electrical";
                        }
                    }

                    if(StatusToDeptDict[projSch.Status] == "CSS")
                    {
                        queryCondition.Department = "CSS";
                        queryCondition.Facility = null;
                        queryCondition.IsLine = false;
                        queryCondition.Type = null;
                    }

                    Task<List<ActionOwner>> actionowners = SearchActionOwnerTabe(queryCondition);

                    if(actionowners.Result.Count() == 0){result.message = "Find nothing in ActionOwner Table";}
                    else
                    {
                        foreach( ActionOwner owner in actionowners.Result)
                        {
                            Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                            employees.Add(employee);
                        }

                        result.actionOwner = employees;
                        result.actionTemplate = actionTemplate;
                        result.message = "success";
                    }
                }
            }
            return result;
        }


        private async Task<ActionTemplateResult> TestEngTemplate(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
            string actionTemplate = "";

            if(actionTemplateDict.ContainsKey("TestEng")){
                actionTemplate = actionTemplateDict["TestEng"];
            }

            ProjectSchedule projSch = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync();

            if(projSch == null ||projSch.Status ==null || StatusToDeptDict.ContainsKey(projSch.Status)==false){ result.message = "projsch/status Not in DB";}
            else
            {
                if(StatusToDeptDict[projSch.Status] == "SIT")
                {
                    Employee employee = await _context.Employees.Where(x => x.Id == projSch.TestEng).FirstOrDefaultAsync();
                    if(employee == null){ result.message = "TSTENG not found in DB";}
                    else{
                        employees.Add(employee);
                        result.actionOwner = employees;
                        result.actionTemplate = actionTemplate;
                        result.message = "success";
                    }
                }else
                {
                    queryCondition.Department = "SIT";
                    queryCondition.IsLine = projSch.IsLine;
                    queryCondition.Facility = projSch.Facility;
                    queryCondition.Type = null;

                    Task<List<ActionOwner>> actionowners = SearchActionOwnerTabe(queryCondition);

                    if(actionowners.Result.Count() == 0){result.message = "Can't find result in Action Owner Table";}
                    else
                    {
                        foreach( ActionOwner owner in actionowners.Result)
                        {
                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                        employees.Add(employee);
                        }
                        
                        result.actionOwner = employees;
                        result.actionTemplate = actionTemplate;
                        result.message = "success";
                    }
                }
            }
            return result;
        }

        private async Task<ActionTemplateResult> CSSTemplate(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
            string actionTemplate = "";

            if(actionTemplateDict.ContainsKey("CSS")){
                actionTemplate = actionTemplateDict["CSS"];
            }

            queryCondition.Department = "CSS";
            queryCondition.Facility = null;
            queryCondition.IsLine = false;
            queryCondition.Type = null;

            Task<List<ActionOwner>> actionowners = SearchActionOwnerTabe(queryCondition);
            
            if(actionowners.Result.Count == 0 ){
                result.actionOwner = null;
                result.message = "Can't find result in Action Owner Table";
            }

            foreach( ActionOwner owner in actionowners.Result){
                Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                if(employee != null)
                {
                    employees.Add(employee);
                }
            }

            if(employees.Count() != 0){
                result.actionOwner = employees;
                result.actionTemplate = actionTemplate;
                result.message = "success";
            }
            return result;
        }


        /**
        * The function query the ActionOwner Table to get the action owner by using a query condition
        */
        private async Task<List<ActionOwner>> SearchActionOwnerTabe(ActionOwner querycondition)
        {  
            List<ActionOwner> actionOwners = new List<ActionOwner>();
            IQueryable<ActionOwner> query = _pacsContext.ActionOwners;  

            if(querycondition == null)
            {
                actionOwners = null;
            }else
            {
                query = query.Where(x => x.Department == querycondition.Department);

                if(query.ToList().Count() != 1 && querycondition.Facility != null &&  querycondition.Facility != "") 
                {
                    query = query.Where(x => x.Facility == querycondition.Facility);
                }

                if(query.ToList().Count() != 1 && querycondition.Type != null && querycondition.Type != "")
                {
                     query = query.Where(x => x.Type == querycondition.Type);
                }

                if(query.ToList().Count() != 1)
                {
                     query = query.Where(x => x.IsLine == querycondition.IsLine);
                }

                actionOwners = await query.AsNoTracking().ToListAsync();
            }
            return actionOwners;
        }

        public async Task<ActionTemplateResult> GetEcDrawingOwner (ActionOwnerCondition condition)
        {   
            ActionTemplateResult result = new ActionTemplateResult();
            String template = condition.template;
            Issue issue = condition.issue;

            switch(template)
            {
             case "Mech":
                result = await MechDrawing(issue);
                break;
            
             case "Elec":
                result = await ElecDrawing(issue);
                break;
            
             case "MechComplete":
                result = await MechDrawingComplete(issue);
                break;
            
             case "ElecComplete":
                result = await ElecDrawingComplete(issue);
                break;
            

            default:
                result = null;
                break;
            }

            return result;
        }

        private async Task<ActionTemplateResult> MechDrawing(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionOwner queryCondition_next = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
            List<Employee> employees_next = new List<Employee>();

            ProjectSchedule projSch = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync();

            if(projSch == null || projSch.Status ==null || StatusToDeptDict.ContainsKey(projSch.Status)==false){ result.message = "projsch/status Not in DB";}
            else
            {
                if(StatusToDeptDict[projSch.Status] == "CSS")
                {
                    result.actionOwner = null;
                    result.actionTemplate = "DDE to upload the drawings to issue log";
                    result.message = "success";
                }else
                {
                    DateTime? mfg_start = projSch.MfgStartDate;
                    DateTime? mfg_end = projSch.MfgEndDate;

                    if(mfg_start == null || mfg_end == null){result.message = "MFG start or end date Unknown, check DB";}
                    else
                    {
                        if(DateTime.Today.AddDays(-1) < mfg_start || StatusToDeptDict[projSch.Status] == "ENG")
                        {
                            //first
                            if(projSch.IsLine)
                            {
                                queryCondition.Department = "MSE";
                                queryCondition.IsLine = true;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = "Mechanical";

                                Task<List<ActionOwner>> actionowners =  SearchActionOwnerTabe(queryCondition);
                                if(actionowners.Result.Count() == 0){result.message = "Find nothing in ActionOwner Table";}
                                else
                                {
                                    foreach( ActionOwner owner in actionowners.Result)
                                    {
                                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                        employees.Add(employee);
                                    }
                                    if(employees.Count() != 0)
                                    {
                                        result.actionOwner = employees;
                                        result.actionTemplate = "DDE to upload the drawings to issue log or deliver the hard copy drawings to " +
                                                                employees[0].FirstName + " " + employees[0].LastName + Environment.NewLine +
                                                                employees[0].FirstName + " " + employees[0].LastName + " to print and put EC drawings in the project drawing package";
                                        result.message = "success";
                                    }
                                    else{result.message = "Failed when transfering action owner to employee";}
                                }
                            }else
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "DDE to deliver the drawings to EL DWG cabinet";
                                result.message = "success";
                            }
                        }
                        else if(DateTime.Today > mfg_end && (StatusToDeptDict[projSch.Status] == "MFG" || StatusToDeptDict[projSch.Status] == "SIT"))
                        {
                            //third
                            if(projSch.Facility == "EL-001")
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "DDE to deliver the drawings to the cabinet near the SIT cage";
                                result.message = "success";
                            }else if(projSch.Facility == "BR-104")
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "DDE to deliver the drawing to the black cabinet near the front door";
                                result.message = "success";
                            }else if(projSch.Facility == "BR-110"){
                                result.actionOwner = null;
                                result.actionTemplate = "DDE to deliver the drawing to the black cabinet near the front door";
                                result.message = "success";
                            }
                            else{
                                result.message = "Station Location Missing or Invlid in DB.";
                            }
                        }
                        else
                        {
                            //second
                            if(projSch.IsLine)
                            {
                                queryCondition.Department = "MSE";
                                queryCondition.IsLine = true;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = "Mechanical";

                                queryCondition_next.Department = "MFG";
                                queryCondition_next.IsLine = true;
                                queryCondition_next.Facility = projSch.Facility;
                                queryCondition_next.Type = "Mechanical";

                                List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                                List<ActionOwner> actionowners_next = await SearchActionOwnerTabe(queryCondition_next);
                                if(actionowners.Count() == 0 || actionowners_next.Count() ==0 ){result.message = "Find nothing in ActionOwner Table";}
                                else
                                {
                                    foreach( ActionOwner owner in actionowners)
                                    {
                                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                        employees.Add(employee);
                                    }
                                    foreach( ActionOwner owner in actionowners_next)
                                    {
                                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                        employees_next.Add(employee);
                                    }
                                    if(employees.Count() != 0 || employees_next.Count() != 0 )
                                    {
                                        result.actionOwner = employees;
                                        result.actionTemplate = "DDE to upload the drawings to issue log or deliver the hard copy drawings to " +
                                                                employees[0].FirstName + " " + employees[0].LastName + Environment.NewLine +
                                                                employees[0].FirstName + " " + employees[0].LastName + " to deliver EC drawing to " +  employees_next[0].FirstName + " " + employees_next[0].LastName ;
                                        result.message = "success";
                                    }
                                    else{result.message = "Failed when transfering action owner to employee";}
                                }
                            }else
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "DDE to deliver the drawing to the EL DWG cabinet";
                                result.message = "success";
                            }
                        }   
                    }

                }


            }
            return result;
        }

        private async Task<ActionTemplateResult> ElecDrawing(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionOwner queryCondition_next = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
            List<Employee> employees_next = new List<Employee>();

            ProjectSchedule projSch = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync();

            if(projSch == null | projSch.Status ==null || StatusToDeptDict.ContainsKey(projSch.Status)==false){ result.message = "projsch/status Not in DB";}
            else
            {
                if(StatusToDeptDict[projSch.Status] == "CSS")
                {
                    result.actionOwner = null;
                    result.actionTemplate = "EE to upload the drawings to issue log";
                    result.message = "success";
                }else
                {
                    DateTime? mfg_start = projSch.MfgStartDate;
                    DateTime? mfg_end = projSch.MfgEndDate;

                    if(mfg_start == null || mfg_end == null){result.message = "MFG start or end date Unknown, check DB";}
                    else
                    {
                        if(DateTime.Today.AddDays(-1) < mfg_start || StatusToDeptDict[projSch.Status] == "ENG")
                        {
                            //first
                            if(projSch.IsLine)
                            {
                                queryCondition.Department = "MSE";
                                queryCondition.IsLine = true;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = "Electrical";

                                Task<List<ActionOwner>> actionowners =  SearchActionOwnerTabe(queryCondition);
                                if(actionowners.Result.Count() == 0){result.message = "Find nothing in ActionOwner Table";}
                                else
                                {
                                    foreach( ActionOwner owner in actionowners.Result)
                                    {
                                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                        employees.Add(employee);
                                    }
                                    if(employees.Count() != 0)
                                    {
                                        result.actionOwner = employees;
                                        result.actionTemplate = "EE to deliver the hard copy drawings to " +
                                                                employees[0].FirstName + " " + employees[0].LastName + Environment.NewLine +
                                                                employees[0].FirstName + " " + employees[0].LastName + " to put EC drawings in the project drawing package";
                                        result.message = "success";
                                    }
                                    else{result.message = "Failed when transfering action owner to employee";}
                                }
                            }else
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "EE to deliver the drawing to EL DWG cabinet";
                                result.message = "success";
                            }
                        }
                        else if(DateTime.Today > mfg_end && (StatusToDeptDict[projSch.Status] == "MFG" || StatusToDeptDict[projSch.Status] == "SIT"))
                        {
                            //third
                            if(projSch.Facility == "EL-001")
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "EE to deliver the drawing to the cabinet near the SIT cage";
                                result.message = "success";
                            }else if(projSch.Facility == "BR-104")
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "EE to deliver the drawing to the black cabinet near the front door";
                                result.message = "success";
                            }else if(projSch.Facility == "BR-110"){
                                result.actionOwner = null;
                                result.actionTemplate = "EE to deliver the drawing to the black cabinet near the front door";
                                result.message = "success";
                            }
                            else{
                                result.message = "Station Location Missing or Invlid in DB.";
                            }
                        }
                        else
                        {
                            //second
                            if(projSch.IsLine)
                            {
                                queryCondition.Department = "MSE";
                                queryCondition.IsLine = true;
                                queryCondition.Facility = projSch.Facility;
                                queryCondition.Type = "Electrical";

                                queryCondition_next.Department = "MFG";
                                queryCondition_next.IsLine = true;
                                queryCondition_next.Facility = projSch.Facility;
                                queryCondition_next.Type = "Electrical";

                                List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                                List<ActionOwner> actionowners_next = await SearchActionOwnerTabe(queryCondition_next);
                                if(actionowners.Count() == 0 || actionowners_next.Count() ==0 ){result.message = "Find nothing in ActionOwner Table";}
                                else
                                {
                                    foreach( ActionOwner owner in actionowners)
                                    {
                                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                        employees.Add(employee);
                                    }
                                    foreach( ActionOwner owner in actionowners_next)
                                    {
                                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                        employees_next.Add(employee);
                                    }
                                    if(employees.Count() != 0 || employees_next.Count() != 0 )
                                    {
                                        result.actionOwner = employees;
                                        result.actionTemplate = "EE deliver the hard copy drawings to " +
                                                                employees[0].FirstName + " " + employees[0].LastName + Environment.NewLine +
                                                                employees[0].FirstName + " " + employees[0].LastName + " to deliver EC drawing to " +  employees_next[0].FirstName + " " + employees_next[0].LastName ;
                                        result.message = "success";
                                    }
                                    else{result.message = "Failed when transfering action owner to employee";}
                                }
                            }else
                            {
                                result.actionOwner = null;
                                result.actionTemplate = "EE to deliver the drawing to the EL DWG cabinet";
                                result.message = "success";
                            }
                        }   
                    }

                }


            }
            return result;
        }

        private async Task<ActionTemplateResult> MechDrawingComplete(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
           
            ProjectSchedule projSch = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync();

            if(projSch == null || projSch.Status ==null || StatusToDeptDict.ContainsKey(projSch.Status)==false){ result.message = "projsch/status Not in DB";}
            else
            {

                if(StatusToDeptDict[projSch.Status] == "SIT")
                {
                    Employee employee = await _context.Employees.Where(x => x.Id == projSch.TestEng).FirstOrDefaultAsync();
                    if(employee == null){ result.message = "TSTENG not found in DB";}
                    else{
                        employees.Add(employee);
                        result.actionOwner = employees;
                        result.actionTemplate = "EC drawing has been delivered";
                        result.message = "success_completion";
                    }
                }else if(StatusToDeptDict[projSch.Status] == "CSS")
                {
                    queryCondition.Department = "CSS";
                    queryCondition.Facility = null;
                    queryCondition.IsLine = false;
                    queryCondition.Type = null;

                    List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                    if(actionowners.Count == 0 ){
                        result.actionOwner = null;
                        result.message = "Can't find CSS Owner in Action Owner Table";
                    }
                    foreach( ActionOwner owner in actionowners){
                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                        if(employee != null)
                        {
                            employees.Add(employee);
                        }
                    }
                    if(employees.Count() != 0){
                        result.actionOwner = employees;
                        result.actionTemplate = "EC drawing has been delivered";
                        result.message = "success_completion";
                    }
                }else
                {
                    DateTime? mfg_end = projSch.MfgEndDate;
                    DateTime? sit_start = projSch.SitStartDate;

                    if(mfg_end == null || sit_start == null){result.message = "MFG or SIT date not available in DB";}
                    else
                    {
                        //PRE SIT
                        if(DateTime.Today>mfg_end && DateTime.Today < sit_start)
                        {
                            queryCondition.Department = "SIT";
                            queryCondition.Facility = projSch.Facility;
                            queryCondition.IsLine = projSch.IsLine;
                            queryCondition.Type = "Mechanical";

                            List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                            if(actionowners.Count == 0 ){
                                result.actionOwner = null;
                                result.message = "Can't find CSS Owner in Action Owner Table";
                            }
                            foreach( ActionOwner owner in actionowners){
                                Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                if(employee != null)
                                {
                                    employees.Add(employee);
                                }
                            }
                            if(employees.Count() != 0){
                                result.actionOwner = employees;
                                result.actionTemplate = "EC drawing has been delivered";
                                result.message = "success_completion";
                            }                            
                        }else
                        {
                            //Pre-MFG
                            queryCondition.Department = "MFG";
                            queryCondition.Facility = projSch.Facility;
                            queryCondition.IsLine = projSch.IsLine;
                            queryCondition.Type = "Mechanical";

                            List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                            if(actionowners.Count == 0 ){
                                result.actionOwner = null;
                                result.message = "Can't find CSS Owner in Action Owner Table";
                            }
                            foreach( ActionOwner owner in actionowners){
                                Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                if(employee != null)
                                {
                                    employees.Add(employee);
                                }
                            }
                            if(employees.Count() != 0){
                                result.actionOwner = employees;
                                result.actionTemplate = "EC drawing has been delivered";
                                result.message = "success_completion";
                            }  
                        }
                    }   
                }
            }
            return result;
        }

        private async Task<ActionTemplateResult> ElecDrawingComplete(Issue issue)
        {
            ActionOwner queryCondition = new ActionOwner();
            ActionTemplateResult result = new ActionTemplateResult();
            List<Employee> employees = new List<Employee>();
           
            ProjectSchedule projSch = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync();

            if(projSch == null || projSch.Status ==null || StatusToDeptDict.ContainsKey(projSch.Status)==false){ result.message = "projsch/status Not in DB";}
            else
            {

                if(StatusToDeptDict[projSch.Status] == "SIT")
                {
                    Employee employee = await _context.Employees.Where(x => x.Id == projSch.TestEng).FirstOrDefaultAsync();
                    if(employee == null){ result.message = "TSTENG not found in DB";}
                    else{
                        employees.Add(employee);
                        result.actionOwner = employees;
                        result.actionTemplate = "EC drawing has been delivered";
                        result.message = "success_completion";
                    }
                }else if(StatusToDeptDict[projSch.Status] == "CSS")
                {
                    queryCondition.Department = "CSS";
                    queryCondition.Facility = null;
                    queryCondition.IsLine = false;
                    queryCondition.Type = null;

                    List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                    if(actionowners.Count == 0 ){
                        result.actionOwner = null;
                        result.message = "Can't find CSS Owner in Action Owner Table";
                    }
                    foreach( ActionOwner owner in actionowners){
                        Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                        if(employee != null)
                        {
                            employees.Add(employee);
                        }
                    }
                    if(employees.Count() != 0){
                        result.actionOwner = employees;
                        result.actionTemplate = "EC drawing has been delivered";
                        result.message = "success_completion";
                    }
                }else
                {
                    DateTime? mfg_end = projSch.MfgEndDate;
                    DateTime? sit_start = projSch.SitStartDate;

                    if(mfg_end == null || sit_start == null){result.message = "MFG or SIT date not available in DB";}
                    else
                    {
                        //PRE SIT
                        if(DateTime.Today>mfg_end && DateTime.Today < sit_start)
                        {
                            queryCondition.Department = "SIT";
                            queryCondition.Facility = projSch.Facility;
                            queryCondition.IsLine = projSch.IsLine;
                            queryCondition.Type = "Electrical";

                            List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                            if(actionowners.Count == 0 ){
                                result.actionOwner = null;
                                result.message = "Can't find CSS Owner in Action Owner Table";
                            }
                            foreach( ActionOwner owner in actionowners){
                                Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                if(employee != null)
                                {
                                    employees.Add(employee);
                                }
                            }
                            if(employees.Count() != 0){
                                result.actionOwner = employees;
                                result.actionTemplate = "EC drawing has been delivered";
                                result.message = "success_completion";
                            }                            
                        }else
                        {
                            //Pre-MFG
                            queryCondition.Department = "MFG";
                            queryCondition.Facility = projSch.Facility;
                            queryCondition.IsLine = projSch.IsLine;
                            queryCondition.Type = "Electrical";

                            List<ActionOwner> actionowners =  await SearchActionOwnerTabe(queryCondition);
                            if(actionowners.Count == 0 ){
                                result.actionOwner = null;
                                result.message = "Can't find CSS Owner in Action Owner Table";
                            }
                            foreach( ActionOwner owner in actionowners){
                                Employee employee = await _context.Employees.Where(x => x.Id == owner.EmployeeId).FirstOrDefaultAsync();
                                if(employee != null)
                                {
                                    employees.Add(employee);
                                }
                            }
                            if(employees.Count() != 0){
                                result.actionOwner = employees;
                                result.actionTemplate = "EC drawing has been delivered";
                                result.message = "success_completion";
                            }  
                        }
                    }   
                }
            }
            return result;
        }
    
    }
}