using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Communication;
using System.Globalization;

namespace Controller
{
    public class MainClass
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // data container size
        public static int n_steps = 100;
  
        // data containers (dictionaries)
        static Dictionary<string, DataContainer> recieved_packages = new Dictionary<string, DataContainer>();
        static Dictionary<string, DataContainer> references = new Dictionary<string, DataContainer>();

        // container for all controllers in the module
        private static List<PID> PIDList = new List<PID>();

        // state variables
        static bool is_listening_on_plant = false;

        static void Main(string[] args)
        {
            // parse the command line arguments
            string IP_GUI = args[0];
            int port_GUI_endpoint = Convert.ToInt16(args[1]);
            int port_GUI_recieve = Convert.ToInt16(args[2]);
            string IP_plant = args[3];
            int port_plant_endpoint = Convert.ToInt16(args[4]);
            int port_plant_recieve = Convert.ToInt16(args[5]);

            // create a thread for sending to the GUI
            Thread thread_send_GUI = new Thread(() => SendGUI(IP_GUI, port_GUI_endpoint, PIDList));
            thread_send_GUI.Start();

            // create a thread for listening on the GUI
            Thread thread_listen_GUI = new Thread(() => ListenGUI(IP_GUI, port_GUI_recieve, PIDList));
            thread_listen_GUI.Start();

            // create a thread for sending to the plant
            Thread thread_send_plant = new Thread(() => SendPlant(IP_plant, port_plant_endpoint, PIDList));
            thread_send_plant.Start();

            // create a thread for listening on the plant
            Thread thread_listen_plant = new Thread(() => ListenPlant(IP_plant, port_plant_recieve, PIDList));
            thread_listen_plant.Start();
        }

        public static void SendGUI(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the GUI
            Client Sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(100);
                if (is_listening_on_plant == true)
                {
                    // send time, u, and y
                    string message = ConstructMessageGUI(PIDList);
                    Sender.send(message);
                }
            }
        }

        public static void ListenGUI(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the GUI
            Server Listener = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(10);
                // send y and u (and recieve data)
                try
                {
                    Listener.listen();
                    ParseMessage(Listener.last_recieved);

                    // compute the new control signal for each controller (with updated r)
                    ComputeControlSignals(PIDList, "from_GUI");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static public void SendPlant(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the plant
            Client Sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);

                if (is_listening_on_plant == true)
                {
                    // send u to the plant
                    string message = ConstructMessagePlant(PIDList);

                    Sender.send(message);
                    //Console.WriteLine("sent plant: " + message);
                }
            }
        }

        public static void ListenPlant(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the plant
            Server Listerner = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    Listerner.listen();
                    ParseMessage(Listerner.last_recieved);

                    // compute the new control signal for each controller
                    ComputeControlSignals(PIDList, "from_Plant");

                    is_listening_on_plant = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static string ConstructMessageGUI(List<PID> PIDList)
        {
            string message = "";
            message += Convert.ToString("time_" + DateTime.UtcNow.ToString(FMT));

            // append control signals
            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;
                message += "#u" + index + "_" + controller.get_u();
            }

            // append measurements signals
            for (int i = 0; i < recieved_packages.Keys.Count(); i++)
            {   
                var item = recieved_packages.ElementAt(i);
                string key = item.Key;

                if (key.Contains("y")) message += "#" + key + "_" + recieved_packages[key].GetLastValue();                 
            }
            return message;
        }

        public static string ConstructMessagePlant(List<PID> PIDList)
        {
            string message = "";

            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;
                message += "#u" + index + "_" + controller.get_u();
            }

            // remove the first delimiter
            if (message.Length > 0) message = message.Substring(1);

            return message;
        }

        public static void ParseMessage(string message)
        {
            string time = ""; // denotes the time stamp
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

                // detect the time (don't add it as a separate key)
                if (key == "time")
                {
                    time = value;
                    continue;
                }

                // detect corrupt values
                if (isDouble(value) == false)
                {
                    Console.WriteLine("Error: corrupt package  <" + key + "_" + value + ">");
                    continue;
                }

                // if a new key recieved, add it
                if (recieved_packages.ContainsKey(key) == false)
                {
                    recieved_packages.Add(key, new DataContainer(n_steps));

                    // add controller and reference tracker
                    if (key.Contains("yc")) PIDList.Add(new PID());
                }

                // insert the recieved data to corresponding tag
                recieved_packages[key].InsertData(time, value);
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

        private static void ComputeControlSignals(List<PID> PIDList, string mode)
        {
            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;

                // check if corresponing reference value exist
                if (recieved_packages.ContainsKey("r" + index) == false) continue;

                // check if both the reference and last recieved measurement are up to date (else don't update the control signal) 
                if (recieved_packages["yc" + index].isUpToDate())
                {

                    double reference = Convert.ToDouble(recieved_packages["r" + index].GetLastValue());
                    double measurement = Convert.ToDouble(recieved_packages["yc" + index].GetLastValue());

                    // update control signal
                    if (is_listening_on_plant) controller.ComputeControlSignal(reference, measurement);

                    // update controller parameters
                    if (mode == "from_GUI") controller.UpdateParameters(Convert.ToDouble(recieved_packages["Kp"].GetLastValue()),
                                                                        Convert.ToDouble(recieved_packages["Ki"].GetLastValue()),
                                                                        Convert.ToDouble(recieved_packages["Kd"].GetLastValue()));
                }

            }
        }
    }


    public struct PIDparameters
    {
        public double Kp, Ki, Kd;
        
        public PIDparameters(double Kp, double Ki, double Kd)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
        }
    }


    public class PID
    {
        // anti wind-up
        private bool anti_wind_up = true;

        // flags
        private bool parameters_assigned = false;

        // states
        private double I = 0; // error integral
        private double e = 0, ep = 0; // current and prior error
        private double de = 0; // error derivative
        private double de_temp = 0; // error derivative holder
        private double u = 0; // control signal
        private DateTime update_last = DateTime.Now; // time stamp of prior execution

        // controller coefficients
        private double Kp; // proportional
        private double Ki; // integral
        private double Kd; // derivative

        // controller limitations
        private double u_max = 7.5;
        private double u_min = -7.5*0;

        // constructor
        public PID() { }

        public void ComputeControlSignal(double r, double y)
        {
            // calculate the dime duration from the last update
            DateTime nowTime = DateTime.Now;

            if (parameters_assigned)
            {
                // calculte error
                e = r - y;

                if (update_last != null)
                {
                    double dt = (nowTime - update_last).TotalSeconds;

                    //integrator with anti wind-up (only add intergral action if the control signal is not saturated)
                    if (anti_wind_up == true)
                        if (u > u_min && u < u_max) I += dt * e;
                    else
                        I += dt * e;

                    // derivator (with low pass)
                    de = 1 / (dt + 1) * y - de_temp;
                    de_temp = (y - de) / (dt + 1);

                    // control signal
                    u = Kp * e + Ki * I - Kd * de;

                    ep = e; // update prior error

                    // saturation
                    if (u > u_max) u = u_max;
                    if (u < u_min) u = u_min;
                }
            }
            
            // update prior time
            update_last = nowTime;
        }

        public void UpdateParameters(double Kp, double Ki, double Kd)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
            parameters_assigned = true;
        }

        public double get_u()
        {
            return u;
        }
    }
}
