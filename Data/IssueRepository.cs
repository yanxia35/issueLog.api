using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace IssueLog.API.Data
{
    public class IssueRepository : IIssueRepository
    {
        private readonly DataContext _context;
        private readonly PacsContext _pacsContext;
        private readonly IEmailService _emailService;

        public IssueRepository(DataContext context, PacsContext pacsContext, IEmailService emailService)
        {
            _emailService = emailService;
            _context = context;
            _pacsContext = pacsContext;
        }

        public async Task<Issue> AddIssue(Issue newIssue)
        {
            newIssue = ValidateNewIssue(newIssue);
            var issueSaved = await _context.Issues.AddAsync(newIssue);
            await _context.SaveChangesAsync();
            var issue = await GetIssueById(newIssue.Id);
            var projectSchedule = await _pacsContext.ProjectSchedules.Where(x => x.Id == issue.Project).FirstOrDefaultAsync(); //

            if(issue!=null)
            {
                var newIssueSchedule = GenerateSchedule(issue,projectSchedule);
                var IssueScheduleSaved = await _context.IssueSchedule.AddAsync(newIssueSchedule);                               // Add Issue to IssueSchedule Table when add new issue
                await _context.SaveChangesAsync();
            }

            if(projectSchedule != null && projectSchedule.IsLine){                                                               // When adding new issues, If the issue is 
                await _emailService.SendEmail_line(issue, "Urgent new issue created for GI Express project ");                   // a line-ready project, send a different 
            }else{                                                                                                               // email to notify the issue owner.
                await _emailService.SendEmail(issue, "New Issue Created");                                                       //
            }                                                                                                                    //
            return issue;
        }

        private Issue ValidateNewIssue(Issue newIssue)
        {
            newIssue.Id = 0;
            newIssue.ProjectNo = newIssue.ProjectNo.ToUpper();
            newIssue.CreatedOn = DateTime.Now;
            newIssue.RootCauseProcessId = 27;
            newIssue.FailureModeId = 19;
            newIssue.IssueNo = GetIssueNumber();
            return newIssue;
        }

        /**
        * The function generate a IssueSchedule based on the New Issue created
        *
        */
        private IssueSchedule GenerateSchedule(Issue newIssue, ProjectSchedule projectSchedule)
        {
            var newIssueSchedule = new IssueSchedule();
            
            newIssueSchedule.IssueId = newIssue.Id;
            newIssueSchedule.CreatedOn = newIssue.CreatedOn;
            newIssueSchedule.IssueOwnerId = newIssue.IssueOwnerId;
            newIssueSchedule.ActionIds = new int[] {};
            newIssueSchedule.RespondedAt = null;
            newIssueSchedule.Late = false;
            newIssueSchedule.ReminderSent = false;
            newIssueSchedule.IssueStatus = newIssue.IssueStatus;

            if(projectSchedule != null && projectSchedule.IsLine)
            {
                newIssueSchedule.DueDate = newIssue.CreatedOn.AddHours(2);
                newIssueSchedule.IsLine = true;
            }else
            {
                newIssueSchedule.DueDate = newIssue.CreatedOn.AddDays(1);
                newIssueSchedule.IsLine = false;
            }
            
            return newIssueSchedule;
        }

        /**
        * The function modify the IssueSchedule based on the the Issue modified
        *
        */
        private IssueSchedule ModifySchedule(Issue modifiedIssue, ProjectSchedule projectSchedule, IssueSchedule issueScheduleInDb)
        {
            var newIssueSchedule = new IssueSchedule();
            
            newIssueSchedule.IssueId = modifiedIssue.Id;
            newIssueSchedule.CreatedOn = modifiedIssue.CreatedOn;
            newIssueSchedule.IssueOwnerId = modifiedIssue.IssueOwnerId;
            newIssueSchedule.IssueStatus = modifiedIssue.IssueStatus;

            newIssueSchedule.ActionIds = issueScheduleInDb.ActionIds;
            newIssueSchedule.RespondedAt = issueScheduleInDb.RespondedAt;
            newIssueSchedule.Late = issueScheduleInDb.Late;
            newIssueSchedule.ReminderSent = issueScheduleInDb.ReminderSent;
            
            if(projectSchedule != null && projectSchedule.IsLine)
            {
                newIssueSchedule.DueDate = modifiedIssue.CreatedOn.AddHours(2);
                newIssueSchedule.IsLine = true;
            }else
            {
                newIssueSchedule.DueDate = modifiedIssue.CreatedOn.AddDays(1);
                newIssueSchedule.IsLine = false;
            }
            
            return newIssueSchedule;
        }


        private string GetIssueNumber()
        {
            string issueNo;
            int number;
            var lastIssue = _context.Issues.OrderByDescending(x => x.IssueNo).FirstOrDefault();
            if (lastIssue?.IssueNo != null)
            {
                if (lastIssue.IssueNo.Substring(0, 2) != DateTime.Now.Year.ToString().Substring(2, 2))
                {
                    issueNo = DateTime.Now.Year.ToString().Substring(2, 2) + "-0001";
                }
                else if (Int32.TryParse(lastIssue.IssueNo.Substring(3, 4), out number))
                {
                    number = number + 1;
                    issueNo = lastIssue.IssueNo.Substring(0, 3) + number.ToString().PadLeft(4, '0');
                }
                else
                {
                    issueNo = DateTime.Now.Year.ToString().Substring(2, 2) + "-0001";
                }
            }
            else
            {
                issueNo = DateTime.Now.Year.ToString().Substring(2, 2) + "-0001";
            }
            return issueNo;
        }
        public async Task<Issue> GetIssue(string issueNo)
        {
            var issue = await _context.Issues.Where(x => x.IssueNo == issueNo).Include(x => x.Originator)
            .Include(x => x.IssueOwner)
            .Include(x => x.IssueActions).ThenInclude(x => x.CreatedBy)
            .Include(x => x.IssueActions).ThenInclude(x => x.Responsible)
            .Include(x => x.SubProjectStatuses)
            .Include(e => e.PartIssues).Include(x => x.Subscribers).Include(x => x.EcParts).AsNoTracking().FirstOrDefaultAsync();
            return issue;
        }

        public async Task<Issue> SaveEdittedIssue(Issue issue)
        {

            //check data, fix information, adding dates
            issue.ProjectNo = issue.ProjectNo.ToUpper();
            bool requireSend = false;
            bool requireSendSubscriber = false;
            var issueInDb = await _context.Issues.Where(x => x.Id == issue.Id).FirstOrDefaultAsync();
            var issueScheduleInDb = await _context.IssueSchedule.Where(x => x.IssueId == issue.Id).FirstOrDefaultAsync();
            if (issueInDb == null)
            {
                return null;
            }
            if (issueInDb.IssueOwnerId != issue.IssueOwnerId)
            {
                var newOwner = await _context.Employees.Where(x => x.Id == issue.IssueOwnerId).FirstOrDefaultAsync();
                var oldOwner = await _context.Employees.Where(x => x.Id == issueInDb.IssueOwnerId).FirstOrDefaultAsync();
                if (oldOwner != null && newOwner != null)
                {
                    issue.IssueNotes = oldOwner.ShortName + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm tt") + "\nChanged ownership to " + newOwner.ShortName + "\n------------------------------------------------------------\n" + issue.IssueNotes;
                }
                requireSend = true;
            }
            if (issueInDb.IssueStatus != issue.IssueStatus)
            {
                requireSendSubscriber = true;
                if (issue.IssueStatus == "closed" || issue.IssueStatus == "ec")
                {
                    issue.IssueClosedDate = DateTime.Now;
                }
                else
                {
                    issue.IssueClosedDate = null;
                }
            }

            if (issueInDb.IssueNotes != issue.IssueNotes)
            {
                requireSendSubscriber = true;
            }
            _context.Entry(issueInDb).CurrentValues.SetValues(issue);
            await _context.SaveChangesAsync();
            var issueReturn = await GetIssueById(issueInDb.Id);
            var projectSchedule = await _pacsContext.ProjectSchedules.Where(x => x.Id == issueReturn.Project).FirstOrDefaultAsync(); 

            if(issueReturn != null && projectSchedule != null && issueScheduleInDb != null){
                var edittedIssueSchedule = ModifySchedule(issueReturn,projectSchedule,issueScheduleInDb);
                _context.Entry(issueScheduleInDb).CurrentValues.SetValues(edittedIssueSchedule);
                await _context.SaveChangesAsync();
            }

            if (requireSend && issue.IssueOwnerId != null)
            {
                
                if(projectSchedule != null && projectSchedule.IsLine){                                                               
                    await _emailService.SendEmail_line(issueReturn, "Urgent Issue Ownership Transfered for GI Express project ");                 // when issue changed to a line-ready project                  
                }else{                                                                                                          // send a different email
                    await _emailService.SendEmail(issueReturn, "Issue Ownership Transfer");
                }     
            }
            // send email
            if (requireSendSubscriber)
            {
                var subscribers = await _context.Subscribers.Where(x => x.IssueId == issueReturn.Id).Include(e => e.Employee).ToListAsync();
                if (subscribers.Count > 0)
                {
                    List<string> toRecipientList = new List<string>();
                    foreach (var item in subscribers)
                    {
                        toRecipientList.Add(item.Employee.Email);
                    }
                    if (toRecipientList.Count > 0)
                    {
                        await _emailService.SendEmail(issueReturn, toRecipientList, "Subscriber - Issue Notes, Action Status, Issue Status, Change Notification");
                    }
                }
            }
            return issueReturn;
        }

        public async Task<List<Issue>> Search(SearchParaDto searchValues)
        {
            bool searchAction = false;
            IQueryable<Issue> query = _context.Issues;
            IQueryable<IssueAction> queryAction = _context.IssueActions;
            if (searchValues.IssueNo != null && searchValues.IssueNo != "")
            {
                query = query.Where(x => x.IssueNo.Contains(searchValues.IssueNo));
                searchAction = false;
            }
            else
            {
                // issue filtering
                if (searchValues.IssueStatus != null && searchValues.IssueStatus != "" && searchValues.IssueStatus != "all")
                {
                    query = query.Where(x => x.IssueStatus == searchValues.IssueStatus);
                }
                if (searchValues.Originator != "" && searchValues.Originator != null)
                {
                    query = query.Where(x => x.OriginatorId == searchValues.Originator);
                }
                if (searchValues.OwnerDepartment != "" && searchValues.OwnerDepartment != null)
                {
                    query = query.Where(x => x.IssueOwner.GroupId == searchValues.OwnerDepartment);
                }
                else if (searchValues.IssueOwner != "" && searchValues.IssueOwner != null)
                {
                    query = query.Where(x => x.IssueOwnerId == searchValues.IssueOwner);
                }
                // web interface 0 for not to search, 1 for yes, -1 for no
                if (searchValues.IsMissingParts != 0)
                {
                    if (searchValues.IsMissingParts == 1)
                    {

                        query = query.Where(x => x.IsMissingParts == true);
                    }
                    else
                    {
                        query = query.Where(x => x.IsMissingParts == false);
                    }
                }
                // web interface 0 for not to search, 1 for yes, -1 for no
                if (searchValues.IsReady != 0)
                {
                    if (searchValues.IsReady == 1)
                    {
                        query = query.Where(x => x.IsReady == true);
                    }
                    else
                    {
                        query = query.Where(x => x.IsReady == false);
                    }

                }
                if (searchValues.IssueNotes != "" && searchValues.IssueNotes != null)
                {
                    List<string> notes = searchValues.IssueNotes.Split(" ").ToList();
                    foreach (var note in notes)
                    {
                        string searchNote = note.Trim();
                        if (searchNote != "")
                        {
                            if (searchNote.Substring(0, 1) == "!")
                            {
                                query = query.Where(x => !x.IssueNotes.ToLower().Contains(searchNote.ToLower()));
                            }
                            else
                            {
                                query = query.Where(x => x.IssueNotes.ToLower().Contains(searchNote.ToLower()));
                            }
                        }
                    }

                }

                if (searchValues.ProjectNo != "" && searchValues.ProjectNo != null)
                {
                    query = query.Where(x => x.ProjectNo.ToLower().Contains(searchValues.ProjectNo.ToLower()) || x.Project.ToLower().Contains(searchValues.ProjectNo.ToLower()));
                }
                if (searchValues.RootCauseProcess != 0)
                {

                    query = query.Where(x => x.RootCauseProcessId == searchValues.RootCauseProcess);
                }

                if (searchValues.ActionOwnerDept != "" && searchValues.ActionOwnerDept != null)
                {
                    queryAction = queryAction.Where(x => x.Responsible.GroupId == searchValues.ActionOwnerDept);
                    searchAction = true;
                }
                else if (searchValues.ActionOwner != "" && searchValues.ActionOwner != null)
                {
                    queryAction = queryAction.Where(x => x.ResponsibleId == searchValues.ActionOwner);
                    searchAction = true;

                }
                if (searchValues.ActionStatus != null && searchValues.ActionStatus != "" && searchValues.ActionStatus != "all")
                {
                    queryAction = queryAction.Where(x => x.ActionStatus == searchValues.ActionStatus);
                    searchAction = true;
                }

                if (searchValues.IsActionLate != 0)
                {
                    if (searchValues.IsActionLate == 1)
                    {
                        queryAction = queryAction.Where(x => x.DueDate < DateTime.Today && x.ActionStatus == "active");
                        searchAction = true;
                    }
                    else
                    {
                        queryAction = queryAction.Where(x => x.DueDate >= DateTime.Today && x.ActionStatus == "active");
                        searchAction = true;
                    }
                }
                if (searchValues.IsMissingDueDate != 0)
                {
                    if (searchValues.IsMissingDueDate == 1)
                    {
                        queryAction = queryAction.Where(x => x.DueDate == null);
                        searchAction = true;
                    }
                    else
                    {
                        queryAction = queryAction.Where(x => x.DueDate != null);
                        searchAction = true;
                    }

                }
                else
                {

                    if (searchValues.ActionDueDateMoreThan != 0)
                    {
                        queryAction = queryAction.Where(x => x.ActionStatus == "active");
                        queryAction = queryAction.Where(x => x.ActionActiveDate != null);
                        queryAction = queryAction.Where(x => ((TimeSpan)(x.DueDate - x.ActionActiveDate)).Days > searchValues.ActionDueDateMoreThan);
                        searchAction = true;
                    }
                }
                // apply action filter to issue filter

            }
            if (searchAction == true)
            {
                // var actions = await queryAction();
                query = query.Where(x => queryAction.Any(e => e.IssueId == x.Id));
                query = query.Include(x => x.Originator)
                                .Include(x => x.IssueOwner)
                                .Include(x => x.IssueActions).ThenInclude(x => x.CreatedBy)
                                .Include(x => x.Subscribers)
                                .Include(x => x.IssueActions).ThenInclude(x => x.Responsible)
                                .Include(x => x.SubProjectStatuses)
                                .Include(x => x.PartIssues);

                var issues = await query.OrderByDescending(x => x.IssueNo).AsNoTracking()
                // .Include(x => x.EcParts).AsNoTracking()
                .ToListAsync();

                if (issues != null)
                {

                    var idList = issues.Select(x=>x.Id).ToHashSet();
                    var ecparts = await _context.EcParts.Where(x => idList.Contains(x.Id)).ToListAsync();
                    Task taskA = new Task(() =>
                    {
                        foreach (var issue in issues)
                        {
                            issue.EcParts = ecparts.Where(x => x.IssueId == issue.Id).ToList();
                        }
                    });
                    taskA.Start();
                    taskA.Wait();
                }
                return issues;
            }
            else
            {
                query = query.Include(x => x.Originator)
                                .Include(x => x.IssueOwner)
                                .Include(x => x.IssueActions).ThenInclude(x => x.CreatedBy)
                                .Include(x => x.Subscribers)
                                .Include(x => x.IssueActions).ThenInclude(x => x.Responsible)
                                .Include(x => x.SubProjectStatuses)
                                .Include(x => x.PartIssues);
                var issues = await query.OrderByDescending(x => x.IssueNo)
                .Include(x => x.EcParts).AsNoTracking()
                .ToListAsync();
                return issues;
            }

        }

        public async Task<Issue> AddIssueFromOldDb(Issue newIssue)
        {
            var issueSaved = await _context.Issues.AddAsync(newIssue);
            await _context.SaveChangesAsync();

            // var issueReturn = await GetIssueById(newIssue.Id);
            return newIssue;
        }

        public async Task<Issue> GetIssueById(int id)
        {
            var issue = await _context.Issues.Where(x => x.Id == id).Include(x => x.Originator)
                  .Include(x => x.IssueOwner)
                  .Include(x => x.IssueActions).ThenInclude(x => x.CreatedBy)
                  .Include(x => x.IssueActions).ThenInclude(x => x.Responsible)
                  .Include(x => x.PartIssues)
                  .Include(x => x.SubProjectStatuses)
                  //   .Include(x => x.EcParts)
                  .Include(x => x.Subscribers)
                  .FirstOrDefaultAsync();
            issue.SubProjectStatuses.OrderBy(x => x.SubProjectId).ToList();
            return issue;
        }

        public async Task GetNewIssueReport()
        {
            IQueryable<Issue> query = _context.Issues;
            var queryAction = await _context.IssueActions.GroupBy(x => new { x.IssueId }).Select(g => new { g.Key.IssueId }).ToListAsync();
            var issues = await query.Where(x => x.IssueStatus == "active").Where(x => !queryAction.Any(e => e.IssueId == x.Id)).Include(x => x.IssueOwner).ToListAsync();
            var employees = issues.GroupBy(x => new { x.IssueOwner }).Select(g => new { g.Key.IssueOwner }).ToList();
            employees.ForEach(x =>
            {
                var issuesByOwner = issues.Where(e => e.IssueOwnerId == x.IssueOwner.Id).ToList();
                Email email = new Email();
                issuesByOwner.ForEach(e =>
          {
              email.mailBody = email.mailBody + ' ' + e.IssueNo;
          });
            });

        }

        public void GetNewActionReport()
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateIssueProject()
        {
            Regex prjRegex = new Regex("[a-zA-Z]{1}[0-9]{2}[-]{1}[0-9]{3,4}");
            var issues = await _context.Issues.ToListAsync();
            var projects = await _pacsContext.Projects.ToListAsync();
            int count = 0;
            foreach (var issue in issues)
            {
                if (issue.Project == "")
                {
                    if (prjRegex.IsMatch(issue.ProjectNo))
                    {
                        var matches = prjRegex.Matches(issue.ProjectNo);
                        foreach (var item in matches)
                        {
                            if (projects.Any(x => x.Id.Trim() == item.ToString()))
                            {
                                issue.Project = item.ToString();
                                issue.ProjectNo.Replace(issue.Project + ", ", "");
                                issue.ProjectNo.Replace(issue.Project + ",", "");
                                issue.ProjectNo.Replace(issue.Project, "");
                                await _context.SaveChangesAsync();
                                count++;
                                break;
                            }
                        }
                    }

                }
                else
                {
                    if (issue.ProjectNo != null && issue.Project != null)
                    {
                        if (issue.ProjectNo.Contains(issue.Project, StringComparison.OrdinalIgnoreCase))
                        {

                            issue.ProjectNo = Regex.Replace(issue.ProjectNo, issue.Project + ", ", "", RegexOptions.IgnoreCase);
                            issue.ProjectNo = Regex.Replace(issue.ProjectNo, issue.Project + "", "", RegexOptions.IgnoreCase);
                            issue.ProjectNo = Regex.Replace(issue.ProjectNo, issue.Project + "", "", RegexOptions.IgnoreCase);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            return count;
        }

        public async Task<List<Issue>> GetIssuesByMonth(int month)
        {
            var issues = await _context.Issues.Where(x => x.CreatedOn >= DateTime.Today.AddMonths(-month)).Include(x => x.IssueOwner).Include(x => x.IssueActions).ToListAsync();
            return issues;
        }

        /// <summary>
        ///     link issues to the sub projects
        /// </summary>
        /// <param name="link"> LinkSubProjectDto</param>
        /// <returns>Issue</returns>
        public async Task<Issue> LinkSubProjects(LinkSubProjectsDto link)
        {
            var issue = await this.GetIssueById(link.Id);
            if (issue == null)
                return null;
            if (await _context.subProjectStatuses.AnyAsync(x => x.IssueId == issue.Id))
            {
                throw new Exception("Sub Procects Link Exists!");
            }
            var subProjects = await _context.SubProjects.Where(x => x.MainProjectId == issue.Project).OrderBy(x => x.SubProjectId).ToListAsync();
            foreach (var item in subProjects)
            {
                SubProjectStatus sub = new SubProjectStatus
                {
                    Id = 0,
                    IssueId = issue.Id,
                    SubProjectId = item.SubProjectId,
                    Status = "pending"
                };
                await _context.subProjectStatuses.AddAsync(sub);
                await _context.SaveChangesAsync();
            }
            var issueToReturn = await this.GetIssueById(link.Id);
            return issueToReturn;
        }

        public async Task<SubProjectStatus> UpdateSubProjectStatus(SubProjectStatus sub)
        {
            var subInDb = await this._context.subProjectStatuses.Where(x => x.Id == sub.Id).FirstOrDefaultAsync();
            if (subInDb == null)
                return null;
            subInDb.Status = sub.Status;
            await _context.SaveChangesAsync();
            return subInDb;
        }

        public Task<bool> IsMatCompleted(string issueNo){
            var result = this._context.Issues.Any(x=> x.IssueNo == issueNo && x.IssueActions.Any(a => a.Responsible.GroupId=="MAT" && a.ActionStatus=="closed"));
            return Task.FromResult(result);
        }

        public async Task<Issue> LinkCustomProject(LinkCustomProjectDto req) {
            SubProjectStatus sub = new SubProjectStatus{
                Id = 0,
                IssueId = req.IssueId,
                SubProjectId = req.ProjectNo,
                Status = "pending"
            };
            await _context.subProjectStatuses.AddAsync(sub);
            await _context.SaveChangesAsync();
            var issue = await this.GetIssueById(req.IssueId);
            return issue;
        }
        public async Task<Issue> DeleteSubProject(LinkCustomProjectDto req){
            var subproj = await _context.subProjectStatuses.Where(e => e.IssueId == req.IssueId && e.SubProjectId == req.ProjectNo).FirstOrDefaultAsync();
            if(subproj == null){
                return null;
            }
            _context.subProjectStatuses.Remove(subproj);
            await _context.SaveChangesAsync();
            var issue = await this.GetIssueById(req.IssueId);
            return issue;
        }
    }
}