using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
    public class FailureModeRepository : IFailureModeRepository
    {
    private DataContext _context;

    public FailureModeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<FailureMode> AddFailureMode(FailureMode failureMode){
            await _context.FailureModes.AddAsync(failureMode);
            await _context.SaveChangesAsync();
            return failureMode;
        }

    public async Task<List<FailureMode>> GetFailureMode()
    {
      var failureMode = await _context.FailureModes.ToListAsync();
      return failureMode;
    }
  }
}