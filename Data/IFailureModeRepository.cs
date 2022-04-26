using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IFailureModeRepository
    {
         Task<FailureMode> AddFailureMode(FailureMode failureMode);
         Task<List<FailureMode>> GetFailureMode();
    }
}