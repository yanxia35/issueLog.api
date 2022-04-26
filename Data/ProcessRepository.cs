using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
  public class ProcessRepository : IProcessRepository
  {
    private readonly DataContext _context;
    public ProcessRepository(DataContext context)
    {
      _context = context;

    }
    public async Task<Process> AddProcess(Process process)
    {
      await _context.Processes.AddAsync(process);
      await _context.SaveChangesAsync();
      return process;
    }

    public async Task<List<Process>> GetProcesses()
    {
      var process = await _context.Processes.OrderBy(x => x.Name).ToListAsync();
      return process;
    }
  }
}