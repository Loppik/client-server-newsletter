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
        public abstract List<Subscription> GetUserSubscriptions(int userId);
        public abstract User FindUser(string nickname, string password);
    }
}
