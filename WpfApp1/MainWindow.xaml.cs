using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using NewslettersClassLibrary;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User user;

        public MainWindow()
        {
            InitializeComponent();
            
            user = Authorization.user;
            user.socket = Authorization.socket;
            //user = new User(Int32.Parse(data[0]), data[1], DateFormat.Parse(data[2]), (List<int>)Converter.DeserializeObject(data[3]));
            //user.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2000);
            //user.socket.Connect(endPoint);// todo 
            List<News> news = GetLastNews();
            //List<News> news = StorageModel.dao.GetNewsBetweenTimeInterval(user.id, user.lastVisitTime, DateTime.Now.ToString());
            
            Canvas newCanvas;
            TextBlock textBlockName;
            TextBlock textBlockText;
            foreach (News n in news)
            {
                newCanvas = new Canvas();
                newCanvas.Height = 130;
                textBlockName = new TextBlock();
                textBlockText = new TextBlock();
                textBlockName.Text = n.name;
                textBlockText.Text = n.text;
                textBlockText.Padding = new Thickness(0, 20, 0, 0);
                newCanvas.Children.Add(textBlockName);
                newCanvas.Children.Add(textBlockText);
                StackNews.Children.Add(newCanvas);
            }

            StackNews.UpdateLayout();
            /*
            //  alex = new User(2, "Alex", "2018-04-12 13:13:13", new List<int>() { 1, 2 });
            List<Subscription> userSubscriptions = null;//= StorageModel.dao.GetUserSubscriptions(user.id); // todo сделать поиск по id подписки
            TextBlock textBlockUserSubscriptions;
            foreach (Subscription sub in userSubscriptions)
            {
                textBlockUserSubscriptions = new TextBlock();
                textBlockUserSubscriptions.Text = sub.name;
                StackUserSubscriptions.Children.Add(textBlockUserSubscriptions);
            }

            StackUserSubscriptions.UpdateLayout();

            List<Subscription> allSubscriptions = null;//StorageModel.dao.GetAllSubscriptions();
            TextBlock textBlockAllSubscriptions;
            foreach (Subscription sub in allSubscriptions)
            {
                textBlockAllSubscriptions = new TextBlock();
                textBlockAllSubscriptions.Text = sub.name;
                StackAllSubscriptions.Children.Add(textBlockAllSubscriptions);
            }

            StackAllSubscriptions.UpdateLayout();
            */

        }

        public List<News> GetLastNews()
        {
            List<News> newsList = new List<News>();
            List<string> responses = Request.Send(user.socket, "/news");
            foreach (string response in responses)
            {
                newsList.Add((News)Converter.DeserializeObject(response, Converter.NewsType));
            }
            return newsList;
        }

        public void DeleteUserSubscription(object sender, RoutedEventArgs e)
        {

        }
    }
}
