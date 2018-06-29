using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Communication
{
    public class Client
    {
        UdpClient clientSocket = new UdpClient();

        public Client(string IP, int port) // (ip address of the reciever; listenting port on the reciever machine)
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
        IPEndPoint serverEP;

        public Server(string ip_, int port) // (ip address of the sender; listenting port on this machine)
        {
            IPAddress ip_address = IPAddress.Parse(ip_); 
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
