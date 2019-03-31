using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Communication;
using System.Globalization;
using System.Text;
using System.IO;
using GlobalComponents;

namespace Controller
{
    public class ControlModule
    {
        // data containers (dictionaries)
        static Dictionary<string, DataContainer> received_packets = new Dictionary<string, DataContainer>();

        // flag specific keys with pre-defined meanings
        static string[] DEF_controlled_states = new string[] { "yc1", "yc2" }; // initialze new PID for these keys

        // container for all controllers in the module
        private static List<PID> PIDList = new List<PID>();

        // communication states
        static bool listening_on_plant = false;
        static public bool using_channel = false;

        // controller type
        static string controller_type = ControllerType.PID_STANDARD; //PID_normal, PID_plus, PID_suppress
        static double[] u_saturation = new double[] { 0, 7.5 };

        // logger (write to file)
        static bool FLAG_LOG = false;
        static StringBuilder sb = new StringBuilder();

        // GUI and Plant endpoints
        static ConnectionParameters EP_GUI = new ConnectionParameters();
        static ConnectionParameters EP_Plant = new ConnectionParameters();

        // transmission route (either GUI/plant or Canal EP's)
        static AddressEndPoint EP_Send_GUI = new AddressEndPoint();
        static AddressEndPoint EP_Send_Plant = new AddressEndPoint();

        static void Main(string[] args)
        {
            // parse the command line arguments
            ParseArgs(args);

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

            // create a thread for running a fixed interval control loop (for the standard PID)
            Thread thread_control_loop = new Thread(() => ControlLoop());
            thread_control_loop.Start();
        }

        public static void ControlLoop()
        {
            while (true)
            {
                Thread.Sleep(50);
                ManageControllers(PIDList, flag: "loop");
            }
        }

        public static void SenderGUI(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the GUI
            Client Sender = new Client(IP, port);
       
            while (true)
            {
                Thread.Sleep(100);
                if (listening_on_plant == true)
                {
                    // send time, u, and y
                    string message = ConstructMessageToGUI(PIDList);
                    Sender.Send(message);
                    //Console.WriteLine("to GUI: " + message);
                }
            }
        }

