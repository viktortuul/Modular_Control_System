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
        static Dictionary<string, DataContainer> received_packages = new Dictionary<string, DataContainer>();

        // flag specific keys with pre-defined meanings
        static string[] def_controlled_states = new string[] { "yc1", "yc2" }; // initialze new PID for these keys

        // container for all controllers in the module
        private static List<PID> PIDList = new List<PID>();

        // communication states
        static bool listening_on_plant = false;
        static public bool using_canal = false;

        // controller type
        static string controller_type = "PID_normal";

        // logger (write to file)
        static string log_flag = "false";
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


            // create a thread for listening on the plant
            Thread thread_loop = new Thread(() => loop());
            thread_loop.Start();
        }

        public static void loop()
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
            Client sender = new Client(IP, port);
       
            while (true)
            {
                Thread.Sleep(100);
                if (listening_on_plant == true)
                {
                    // send time, u, and y
                    string message = ConstructMessageGUI(PIDList);
                    sender.Send(message);
                    //Console.WriteLine("to GUI: " + message);
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

                    // logging
                    if (log_flag == "true")
                    {
                        sb.Append(message + "\n");
                        File.AppendAllText("log_sent.txt", sb.ToString());
                        sb.Clear();
                    }
            
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
                    //Console.WriteLine("from plant: " + listener.last_recieved);

                    // compute the new control signal for each controller
                    ManageControllers(PIDList, flag : "plant");

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
                string key = subitem[0]; // yo1, yc1, uc1, (yo2, yc1, uc1)
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
                    received_packages.Add(key, new DataContainer(Constants.n_steps_small));

                    // add controller if the tag corresponds to a controlled state
                    if (def_controlled_states.Contains(key))
                    {
                        Console.WriteLine("PID added: " + controller_type.ToString());
                        PIDList.Add(new PID(controller_type));
                    }

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
                    else
                    {
                        // update control signal
                        double reference = Convert.ToDouble(received_packages["r" + index].GetLastValue());
                        double measurement = Convert.ToDouble(received_packages["yc" + index].GetLastValue());

                        if (controller_type == "PID_normal" && flag == "loop")
                        {
                            // continous constant interval loop when a standard PID is used
                            controller.ComputeControlSignal(reference, measurement, new_actuator_flag: false, new_measurement_flag: false, actuator_position: 0);
                        }
                        else if (controller_type == "PID_plus" && flag == "plant")
                        {
                            // detect if actuator signal actually has moved (would indicate the control signals are beging transmitted)
                            if (received_packages["uc" + index].hasChanged(0, 2) || received_packages["uc" + index].getCounter() < 5 || received_packages["uc" + index].GetLastValue() == "0")
                            {
                                // PIDplus update (new actuator flag)
                                controller.ComputeControlSignal(reference, measurement, new_actuator_flag: true, new_measurement_flag: true, actuator_position: Convert.ToDouble(received_packages["uc" + index].GetLastValue()));
                            }
                            else
                            {
                                // PIDplus update (actuator communication lost)
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
                        controller_type = arg_sep[1];
                        break;
                    case "log":
                        log_flag = arg_sep[1];
                        break;
        
                    case "channel_gui":
                        EP_Send_GUI = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                        using_canal = true;
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
                }
            }
        }
    }
}
