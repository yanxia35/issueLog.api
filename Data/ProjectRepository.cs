using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Reflection;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IssueLog.API.Data
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly PacsContext _context;
        private readonly IConfiguration _config;
        private IDictionary<string,string> projectStatus = new Dictionary<string, string>();
        public ProjectRepository(PacsContext context,IConfiguration configuration)
        {
            _config = configuration;
            _context = context;
            projectStatus = _config.GetSection("ProjectStatus").Get<Dictionary<string,string>>();
        }

        public async Task<TbProject> GetProjectComplexity(string projectId)
        {
            var proj = await _context.TbProjects.Where(x => x.ProjectId == projectId).FirstOrDefaultAsync();
            return proj;
        }

        public async Task<List<Project>> GetProjects()
        {
            var projects = await _context.Projects
            .Where(x => x.Status.Trim() != "Done" && x.Status.Trim() != "Template").OrderBy(x => x.Id).ToListAsync();
            return projects;
        }

        public async Task<List<SubProject>> GetSubProjects()
        {
            var subProjects = await _context.SubProjects.OrderBy(x => x.SubProjectId).ToListAsync();
            return subProjects;
        }
        public async Task<bool> UpdateProjectComplexity(ProjectComplexityDto project)
        {
            var projectDb = await _context.TbProjects.Where(x => x.ProjectId.ToUpper() == project.ProjectId.ToUpper()).FirstOrDefaultAsync();
            if (projectDb == null)
            {
                return false;
            }
            projectDb.Product = project.Product;
            projectDb.MechComplexity = project.MechComplexity;
            projectDb.ElecComplexity = project.ElecComplexity;
            projectDb.SocialComplexity = project.SocialComplexity;
            if (project.Status != "")
            {
                projectDb.Status = project.Status;
            }
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<SubProject> UpdateSubProjectStatus(SubProject sub)
        {
            var subInDb = await _context.SubProjects.Where(x => x.Id == sub.Id).FirstOrDefaultAsync();
            if (subInDb == null)
                return null;
            subInDb.Status = sub.Status;
            await _context.SaveChangesAsync();
            return subInDb;
        }
        public async Task<ProjectSchedule> UpdateProjectSchedule(ProjectSchedule projectSch)
        {
            var projSchDb = await _context.ProjectSchedules.Where(x => x.Id == projectSch.Id).FirstOrDefaultAsync();
            string status;

            if(projectSch.Status == null){
                projectSch.Status = projSchDb.Status;
            }else if(projectStatus.ContainsKey(projectSch.Status)){
                status = projectStatus[projectSch.Status];
                projectSch.Status = status;
            }else{
                status = "Done";
                projectSch.Status = status;
            }
            
            if (projSchDb == null)
            {
                await _context.ProjectSchedules.AddAsync(projectSch);
            }
            else
            {   
                foreach(PropertyInfo prop in projectSch.GetType().GetProperties()){
                    if(prop.GetValue(projectSch) == null){
                        prop.SetValue(projectSch,prop.GetValue(projSchDb));    
                    }
                }
                _context.Entry(projSchDb).CurrentValues.SetValues(projectSch);
            }

            await _context.SaveChangesAsync();
            return projSchDb;
        }
    
          public async Task<List<string>> IsValidEmployee()
        {
            var employeeList = await _context.Employees.Select(x => x.Id).ToListAsync();
            if (employeeList == null)
            {
                return null;
            }
            
            return employeeList;
        }

        public async Task<TbProject> UpdateProjectStatus(TbProject proj)
        {
            string status;
    
            if(projectStatus.ContainsKey(proj.Status)){
                status = projectStatus[proj.Status];
            }else{
                status = "Done";
            }

            var projDb = await _context.TbProjects.Where(x => x.ProjectId == proj.ProjectId).FirstOrDefaultAsync();
            if (projDb != null)
            {
                projDb.Status = status;
                _context.Entry(projDb).CurrentValues.SetValues(projDb);
            }
            
            await _context.SaveChangesAsync();
            return projDb;
        }


        public async Task<ActionOwner> UpdateActionOwners (ActionOwner owner)
        {
            var ownerDb = await _context.ActionOwners.Where(x => x.GroupId == owner.GroupId).FirstOrDefaultAsync();
            
            if (ownerDb == null)
            {   
                await _context.ActionOwners.AddAsync(owner);
            }
            else
            {
                _context.Entry(ownerDb).CurrentValues.SetValues(owner);
            }

            await _context.SaveChangesAsync();
            return ownerDb;
        }
    }
}