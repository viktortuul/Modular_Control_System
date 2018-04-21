using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Xml.Linq;

namespace Communication
{
    public class Client
    {
        UdpClient clientSocket = new UdpClient();

        public Client(string IP, int port)
        {
            clientSocket.Connect(IP, port);
        }
        public void send(string message)
        {
            try
            {
                var data = Encoding.ASCII.GetBytes(message);
                clientSocket.Send(data, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    
    public class Server
    {
        public string last_recieved = "0";

        UdpClient listener;
        IPAddress ip_address;
        int port;
        IPEndPoint serverEP;

        public Server(string ip_, int port_)
        {
            ip_address = IPAddress.Parse(ip_); // convert to an ip address
            port = port_;
            serverEP = new IPEndPoint(ip_address, port);
            listener = new UdpClient(port);
        }

        public void listen()
        {
            byte[] receivedBytes = listener.Receive(ref serverEP);
            last_recieved = Encoding.ASCII.GetString(receivedBytes);
        } 
    }

}
