using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class News
    {
        public string name;
        public string text;
        public int subscription;
        public string datetime;

        public News(string name, string text, int subscription, string datetime)
        {
            this.name = name;
            this.text = text;
            this.subscription = subscription;
            this.datetime = datetime;
        }
    }
}
