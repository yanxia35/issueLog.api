using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IEcPartRepository
    {
        Task<bool> AddPart(List<EcPartToAddDto> ecPart);
        Task<bool> DeleteAll();
        Task<bool> DeleteEcPartsByProject(string projectNo);
    }
}