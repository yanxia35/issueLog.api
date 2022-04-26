using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
    public class KpiRepo : IKpiRepo
    {
        private readonly PacsContext _context;
        private readonly DataContext _issueContext;
        public KpiRepo(PacsContext context, DataContext issueContext)
        {
            _issueContext = issueContext;
            _context = context;

        }
        public async Task<List<ProjectOtdKpiDto>> GetProjectOtdKpiDtos()
        {
            List<ProjectOtdKpiDto> projectOtdKpiDtoList = new List<ProjectOtdKpiDto>();
            var projectNumbers = new string[]{"G20","G21","G22","G19"};
            var projectSchs = await _context.ProjectSchedules.Where(x=> (x.IsSubProj == null || x.IsSubProj!= true) && x.Id.Substring(x.Id.Length-1)=="0").Where(x=>  new string[]{"G20","G21","G22","G19"}.Contains(x.Id.Substring(0,3))).ToListAsync();
            if (projectSchs == null){
                return null;
            }
            foreach (var projectSch in projectSchs)
            {
                var adDevlierable = await _context.Deliverables.Where(x => x.Type == "AD" && x.ProjectId == projectSch.Id).FirstOrDefaultAsync();
                var kbDeliverable = await _context.Deliverables.Where(x => x.Type == "KB" && x.ProjectId == projectSch.Id).FirstOrDefaultAsync();
                var edDeliverable = await _context.Deliverables.Where(x => x.Type == "ED" && x.ProjectId == projectSch.Id).FirstOrDefaultAsync();
                var rdDeliverable = await _context.Deliverables.Where(x => x.Type == "RD" && x.ProjectId == projectSch.Id).FirstOrDefaultAsync();
                var ceDeliverable = await _context.Deliverables.Where(x => x.Type == "CE" && x.ProjectId == projectSch.Id).FirstOrDefaultAsync();
                var heDeliverable = await _context.Deliverables.Where(x => x.Type == "HE" && x.ProjectId == projectSch.Id).FirstOrDefaultAsync();

                var projectOtd = new ProjectOtdKpiDto();
                projectOtd.ProjectNo = projectSch.Id;
                projectOtd.HeDueDate = projectSch.HeDueDate;
                projectOtd.AdDueDate = projectSch.RdDueDate;
                projectOtd.EdDueDate = projectSch.CeDueDate;
                projectOtd.IsLine = projectSch.IsLine;
                if (rdDeliverable != null)
                {
                    projectOtd.AdCompletionDate = rdDeliverable.CompletionDate;
                } else if (adDevlierable != null){
                    projectOtd.AdCompletionDate = adDevlierable.CompletionDate;
                }
                if (ceDeliverable != null){
                     projectOtd.EdCompletionDate = ceDeliverable.CompletionDate;
                }
                else if (edDeliverable != null)
                {
                    projectOtd.EdCompletionDate = edDeliverable.CompletionDate;
                }
                if (kbDeliverable != null)
                {
                    projectOtd.KbDueDate = kbDeliverable.EndDate;
                    projectOtd.KbCompletionDate = kbDeliverable.CompletionDate;
                }
                if (heDeliverable != null){
                    projectOtd.HeCompletionDate = heDeliverable.CompletionDate;
                }
                projectOtd.EngDueDate = projectSch.EngDueDate;
                projectOtd.HmDate = projectSch.HmDate;
                projectOtdKpiDtoList.Add(projectOtd);
            }
            return projectOtdKpiDtoList;
        }
        public async Task<List<IssueMissingPartResponseReportDto>> GetIssueMissingPartResponseReport()
        {
            var query = _issueContext.Issues.Where(x => x.IsMissingParts).Include(x => x.IssueOwner);
            var query2 = _issueContext.EcParts.GroupBy(o => new { o.IssueId }).Select(g => new
            {
                g.Key.IssueId,
                FirstPartOrderDate = g.Min(o => o.ReleaseDate)
            });
            var results = await query.Join(query2, query => query.Id, query2 => query2.IssueId, (query, query2) => new IssueMissingPartResponseReportDto
            {
                IssueNo = query.IssueNo,
                CreatedOn = query.CreatedOn,
                ProjectNo = query.Project,
                IssueOwner = query.IssueOwner,
                FirstPartOrderDate = query2.FirstPartOrderDate
            }).OrderBy(x => x.CreatedOn).ToListAsync();
            return results;
        }
        public async Task<List<PartReleaseOtoDto>> GetPartReleaseOtoLR(){
            DateTime today = DateTime.Today;
            var query1 = _context.MbomParts.Where(x=>x.ReleaseDate >= new DateTime(today.Year,today.Month, 1).AddMonths(-2)
            && x.ReleaseDate <=  new DateTime(today.Year,today.Month, 1).AddMonths(1).AddDays(-1));
            var query2 = _context.ProjectSchedules;
            var query3 = query1.Join(query2, query1=>query1.ProjectNo, query2=> query2.Id, (query1, query2)=> new PartReleaseOtoDto{
                ProjectNo = query1.ProjectNo,
                PartNo = query1.PartNo,
                Qty = query1.Quantity,
                ReleaseDate = query1.ReleaseDate,
                MfgStartDate = query2.MfgStartDate,
                IsLine = query2.IsLine,
                Supplier = query1.SupplierName,
                LeadTime = query1.LeadTime
            });
            
            var items = await query3.OrderBy(x=>x.ReleaseDate).Where(x=>x.IsLine).ToListAsync();
            return items;
        }
        public async Task<List<PartReleaseOtoDto>> GetPartReleaseOto(){
            DateTime today = DateTime.Today;
            var query1 = _context.MbomParts.Where(x=>x.ReleaseDate >= new DateTime(today.Year,today.Month, 1).AddMonths(-2)
            && x.ReleaseDate <=  new DateTime(today.Year,today.Month, 1).AddMonths(1).AddDays(-1));
            var query2 = _context.ProjectSchedules;
            var query3 = query1.Join(query2, query1=>query1.ProjectNo, query2=> query2.Id, (query1, query2)=> new PartReleaseOtoDto{
                ProjectNo = query1.ProjectNo,
                PartNo = query1.PartNo,
                Qty = query1.Quantity,
                ReleaseDate = query1.ReleaseDate,
                MfgStartDate = query2.MfgStartDate,
                IsLine = query2.IsLine,
                Supplier = query1.SupplierName,
                LeadTime = query1.LeadTime
            });
            
            var items = await query3.OrderBy(x=>x.ReleaseDate).ToListAsync();
            return items;
        }
    }
}