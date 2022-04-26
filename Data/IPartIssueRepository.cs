using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
namespace IssueLog.API.Data
{
    public interface IPartIssueRepository
    {
        Task<PartIssue> AddPartIssue(PartIssue partIssue);
        Task<int> DeletePartIssue(PartIssue partIssue);
        Task<IEnumerable<PartIssue>> GetPartIssuesByPartNo(string partNo);
        Task<PartIssue> FlagPart(PartToFlag part);
        Task<PartIssue> ResolveFlag(PartIssue partIssue);
        Task<PartIssue> ReopenFlag(PartIssue partIssue);

        Task<int> DeleteFlag(PartIssue partIssue);

    }
}