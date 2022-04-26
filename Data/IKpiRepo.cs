using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IKpiRepo
    {
        Task<List<ProjectOtdKpiDto>> GetProjectOtdKpiDtos();
        Task<List<IssueMissingPartResponseReportDto>> GetIssueMissingPartResponseReport();
        Task<List<PartReleaseOtoDto>> GetPartReleaseOto();
        Task<List<PartReleaseOtoDto>> GetPartReleaseOtoLR();

    }
}