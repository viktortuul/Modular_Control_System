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
        public void Send(string message)
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

        public Server(string IP, int Port) // (ip address of the sender; listenting port on this machine)
        {
            IPAddress IP_address;

            if (IP == "ANY_IP") IP_address = IPAddress.Any;
            else IP_address = IPAddress.Parse(IP);

            serverEP = new IPEndPoint(IP_address, Port);
            listener = new UdpClient(Port);
        }

        public void Listen()
        {
            byte[] receivedBytes = listener.Receive(ref serverEP);
            last_recieved = Encoding.ASCII.GetString(receivedBytes);
        } 
    }


    // end-point container
    public struct AddressEndPoint
    {
        public string IP;
        public int Port;

        public AddressEndPoint(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
        }

        public override string ToString()
        {
            return IP + ":" + Port;
        }
    }

    // same as the end-point container, but also contains a local port
    public struct ConnectionParameters
    {
        public string IP;
        public int Port, PortThis;

        public ConnectionParameters(string IP, int Port, int PortThis)
        {
            this.IP = IP;
            this.Port = Port;
            this.PortThis = PortThis;
        }
    }



}
