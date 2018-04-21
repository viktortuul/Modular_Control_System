using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Threading;
using Communication;

namespace Model_watertank
{
    class Program
    {

        static void Main(string[] args)
        {
            // initialize the watertank model
            double dt = 0.01;
            Watertank plant = new Watertank(dt);

            string ip_controller_send = args[0];
            string ip_plant_recieve = args[1];
            int port_controller_send = Convert.ToInt16(args[2]);
            int port_plant_recieve = Convert.ToInt16(args[3]);

            // create a thread for listening on the controller
            Thread thread_listen_controller = new Thread(() => listen_controller(ip_plant_recieve, port_plant_recieve, plant));
            thread_listen_controller.Start();

            // create a thread for communication with the controller
            Thread thread_send_controller = new Thread(() => send_controller(ip_controller_send, port_controller_send, plant));
            thread_send_controller.Start();

            // create a thread for the watertank simulation
            Thread watertank = new Thread(() => process(plant));
            watertank.Start();

        }

        static public void listen_controller(string IP, int port, Watertank plant)
        {
            // initialize a connection to the controller
            Server server = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    server.listen();
                    plant.set_u(Convert.ToDouble(server.last_recieved));
                }
                catch { }
            }
        }

        static public void send_controller(string IP, int port, Watertank plant)
        {
            // initialize a connection to the controller
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);

                // send y (and recieve u)
                string y1 = Convert.ToString(plant.get_y1());
                string y2 = Convert.ToString(plant.get_y2());
                client.send("y1_" + y1 + "#y2_" + y2);
            }
        }

        static public void process(Watertank plant)
        {
            while (true)
            {
                Thread.Sleep(10);
                plant.update_state();
                Console.WriteLine("y1: " + Math.Round(plant.get_y1(), 2) + "y2: " + Math.Round(plant.get_y2(), 2));
            }
        }
    }


    class Watertank
    {
        // internal states
        double h1 = 0; // height tank 1 (top)
        double h2 = 0; // height tank 2 (bottom)

        // signals
        double u = 0; // control signal
        double w; // inflow

        // parameters
        double alpha1 = 0.1; // outflow characteristics  tank 1
        double alpha2 = 0.1; // outflow characteristics tank 2
        double g = 9.81;

        double dt; // time step

        public Watertank(double _dt)
        {
            dt = _dt;
        }

        public void update_state()
        {
            w = 0.01 * u;
            h1 += w; // inflow

            // top tank
            h1 = h1 - dt * (2 * alpha1 * Math.Sqrt(2 * g * h1));
            if (h1 < 0) h1 = 0; // empty tank

            // bottom tank
            h2 = h2 + dt * (2 * alpha1 * Math.Sqrt(2 * g * h1)) - dt * (2 * alpha2 * Math.Sqrt(2 * g * h2));
            if (h2 < 0) h2 = 0; // empty tank
        }

        public double get_y1()
        {
            return h1;
        }

        public double get_y2()
        {
            return h2;
        }

        public void set_u(double _u)
        {
            u = _u;
        }

    }
}
