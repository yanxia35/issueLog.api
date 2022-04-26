using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
  public interface IIssueRepository
  {
    Task<Issue> AddIssue(Issue newIssue);
    Task<Issue> AddIssueFromOldDb(Issue newIssue);

    Task<Issue> GetIssue(string issueNo);
    Task<Issue> GetIssueById(int id);
    Task<Issue> SaveEdittedIssue(Issue issue);
    Task<List<Issue>> Search(SearchParaDto searchValues);
    Task<List<Issue>> GetIssuesByMonth(int month);
    Task<Issue> LinkSubProjects(LinkSubProjectsDto link);
    Task<SubProjectStatus> UpdateSubProjectStatus(SubProjectStatus sub);
    Task GetNewIssueReport();
    void GetNewActionReport();
    Task<int> UpdateIssueProject();
    Task<bool> IsMatCompleted(string issueNo);
    Task<Issue> LinkCustomProject(LinkCustomProjectDto req);
    Task<Issue> DeleteSubProject(LinkCustomProjectDto req);

  }
}