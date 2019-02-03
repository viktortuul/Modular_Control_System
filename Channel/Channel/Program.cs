using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using System.Threading;
using System.Reflection;

namespace Canal
{
    class Program
    {
        // drop out models
        static Bernoulli Bernoulli;
        static MarkovChain Markov;
        static Object DropOutModel = null;

        static void Main(string[] args)
        {
            // parse the command line arguments
            int port_recieve = Convert.ToInt16(args[0]); // listening port

            if (args.Length == 2) // Bernoulli
            {
                double treshold = Convert.ToDouble(args[1]);
                Bernoulli = new Bernoulli(treshold / 100);
                DropOutModel = Bernoulli;
            }
            else if (args.Length == 3) // Markov
            {
                double P_pd = Convert.ToDouble(args[1]);
                double P_dp = Convert.ToDouble(args[2]);
                Markov = new MarkovChain(1, (100 - P_pd) / 100, (100 - P_dp) / 100);
                DropOutModel = Markov;
            }

            // create a new thread for the listener
            Thread thread_listener = new Thread(() => Listener("ANY_IP", port_recieve)); // listen on port 8000 (packages from any IP address)
            thread_listener.Start();
        }

        public static void Listener(string IP, int port)
        {
            // initialize a connection to the GUI
            Server Listener = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(1);
                try
                {
                    Listener.Listen();
                    ParseMessage(Listener.last_recieved);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static void ParseMessage(string message)
        {
            string text = message;

            // split the message with the delimiter '#'
            string[] container = text.Split('#');

            foreach (string item in container)
            {
                // split each subtext (key and value)
                string[] subitem = item.Split('_');

                // extract key and value
                string key = subitem[0];
                string value = subitem[1];

                // detect the end point address
                if (key == "EP")
                {
                    // extract key and value
                    string[] EP = value.Split(':');
                    string IP = EP[0];
                    int Port = Convert.ToInt16(EP[1]);
                    string submessage = message.Substring((key + "_" + EP).Length); // remove the EP part
                    SendMessage(IP, Port, submessage);
                    return; // no need to keep parsing the message
                }
                //if (key == "ORIGIN") // detect the ORIGIN
                //{
                //    string origin_ID = value;
                //}
            }
        }

        public static void SendMessage(string IP, int port, string message)
        {
            // initialize a sender
            Client Sender = new Client(IP, port);

            // drop out
            MethodInfo isPass = DropOutModel.GetType().GetMethod("isPass");
            bool pass = (bool)isPass.Invoke(DropOutModel, null);

            if (pass == true)
            {
                Sender.Send(message);
                Console.WriteLine("Pass: " + message);
            }
            else
                Console.WriteLine("Drop - " + DropOutModel.ToString());
        }
    }
}
