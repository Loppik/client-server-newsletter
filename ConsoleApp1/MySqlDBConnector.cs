using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NewslettersClassLibrary;

namespace WpfApp1
{
    class MySqlDBConnector : DBConnector
    {
        public MySqlConnection connection;

        public MySqlDBConnector(string host, string userName, string password, string databaseName)
        {
            string connectionString = "server=" + host + ";user=" + userName + ";password=" + password + ";database=" + databaseName + ";SslMode=none";
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public override List<News> GetNewsBetweenTimeInterval(int userId, string afterDatetime, string untilDatetime)
        {
            List<News> newsList = new List<News>(5);
            News news;
            string sqlRequest = "SELECT * FROM newsletter.news WHERE subscriptionId = " + userId + " AND Date(datetime) > Date('" + afterDatetime + "') AND Date(datetime) < Date('" + untilDatetime + "');";

            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            while (reader.Read())
            {
                news = new News(reader[0].ToString(), reader[1].ToString(), Int32.Parse(reader[2].ToString()), DateFormat.Parse(reader[3].ToString()));
                newsList.Add(news);
            }
            reader.Close();
            return newsList;
        }

        public override List<Subscription> GetAllSubscriptions()
        {
            List<Subscription> subscriptions = new List<Subscription>();
            Subscription subscription;

            string sqlRequest = "SELECT name FROM newsletter.subscription";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            while (reader.Read())
            {
                subscription = new Subscription(1, reader[0].ToString());
                subscriptions.Add(subscription);
            }
            reader.Close();
            return subscriptions;
        }

        // todo Finish
        public override List<Subscription> GetUserSubscriptions(int userId)
        {
            List<Subscription> subscriptions = new List<Subscription>();
            Subscription subscription;

            // Get subscriptsId
            string sqlRequest = "SELECT subscriptionsId FROM newsletter.user WHERE id = " + userId;
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Read(); // todo change only for one data
            List<int> subscriptionsId = (List<int>)Converter.DeserializeObject(reader[0].ToString());
            reader.Close();

            foreach (int subscriptionId in subscriptionsId)
            {
                sqlRequest = "SELECT name FROM newsletter.subscription WHERE id = " + subscriptionId;
                reader = GetReaderOfCommandExecute(sqlRequest);
                while (reader.Read())
                {
                    subscription = new Subscription(1, reader[0].ToString());
                    subscriptions.Add(subscription);
                }
                reader.Close();
            }

            return subscriptions;
        }
        
        public override User FindUser(string nickname, string password)
        {
            User user;
            string sqlRequest = "SELECT * FROM newsletter.user WHERE nickname = '" + nickname + "' AND password = '" + "1234" + "'"; // todo change
            Console.WriteLine(sqlRequest);
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            if (reader.Read()) // todo change only for one data
            {
                user = new User((int)reader[0], reader[1].ToString(), DateFormat.Parse(reader[3].ToString()), (List<int>)Converter.DeserializeObject(reader[4].ToString()));
            }
            else
            {
                user = new User("noname");
            }
            reader.Close();
            return user;
        }

        public MySqlDataReader GetReaderOfCommandExecute(string sqlRequest)
        {
            MySqlCommand command = new MySqlCommand(sqlRequest, connection);
            MySqlDataReader reader = command.ExecuteReader();
            return reader;
        }
    }
}
