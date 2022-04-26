using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IProcessRepository
    {
        Task<Process> AddProcess(Process process);
        Task<List<Process>> GetProcesses();
    }
}