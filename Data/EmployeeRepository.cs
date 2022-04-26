using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
  public class EmployeeRepository : IEmployeeRepository
  {
    private readonly PacsContext _context;

    public EmployeeRepository(PacsContext context)
    {
      _context = context;
    }
    public async Task<List<Employee>> GetEmployees()
    {
      var employees = await _context.Employees
      .OrderBy(x => x.FirstName).Where(x=> !x.IsFormer)
      .ToListAsync();
      return employees;
    }
        public async Task<List<Employee>> GetAllEmployees()
    {
      var employees = await _context.Employees
      .OrderBy(x => x.FirstName)
      .ToListAsync();
      return employees;
    }

        public async Task<bool> IsCorrectEmployeeId(string id)
        {
            return await _context.Employees.AnyAsync(x=> x.Id == id);
        }
    }
}