using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NewslettersClassLibrary;

namespace ServerWPF
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

        public override List<News> GetNewsBetweenTimeInterval(List<int> subscriptionId, DateTime afterDatetime, DateTime untilDatetime)
        {
            List<News> newsList = new List<News>(5);
            if (subscriptionId.Count == 0)
                return newsList;
            News news;
            string sqlRequest = "SELECT * FROM newsletter.news WHERE subscription_id IN (" + string.Join(", ", subscriptionId.ToArray()) + ") AND datetime > '" + afterDatetime.ToString("yyyy-MM-dd hh:mm:ss") + "' AND datetime < '" + untilDatetime.ToString("yyyy-MM-dd hh:mm:ss") + "';";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            while (reader.Read())
            {
                news = new News(Int32.Parse(reader[0].ToString()), reader[1].ToString(), reader[2].ToString(), Int32.Parse(reader[3].ToString()), DateTime.Parse(reader[4].ToString()));
                newsList.Add(news);
            }
            reader.Close();
            return newsList;
        }

        public override List<Subscription> GetAllSubscriptions()
        {
            List<Subscription> subscriptions = new List<Subscription>();
            Subscription subscription;

            string sqlRequest = "SELECT * FROM newsletter.subscription";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            while (reader.Read())
            {
                subscription = new Subscription(Int32.Parse(reader[0].ToString()), reader[1].ToString(), reader[2].ToString());
                subscriptions.Add(subscription);
            }
            reader.Close();
            return subscriptions;
        }

        // todo Finish
        public override List<Subscription> GetUserSubscriptions(List<int> subscriptionsId)
        {
            List<Subscription> subscriptions = new List<Subscription>();
            if (subscriptionsId.Count == 0)
                return subscriptions;
            Subscription subscription;
            string sqlRequest;
            MySqlDataReader reader;
            sqlRequest = "SELECT * FROM newsletter.subscription WHERE id IN (" + string.Join(", ", subscriptionsId.ToArray()) + ")";
            reader = GetReaderOfCommandExecute(sqlRequest);
            while (reader.Read())
            {
                subscription = new Subscription(Int32.Parse(reader[0].ToString()), reader[1].ToString(), reader[2].ToString());
                subscriptions.Add(subscription);
            }
            reader.Close();

            return subscriptions;
        }
        
        public override User FindUser(string nickname, string password)
        {
            User user;
            string sqlRequest = "SELECT id, last_visit_time FROM newsletter.user WHERE nickname = '" + nickname + "' AND password = '" + password + "'"; // todo change
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            if (reader.Read())
            {
                int userId = Int32.Parse(reader[0].ToString());
                DateTime lastVisitTime = DateTime.Parse(reader[1].ToString());
                reader.Close();
                string subsRequest = "SELECT subscription_id FROM newsletter.users_subscriptions_id WHERE user_id = '" + userId + "'";
                MySqlDataReader subsReader = GetReaderOfCommandExecute(subsRequest);
                List<int> subscriptionsId = new List<int>();
                while (subsReader.Read())
                {
                    subscriptionsId.Add(Int32.Parse(subsReader[0].ToString()));
                }
                subsReader.Close();
                user = new User(userId, nickname, lastVisitTime, subscriptionsId);
            }
            else
            {
                reader.Close();
                user = new User();
            }
            return user;
        }

        public override void DeleteUserSubscription(int userId, int subscriptionId)
        {
            string sqlRequest = "DELETE FROM newsletter.users_subscriptions_id WHERE user_id = '" + userId + "' AND subscription_id = '" + subscriptionId + "'";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Close();
        }

        public override void AddUserSubscription(int userId, int subscriptionId)
        {
            string sqlRequest = "INSERT INTO newsletter.users_subscriptions_id (user_id, subscription_id) VALUES ('" + userId + "', '" + subscriptionId + "')";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Close();
        }

        public override void UpdateLastVisitTime(int userId, DateTime time)
        {
            string dateAndTime = time.ToString("yyyy-MM-dd hh:mm:ss");
            string request = "UPDATE newsletter.user SET last_visit_time = '" + dateAndTime + "' WHERE id = '" + userId + "'";
            MySqlDataReader reader = GetReaderOfCommandExecute(request);
            reader.Close();
        }

        public override void AddNews(News news)
        {
            string sqlRequest = "INSERT INTO newsletter.news (name, text, subscription_id, datetime) VALUES ('" + news.name + "', '" + news.text + "', '" + news.subscription + "', '" + news.datetime.ToString("yyyy/MM/dd hh:mm:ss") + "')";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Close();
        }

        public override Subscription GetSubscription(string subscriptionName)
        {
            string sqlRequest = "SELECT * FROM newsletter.subscription WHERE name = '" + subscriptionName + "'";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Read();
            Subscription subscription = new Subscription(Int32.Parse(reader[0].ToString()), reader[1].ToString(), reader[2].ToString());
            reader.Close();
            return subscription;
        }

        public override User AddUser(User user)
        {
            string sqlRequest = "INSERT INTO newsletter.user (nickname, password, last_visit_time) VALUES ('" + user.nickname + "', '" + user.password + "', '" + user.lastVisitTime.ToString("yyyy/MM/dd hh:mm:ss") + "')";
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Close();
            sqlRequest = "SELECT id FROM newsletter.user WHERE nickname = '" + user.nickname + "'";
            reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Read();
            int userId = Int32.Parse(reader[0].ToString());
            reader.Close();
            user.id = userId;
            user.subscriptionsId = new List<int>();
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
