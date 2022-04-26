using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface IActionRepository
    {
        Task<IssueAction> AddAction(IssueAction newAction);
        Task<IssueAction> UploadAction(IssueAction newAction);
        Task<IssueAction> GetAction(int id);
        Task<IssueAction> SaveAction(IssueAction action);

        Task<List<IssueAction>> GetActions(int issueId);
         Task<List<IssueAction>> GetAllActions();
         Task<List<IssueAction>> GetActionsByMonth(int month);
         Task<int> DeleteAction(IssueAction action);
         Task<ActionTemplateResult> GetActionOwner(ActionOwnerCondition condition);
         Task<ActionTemplateResult> GetEcDrawingOwner(ActionOwnerCondition condition);

    }
}