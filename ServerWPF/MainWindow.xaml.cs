using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NewslettersClassLibrary;
using System.ComponentModel;

namespace ServerWPF
{
    public partial class MainWindow : Window
    {
        static Socket socket;
        Thread waitConnectionsThread;
        static TextBoxOutput output;
        static News lastNews;

        public MainWindow()
        {
            this.Closing += OnWindowClose;
            InitializeComponent();
            LoadComboBoxItems();

            output = new TextBoxOutput(TextBoxLog);

            string address = "127.0.0.1";
            int port = 2000;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socket.Bind(endPoint);
            socket.Listen(5);

            waitConnectionsThread = new Thread(new ThreadStart(WaitForConnections));
            waitConnectionsThread.Start();
        }

        public void WaitForConnections()
        {
            Output("Waiting for connections");
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
                    Output(e.StackTrace);
                    Output("Error");
                }

            }

        }

        public void ProcessMessages(User user)
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
                    string response, nickname, password;
                    List<Subscription> subscriptions;
                    switch (request[0])
                    {
                        case Request.AuthorizationRequest:
                            nickname = request[1];
                            password = request[2];
                            tempUser = StorageModel.dao.FindUser(nickname, password);
                            if (tempUser.nickname != null)
                            {
                                user.id = tempUser.id;
                                user.nickname = tempUser.nickname;
                                user.lastVisitTime = tempUser.lastVisitTime;
                                user.subscriptionsId = tempUser.subscriptionsId;
                                Output("User connected: " + user.nickname);
                                response = Converter.SerializeUser(user);
                            }
                            else
                            {
                                Output("An attempt to connect a user(nickname: " + nickname + ", password: " + password + ") incorrect nickname or password");
                                response = "Invalid data";
                            }
                            // todo create as thread
                            user.socket.Send(Encoding.Unicode.GetBytes(response));
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            // if (response == "Invalid data") { return; }
                            break;

                        case Request.RegRequest:
                            nickname = request[1];
                            password = request[2];
                            tempUser = StorageModel.dao.FindUser(nickname, password);
                            if (tempUser.nickname == null)
                            {
                                tempUser = StorageModel.dao.AddUser(new User(nickname, password, DateTime.Now));
                                user.id = tempUser.id;
                                user.nickname = tempUser.nickname;
                                user.lastVisitTime = tempUser.lastVisitTime;
                                user.subscriptionsId = tempUser.subscriptionsId;
                                Output("User registered: " + user.nickname);
                                response = Converter.SerializeUser(user);
                            }
                            else
                            {
                                Output("An attempt to registration a user(nickname: " + nickname + ") this nickname already registered");
                                response = "Invalid data";
                            }
                            // todo create as thread
                            user.socket.Send(Encoding.Unicode.GetBytes(response));
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            // if (response == "Invalid data") { return; }
                            break;

                        case Request.LastNewsRequest:
                            Output("Request from " + user.nickname + " for the latest news");
                            List<News> newsList = StorageModel.dao.GetNewsBetweenTimeInterval(user.subscriptionsId, user.lastVisitTime, DateTime.Now);
                            foreach (News news in newsList)
                            {
                                response = Converter.SerializeNews(news);
                                user.socket.Send(Encoding.Unicode.GetBytes(response));
                            }
                            Output(user.nickname + " received " + newsList.Count + " news");
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            break;

                        case Request.UserSubscriptionsRequest:
                            Output("Request from " + user.nickname + " for the your subscriptions");
                            subscriptions = StorageModel.dao.GetUserSubscriptions(user.subscriptionsId);
                            Output(user.nickname + " received " + subscriptions.Count + " subscriptions");
                            SendListOfSubscriptions(user.socket, subscriptions);
                            break;

                        case Request.AllSubscriptionsRequest:
                            Output("Request from " + user.nickname + " for the all subscriptions");
                            subscriptions = StorageModel.dao.GetAllSubscriptions();
                            Output(user.nickname + " received " + subscriptions.Count + " subscriptions");
                            SendListOfSubscriptions(user.socket, subscriptions);
                            break;

                        case Request.DeleteUserSubcription:
                            int delSubId = Int32.Parse(request[1].ToString());
                            Output("Request from " + user.nickname + " for delete subscription with id = " + delSubId);
                            StorageModel.dao.DeleteUserSubscription(user.id, delSubId);
                            user.subscriptionsId.Remove(delSubId);
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            break;

                        case Request.AddUserSubcription:
                            int addSubId = Int32.Parse(request[1].ToString());
                            Output("Request from " + user.nickname + " for add subscription with id = " + addSubId);
                            StorageModel.dao.AddUserSubscription(user.id, addSubId);
                            user.subscriptionsId.Add(addSubId);
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            break;

                        case Request.CloseConnection:
                            StorageModel.dao.UpdateLastVisitTime(user.id, DateTime.Now);
                            user.socket.Send(Encoding.Unicode.GetBytes("end"));
                            Output(user.nickname + " interrupted the connection");
                            user.socket.Close();
                            return;
                    }
                }
            }
        }

        public void SendListOfSubscriptions(Socket socket, List<Subscription> subscriptions)
        {
            string response;
            foreach (Subscription subscription in subscriptions)
            {
                response = Converter.Serialize(subscription);
                Output(response);
                socket.Send(Encoding.Unicode.GetBytes(response));
            }
            socket.Send(Encoding.Unicode.GetBytes("end"));
        }

        public void Output(string text)
        {
            this.Dispatcher.Invoke(() => { output.Add(text); });
        }

        public void LoadComboBoxItems()
        {
            TextBlock textBlock;
            List<Subscription> subscriptions = StorageModel.dao.GetAllSubscriptions();
            foreach (Subscription sub in subscriptions)
            {
                textBlock = new TextBlock();
                textBlock.Text = sub.name;
                SubscriptionsComboBox.Items.Add(textBlock);
            }
        }

        public void AddNewsEvent(object sender, RoutedEventArgs e)
        {
            Thread addNewsThread = new Thread(new ThreadStart(AddNews));
            addNewsThread.Start();
        }

        public void AddNews()
        {
            string name = GetTextOfTextBox(NewsNameTextBox);
            string text = GetTextOfTextBox(NewsTextTextBox);
            string subscriptionName = GetTextOfComboBox(SubscriptionsComboBox);
            Subscription subscription = StorageModel.dao.GetSubscription(subscriptionName);
            News news = new News(name, text, subscription.id, DateTime.Now);
            StorageModel.dao.AddNews(news);
            SetTextOfTextBox(NewsNameTextBox, "");
            SetTextOfTextBox(NewsTextTextBox, "");
            MessageBox.Show("News successfully added");
        }

        public string GetTextOfTextBox(TextBox textBox)
        {
            return this.Dispatcher.Invoke(() => { return textBox.Text; });
        }

        public void SetTextOfTextBox(TextBox textBox, string text)
        {
            this.Dispatcher.Invoke(() => { textBox.Text = text; });
        }

        public string GetTextOfComboBox(ComboBox comboBox)
        {
            return this.Dispatcher.Invoke(() => { return ((TextBlock)(comboBox.SelectedItem)).Text; });
        }

        public void OnWindowClose(object sender, CancelEventArgs e)
        {
            // waitConnectionsThread.Abort();
        }
    }
}
