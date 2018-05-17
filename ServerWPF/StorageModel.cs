using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    class StorageModel
    {
        public static DBConnector dao = new MySqlDBConnector("127.0.0.1", "root", "1234", "newsletter");
    }
}