        public static void ListenerGUI(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the GUI
            Server Listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    Listener.Listen();
                    ParseReceivedMessage(Listener.getMessage());

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
                    string message = ConstructMessageToPlant(PIDList);
                    Sender.Send(message);

                    // log sent message (used for package delivery analysis)
                    Helpers.Log(sb, message, FLAG_LOG);
                }
            }
        }

        public static void ListenerPlant(string IP, int port, List<PID> PIDList)
        {
            // initialize a connection to the plant
            Server Listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    Listener.Listen();
                    ParseReceivedMessage(Listener.getMessage());
                    //Console.WriteLine("from plant: " + listener.last_recieved);

                    // compute the new control signal for each controller
                    ManageControllers(PIDList, flag : "plant");

                    // flag that packets from the plant are being received
                    listening_on_plant = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static string ConstructMessageToGUI(List<PID> PIDList)
        {
            string message = "";

            // if a canal is used, append the end-point address
            if (using_channel == true) message += Convert.ToString("EP_" + EP_GUI.IP + ":" + EP_GUI.Port + "#");

            // add time-stamp
            message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT));

            // append control signals
            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;
                message += "#u" + index + "_" + controller.get_u();
            }

            // append measurements signals
            for (int i = 0; i < received_packets.Keys.Count(); i++)
            {   
                var item = received_packets.ElementAt(i);
                string key = item.Key;

                if (key.Contains("y")) message += "#" + key + "_" + received_packets[key].GetLastValue();                 
            }

            return message;
        }

        public static string ConstructMessageToPlant(List<PID> PIDList)
        {
            string message = "";

            // if a canal is used, append the end-point address
            if (using_channel == true) message += Convert.ToString("EP_" + EP_Plant.IP + ":" + EP_Plant.Port + "#");

            // add time-stamp
            message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT));

            // append control signals
            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;
                message += "#u" + index + "_" + controller.get_u();
            }

            return message;
        }

        public static void ParseReceivedMessage(string message)
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
                string key = subitem[0]; // yo1, yc1, uc1, (yo2, yc1, uc1)
                string value = subitem[1];

                // detect the time (don't add it as a separate key)
                if (key == "time") { time = value; continue; }

                // detect corrupt values
                if (Helpers.isDouble(value) == false)
                {
                    Console.WriteLine("Error: corrupt packet  <" + key + "_" + value + ">");
                    continue;
                }

                // if a new key is recieved, add it
                if (received_packets.ContainsKey(key) == false)
                {
                    received_packets.Add(key, new DataContainer(Constants.n_steps_small));

                    // add controller if the tag corresponds to a controlled state
                    if (DEF_controlled_states.Contains(key))
                    {
                        PIDList.Add(new PID(controller_type, u_saturation));
                    }
                }

                // insert the recieved data to corresponding tag
                received_packets[key].InsertData(time, value);
            }
        }

        private static void ManageControllers(List<PID> PIDList, string flag)
        {
            int index = 0;
            foreach (PID controller in PIDList)
            {
                index++;

                // check if corresponing reference value exist
                if (received_packets.ContainsKey("r" + index) == false) continue;

                // check if the last recieved measurement is up to date (else don't update the control signal) 
                if (received_packets["yc" + index].isUpToDate())
                {             
                    if (flag == "GUI")
                    {
                        // update controller parameters
                        controller.UpdateParameters(Convert.ToDouble(received_packets["Kp"].GetLastValue()), Convert.ToDouble(received_packets["Ki"].GetLastValue()), Convert.ToDouble(received_packets["Kd"].GetLastValue()));
                    }
                    else
                    {
                        // update control signal
                        double reference = Convert.ToDouble(received_packets["r" + index].GetLastValue());
                        double measurement = Convert.ToDouble(received_packets["yc" + index].GetLastValue());

                        if (controller_type == ControllerType.PID_STANDARD && flag == "loop")
                        {
                            // continous constant interval loop when a standard PID is used
                            controller.ComputeControlSignal(reference, measurement, new_actuator_flag: false, new_measurement_flag: false, actuator_position: 0);
                        }
                        else if ((controller_type == ControllerType.PID_PLUS || controller_type == ControllerType.PID_SUPPRESS) && flag == "plant")
                        {
                            // detect if actuator signal actually has moved (would indicate the control signals are beging transmitted)
                            if (received_packets["uc" + index].hasChanged(0, 2) || received_packets["uc" + index].getCounter() < 5 || Convert.ToDouble(received_packets["uc" + index].GetLastValue()) <= 0 || Convert.ToDouble(received_packets["uc" + index].GetLastValue()) >= 20)
                            {
                                // PIDplus update (integral and deriavative action included)
                                controller.ComputeControlSignal(reference, measurement, new_actuator_flag: true, new_measurement_flag: true, actuator_position: Convert.ToDouble(received_packets["uc" + index].GetLastValue()));
                            }
                            else
                            {
                                // PIDplus update (only proportinal action)
                                controller.ComputeControlSignal(reference, measurement, new_actuator_flag: false, new_measurement_flag: true, actuator_position: 0);
                            }
                        }
                    }
                }
            }
        }

        public static void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                List<string> arg_sep = Tools.ArgsParser(arg);
                string arg_name = arg_sep[0];

                switch (arg_name)
                {
                    case "controller":
                        if (arg_sep[1] == "PID_STANDARD") controller_type = ControllerType.PID_STANDARD;
                        if (arg_sep[1] == "PID_PLUS") controller_type = ControllerType.PID_PLUS;
                        if (arg_sep[1] == "PID_SUPPRESS") controller_type = ControllerType.PID_SUPPRESS;
                        break;
                    case "control_range":
                        u_saturation = new double[] { Convert.ToDouble(arg_sep[1]), Convert.ToDouble(arg_sep[2]) }; 
                        break;
                    case "log":
                        if (arg_sep[1] == "false") FLAG_LOG = false;
                        if (arg_sep[1] == "true") FLAG_LOG = true;
                        break;       
                    case "channel_gui":
                        EP_Send_GUI = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                        using_channel = true;
                        break;
                    case "channel_plant":
                        EP_Send_Plant = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                        break;
                    case "gui_ep":
                        EP_GUI = new ConnectionParameters(arg_sep[1], Convert.ToInt16(arg_sep[2]), Convert.ToInt16(arg_sep[3]));
                        break;
                    case "plant_ep":
                        EP_Plant = new ConnectionParameters(arg_sep[1], Convert.ToInt16(arg_sep[2]), Convert.ToInt16(arg_sep[3]));
                        break;
                    default:
                        Console.WriteLine("Unknown argument not used: " + arg_name);
                        Console.WriteLine("Press any key to proceed...");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
