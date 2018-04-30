using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NewslettersClassLibrary;

namespace WpfApp1
{
    class Program
    {
        static Socket socket;

        static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 2000;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socket.Bind(endPoint);
            socket.Listen(5);

            Thread thread = new Thread(new ThreadStart(WaitForConnections));
            thread.Start();
        }

        public static void WaitForConnections()
        {
            Console.WriteLine("Waiting for connections");
            while (true)
            {
                User user = new User("newUser");
                try
                {
                    user.socket = socket.Accept();
                    user.thread = new Thread(() => { ProcessMessages(user); });
                    user.thread.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine("Error");
                }

            }

        }

        public static void ProcessMessages(User user)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                int len = user.socket.Receive(bytes);
                if (len > 0)
                {
                    string message = Encoding.Unicode.GetString(bytes);
                    if (message.Substring(0, 5) == "/auth")
                    {
                        int index = message.Substring(6).IndexOf(" ");
                        string nickname = message.Substring(6, index);
                        string password = message.Substring(index + 1);
                        User tempUser = StorageModel.dao.FindUser(nickname, password);
                        // todo
                        string response;
                        if (tempUser.nickname != "noname")
                        {
                            user.id = tempUser.id;
                            user.nickname = tempUser.nickname;
                            user.lastVisitTime = tempUser.lastVisitTime;
                            user.subscriptionsId = tempUser.subscriptionsId;
                            Console.WriteLine("User connected: " + user.nickname);
                            response = user.id + "*" + user.nickname + "*" + user.lastVisitTime + "*" + Converter.SerializeListOfInt(user.subscriptionsId);
                        }
                        else
                        {
                            Console.WriteLine("An attempt to connect a user(" + nickname + ") which is't in database");
                            response = "invalid data";
                        }
                        // todo create as thread
                        user.socket.Send(Encoding.Unicode.GetBytes(response));
                    }
                }
            }
        }
    }
}
