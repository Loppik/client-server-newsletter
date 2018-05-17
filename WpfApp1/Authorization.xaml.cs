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
        private static Window window;

        public Authorization()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            socket.Connect(endPoint);
        }
        

        public void UserConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                string request = Request.AuthorizationRequest + "*" + tbNickname.Text + "*" + tbPassword.Password;
                List<string> responses = Request.Send(socket, request);
                foreach (string response in responses)
                {
                    if (response != "Invalid data")
                    {
                        user = (User)Converter.DeserializeObject(response, Converter.UserType);
                        window = App.Current.MainWindow;
                        window.Hide();
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
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void RegUser(object sender, RoutedEventArgs e)
        {
            try
            {
                string request = Request.RegRequest + "*" + tbNickname.Text + "*" + tbPassword.Password;
                List<string> responses = Request.Send(socket, request);
                foreach (string response in responses)
                {
                    if (response != "Invalid data")
                    {
                        user = (User)Converter.DeserializeObject(response, Converter.UserType);
                        window = App.Current.MainWindow;
                        window.Hide();
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("User with this nickname already registered");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }
        }
        public static void CloseWindow()
        {
            socket.Close();
            window.Close();
        }
    }
}
