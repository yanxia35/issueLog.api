using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public class MbomRepo : IMbomRepo
    {
        private readonly DataContext _context;

        public MbomRepo(DataContext context)
        {
            _context = context;

        }

        public async Task<bool> AddPart(List<MbomPart> parts)
        {
            await _context.MbomParts.AddRangeAsync(parts);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> deleteMbomByProject(string project)
        {
            var mbomParts = _context.MbomParts.Where(x=>x.ProjectNo== project);
            _context.MbomParts.RemoveRange(mbomParts);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAll()
        {
            var mbomParts = _context.MbomParts;
            _context.MbomParts.RemoveRange(mbomParts);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}