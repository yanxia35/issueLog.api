using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Data
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly DataContext _context;

        public SubscriberRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Subscriber> AddSubscriber(Subscriber subscriber)
        {
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();
            return subscriber;
        }

        public async Task<int> DeleteSubscriber(Subscriber subscriber)
        {
            var subscriberInDb = await _context.Subscribers.Where(x => x.IssueId == subscriber.IssueId && x.EmployeeId == subscriber.EmployeeId).FirstOrDefaultAsync();
            if(subscriberInDb == null){
              return 0;
            }
            _context.Subscribers.Remove(subscriberInDb);
            await _context.SaveChangesAsync();
            return 1;
        }
    }
}