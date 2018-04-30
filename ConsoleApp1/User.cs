using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class User
    {
        public int id;
        public string nickname;
        public string lastVisitTime;
        public List<int> subscriptionsId;
        public Socket socket;
        public Thread thread;

        public User(int id, string nickname, string lastVisitTime, List<int> subscriptionsId)
        {
            this.id = id;
            this.nickname = nickname;
            this.lastVisitTime = lastVisitTime;
            this.subscriptionsId = subscriptionsId;
        }

        public User(string nickname)
        {
            this.nickname = nickname;
        }
    }
}
