using System.Collections.Generic;
using System.Threading.Tasks;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public interface ISubscriberRepository
    {
         Task<Subscriber> AddSubscriber(Subscriber subscriber);
         Task<int> DeleteSubscriber(Subscriber subscriber);
    }
}