using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Models;
using IssueLog.API.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace IssueLog.API.Data
{
    public class ReportRepository : IReportRepository
    {
        private readonly ILogger<ReportRepository> _logger;
        private readonly DataContext _context;
        private readonly IEmailService _emailService;
        private readonly PacsContext _pacsContext;

        private readonly IConfiguration _config;

        public ReportRepository(DataContext context,
        PacsContext pacsContext,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<ReportRepository> logger)
        {
            _config = configuration;
            _pacsContext = pacsContext;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<List<ReportProjectIssueCount>> GetReportProjectIssueCount()
        {
            var report = await _context.ReportProjectIssueCounts.ToListAsync();
            return report;
        }

        public async Task<List<ProjectReportedHour>> GetProjectHours()
        {
            var projectHours = await _pacsContext.ProjectReportedHours.ToListAsync();
            return projectHours;
        }

        public async Task<int> SendReportActionPastDueDate()
        {
            var actions = await _context.IssueActions.Where(x => x.DueDate != null && x.DueDate.Value.Date < DateTime.Now.Date ).Where(x => x.DueDate.Value.Date > DateTime.Now.AddDays(-60).Date).Where(x => x.ResponsibleId != null).Where(p => p.ActionStatus == "active").Where(p => p.Issue.IssueStatus == "active").Include(x => x.Issue).
            OrderBy(x => x.ResponsibleId).OrderBy(x => x.IssueId).Include(x => x.Responsible).ToListAsync();
            if (actions == null)
                return 0;
            var actionOwners = await _context.IssueActions.Where(x => x.DueDate != null && x.DueDate.Value.Date < DateTime.Now.Date && x.DueDate.Value.Date > DateTime.Now.AddDays(-60).Date).Where(x => x.ResponsibleId != null).Where(p => p.ActionStatus == "active").Where(p => p.Issue.IssueStatus == "active").Select(p => p.Responsible).Distinct().ToListAsync();
            foreach (var employee in actionOwners)
            {
                var report = actions.Where(x => x.ResponsibleId == employee.Id).ToList();
                Email email = new Email();
                email.ToRecipientList.Add(employee.Email);
                email.mailBody = CreateActionPastDueDateBody(report);
                email.Subject = "Action past due date report";
                await _emailService.SendEmail(email);
            }
            return 1;
        }
        private string CreateActionPastDueDateBody(List<IssueAction> actions)
        {
            string body = "Following actions are past due date. Please adjust the due date or resolve the actions. \n";
            foreach (var action in actions)
            {
                body = body + "Issue No: " + action.Issue.IssueNo +
                "\nhttp://issuelog.greenlightinnovation.com/issue/" + action.Issue.IssueNo
                + "\nProject No: " + action.Issue.ProjectNo + " " + action.Issue.Project
                + "\n" + "Action Notes:\n" + action.ActionNotes + "\n";
            }
            return body;
        }
        public async Task<int> SendReportIssueMissingParts()
        {
            var issues = await _context.Issues.Include(p => p.IssueOwner).Include(p => p.EcParts).Where(x => x.IsMissingParts && x.EcParts.Count == 0).Where(p => (DateTime.Now - p.CreatedOn).Days > 2).Where(p => p.IssueStatus == "active").Include(p => p.IssueOwner).ToListAsync();
            if (issues == null)
                return 0;
            var IssueOwners = await _context.Issues.Include(p => p.IssueOwner).Include(p => p.EcParts).Where(x => x.IsMissingParts && x.EcParts.Count == 0).Where(p => (DateTime.Now - p.CreatedOn).Days > 2).Where(p => p.IssueStatus == "active").Select(p => p.IssueOwner).Distinct().ToListAsync();
            foreach (var employee in IssueOwners)
            {
                var report = issues.Where(x => x.IssueOwnerId == employee.Id).ToList();
                Email email = new Email();
                email.ToRecipientList.Add(employee.Email);
                email.mailBody = CreateMissingPartBody(report);
                email.Subject = "Issue missing parts report!";
                // System.Diagnostics.Debug.WriteLine(email.mailBody);
                await _emailService.SendEmail(email);
            }
            return 1;
        }
        private string CreateMissingPartBody(List<Issue> issues)
        {
            string body = "";
            foreach (var issue in issues)
            {
                body = body + "Issue No: " + issue.IssueNo + "\nhttp://issuelog.greenlightinnovation.com/issue/" + issue.IssueNo
                + "\nProject No: " + issue.ProjectNo + " " + issue.Project
                + "\nIssue Created On: " + issue.CreatedOn.ToString("yyyy/MM/dd")
                + "\nIssue Notes: \n" + issue.IssueNotes;
            }
            return body;
        }

        public async Task<ReportProjectIssueCount> GetReportProjectIssueCountByProjectNo(string projectNo)
        {
            var report = await _context.ReportProjectIssueCounts.Where(x => x.projectId.ToUpper().Contains(projectNo.Trim().ToUpper())).FirstOrDefaultAsync();
            return report;
        }

        public async Task<List<ProjectReportedHour>> GetProjectHoursByProjectNo(string projectNo)
        {
            var projectHours = await _pacsContext.ProjectReportedHours.Where(x => x.ProjectId.ToUpper().Contains(projectNo.Trim().ToUpper())).ToListAsync();
            return projectHours;
        }

        public async Task<List<Issue>> GetIssuesWithoutEcTrackerActionReport()
        {
            var query = _context.Issues.Include(x => x.EcParts).Where(x => x.IsMissingParts || x.EcParts.Count != 0).Where(x => x.IssueStatus == "active");
            var queryAction = _context.IssueActions.Where(x => x.ResponsibleId == "SW246");
            var result = await query.Where(x => !queryAction.Any(e => e.IssueId == x.Id)).Include(x => x.IssueOwner).OrderByDescending(x => x.IssueNo).ToListAsync();
            return result;
            // 
            //        

        }

        public async Task<List<ProjectReportedHoursByYears>> GetProjectReportedHoursByProjectNoByYears(string projectNo, int year)
        {
            var projectHours = await _pacsContext.ProjectReportedHoursByYears.Where(x => x.ProjectId.ToUpper().Contains(projectNo.Trim().ToUpper())).Where(x => x.Year == year).ToListAsync();
            return projectHours;
        }

        public async Task<EcEtaDto> GetEcEta(EcEtaDto ecEtaDto)
        {
            var activePO = await _context.ActivePOs.Where(x => x.ProjectNo.Trim() == ecEtaDto.ProjectNo
                                                && x.IssueNo.Contains(ecEtaDto.IssueNo)
                                                && x.PartNo.Trim() == ecEtaDto.PartNo).FirstOrDefaultAsync();

            if (activePO.BuyerEta == null)
            {
                ecEtaDto.Eta = activePO.SystemEta;
            }
            else
            {
                if (activePO.BuyerEta == new DateTime(1900, 01, 01))
                {
                    ecEtaDto.Eta = activePO.SystemEta;
                }
                else
                {
                    ecEtaDto.Eta = activePO.BuyerEta;
                }

            }
            if (activePO.Status == "RECEIVED")
            {
                ecEtaDto.Eta = activePO.SystemEta;
            }
            ecEtaDto.Status = activePO.Status;
            return ecEtaDto;
        }

        public async Task<int> CheckMissingDeliverable()
        {
            Email emailScheduler = new Email
            {
                // ToRecipientList = new List<string>{"yxia@greenlightinnovation.com"},
                ToRecipientList = _config.GetSection("Scheduler").Get<List<string>>(),
                Subject = "Missing deliverables in Project Dashboard for line-ready project",
                mailBody = "Hi All,\n"
            };
            bool toSend = false;
            var projSchs = await _pacsContext.ProjectSchedules.Where(x => x.IsLine).ToListAsync();
            if (projSchs != null)
            {
                foreach (var projsch in projSchs)
                {
                    if (!await _pacsContext.Deliverables.AnyAsync(x => x.Type == "KB" && x.ProjectId == projsch.Id))
                    {
                        toSend = true;
                        emailScheduler.mailBody = String.Format("{0} Project:{1} is missing \"Kitting BOM\" deliverable\n"
                        , emailScheduler.mailBody, projsch.Id);
                    }
                    if (!await _pacsContext.Deliverables.AnyAsync(x => x.Type == "AD" && x.ProjectId == projsch.Id))
                    {
                        toSend = true;
                        emailScheduler.mailBody = String.Format("{0} Project:{1} is missing \"Assembly Drawing\" deliverable\n"
                        , emailScheduler.mailBody, projsch.Id);
                    }
                    if (!await _pacsContext.Deliverables.AnyAsync(x => x.Type == "ED" && x.ProjectId == projsch.Id))
                    {
                        toSend = true;
                        emailScheduler.mailBody = String.Format("{0} Project:{1} is missing \"Electrical Drawing\" deliverable\n"
                        , emailScheduler.mailBody, projsch.Id);
                    }
                }
                if (toSend)
                {
                    await _emailService.SendEmail(emailScheduler);
                }

                return 1;

            }
            else
            {
                return 0;
            }
        }

        public async Task<int> SendOTDReport()
        {
            bool toSendMe = false;
            bool toSendDde = false;
            bool toSendElec = false;
            bool toSendSys = false;
            Email meEmail = new Email
            {
                ToRecipientList = _config.GetSection("Scheduler").Get<List<string>>(),
                Subject = "ME failed OTD",
                mailBody = "Hi All,\n"
            };
            Email ddeEmail = new Email
            {
                ToRecipientList = _config.GetSection("DDELeader").Get<List<string>>(),
                Subject = "DDE failed OTD",
                mailBody = "Hi All,\n"
            };
            Email elecEmail = new Email
            {
                ToRecipientList = _config.GetSection("ELECLeader").Get<List<string>>(),
                Subject = "Elec failed OTD",
                mailBody = "Hi All,\n"
            };

            var projSchs = await _pacsContext.ProjectSchedules.Where(x => x.EngDueDate == DateTime.Today && x.IsLine).ToListAsync();
            if (projSchs != null)
            {
                foreach (var projsch in projSchs)
                {
                    var meDeliverable = await _pacsContext.Deliverables.Where(x => x.Type == "KB" && x.ProjectId == projsch.Id).FirstOrDefaultAsync();
                    bool isMeLate = false;
                    if (meDeliverable == null)
                    {
                        toSendMe = true;
                        isMeLate = true;

                    }
                    else
                    {
                        if (meDeliverable.CompletionDate == null)
                        {
                            toSendMe = true;
                            isMeLate = true;
                        }
                        else if (meDeliverable.CompletionDate > projsch.EngDueDate)
                        {
                            toSendMe = true;
                            isMeLate = true;
                        }
                    }
                    if (isMeLate)
                    {
                        meEmail.mailBody = String.Format("{0} Project:{1} ,ME failed deliver Kitting BOM ontime.\n", meEmail.mailBody, projsch.Id);
                    }
                    var ddeDeliverable = await _pacsContext.Deliverables.Where(x => x.Type == "AD" && x.ProjectId == projsch.Id).FirstOrDefaultAsync();
                    bool isDdeLate = false;
                    if (ddeDeliverable == null)
                    {
                        toSendDde = true;
                        isDdeLate = true;
                    }
                    else
                    {
                        if (ddeDeliverable.CompletionDate == null)
                        {
                            toSendDde = true;
                            isDdeLate = true;
                        }
                        else if (ddeDeliverable.CompletionDate > projsch.HmDate)
                        {
                            toSendDde = true;
                            isDdeLate = true;
                        }
                    }
                    if (isDdeLate)
                    {
                        meEmail.mailBody = String.Format("{0} Project:{1}, DDE failed to finish Assembly Drawing ontime.\n", meEmail.mailBody, projsch.Id);
                    }
                    var elecDeliverable = await _pacsContext.Deliverables.Where(x => x.Type == "AD" && x.ProjectId == projsch.Id).FirstOrDefaultAsync();
                    bool isElecLate = false;
                    if (elecDeliverable == null)
                    {
                        toSendElec = true;
                        isElecLate = true;
                    }
                    else
                    {
                        if (elecDeliverable.CompletionDate == null)
                        {
                            toSendElec = true;
                            isElecLate = true;
                        }
                        else if (elecDeliverable.CompletionDate > projsch.HmDate)
                        {
                            toSendElec = true;
                            isElecLate = true;
                        }
                    }
                    if (isElecLate)
                    {
                        meEmail.mailBody = String.Format("{0} Project:{1}, Elec failed to finish Electrical Drawing ontime.\n", meEmail.mailBody, projsch.Id);
                    }
                }
            }
            if (toSendMe)
            {
                await _emailService.SendEmail(meEmail);
            }
            if (toSendDde)
            {
                await _emailService.SendEmail(ddeEmail);
            }
            if (toSendElec)
            {
                await _emailService.SendEmail(elecEmail);
            }
            return 1;
        }

        // function to get a list of issues that are missing parts and the response date


        /**
        * The function send a email to users to notify the late GI Express issues, and set the late to True in the IssueSchedule table
        */
        public async Task<int> SendGIExpressPastDueDate()
        {
            var lateIssues = await _context.IssueSchedule.Where(x => x.DueDate < DateTime.Now).Where(p => p.IssueStatus == "active").Where(x => x.IssueOwnerId != null).Where(s => s.ReminderSent == false).Where(r => r.RespondedAt ==null).Include(x => x.Issue).
            OrderBy(x => x.IssueOwnerId).OrderBy(x => x.IssueId).Include(x => x.Responsible).ToListAsync();
            if (lateIssues == null)
                return 0;
            var IssueOwners = await _context.IssueSchedule.Where(x => x.DueDate < DateTime.Now).Where(p => p.IssueStatus == "active").Where(x => x.IssueOwnerId != null).Where(s => s.ReminderSent == false).Where(r => r.RespondedAt ==null).Select(x => x.Responsible).Distinct().ToListAsync();
            
            foreach (var employee in IssueOwners)
            {
                var report = lateIssues.Where(x => x.IssueOwnerId == employee.Id).ToList();
                Email email = new Email();
                email.ToRecipientList.Add(employee.Email);
                email.mailBody = CreateIssuePastDueDateBody(report);
                email.Subject = "Issue past due date report";
                await _emailService.SendEmail(email);
            }
            foreach(var issueSchedule in lateIssues)
            {
                var newIssueSchedule = MarkLate(issueSchedule);
                _context.Entry(issueSchedule).CurrentValues.SetValues(newIssueSchedule);
                await _context.SaveChangesAsync();
            }
            return 1;
        }


        /**
        * The function generate the Email body for Sending late GI Express issue
        */
        private string CreateIssuePastDueDateBody(List<IssueSchedule> lateIssues)
        {
            string body = "";
            string title = " Following Issues are past due date. Please respond. \n";
            string GIbody = "";
            string NONGIbody = "";
            
            foreach (var issueSchedule in lateIssues)
            {
                if(issueSchedule.IsLine)
                {
                    GIbody = GIbody + "Issue No: " + issueSchedule.Issue.IssueNo +
                    "\nhttp://issuelog.greenlightinnovation.com/issue/" + issueSchedule.Issue.IssueNo
                    + "\nProject No: " + issueSchedule.Issue.ProjectNo + " " + issueSchedule.Issue.Project
                    + "\nDue Date: " + issueSchedule.DueDate
                    + "\n\n";
                }else
                {
                    NONGIbody = NONGIbody + "Issue No: " + issueSchedule.Issue.IssueNo +
                    "\nhttp://issuelog.greenlightinnovation.com/issue/" + issueSchedule.Issue.IssueNo
                    + "\nProject No: " + issueSchedule.Issue.ProjectNo + " " + issueSchedule.Issue.Project
                    + "\nDue Date: " + issueSchedule.DueDate
                    + "\n\n";
                }
                
            }
            body = title + "\nGI Express Issues: \n" + GIbody +"\nRegular Issues: \n" + NONGIbody; 
             
            return body;
        }

        /**
        * The function mark the Issue in IssueShcdule to late 
        */
        private IssueSchedule MarkLate(IssueSchedule issueScheduleInDb)
        {
            var newIssueSchedule = new IssueSchedule();
            
            newIssueSchedule.IssueId = issueScheduleInDb.IssueId;
            newIssueSchedule.CreatedOn = issueScheduleInDb.CreatedOn;
            newIssueSchedule.IssueOwnerId = issueScheduleInDb.IssueOwnerId;
            newIssueSchedule.DueDate = issueScheduleInDb.DueDate;
            newIssueSchedule.ActionIds = issueScheduleInDb.ActionIds;
            newIssueSchedule.RespondedAt = issueScheduleInDb.RespondedAt;
            newIssueSchedule.Late = true;
            newIssueSchedule.ReminderSent =true;
            newIssueSchedule.IssueStatus = issueScheduleInDb.IssueStatus;
            newIssueSchedule.IsLine = issueScheduleInDb.IsLine;
            
            return newIssueSchedule;
        }
    }
}