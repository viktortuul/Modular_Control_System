using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Threading;
using Communication;
using System.Globalization;

namespace Controller
{
    public class Program
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // data container size
        public static int n_steps = 500;

        // data containers (dictionaries)
        // static Dictionary<string, string> recieved_packages = new Dictionary<string, string>(); // contains control and measurement values

        // data containers (dictionaries)
        static Dictionary<string, DataContainer> recieved_packages = new Dictionary<string, DataContainer>();
        static Dictionary<string, DataContainer> references = new Dictionary<string, DataContainer>();

        // container for all controllers in the module
        static List<PID> PIDList = new List<PID>();

        // state variables
        static bool isListeningOnPlant = false;

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
                if (isListeningOnPlant == true)
                {
                    // send time, u, and y
                    string message = ConstructMessageGui(PIDList_);
                    client.send(message);
                }
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
                        double reference = Convert.ToDouble(recieved_packages["r" + index].GetLastValue());
                        double measurement = Convert.ToDouble(recieved_packages["yc" + index].GetLastValue());

                        // update controller parameters
                        controller.update_parameters(Convert.ToDouble(recieved_packages["Kp"].GetLastValue()), 
                                                    Convert.ToDouble(recieved_packages["Ki"].GetLastValue()), 
                                                    Convert.ToDouble(recieved_packages["Kd"].GetLastValue()));


                        // update control signal
                        if (isListeningOnPlant) controller.compute_control_signal(reference, measurement);                   
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

                if (isListeningOnPlant == true)
                {
                    // send u to the plant
                    string message = ConstructMessagePlant(PIDList_);

                    client.send(message);
                    //Console.WriteLine("sent plant: " + message);
                }
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
                        double reference = Convert.ToDouble(recieved_packages["r" + index].GetLastValue());
                        double measurement = Convert.ToDouble(recieved_packages["yc" + index].GetLastValue());
                        controller.compute_control_signal(reference, measurement);    
                    }
                    isListeningOnPlant = true;
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

            for (int i = 0; i < recieved_packages.Keys.Count(); i++)
            {
                try
                {            
                    var item = recieved_packages.ElementAt(i);
                    string key = item.Key;

                    if (key.Contains("yo"))
                    {
                        message += "#" + key + "_" + recieved_packages[key].GetLastValue();
                    }
                    else if (key.Contains("yc"))
                    {
                        message += "#" + key + "_" + recieved_packages[key].GetLastValue();
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return message;
        }

        public static void parse_message(string message)
        {
            string time = "";
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

                if (recieved_packages.ContainsKey(key) == false)
                {
                    recieved_packages.Add(key, new DataContainer(n_steps));

                    // add controller to detected output 
                    if (key.Contains("yc"))
                    {
                        PIDList.Add(new PID(1, 1, 1));

                        recieved_packages.Add("r" + key.Substring(2), new DataContainer(n_steps));
                    }
                }

                recieved_packages[key].InsertData(time, value);
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

            // calculte error
            e = r - y;

            if (update_last != null)
            {
                double dt = (nowTime - update_last).TotalSeconds;
 
                //integrator with anti wind-up
                if (anti_wind_up == true)
                {
                    // only add intergral action if the control signal is not saturated
                    if (u > u_min  && u < u_max)  I += dt * e;
                }
                else
                {
                    I += dt * e;
                }

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

    public class DataContainer
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // store the time:value pair in string arrays
        public string[] time;
        public string[] value;


        // new/old data flag
        public bool data_up_to_date = true;

        // constructor
        public DataContainer(int size)
        {
            time = new string[size];
            value = new string[size];
        }

        // instert a new time:value pair data point
        public void InsertData(string time_, string value_)
        {
            // compare timestamps
            if (GetLastTime() != null)
            { 
                if (isMostRecent(time_) == true)
                {
                    data_up_to_date = true;

                    Array.Copy(time, 1, time, 0, time.Length - 1);
                    time[time.Length - 1] = time_;

                    Array.Copy(value, 1, value, 0, value.Length - 1);
                    value[value.Length - 1] = value_;
                }
                else
                {
                    data_up_to_date = false;
                    Console.WriteLine(DateTime.UtcNow.ToString() + " > ignoring measurement data (wrong order)");
                }
            }
            else
            {
                time[time.Length - 1] = time_;
                value[value.Length - 1] = value_;
            }
        }

        public bool isMostRecent(string time)
        {
            DateTime t_new = DateTime.ParseExact(time, FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            DateTime t_prev = DateTime.ParseExact(GetLastTime(), FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            TimeSpan timeDiff = t_new - t_prev;

            if (timeDiff.TotalMilliseconds > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetLastTime()
        {
            return time[time.Length - 1];
        }
        public string GetLastValue()
        {
            return value[value.Length - 1];
        }

        public void CopyAndPushArray()
        {
            Array.Copy(time, 1, time, 0, time.Length - 1);
            time[time.Length - 1] = DateTime.UtcNow.ToString(FMT);

            Array.Copy(value, 1, value, 0, value.Length - 1);
            value[value.Length - 1] = value[value.Length - 2];
        }
    }
}
