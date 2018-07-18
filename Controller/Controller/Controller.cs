using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Communication;
using System.Globalization;

namespace Controller
{
    public class Controller
    {
        // data containers (dictionaries)
        static Dictionary<string, DataContainer> recieved_packages = new Dictionary<string, DataContainer>();
        static Dictionary<string, DataContainer> references = new Dictionary<string, DataContainer>();

        // container for all controllers in the module
        private static List<PID> PIDList = new List<PID>();

        // state variables
        static bool listening_on_plant = false;

        // canal flag
        static public bool using_canal = false;

        // GUI and Plant endpoints
        static ConnectionParameters EP_GUI = new ConnectionParameters();
        static ConnectionParameters EP_Plant = new ConnectionParameters();

        // transmission route (either GUI/plant or Canal EP's)
        static AddressEndPoint EP_Send_GUI = new AddressEndPoint();
        static AddressEndPoint EP_Send_Plant = new AddressEndPoint();

        static void Main(string[] args)
        {
            // parse the command line arguments
            EP_GUI = new ConnectionParameters(args[0], Convert.ToInt16(args[1]), Convert.ToInt16(args[2]));
            EP_Plant = new ConnectionParameters(args[3], Convert.ToInt16(args[4]), Convert.ToInt16(args[5]));

            if (args.Length == 6)
            {
                EP_Send_GUI = new AddressEndPoint(EP_GUI.IP, EP_GUI.Port);
                EP_Send_Plant = new AddressEndPoint(EP_Plant.IP, EP_Plant.Port);
            }
            else if (args.Length == 10)
            {
                EP_Send_GUI = new AddressEndPoint(args[6], Convert.ToInt16(args[7]));
                EP_Send_Plant = new AddressEndPoint(args[8], Convert.ToInt16(args[9]));
                using_canal = true;
            }

            // create a thread for sending to the GUI
            Thread thread_send_GUI = new Thread(() => SenderGUI(EP_Send_GUI.IP, EP_Send_GUI.Port, PIDList));
            thread_send_GUI.Start();

            // create a thread for listening on the GUI
            Thread thread_listen_GUI = new Thread(() => ListenerGUI(EP_Send_GUI.IP, EP_GUI.PortThis, PIDList));
            thread_listen_GUI.Start();

            // create a thread for sending to the plant
            Thread thread_send_plant = new Thread(() => SenderPlant(EP_Send_Plant.IP, EP_Send_Plant.Port, PIDList));
            thread_send_plant.Start();

            // create a thread for listening on the plant
            Thread thread_listen_plant = new Thread(() => ListenerPlant(EP_Send_Plant.IP, EP_Plant.PortThis, PIDList));
            thread_listen_plant.Start();
        }

        public static void SenderGUI(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the GUI
            Client sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(100);
                if (listening_on_plant == true)
                {
                    // send time, u, and y
                    string message = ConstructMessageGUI(PIDList);
                    sender.Send(message);
                }
            }
        }

        public static void ListenerGUI(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the GUI
            Server listener = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(10);
                // send y and u (and recieve data)
                try
                {
                    listener.Listen();
                    ParseMessage(listener.last_recieved);

                    // compute the new control signal for each controller (with updated r)
                    ComputeControlSignals(PIDList, "from_GUI");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static public void SenderPlant(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the plant
            Client Sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(50);

                if (listening_on_plant == true)
                {
                    // send u to the plant
                    string message = ConstructMessagePlant(PIDList);
                    Sender.Send(message);
                }
            }
        }

        public static void ListenerPlant(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the plant
            Server listener = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    listener.Listen();
                    ParseMessage(listener.last_recieved);

                    // compute the new control signal for each controller
                    ComputeControlSignals(PIDList, "from_Plant");

                    listening_on_plant = true;
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
            if (using_canal == true) message += Convert.ToString("EP_" + EP_GUI.IP + ":" + EP_GUI.Port + "#");
            message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT));

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
            if (using_canal == true) message += Convert.ToString("EP_" + EP_Plant.IP + ":" + EP_Plant.Port);

            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;
                message += "#u" + index + "_" + controller.get_u();
            }

            // remove the first delimiter
            if (message[0] == '#') message = message.Substring(1);
            // Console.WriteLine(message);
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
                    recieved_packages.Add(key, new DataContainer(Constants.n_steps));

                    // add controller and reference tracker
                    if (key.Contains("yc")) PIDList.Add(new PID());
                }

                // insert the recieved data to corresponding tag
                recieved_packages[key].InsertData(time, value);
            }
        }

        public static bool isDouble(string str)
        {
            try
            {
                Double.Parse(str);
                return true;
            }
            catch { return false; }
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
                    if (listening_on_plant) controller.ComputeControlSignal(reference, measurement);

                    // update controller parameters
                    if (mode == "from_GUI") controller.UpdateParameters(Convert.ToDouble(recieved_packages["Kp"].GetLastValue()),
                                                                        Convert.ToDouble(recieved_packages["Ki"].GetLastValue()),
                                                                        Convert.ToDouble(recieved_packages["Kd"].GetLastValue()));
                }

            }
        }
    }
}
