using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewslettersClassLibrary;

namespace WpfApp1
{
    abstract class DBConnector
    {
        public abstract List<News> GetNewsBetweenTimeInterval(List<int> subscriptionsId, DateTime afterDatetime, DateTime untilDatetime);
        public abstract List<Subscription> GetAllSubscriptions();
        public abstract List<Subscription> GetUserSubscriptions(List<int> subscriptionsId);
        public abstract User FindUser(string nickname, string password);
        public abstract void DeleteUserSubscription(int userId, int subscriptionId);
        public abstract void AddUserSubscription(int userId, int subscriptionId);
        public abstract void UpdateLastVisitTime(int userId, DateTime time);
    }
}
