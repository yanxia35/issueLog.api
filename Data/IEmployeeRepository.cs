using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IEmployeeRepository
    {
        // function to get all the employees
         Task<List<Employee>> GetEmployees();
         Task<List<Employee>> GetAllEmployees();
         Task<bool> IsCorrectEmployeeId(string id);
    }
}