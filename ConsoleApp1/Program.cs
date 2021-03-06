﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
        static News lastNews;

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
            User tempUser = null;
            while (true)
            {
                byte[] bytes = new byte[1024];
                int len = user.socket.Receive(bytes);
                if (len > 0)
                {
                    string message = Encoding.Unicode.GetString(bytes, 0, len);
                    string[] request = message.Split('*');
                    string response;
                    List<Subscription> subscriptions;
                    switch (request[0])
                    {
                        case Request.AuthorizationRequest:
                            string nickname = request[1];
                            string password = request[2];
                            tempUser = StorageModel.dao.FindUser(nickname, password);
                            if (tempUser.nickname != "noname")
                            {
                                user.id = tempUser.id;
                                user.nickname = tempUser.nickname;
                                user.lastVisitTime = tempUser.lastVisitTime;
                                user.subscriptionsId = tempUser.subscriptionsId;
                                Console.WriteLine("User connected: " + user.nickname);
                                //response = user.id + "*" + user.nickname + "*" + user.lastVisitTime + "*" + Converter.SerializeListOfInt(user.subscriptionsId);
                                response = Converter.SerializeUser(user);
                            }
                            else
                            {
                                Console.WriteLine("An attempt to connect a user(" + nickname + ") which is't in database");
                                response = "";
                            }
                            // todo create as thread
                            user.socket.Send(Encoding.Unicode.GetBytes(response));
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            break;

                        case Request.LastNewsRequest:
                            Console.WriteLine("Request from " + user.nickname + " for the latest news");
                            List<News> newsList = StorageModel.dao.GetNewsBetweenTimeInterval(user.subscriptionsId, user.lastVisitTime, DateTime.Now);
                            foreach (News news in newsList)
                            {
                                response = Converter.SerializeNews(news);
                                user.socket.Send(Encoding.Unicode.GetBytes(response));
                            }
                            Console.WriteLine(user.nickname + " received " + newsList.Count + " news");
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            break;

                        case Request.UserSubscriptionsRequest:
                            Console.WriteLine("Request from " + user.nickname + " for the your subscriptions");
                            subscriptions = StorageModel.dao.GetUserSubscriptions(user.subscriptionsId);
                            Console.WriteLine(user.nickname + " received " + subscriptions.Count + " subscriptions");
                            SendListOfSubscriptions(user.socket, subscriptions);
                            break;

                        case Request.AllSubscriptionsRequest:
                            Console.WriteLine("Request from " + user.nickname + " for the all subscriptions");
                            subscriptions = StorageModel.dao.GetAllSubscriptions();
                            Console.WriteLine(user.nickname + " received " + subscriptions.Count + " subscriptions");
                            SendListOfSubscriptions(user.socket, subscriptions);
                            break;

                        case Request.DeleteUserSubcription:
                            int delSubId = Int32.Parse(request[1].ToString());
                            Console.WriteLine("Request from " + user.nickname + " for delete subscription with id = " + delSubId);
                            StorageModel.dao.DeleteUserSubscription(user.id, delSubId);
                            user.subscriptionsId.Remove(delSubId);
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            break;

                        case Request.AddUserSubcription:
                            int addSubId = Int32.Parse(request[1].ToString());
                            Console.WriteLine("Request from " + user.nickname + " for add subscription with id = " + addSubId);
                            StorageModel.dao.AddUserSubscription(user.id, addSubId);
                            user.subscriptionsId.Add(addSubId);
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            break;

                        case Request.CloseConnection:
                            StorageModel.dao.UpdateLastVisitTime(user.id, DateTime.Now);
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            Console.WriteLine(user.nickname + " interrupted the connection");
                            user.socket.Close();
                            return;
                    }
                }
            }
        }

        public static void SendListOfSubscriptions(Socket socket, List<Subscription> subscriptions)
        {
            string response;
            foreach (Subscription subscription in subscriptions)
            {
                response = Converter.Serialize(subscription);
                Console.WriteLine(response);
                socket.Send(Encoding.Unicode.GetBytes(response));
            }
            socket.Send(Encoding.Unicode.GetBytes("end"));
        }
    }
}
