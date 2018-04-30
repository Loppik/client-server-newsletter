using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    abstract class DBConnector
    {
        public abstract List<News> GetNewsBetweenTimeInterval(int userId, string afterDatetime, string untilDatetime);
        public abstract List<Subscription> GetAllSubscriptions();
        public abstract List<Subscription> GetUserSubscriptions(int userId);
        public abstract ConsoleApp1.User findUser(string nickname, string password);
    }
}
