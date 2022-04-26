using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
  public interface IIssueFileRepository
  {
    Task<IssueFile> AddIssueFile(IssueFileToAddDto issueFileToAdd);
    Task<List<IssueFile>> GetIssueFileById(int issueId);
     Task<bool> DeleteIssueFile(int id);
  }
}