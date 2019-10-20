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
        static string[] DEF_controlled_states = new string[] { "yc1", "yc2" };  // initialize a new PID for these keys

        // container for all controllers in the module
        private static List<PID> PIDList = new List<PID>();                     // stores the PID controllers

        // communication states
        static bool listening_on_plant = false;                                 // a flag telling if packets from the plant have been received
        static public bool using_channel = false;                               // channel module flag

        // controller settings
        static string controller_type = ControllerType.PID_STANDARD;            // PID_normal, PID_plus, PID_suppress
        static double[] u_saturation = new double[] { 0, 7.5 };                 // default PID saturation
        static int dt_pid_normal = 50;                                          // PID normal update rate

        // logger (write to file)
        static bool FLAG_LOG = false;
        static StringBuilder sb = new StringBuilder();
        static string log_file_name = "log_controller.txt";                     // default log text file name

        // HMI and Plant endpoints
        static ConnectionParameters EP_HMI = new ConnectionParameters();        // HMI module endpoint
        static ConnectionParameters EP_Plant = new ConnectionParameters();      // Plant module endpoint

        // transmission route (either HMI/plant or Canal EP's)
        static AddressEndPoint EP_Route_HMI = new AddressEndPoint();            // same as EP_HMI if no channel is used
        static AddressEndPoint EP_Route_Plant = new AddressEndPoint();          // same as EP_Plant if no channel is used

        // data transmission interval
        static int T_HMI = 100;                                                 // [ms]
        static int T_Plant = 50;                                                // [ms]

        static void Main(string[] args)
        {
            // parse the command line arguments
            ParseArgs(args);

            // if no channel is used, don't re-direct the packets anywhere
            if (using_channel == false)
            {
                EP_Route_Plant.IP = EP_Plant.IP;
                EP_Route_Plant.Port = EP_Plant.Port;
                EP_Route_HMI.IP = EP_HMI.IP;
                EP_Route_HMI.Port = EP_HMI.Port;
            }

            // create a thread for sending to the HMI
            Thread thread_send_HMI = new Thread(() => SenderHMI(EP_Route_HMI.IP, EP_Route_HMI.Port));
            thread_send_HMI.Start();

            // create a thread for listening on the HMI
            Thread thread_listen_HMI = new Thread(() => ListenerHMI(EP_Route_HMI.IP, EP_HMI.PortThis));
            thread_listen_HMI.Start();

            // create a thread for sending to the plant
            Thread thread_send_plant = new Thread(() => SenderPlant(EP_Route_Plant.IP, EP_Route_Plant.Port));
            thread_send_plant.Start();

            // create a thread for listening on the plant
            Thread thread_listen_plant = new Thread(() => ListenerPlant(EP_Route_Plant.IP, EP_Plant.PortThis));
            thread_listen_plant.Start();

            // create a thread for running a fixed interval control loop (for the standard PID)
            if (controller_type == ControllerType.PID_STANDARD)
            {
                Thread thread_control_loop = new Thread(() => ControlLoop());
                thread_control_loop.Start();
            }
        }

        public static void SenderHMI(string IP, int port)
        {
            // initialize a connection to the HMI
            Client Sender = new Client(IP, port);
       
            while (true)
            {
                Thread.Sleep(T_HMI);
                if (listening_on_plant == true)
                {
                    // send time, u, and y
                    string message = ConstructMessageToHMI();
                    Sender.Send(message);
                    //Console.WriteLine("packet to HMI: " + message);
                }
            }
        }

        public static void ListenerHMI(string IP, int port)
        {
            // initialize a connection to the HMI
            Server Listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    Listener.Listen();
                    ParseReceivedMessage(Listener.getMessage());

                    // update controller settings (reference set-point and PID parameters)
                    UpdateControllers(flag : "HMI");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static public void SenderPlant(string IP, int port)
        {
            // initialize a connection to the plant
            Client Sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(T_Plant);

                if (listening_on_plant == true)
                {
                    // send control signal u to the plant
                    string message = ConstructMessageToPlant();
                    Sender.Send(message);

                    // log sent message (used for package delivery analysis)
                    if (FLAG_LOG == true)
                    {
                        Helpers.WriteToLog(sb, filename : log_file_name, text : message);
                    }
                    
                }
            }
        }

        public static void ListenerPlant(string IP, int port)
        {
            // initialize a connection to the plant
            Server Listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    Listener.Listen();
                    ParseReceivedMessage(Listener.getMessage());
                    //Console.WriteLine("from plant: " + Listener.getMessage());

                    // compute the new control signal for each controller
                    UpdateControllers(flag : "plant");

                    // flag that packets from the plant are being received
                    listening_on_plant = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static string ConstructMessageToHMI()
        {
            string message = "";

            // if a channel is used, append the end-point address
            if (using_channel == true) message += Convert.ToString("EP_" + EP_HMI.IP + ":" + EP_HMI.Port + "#");

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

        public static string ConstructMessageToPlant()
        {
            string message = "";

            // if a channel is used, append the end-point address
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
                string key = subitem[0]; // e.g. time, yo1, yc1, uc1
                string value = subitem[1]; // e.g. 0.2124234

                // detect the time (don't add it as a separate key)
                if (key == "time") { time = value; continue; }

                // detect and skip corrupt values
                if (Helpers.isDouble(value) == false) continue;

                // if a new key is recieved, add it
                if (received_packets.ContainsKey(key) == false)
                {
                    received_packets.Add(key, new DataContainer(Constants.n_datapoints_controller));

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

        public static void ControlLoop()
        {
            // this is a time triggered control loop for the standar PID
            while (true)
            {
                Thread.Sleep(dt_pid_normal);
                UpdateControllers(flag: "loop");
            }
        }

        private static void UpdateControllers(string flag)
        {
            int index = 0; // PID number
            foreach (PID controller in PIDList)
            {
                index++;

                // check if corresponing reference value exist
                if (received_packets.ContainsKey("r" + index) == false) continue;

                // check if the last recieved measurement is up to date (else don't update the control signal) 
                //if (received_packets["yc" + index].isUpToDate())
                //{             
                    if (flag == "HMI") // flag = HMI means that this method is triggered after a packet received from the HMI-interface
                    {
                        // update controller parameters
                        controller.UpdateParameters(Convert.ToDouble(received_packets["Kp"].GetLastValue()), Convert.ToDouble(received_packets["Ki"].GetLastValue()), Convert.ToDouble(received_packets["Kd"].GetLastValue()));
                    }
                    else
                    {
                        // update the latest received reference and sensor measurement values (and last realized actuator value)
                        double reference = Convert.ToDouble(received_packets["r" + index].GetLastValue());
                        double measurement = Convert.ToDouble(received_packets["yc" + index].GetLastValue());
                        double last_realized_actuator_value = Convert.ToDouble(received_packets["uc" + index].GetLastValue());

                        // update PID-controller
                        switch (controller_type)
                        {
                            case ControllerType.PID_STANDARD:
                                if (flag != "loop") continue;
                                controller.ComputeControlSignal(reference, measurement, new_actuator_flag: false, actuator_position: 0);
                                break;

                            case ControllerType.PID_PLUS:
                                if (flag != "plant") continue;
                                controller.ComputeControlSignal(reference, measurement, new_actuator_flag: true, actuator_position: last_realized_actuator_value);
                                break;

                            case ControllerType.PID_SUPPRESS:
                                if (flag != "plant") continue;

                                // only update PIDsuppress if the control signals are being transmitted to the plant
                                if (isAcutatorPositionUpdated(index, controller) == true)
                                {
                                    controller.ComputeControlSignal(reference, measurement, new_actuator_flag: true, actuator_position: 0);
                                }
                                else
                                {
                                    //controller.ComputeControlSignal(reference, measurement, new_actuator_flag: false, actuator_position: last_realized_actuator_value);
                                }
                                break;
                        }
                    }
                //}
            }
        }

        public static bool isAcutatorPositionUpdated(int index, PID controller)
        {
            return
                received_packets["uc" + index].isUpToDate() || // CHECK THIS -----------------------------------------
                received_packets["uc" + index].hasChanged(0, 2) || 
                received_packets["uc" + index].getCounter() < 5 || 
                Convert.ToDouble(received_packets["uc" + index].GetLastValue()) <= controller.get_u_min() || 
                Convert.ToDouble(received_packets["uc" + index].GetLastValue()) >= controller.get_u_max();
        }

        public static void ParseArgs(string[] args)
        {
            Console.WriteLine("Control Module initialized. Parsing command line arguments...");
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
                    case "channel_hmi":
                        EP_Route_HMI = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                        Console.WriteLine("Channel (HMI) endpoint: " + arg_sep[1] + ":" + arg_sep[2]);
                        using_channel = true;
                        break;
                    case "channel_plant":
                        EP_Route_Plant = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                        Console.WriteLine("Channel (Plant) endpoint: " + arg_sep[1] + ":" + arg_sep[2]);
                        break;
                    case "hmi_ep":
                        EP_HMI = new ConnectionParameters(arg_sep[1], Convert.ToInt16(arg_sep[2]), Convert.ToInt16(arg_sep[3]));
                        Console.WriteLine("HMI endpoint: " + arg_sep[1] + ":" + arg_sep[2]);
                        break;
                    case "plant_ep":
                        EP_Plant = new ConnectionParameters(arg_sep[1], Convert.ToInt16(arg_sep[2]), Convert.ToInt16(arg_sep[3]));
                        Console.WriteLine("Plant endpoint: " + arg_sep[1] + ":" + arg_sep[2]);
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
