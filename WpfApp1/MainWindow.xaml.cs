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
            DisplayNews(StackNews, news);
            //List<News> news = StorageModel.dao.GetNewsBetweenTimeInterval(user.id, user.lastVisitTime, DateTime.Now.ToString());
            
            
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
            List<string> responses = Request.Send(user.socket, Request.LastNewsRequest);
            foreach (string response in responses)
            {
                newsList.Add((News)Converter.DeserializeObject(response, Converter.NewsType));
            }
            return newsList;
        }

        public void DisplayNews(StackPanel stackPanel, List<News> news)
        {
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
                stackPanel.Children.Add(newCanvas);
            }

            stackPanel.UpdateLayout();
        }
        

        public void DeleteUserSubscription(object sender, RoutedEventArgs e)
        {

        }

        public List<Subscription> GetSubscriptions(string request)
        {
            List<Subscription> subsList = new List<Subscription>();
            List<string> responses = Request.Send(user.socket, request);
            foreach (string response in responses)
            {
                subsList.Add((Subscription)Converter.DeserializeObject(response, Converter.SubscriptionType));
            }
            return subsList;
        }

        public void DisplaySubscriptions(StackPanel stackPanel, List<Subscription> subscriptions)
        {
            Canvas newCanvas;
            TextBlock textBlockName;
            TextBlock textBlockDescription;
            Button button;
            foreach (Subscription sub in subscriptions)
            {
                newCanvas = new Canvas();
                newCanvas.Height = 70;
                textBlockName = new TextBlock();
                textBlockName.Text = sub.name;
                textBlockName.Width = 500;
                textBlockDescription = new TextBlock();
                textBlockDescription.Text = sub.description;
                textBlockDescription.Padding = new Thickness(0, 20, 0, 0);
                button = new Button();
                button.Width = 20;
                button.Content = "X";
                button.Margin = new Thickness(500, 0, 0, 0);
                button.Click += DeleteUserSubscription;
                newCanvas.Children.Add(textBlockName);
                newCanvas.Children.Add(button);
                newCanvas.Children.Add(textBlockDescription);
                stackPanel.Children.Add(newCanvas);
            }
            stackPanel.UpdateLayout();
        }

        public void UserSubscriptionsLoad(object sender, RoutedEventArgs e)
        {
            if (StackUserSubscriptions.Children.Count == 0)
            {
                List<Subscription> userSubscriptions = GetSubscriptions(Request.UserSubscriptionsRequest);
                DisplaySubscriptions(StackUserSubscriptions, userSubscriptions);
            }   
        }

        public void AllSubscriptionsLoad(object sender, RoutedEventArgs e)
        {
            if (StackAllSubscriptions.Children.Count == 0)
            {
                List<Subscription> allSubscriptions = GetSubscriptions(Request.AllSubscriptionsRequest);
                DisplaySubscriptions(StackAllSubscriptions, allSubscriptions);
            }
            
        }
    }
}
