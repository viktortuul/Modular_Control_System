using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Threading;
using Communication;

namespace Controller
{
    public class Program
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fffffff";

        // other
        static double reference; // reference from GUI
        static Dictionary<string, string> dict = new Dictionary<string, string>(); // contains recieved values
        static bool connected_to_plant = false; // marker if a plant is connected to the plant (otherwise don't run cuntroller)

        static void Main(string[] args)
        {
            // initialize a dicationary
            dict.Add("u", "0");
            dict.Add("y1", "0");
            dict.Add("y2", "0");


            // initialize the controller
            PID controller_pid = new PID(1, 40, 3);

            // create a thruead for sending to  the GUI
            Thread thread_send_GUI = new Thread(() => send_GUI("127.0.0.1", 8885, controller_pid));
            thread_send_GUI.Start();

            // create a thread for listening on the GUI
            Thread thread_listen_GUI = new Thread(() => listen_GUI("127.0.0.1", 8886, controller_pid));
            thread_listen_GUI.Start();

            // create a thruead for sending to  the plant
            Thread thread_send_plant = new Thread(() => send_plant("127.0.0.1", 8887, controller_pid));
            thread_send_plant.Start();

            // create a thread for listening on the plant
            Thread thread_listen_plant = new Thread(() => listen_plant("127.0.0.1", 8888, controller_pid));
            thread_listen_plant.Start();

        }

        public static void send_GUI(string IP, int port, PID controller_pid)
        {
            // initialize a connection to the GUI
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(100);
                // send time, u, and y
                string message = Convert.ToString("time_" + DateTime.UtcNow.ToString(FMT) + "#u_" + controller_pid.get_u() + "#y1_" + dict["y1"] + "#y2_" + dict["y2"]);

                client.send(message);
            }
        }

        public static void listen_GUI(string IP, int port, PID controller_pid)
        {
            // initialize a connection to the GUI
            Server server = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(10);
                // send y and u (and recieve data)
                try
                {
                    server.listen();
                    parse_message(server.last_recieved);

                    // compute the new control signal (with updated r)
                    reference = Convert.ToDouble(dict["r"]);
                    double measurement = Convert.ToDouble(dict["y2"]);
                    controller_pid.update_parameters(Convert.ToDouble(dict["Kp"]), Convert.ToDouble(dict["Ki"]), Convert.ToDouble(dict["Kd"]));
                    if (connected_to_plant) controller_pid.compute_control_signal(reference, measurement);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static public void send_plant(string IP, int port, PID controller_pid)
        {
            // initialize a connection to the plant
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);
                // send u to the plant
                client.send(Convert.ToString(controller_pid.get_u()));
                Console.WriteLine("sent u: " + Math.Round(controller_pid.get_u(), 2));
            }
        }

        public static void listen_plant(string IP, int port, PID controller_pid)
        {
            // initialize a connection to the plant
            Server server = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);

                try
                {
                    server.listen();
                    parse_message(server.last_recieved);
                    //Console.WriteLine("recieved y: " + Math.Round(y,2));

                    // compute the new control signal (with updated y2)    
                    double measurement = Convert.ToDouble(dict["y2"]);
                    controller_pid.compute_control_signal(reference, measurement);

                    connected_to_plant = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

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

                if (dict.ContainsKey(key) == false)
                {
                    dict.Add(key, value);
                    }
                dict[key] = value;
            }
        }
    }


    public class PID
    {
        // states
        private double I = 0; // error integral
        private double e = 0, ep = 0; // current and prior error
        private double de = 0; // error derivative
        private double u = 0; // control signal
        private DateTime time_prior = DateTime.Now; // time stamp of prior execution

        // controller coefficients
        private double Kp; // proportional
        private double Ki; // integrator
        private double Kd; // derivator

        // controller limitations
        private double u_max = 5;
        private double u_min = 0;

        // constructor
        public PID(double Kp_, double Ki_, double Kd_) 
        {
            Kp = Kp_;
            Ki = Ki_;
            Kd = Kd_;
        }

        public void compute_control_signal(double r, double y)
        {
            // calculate the dime duration from the last update
            // double dt = Convert.ToDouble(DateTime.Now - time_prior);
            double dt = 0.1;

            // error
            e = r - y;

            //integrator
            I += dt*e;

            // derivator
            de = (e - ep) / dt;

            // control signal
            u = Kp*e + (1/Ki)*I + (1/Kd)*de;

            // saturation
            if (u > u_max) u = u_max;
            if (u < u_min) u = u_min;

            ep = e; // update prior error

            // update prior time
            // time_prior = DateTime.Now;
        }

        public void update_parameters(double Kp_, double Ki_, double Kd_)
        {
            Kp = Kp_;
            Ki = Ki_;
            Kd = Kd_;
        }

        public double get_u()
        {
            return u;
        }

    }
}
