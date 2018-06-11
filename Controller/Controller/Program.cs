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
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // data containers (dictionaries)
        static Dictionary<string, string> package_last = new Dictionary<string, string>(); // contains control and measurement values
        static bool connected_to_plant = false; // marker if a plant is connected to the plant (otherwise don't run cuntroller)

        // container for all controllers in the module
        static List<PID> PIDList = new List<PID>();

        static void Main(string[] args)
        {
            // parse the command line arguments
            string ip_gui_send = args[0];
            string ip_gui_recieve = args[1];
            int port_gui_send = Convert.ToInt16(args[2]);
            int port_gui_recieve = Convert.ToInt16(args[3]);
            string ip_plant_send = args[4];
            string ip_plant_recieve = args[5];
            int port_plant_send = Convert.ToInt16(args[6]);
            int port_plant_recieve = Convert.ToInt16(args[7]);

            // initialize the controller
            PID controller_pid = new PID(2, 70, 3);

            // create a thruead for sending to the GUI
            Thread thread_send_GUI = new Thread(() => send_GUI(ip_gui_send, port_gui_send, PIDList));
            thread_send_GUI.Start();

            // create a thread for listening on the GUI
            Thread thread_listen_GUI = new Thread(() => listen_GUI(ip_gui_recieve, port_gui_recieve, PIDList));
            thread_listen_GUI.Start();

            // create a thruead for sending to  the plant
            Thread thread_send_plant = new Thread(() => send_plant(ip_plant_send, port_plant_send, PIDList));
            thread_send_plant.Start();

            // create a thread for listening on the plant
            Thread thread_listen_plant = new Thread(() => listen_plant(ip_plant_recieve, port_plant_recieve, PIDList));
            thread_listen_plant.Start();

        }

        public static void send_GUI(string IP, int port, List<PID> PIDList_)
        {
            // initialize a connection to the GUI
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(100);

                // send time, u, and y
                string message = ConstructMessageGui(PIDList_);
                client.send(message);
            }
        }

        public static void listen_GUI(string IP, int port, List<PID> PIDList_)
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

                    // compute the new control signal for each controller (with updated r)
                    int index = 0;
                    foreach (PID controller in PIDList_)
                    {           
                        index++;
                        double reference = Convert.ToDouble(package_last["r" + index]);
                        double measurement = Convert.ToDouble(package_last["yc" + index]);

                        controller.update_parameters(Convert.ToDouble(package_last["Kp"]), Convert.ToDouble(package_last["Ki"]), Convert.ToDouble(package_last["Kd"]));

                        if (connected_to_plant)
                        {
                            controller.compute_control_signal(reference, measurement);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static public void send_plant(string IP, int port, List<PID> PIDList_)
        {
            // initialize a connection to the plant
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);

                // send u to the plant
                string message = ConstructMessagePlant(PIDList_);

                client.send(message);
                //Console.WriteLine("sent plant: " + message);
            }
        }

        public static void listen_plant(string IP, int port, List<PID> PIDList_)
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

                    // compute the new control signal for each controller
                    int index = 0;
                    foreach (PID controller in PIDList_)
                    {
                        index++;
                        double reference = Convert.ToDouble(package_last["r" + index]);
                        double measurement = Convert.ToDouble(package_last["yc" + index]);
                        //Console.WriteLine("reci: " + "yc" + index + ":" + measurement.ToString());
                        controller.compute_control_signal(reference, measurement);
                    }
                    connected_to_plant = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }

        public static string ConstructMessageGui(List<PID> PIDList_)
        {
            string message = "";
            message += Convert.ToString("time_" + DateTime.UtcNow.ToString(FMT));


            int index = 0;
            foreach (PID controller in PIDList_)
            {
                index++;
                message += "#u" + index + "_" + controller.get_u();
            }

            for (int i = 0; i < package_last.Keys.Count(); i++)
            {
                try
                {            
                    var item = package_last.ElementAt(i);
                    string key = item.Key;

                    if (key.Contains("yo"))
                    {
                        message += "#" + key + "_" + package_last[key].ToString();
                    }
                    else if (key.Contains("yc"))
                    {
                        message += "#" + key + "_" + package_last[key].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return message;
        }

        public static string ConstructMessagePlant(List<PID> PIDList_)
        {
            string message = "";

            int index = 0;
            foreach (PID controller in PIDList_)
            {
                index++;
                message += "#u" + index + "_" + controller.get_u();
            }
            try
            {
                message = message.Substring(1);
            }
            catch { }
            return message;
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

                    // add controller to detected output 
                    if (key.Contains("yc"))
                    {
                        PIDList.Add(new PID(1, 1, 1));

                        package_last.Add("r" + key.Substring(2), "0");
                    }
                }
                package_last[key] = value;
            }
        }
    }

    public struct PIDparameters
    {
        public double Kp, Ki, Kd;

        public PIDparameters(double Kp_, double Ki_, double Kd_)
        {
            Kp = Kp_;
            Ki = Ki_;
            Kd = Kd_;
        }
    }


    public class PID
    {
        private bool anti_wind_up = true;

        // states
        private double I = 0; // error integral
        private double e = 0, ep = 0; // current and prior error
        private double de = 0; // error derivative
        private double de_temp = 0; // error derivative holder
        private double u = 0; // control signal
        private DateTime update_last = DateTime.Now; // time stamp of prior execution

        // controller coefficients
        private double Kp; // proportional
        private double Ki; // integrator
        private double Kd; // derivator

        // controller limitations
        private double u_max = 7.5;
        private double u_min = -7.5*0;

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
            DateTime nowTime = DateTime.Now;

            // double dt = 0.005;

            // calculte error
            e = r - y;

            if (update_last != null)
            {
                double dt = (nowTime - update_last).TotalSeconds;

                //integrator with anti wind-up
                if (anti_wind_up == true)
                {
                    // only add intergral action if the control signal is not saturated
                    if (u > u_min  && u < u_max) 
                    {
                        I += dt * e;
                    }
                }
                else
                {
                    I += dt * e;
                }

                // derivator
                de = 1 / (dt + 1) * y - de_temp;
                de_temp = (y - de) / (dt + 1);

                // control signal
                u = Kp * e + Ki * I - Kd * de;

                ep = e; // update prior error

                // saturation
                if (u > u_max) u = u_max;
                if (u < u_min) u = u_min;               
            }

            // update prior time
            update_last = nowTime;
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
