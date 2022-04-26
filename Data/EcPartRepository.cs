using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;


namespace IssueLog.API.Data
{
    public class EcPartRepository : IEcPartRepository
    {
        private readonly DataContext _context;

        public EcPartRepository(DataContext context)
        {

            _context = context;
        }
        public async Task<bool> AddPart(List<EcPartToAddDto> ecParts)
        {
            foreach (var ecPart in ecParts)
            {
                var issue = await _context.Issues.Where(x => x.IssueNo == ecPart.IssueNo).FirstOrDefaultAsync();
                var config = new MapperConfiguration(cfg => cfg.CreateMap<EcPartToAddDto, TEcPart>());
                var mapper = config.CreateMapper();
                var newPart = mapper.Map<TEcPart>(ecPart);
                var ecPartInDb = await _context.EcParts.Where(x => x.Issue.IssueNo == ecPart.IssueNo
                                && x.partNo == ecPart.PartNo && x.Quantity == ecPart.Quantity && x.ProjectNo == ecPart.ProjectNo).FirstOrDefaultAsync();
                newPart.IssueId = issue.Id;
                _context.TEcParts.Add(newPart);
            }

            await _context.SaveChangesAsync();
            return true;

            // if (ecPartInDb == null)
            // {
            //     if (issue != null)
            //     {
            //         newPart.IssueId = issue.Id;
            //     }
            //     else
            //     {
            //         return false;
            //     }

            // }
            // return true;
        }

        public async Task<bool> DeleteAll()
        {
            var ecparts = _context.TEcParts;
            _context.TEcParts.RemoveRange(ecparts);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEcPartsByProject(string projectNo)
        {
            var ecparts = _context.TEcParts.Where(x=>x.ProjectNo == projectNo);
            _context.TEcParts.RemoveRange(ecparts);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}