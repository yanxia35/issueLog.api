using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IMbomRepo
    {
        Task<bool> AddPart(List<MbomPart> parts);

        Task<bool> deleteMbomByProject(string project);
        Task<bool> DeleteAll();
    }
}