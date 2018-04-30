using System;
using System.Collections.Generic;
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
        public MainWindow(string[] data)
        {
            InitializeComponent();
            User user = new User(Int32.Parse(data[0]), data[1], DateFormat.Parse(data[2]), (List<int>)Converter.DeserializeObject(data[3]));

            List<News> news = null; //= StorageModel.dao.GetNewsBetweenTimeInterval(user.id, user.lastVisitTime, DateTime.Now.ToString());

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

        }

        public void DeleteUserSubscription(object sender, RoutedEventArgs e)
        {

        }
    }
}
