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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            string[] userData = { "1", "loppik", "2018-01-01 13:13:13", "<?xml version=\"1.0\" encoding=\"utf-16\"?><ArrayOfInt xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><int>1</int><int>2</int></ArrayOfInt>" };
            User user = new User(Int32.Parse(userData[0]), userData[0], userData[0], (List<int>)MySqlDBConnector.DeserializeObject(userData[0]));
            List<News> news = StorageModel.dao.GetNewsBetweenTimeInterval(user.id, user.lastVisitTime, DateTime.Now.ToString());

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
            List<Subscription> userSubscriptions = StorageModel.dao.GetUserSubscriptions(user.id); // todo сделать поиск по id подписки
            TextBlock textBlockUserSubscriptions;
            foreach (Subscription sub in userSubscriptions)
            {
                textBlockUserSubscriptions = new TextBlock();
                textBlockUserSubscriptions.Text = sub.name;
                StackUserSubscriptions.Children.Add(textBlockUserSubscriptions);
            }

            StackUserSubscriptions.UpdateLayout();

            List<Subscription> allSubscriptions = StorageModel.dao.GetAllSubscriptions();
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
