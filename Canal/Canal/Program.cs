using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using System.Threading;


namespace Canal
{
    class Program
    {
        static Bernoulli bernoulli = new Bernoulli(0.9);

        static void Main(string[] args)
        {
            // create a new thread for the listener
            Thread thread_listener = new Thread(() => Listener("ANY", 8000)); // listen on port 8000 (packages from any IP address)
            thread_listener.Start();
        }

        public static void Listener(string IP, int port)
        {
            // initialize a connection to the GUI
            Server Listener = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);

                try
                {
                    Listener.listen();
                    ParseMessage(Listener.last_recieved);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static void SendMessage(string IP, int port, string message)
        {
            // initialize a sender
            Client Sender = new Client(IP, port);

            // send message (if it's alloed to pass)
            if (bernoulli.Next() == true)
            {
                Sender.send(message);
                Console.WriteLine("sent: " + message);
            }
            else
                Console.WriteLine("message blocked");
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
                    string[] EP = item.Split(':');
                    string IP = EP[0];
                    int Port = Convert.ToInt16(EP[1]);
                    string submessage = message.Substring((key + "_" + EP).Length + 1); // remove the EP part
                    SendMessage(IP, Port, submessage);
                }
            }
        }
    }


    class Bernoulli
    {
        double treshold;
        Random r = new Random();

        public Bernoulli(double treshold)
        {
            this.treshold = treshold;
        }

        public bool Next()
        {
            return (r.Next(0, 1) > treshold) ? true : false;
        }

    }


    class MarkovChain
    {
        int state; // 1:pass, 0:drop
        double[] transition = new double[] { 0, 0 }; // state transition probabilities
        Random r = new Random();

        public MarkovChain(int state, double pass2drop, double drop2pass)
        {
            this.state = state;
            transition[0] = drop2pass;
            transition[1] = pass2drop;
        }

        public bool GetState()
        {
            Next();
            return (state == 1) ? true : false;
        }

        public void Next()
        {
            double p = r.Next(0, 1);
            if (p > transition[state]) state = (state + 1) % 2;
        }
    }
}
