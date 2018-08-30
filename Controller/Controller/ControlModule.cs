using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Communication;
using System.Globalization;

namespace Controller
{
    public class ControlModule
    {
        // data containers (dictionaries)
        static Dictionary<string, DataContainer> received_packages = new Dictionary<string, DataContainer>();
        static Dictionary<string, DataContainer> references = new Dictionary<string, DataContainer>();

        // flag specific keys with pre-defined meanings
        static string[] flag_controlled_states = new string[] { "yc1", "yc2" };

        // container for all controllers in the module
        private static List<PID> PIDList = new List<PID>();

        // state variables
        static bool listening_on_plant = false;
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

            // 6 arguments --> no canal. 10 arguments --> canal
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
                try
                {
                    listener.Listen();
                    ParseMessage(listener.last_recieved);

                    // update controller settings (reference set-point and PID parameters)
                    ManageControllers(PIDList, flag : "GUI");
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
                    // send control signal u to the plant
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
                try
                {
                    listener.Listen();
                    ParseMessage(listener.last_recieved);

                    // compute the new control signal for each controller
                    ManageControllers(PIDList, flag : "Plant");

                    // flag that packages from the plant are being received
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

            // if a canal is used, append the end-point address
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
            for (int i = 0; i < received_packages.Keys.Count(); i++)
            {   
                var item = received_packages.ElementAt(i);
                string key = item.Key;

                if (key.Contains("y")) message += "#" + key + "_" + received_packages[key].GetLastValue();                 
            }

            return message;
        }

        public static string ConstructMessagePlant(List<PID> PIDList)
        {
            string message = "";

            // if a canal is used, append the end-point address
            if (using_canal == true) message += Convert.ToString("EP_" + EP_Plant.IP + ":" + EP_Plant.Port + "#");
            // message += Convert.ToString("ORIGIN_" + 67 + "#"); // ADD THE ORIGIN_ID (IN THIS CASE 67)

            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;
                message += "u" + index + "_" + controller.get_u() + "#";
            }

            // remove the redundant delimiter
            message = message.Substring(0, message.LastIndexOf('#'));

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
                if (key == "time") { time = value; continue; }

                // detect corrupt values
                if (Helpers.isDouble(value) == false)
                {
                    Console.WriteLine("Error: corrupt package  <" + key + "_" + value + ">");
                    continue;
                }

                // if a new key is recieved, add it
                if (received_packages.ContainsKey(key) == false)
                {
                    received_packages.Add(key, new DataContainer(Constants.n_steps));

                    // add controller if the tag corresponds to a controlled state
                    if (flag_controlled_states.Contains(key)) PIDList.Add(new PID());
                }

                // insert the recieved data to corresponding tag
                received_packages[key].InsertData(time, value);
            }
        }

        private static void ManageControllers(List<PID> PIDList, string flag)
        {
            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;

                // check if corresponing reference value exist
                if (received_packages.ContainsKey("r" + index) == false) continue;

                // check if both the last recieved measurement is up to date (else don't update the control signal) 
                if (received_packages["yc" + index].isUpToDate())
                {
                    // update controller parameters
                    if (flag == "GUI")
                    {
                        controller.UpdateParameters(Convert.ToDouble(received_packages["Kp"].GetLastValue()),
                                                    Convert.ToDouble(received_packages["Ki"].GetLastValue()),
                                                    Convert.ToDouble(received_packages["Kd"].GetLastValue()));
                    }
                    else if (flag == "Plant")
                    {
                        // update control signal
                        double reference = Convert.ToDouble(received_packages["r" + index].GetLastValue());
                        double measurement = Convert.ToDouble(received_packages["yc" + index].GetLastValue());
                        controller.ComputeControlSignal(reference, measurement);
                    }
                }
            }
        }
    }
}
