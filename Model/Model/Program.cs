using System;
using System.Collections.Generic;
using System.Threading;
using Communication;
using PhysicalProcesses;

namespace Model_Watertank
{
    class Program
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // data containers (dictionaries)
        static Dictionary<string, string> package_last = new Dictionary<string, string>(); // recieved package <tag, value>

        // EP addresses
        static string IP_controller;
        static int port_controller_endpoint;

        static void Main(string[] args)
        {
            // parse the command line arguments
            IP_controller = args[0];
            port_controller_endpoint = Convert.ToInt16(args[1]);
            int port_plant_this = Convert.ToInt16(args[2]);
            string model_type = args[3];

            // CANAL PARAMS
            string IP_controller_canal = "127.0.0.1";
            int port_controller_canal = 8222;

            // store the model in a container which generically send and access values 
            Plant plant = new Plant();
            switch (model_type)
            {
                case "dwt": plant = new Plant(new DoubleWatertank()); break;
                case "qwt": plant = new Plant(new QuadWatertank()); break;
                case "ipsiso": plant = new Plant(new InvertedPendulumSISO()); break;
            }

            // create a thread for listening on the controller
            Thread thread_listen_controller = new Thread(() => Listener(IP_controller_canal, port_plant_this, plant));
            thread_listen_controller.Start();

            // create a thread for communication with the controller
            Thread thread_send_controller = new Thread(() => Sender(IP_controller_canal, port_controller_canal, plant));
            thread_send_controller.Start();

            //// create a thread for listening on the controller
            //Thread thread_listen_controller = new Thread(() => Listener(IP_controller, port_plant_this, plant));
            //thread_listen_controller.Start();

            //// create a thread for communication with the controller
            //Thread thread_send_controller = new Thread(() => Sender(IP_controller, port_controller_endpoint, plant));
            //thread_send_controller.Start();

            // create a thread for the DoubleWatertank simulation
            double dt = 0.01; // model update rate
            Thread DoubleWatertank = new Thread(() => Process(plant, dt));
            DoubleWatertank.Start();

        }

        static public void Listener(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Server listener = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    listener.Listen();
                    ParseMessage(listener.last_recieved);

                    // parse dictionary to proper input
                    double[] u = new double[package_last.Count];
                    int i = -1;
                    foreach (string key in package_last.Keys)
                    {
                        i++;
                        u[i] = Convert.ToDouble(package_last[key]);
                        //Console.WriteLine(key + ":" + u[i]);
                    }
                    plant.set_u(u);
                }
                catch { }
            }
        }

        static public void Sender(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Client sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(50);

                // send measurements y      
                string message = "";
                message += Convert.ToString("EP_" + IP_controller + ":" + port_controller_endpoint + "#"); // EP FOR CANAL
                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(FMT) + "#");

                // observed states
                for (int i = 0; i < plant.get_yo().Length; i++)
                    message += "yo" + (i + 1) + "_" + (plant.get_yo()[i]).ToString() + "#";

                // controlled states
                for (int i = 0; i < plant.get_yc().Length; i++)
                    message += "yc" + (i + 1) + "_" + plant.get_yc()[i].ToString() + "#";
                
                message = message.Substring(0, message.LastIndexOf('#')); // remove the last delimiter '#'
                sender.Send(message);
            }
        }

        static public void Process(Plant plant, double dt_)
        {
            int dt = Convert.ToInt16(1000 * dt_); // convert s to ms
            while (true)
            {
                Thread.Sleep(dt);
                plant.update_state(new double[] { 0, 0, 0, 0 });
                // Console.WriteLine("y1: " + Math.Round(plant.get_y()[0], 2) + "  y2: " + Math.Round(plant.get_y()[1], 2));
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

                // detect corrupt values
                if (isDouble(value) == false)
                {
                    Console.WriteLine("Error: corrupt package  <" + key + "_" + value + ">");
                    continue;
                }

                if (package_last.ContainsKey(key) == false)
                    package_last.Add(key, value);
                
                package_last[key] = value;
            }
        }


        public static bool isDouble(String str)
        {
            try
            {
                Double.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
