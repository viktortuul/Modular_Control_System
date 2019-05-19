using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using Communication;
using GlobalComponents;
using System.Threading;
using System.IO;

namespace EV3
{
    class Program
    {
        // local controller parameters. ONLY FOR DEBUGGING
        static DateTime time_last_measurement = DateTime.Now;  // time stamp of prior execution
        static double Kp = 10.0; // 15 10          // proportional
        static double Ki = 45.0; // 20 45          // integral
        static double Kd = 0.9;  // 0.5 0.9        // derivative
        static double r = 0;
        static double u = 0;
        static double err = 0;
        static double I = 0;    // error integral
        static double ep = 0;   // current and prior error
        static double de = 0;   // error derivative
        static double DE = 0;
        static double q = 0.7;

        // lego Mindstorms EV3 API and attributes
        static Brick _brick;
        static uint _time = 1000;               // motor power duration ms
        static uint _time_disturbance = 300;    // motor power duration ms
        static int power_disturbance = 70;      // power disturbance (INVPENDULUM ground wheels)
        static bool apply_motor_disturbance = false;
        static double u_max = 100;              // max motor power
        static double u_min = -100;             // min motor power
        static double power_last_realized = 0;  // stores the last applied motor power value
        static bool isEV3initialized = false;
        static float measurement_offset = 0;    // offset on EV3 measurements
        static float EV3measurement = 0;        // last received sensor measurement
        static string COM_port = "COM7";        // bluetooth com port

        // process type
        static string process_type = PhysicalProcessType.PLATOONING;

        // logger (write to file)
        static bool FLAG_LOG = false;
        static StringBuilder sb = new StringBuilder();

        // flag specific keys with pre-defined meanings
        static string[] DEF_control_signal = new string[] { "u1", "u2" };

        // data containers (dictionaries)
        public static Dictionary<string, string> packet_last = new Dictionary<string, string>(); // recieved packet <tag, value>
        public static Dictionary<string, double[]> packets = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        // data containers (dictionaries), only for plotting
        public static Dictionary<string, DataContainer> states = new Dictionary<string, DataContainer>();
        public static Dictionary<string, DataContainer> perturbations = new Dictionary<string, DataContainer>();

        // channel variables
        public static bool using_channel = false;
        static ConnectionParameters EP_Controller = new ConnectionParameters();
        static AddressEndPoint EP_Send_Controller = new AddressEndPoint();

        static void Main(string[] args)
        {
            ParseArgs(args);

            if (using_channel == false)
            {
                EP_Send_Controller.IP = EP_Controller.IP;
                EP_Send_Controller.Port = EP_Controller.Port;
            }

            // create a thread for listening on the physical plant
            Thread thread_listener = new Thread(() => Listener(EP_Send_Controller.IP, EP_Controller.PortThis));
            thread_listener.Start();

            // create a thread for communication with the controller
            Thread thread_sender_EV3 = new Thread(() => Sender(EP_Send_Controller.IP, EP_Send_Controller.Port));
            thread_sender_EV3.Start();

            // create a thread for the sequential disturbance
            Thread thread_disturbance = new Thread(() => Disturbance());
            thread_disturbance.Start();

            // create a thread for the simulation
            initializeEV3Brick();
        }

        public static void Disturbance()
        {
            int counter = 0;
            while (true)
            {
                Thread.Sleep(1000);
                if (counter >= 10)
                {
                    Console.WriteLine("Disturbance counter: " + counter + "/10");
                    apply_motor_disturbance = true;
                    counter = 0;
                }
                counter++;
            }
        }

        static async void initializeEV3Brick()
        {
            Console.WriteLine("Trying to initialize EV3 in " + process_type + " mode on COM-port " + COM_port);
            _brick = new Brick(new BluetoothCommunication(COM_port));
            //_brick = new Brick(new UsbCommunication()); // no arguments needed

            _brick.BrickChanged += _brick_BrickChanged; ;
            //await _brick.ConnectAsync(TimeSpan.FromMilliseconds(5)); // no argument entails 100ms update rate
            await _brick.ConnectAsync(TimeSpan.FromMilliseconds(1)); // no argument entails 100ms update rate
            await _brick.DirectCommand.PlayToneAsync(10, 1000, 200);

            // motor polarity
            switch (process_type)
            {
                case PhysicalProcessType.PLATOONING:
                    await _brick.DirectCommand.SetMotorPolarity(OutputPort.A | OutputPort.B, Polarity.Forward);
                    // A and B --> motors on ground
                    break;
                case PhysicalProcessType.SEGWAY:
                    await _brick.DirectCommand.SetMotorPolarity(OutputPort.A | OutputPort.B, Polarity.Forward);
                    // A and B --> motors on ground
                    break;
                case PhysicalProcessType.INVPENDULUM:
                    await _brick.DirectCommand.SetMotorPolarity(OutputPort.A | OutputPort.B | OutputPort.C, Polarity.Forward);
                    // A and B --> motors on ground
                    // C --> pendulum motor
                    break;
                default:
                    break;
            }
            await _brick.DirectCommand.StopMotorAsync(OutputPort.All, false);

            isEV3initialized = true;
            Console.WriteLine("Initialization done");
        }

        private static void _brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            // add offset to the measurements
            if (process_type == PhysicalProcessType.SEGWAY || process_type == PhysicalProcessType.INVPENDULUM)
            {
                if (measurement_offset == 0 || measurement_offset > 360 || measurement_offset < -360) measurement_offset = e.Ports[InputPort.One].SIValue;
            } 

            EV3measurement = e.Ports[InputPort.One].SIValue - measurement_offset;

            Console.WriteLine("Measurement: " + EV3measurement);
            // local controller for debugging
            //updateLocalPID();
        }

