using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
    public class PartIssueRepository : IPartIssueRepository
    {
        private readonly DataContext _context;
        private readonly IEmployeeRepository _employeeRepo;

        public PartIssueRepository(DataContext context, IEmployeeRepository employeeRepo)
        {
            _context = context;
            _employeeRepo = employeeRepo;
        }

        public Regex regPart = new Regex("^[0-3]{1}[0-9]{6}$");
        public Regex regAsm = new Regex("^[7]{1}[0-9]{6}$");
        public Regex regFlag = new Regex("[0-9]{2}-[0-9]{4}");
        public async Task<PartIssue> AddPartIssue(PartIssue partIssue)
        {
            var task = await _context.PartIssues.AddAsync(partIssue);
            await _context.SaveChangesAsync();
            return partIssue;
        }

        public async Task<PartIssue> ReopenFlag(PartIssue part)
        {
            var existPartIssue = await _context.PartIssues.Where(x => x.PartNo == part.PartNo && x.IssueId == part.IssueId).Include(x=>x.Employee).Include(x => x.Issue).FirstOrDefaultAsync();
            existPartIssue.IsResolved = part.IsResolved;
            existPartIssue.ResolvedDate = null;
            existPartIssue.ResolvedBy = null;
            await _context.SaveChangesAsync();
            PartToFlag partToFlag = new PartToFlag
            {
                IssueId = existPartIssue.Id,
                IsHardFlag = existPartIssue.IsHardFlag,
                PartNo = existPartIssue.PartNo,
                IssueNo = existPartIssue.Issue.IssueNo
            };
            var flag = await AddFlagToDescription(partToFlag);
            return existPartIssue;
        }

        private async Task<int> AddFlagToDescription(PartToFlag part)
        {
            var res = await _context.Parts.Where(x => x.PartNo == part.PartNo).ToListAsync();
            if (res==null || res.Count == 0)
            {
                throw new Exception(part.PartNo + " does not exist!");
            }
            for (int i = 0; i < res.Count; i++)
            {
                if (part.IsHardFlag)
                {
                    if (res[i].Description != null && res[i].Description.Length >= 5)
                    {
                        if (!res[i].Description.Contains(part.IssueNo))
                        {
                            if (res[i].Description.Substring(0, 5) != "FLAG,")
                            {
                                res[i].Description = "FLAG,EC" + part.IssueNo + "," + res[i].Description;
                            }
                            else
                            {
                                res[i].Description = res[i].Description.Substring(0, 5) + "EC" + part.IssueNo + "," + res[i].Description.Substring(5);
                            }
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        res[i].Description = "FLAG,EC" + part.IssueNo;
                        _context.SaveChanges();
                    }
                }
                else
                {
                    if (res[i].MfgInfo != null && res[i].MfgInfo.Length>= 5)
                    {
                        if (!res[i].MfgInfo.Contains(part.IssueNo))
                        {
                            if ( res[i].MfgInfo.Substring(0, 5).ToUpper() != "FLAG,")
                            {
                                res[i].MfgInfo = "FLAG,EC" + part.IssueNo + "," + res[i].MfgInfo;
                            }
                            else
                            {
                                res[i].MfgInfo = res[i].MfgInfo.Substring(0, 5) + "EC" + part.IssueNo + "," + res[i].MfgInfo.Substring(5);
                            }
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        res[i].MfgInfo = "FLAG,EC" + part.IssueNo;
                        _context.SaveChanges();
                    }
                }
            }
            return 1;
        }
        public async Task<PartIssue> FlagPart(PartToFlag part)
        {
            var flag = await AddFlagToDescription(part);
            var existPartIssue = await _context.PartIssues.Where(x => x.PartNo == part.PartNo && x.IssueId == part.IssueId).FirstOrDefaultAsync();
            if (existPartIssue == null)
            {
                PartIssue partIssue = new PartIssue
                {
                    Id = 0,
                    PartNo = part.PartNo,
                    IssueId = part.IssueId,
                    IsHardFlag = part.IsHardFlag,
                    IsResolved = false
                };
                var partIssueToReturn = await AddPartIssue(partIssue);
                return partIssueToReturn;
            }
            else
            {
                return existPartIssue;
            }
        }

        public async Task<IEnumerable<PartIssue>> GetPartIssuesByPartNo(string partNo)
        {
            var partIssues = await _context.PartIssues.Where(x => x.PartNo == partNo).OrderBy(x=>x.Issue.IssueNo).OrderByDescending(x=>x.IsResolved).Include(x => x.Issue).Include(x=>x.Employee).ToListAsync();
            return partIssues;
        }

        private async Task<int> RemoveFlagFromDescription(PartIssue partIssue)
        {
            var res = await _context.Parts.Where(x => x.PartNo == partIssue.PartNo).ToListAsync();
            var part = await _context.PartIssues.Where(x => x.Id == partIssue.Id).Include(x => x.Issue).FirstOrDefaultAsync();
            if (res == null)
            {
                return 0;
            }
            for (int i = 0; i < res.Count; i++)
            {
                if (res[i].Description != null)
                {
                    if (res[i].Description.Contains(part.Issue.IssueNo))
                    {
                        res[i].Description = res[i].Description.Replace("EC" + part.Issue.IssueNo + ",", "");
                        res[i].Description = res[i].Description.Replace(part.Issue.IssueNo + ",", "");
                        res[i].Description = res[i].Description.Replace("EC" + part.Issue.IssueNo, "");
                        res[i].Description = res[i].Description.Replace(part.Issue.IssueNo, "");
                        if (!regFlag.IsMatch(res[i].Description))
                        {
                            if (res[i].Description.Substring(0, 4).ToLower() == "flag")
                            {
                                res[i].Description = res[i].Description.Substring(5);
                            }
                            res[i].Description.Trim();
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else { }
                if (res[i].MfgInfo != null)
                {
                    if (res[i].MfgInfo.Contains(part.Issue.IssueNo))
                    {
                        res[i].MfgInfo = res[i].MfgInfo.Replace("EC" + part.Issue.IssueNo + ",", "");
                        res[i].MfgInfo = res[i].MfgInfo.Replace(part.Issue.IssueNo + ",", "");
                        res[i].MfgInfo = res[i].MfgInfo.Replace("EC" + part.Issue.IssueNo, "");
                        res[i].MfgInfo = res[i].MfgInfo.Replace(part.Issue.IssueNo, "");
                        if (!regFlag.IsMatch(res[i].MfgInfo))
                        {
                            if (res[i].MfgInfo.Substring(0, 4).ToLower() == "flag")
                            {
                                res[i].MfgInfo = res[i].MfgInfo.Substring(5);
                            }
                            res[i].MfgInfo.Trim();
                        }
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return 1;
        }

        public async Task<PartIssue> ResolveFlag(PartIssue partIssue)
        {           
            partIssue.ResolvedDate= DateTime.Now;
            if(!await _employeeRepo.IsCorrectEmployeeId(partIssue.ResolvedBy) ){
                return null;
            }
            var deleteDesc = await RemoveFlagFromDescription(partIssue);
            var partIssueInDb = await _context.PartIssues.Where(x => x.Id == partIssue.Id).FirstOrDefaultAsync();
            _context.Entry(partIssueInDb).CurrentValues.SetValues(partIssue);
            await _context.SaveChangesAsync();
            var partToReturn = await _context.PartIssues.Where(x=> x.Id == partIssueInDb.Id).Include(e=> e.Employee).FirstOrDefaultAsync();
            return partToReturn;
        }

        public async Task<int> DeleteFlag(PartIssue partIssue)
        {
            var partIssueToRemove = await _context.PartIssues.Where(x => x.Id == partIssue.Id).FirstOrDefaultAsync();
            if (partIssueToRemove == null)
            {
                return 0;
            }
            await RemoveFlagFromDescription(partIssue);
            _context.PartIssues.Remove(partIssueToRemove);
            await _context.SaveChangesAsync();
            return 1;
        }

        public Task<int> DeletePartIssue(PartIssue partIssue)
        {
            throw new System.NotImplementedException();
        }

    }
}