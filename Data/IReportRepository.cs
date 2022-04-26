using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;
using IssueLog.API.Dtos;

namespace IssueLog.API.Data
{
    public interface IReportRepository
    {
        Task<List<ReportProjectIssueCount>> GetReportProjectIssueCount();
        Task<ReportProjectIssueCount> GetReportProjectIssueCountByProjectNo(string projectNo);
        Task<int> SendReportActionPastDueDate();
        Task<int> SendOTDReport();
        Task<int> SendReportIssueMissingParts();
        Task<int> CheckMissingDeliverable();
        Task<List<ProjectReportedHour>> GetProjectHours();
        Task<List<ProjectReportedHour>> GetProjectHoursByProjectNo(string projectNo);
        Task<List<ProjectReportedHoursByYears>> GetProjectReportedHoursByProjectNoByYears(string projectNo, int year);
        Task<List<Issue>> GetIssuesWithoutEcTrackerActionReport();
        Task<EcEtaDto> GetEcEta(EcEtaDto ecEtaDto);
        Task<int> SendGIExpressPastDueDate();

    }
}