        private static async void ApplyMotorPower(int power)
        {
            //Console.WriteLine("power: " + power);
            power_last_realized = Convert.ToDouble(power);
            int power_ = power;

            // saturation
            if (power_ > u_max) power_ = 100;
            if (power_ < u_min) power_ = -100;

            // apply motor power depending on process type
            if (process_type == PhysicalProcessType.SEGWAY)
            {
                // apply power to motors on ground
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.B, -power_, _time, false);
            }
            else if (process_type == PhysicalProcessType.INVPENDULUM)
            {
                // apply power to pendulum motor
                await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.C, -power_, _time, false); 
                
                if (apply_motor_disturbance == true)
                {
                    await _brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A | OutputPort.B, -power_disturbance, _time_disturbance, false);
                    apply_motor_disturbance = false;
                }
            }
        }

        public static void Listener(string IP, int port)
        {
            // initialize a connection to the controller
            Server listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    listener.Listen();

                    // parse recieved message
                    ParseMessage(listener.getMessage());

                    // pre-allocate control signal vector (size = the number of received different control signals)
                    double[] u = new double[packet_last.Count];

                    int i = 0;
                    foreach (string key in packet_last.Keys)
                    {
                        u[i] = Convert.ToDouble(packet_last[key]);
                        i++;
                    }

                    // send actuator signal to the physical process
                    if (isEV3initialized == true)
                    {
                        ApplyMotorPower(Convert.ToInt32(u[0]));
                    }
                    
                    // log received message (used for package delivery analysis)
                    Log(sb, listener.getMessage(), FLAG_LOG);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
        }

        public static void Sender(string IP, int port)
        {
            // initialize a connection to the controller
            Client sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);

                string message = "";
                if (using_channel == true) message += Convert.ToString("EP_" + EP_Controller.IP + ":" + EP_Controller.Port + "#"); // add end-point if canal is used
                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT) + "#");

                message += "yc1" + "_" + (EV3measurement).ToString() + "#";

                // append the last actuator state
                message += "uc1" + "_" + (power_last_realized).ToString() + "#";

                // remove the redundant delimiter
                message = message.Substring(0, message.LastIndexOf('#'));
                sender.Send(message);

                //Console.WriteLine("send to controller: " + message);
            }
        }

        private static void updateLocalPID() // NOTE: ONLY FOR DEBUGGING
        {
            // current time
            DateTime time_now = DateTime.Now;

            if (time_last_measurement != null)
            {
                // calculte error
                err = r - Convert.ToDouble(EV3measurement);

                // duration since the last update
                double dt_m = (time_now - time_last_measurement).TotalSeconds;

                // integral part with anti wind-up (only add intergral action if the control signal is not saturated)
                if ((u >= 100 && err < 0) ||
                    (u <= -100 && err > 0) ||
                    (u > -100 && u < 100))
                {
                    if (dt_m < 0.2) I += dt_m * err;
                }

                // derivative part (with low pass)
                if (dt_m != 0.0f)
                {
                    de = (err - ep) / dt_m;
                }
                DE = q * de + (1 - q) * DE;

                // save the prior error for the next update
                ep = err;

                // resulting control signal
                u = Kp * err + Ki * I + Kd * DE;
                Console.WriteLine("measurement: " + EV3measurement + " u: " + Math.Round(u, 1) + " P: " + Math.Round(Kp * err, 1) + " I: " + Math.Round(Ki * I, 1) + " D: " + Math.Round(Kd * DE, 1));
                ApplyMotorPower(Convert.ToInt16(u));
            }
            // update prior time
            time_last_measurement = time_now;

            // Console.WriteLine(u);        
        }

        public static void ParseMessage(string message)
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

                if (DEF_control_signal.Contains(key))
                {
                    if (packet_last.ContainsKey(key) == false) packet_last.Add(key, value);
                    packet_last[key] = value;
                }
            }
        }

        public static void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                List<string> arg_sep = Tools.ArgsParser(arg);
                if (arg_sep.Count() == 0) continue;
                string arg_name = arg_sep[0];

                switch (arg_name)
                {
                    case "physical_process_type":
                        string arg_physical_process_type = arg_sep[1];
                        switch (arg_physical_process_type)
                        {
                            case "PLATOONING":
                                process_type = PhysicalProcessType.PLATOONING;
                                break;
                            case "SEGWAY":
                                process_type = PhysicalProcessType.SEGWAY;
                                break;
                            case "INVPENDULUM":
                                process_type = PhysicalProcessType.INVPENDULUM;
                                break;
                            default:
                                process_type = PhysicalProcessType.PLATOONING;
                                Console.WriteLine("Unknown plant type: " + arg_physical_process_type + ". Using default: " + PhysicalProcessType.PLATOONING);
                                break;
                        }
                        break;
                        
                    case "channel_controller":
                        EP_Send_Controller = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                        using_channel = true;
                        break;

                    case "controller_ep":
                        EP_Controller = new ConnectionParameters(arg_sep[1], Convert.ToInt16(arg_sep[2]), Convert.ToInt16(arg_sep[3]));
                        break;

                    case "com_port":
                        COM_port = arg_sep[1];
                        break;

                    case "log":
                        if (arg_sep[1] == "false") FLAG_LOG = false;
                        if (arg_sep[1] == "true") FLAG_LOG = true;
                        break;

                    case "ARG_INVALID":
                        break;

                    default:
                        Console.WriteLine("Unknown argument: " + arg_name);
                        break;
                }
            }
        }

        public static void Log(StringBuilder sb, string message, bool FLAG_LOG)
        {
            if (FLAG_LOG == true)
            {
                sb.Append(message + "\n");
                File.AppendAllText("log_received.txt", sb.ToString());
                sb.Clear();
            }
        }
    }
}
