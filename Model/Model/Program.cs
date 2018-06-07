using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

using Communication;
using PhysicalProcesses;

namespace Model_Watertank
{
    class Program
    {
        // measurement noise


        // data containers (dictionaries)
        static Dictionary<string, string> package_last = new Dictionary<string, string>(); // recieved package <tag, value>

        static void Main(string[] args)
        {
            // parse the command line arguments
            string ip_controller_send = args[0];
            string ip_plant_recieve = args[1];
            int port_controller_send = Convert.ToInt16(args[2]);
            int port_plant_recieve = Convert.ToInt16(args[3]);

            // initialize model
            DoubleWatertank watertank = new DoubleWatertank();
            //QuadWatertank watertank = new QuadWatertank();

            // store thge model in a container which generically send and access values 
            Plant plant = new Plant(watertank);

            // create a thread for listening on the controller
            Thread thread_listen_controller = new Thread(() => listen_controller(ip_plant_recieve, port_plant_recieve, plant));
            thread_listen_controller.Start();

            // create a thread for communication with the controller
            Thread thread_send_controller = new Thread(() => send_controller(ip_controller_send, port_controller_send, plant));
            thread_send_controller.Start();

            // create a thread for the DoubleWatertank simulation
            double dt = 0.01; // model update rate
            Thread DoubleWatertank = new Thread(() => process(plant, dt));
            DoubleWatertank.Start();

        }

        static public void listen_controller(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Server server = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    server.listen();
                    parse_message(server.last_recieved);

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

        static public void send_controller(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);

                // send measurements y      
                string message = "";

                // observed states
                int counter = -1;
                for (int i = 0; i < plant.get_yo().Length; i++)
                {
                    counter++;
                    message += "yo" + (i + 1) + "_" + (plant.get_yo()[i]).ToString() + "#";
                }

                // controlled states
                counter = -1;
                for (int i = 0; i < plant.get_yc().Length; i++)
                {
                    counter++;
                    message += "yc" + (i + 1) + "_" + plant.get_yc()[i].ToString() + "#";
                }

                message = message.Substring(0, message.LastIndexOf('#'));
                //Console.WriteLine("sent: " + message);
                client.send(message);
            }
        }

        static public void process(Plant plant, double dt_)
        {
            int dt = Convert.ToInt16(1000 * dt_); // convert s to ms
            while (true)
            {
                Thread.Sleep(dt);
                plant.update_state(new double[] { 0, 0, 0, 0 });
                // Console.WriteLine("y1: " + Math.Round(plant.get_y()[0], 2) + "  y2: " + Math.Round(plant.get_y()[1], 2));
            }
        }

        public static void parse_message(string message)
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

                if (package_last.ContainsKey(key) == false)
                {
                    package_last.Add(key, value);
                }
                package_last[key] = value;
            }
        }
    }
}
