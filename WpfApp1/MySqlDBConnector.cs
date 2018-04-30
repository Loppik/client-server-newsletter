using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            string sqlRequest = "SELECT name, text FROM newsletter.news WHERE subscription_id = " + userId + " AND Date(datetime) > Date('" + afterDatetime + "') AND Date(datetime) < Date('" + untilDatetime + "');";

            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            while (reader.Read())
            {
                news = new News(reader[0].ToString(), reader[1].ToString(), 1, "date!");
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
            string sqlRequest = "SELECT subscriptions_id FROM newsletter.user WHERE id = " + userId;
            MySqlDataReader reader = GetReaderOfCommandExecute(sqlRequest);
            reader.Read(); // todo change only for one data
            List<int> subscriptionsId = (List<int>)DeserializeObject(reader[0].ToString());
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

        public MySqlDataReader GetReaderOfCommandExecute(string sqlRequest)
        {
            MySqlCommand command = new MySqlCommand(sqlRequest, connection);
            MySqlDataReader reader = command.ExecuteReader();
            return reader;
        }

        public static string SerializeListOfInt(List<int> toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static object DeserializeObject(string forDeserialize)
        {
            List<int> i = new List<int>();
            var serializer = new XmlSerializer(i.GetType());
            object result;

            using (TextReader reader = new StringReader(forDeserialize))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
    }
}
