using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IProjectRepository
    {
         Task<List<Project>> GetProjects();
         Task<bool> UpdateProjectComplexity(ProjectComplexityDto project);
         Task<TbProject> GetProjectComplexity(string projectId);
         Task<List<SubProject>> GetSubProjects();
         Task<SubProject> UpdateSubProjectStatus(SubProject sub);
         Task<ProjectSchedule> UpdateProjectSchedule(ProjectSchedule projectSch);
         Task<List<string>> IsValidEmployee();
         Task<TbProject> UpdateProjectStatus(TbProject proj);
         Task<ActionOwner> UpdateActionOwners (ActionOwner owner);
    }
}