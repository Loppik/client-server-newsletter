using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using NewslettersClassLibrary;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        public static Socket socket;
        int port = 2000;
        public static User user;

        public Authorization()
        {
            InitializeComponent();
        }
        

        public void UserConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                socket.Connect(endPoint);
                Console.WriteLine(tbNickname.Text + " " + tbPassword.Text);
                string request = Request.AuthorizationRequest + "*" + tbNickname.Text + "*" + tbPassword.Text;
                List<string> responses = Request.Send(socket, request);
                foreach (string response in responses)
                {
                    if (response != "Invalid request")
                    {
                        user = (User)Converter.DeserializeObject(response, Converter.UserType);
                        App.Current.MainWindow.Hide();
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Invalid nickname or password");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Error in Authorization");
            }
        }
    }
}
