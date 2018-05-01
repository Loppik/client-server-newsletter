using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Request
    {
        public static List<string> Send(Socket socket, string request)
        {
            List<string> responses = new List<string>();
            string response;
            byte[] bytes = new byte[1024];
            bytes = Encoding.Unicode.GetBytes(request);
            socket.Send(bytes);
            while (true)
            {
                bytes = new byte[1024];
                int len = socket.Receive(bytes);
                if (len > 0)
                {
                    response = Encoding.Unicode.GetString(bytes, 0, len);
                    if (response == "end")
                        break;
                    responses.Add(response);
                }
            }
            

            return responses;
        }
       
    }
}
