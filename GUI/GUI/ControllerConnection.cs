using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Communication;
using System.Diagnostics;

namespace GUI
{

    public class ControllerConnection
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // chart settings
        public static int n_steps = 5000;

        // identity parameters
        public string name = "";
        string status = "";

        // connection parameters
        public ConnectionParameters ConnectionParameters;
        public bool is_recieving = false;
        public bool is_sending = false;
        public bool is_connected_to_process = false;

        // time parameters
        public DateTime last_recieved_time;

        // number of controlled states
        public int n_contr_states = 0;

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> recieved_packages = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> references = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> estimates = new Dictionary<string, DataContainer>();
        
        
        // initialize estimator
        KalmanFilter filter = new KalmanFilter(new double[2, 1] { { 0 }, { 0 } }, 0.16 * 2.0, 0.16 * 2.5, 15.0, 100, 6.32);

        // controller parameters
        public PIDparameters ControllerParameters;

        // main form access
        FrameGUI Main_form;

        public ControllerConnection(FrameGUI Main_form, string name, PIDparameters ControllerParameters, ConnectionParameters ConnectionParameters)
        {
            // main form access
            this.Main_form = Main_form;

            // identity parameters
            this.name = name;
            this.ConnectionParameters = ConnectionParameters;

            // update controller parameters
            this.ControllerParameters = ControllerParameters;

            // create a new thread for the listener
            Thread thread_listener = new Thread(() => Listener(ConnectionParameters.ip_recieve, ConnectionParameters.port_recieve));
            thread_listener.Start();

            // create a new thread for the sender
            Thread thread_sender = new Thread(() => Sender(ConnectionParameters.ip_send, ConnectionParameters.port_send));
            thread_sender.Start();

            // add reference keys
            references.Add("r1", new DataContainer(n_steps)); references.Add("r2", new DataContainer(n_steps));
            references["r1"].InsertData(DateTime.UtcNow.ToString(FMT), "0"); references["r2"].InsertData(DateTime.UtcNow.ToString(FMT), "0");
            
            // add estimate keys
            estimates.Add("yo1_hat", new DataContainer(n_steps)); estimates.Add("yc1_hat", new DataContainer(n_steps));
            estimates["yo1_hat"].InsertData(DateTime.UtcNow.ToString(FMT), "0"); estimates["yc1_hat"].InsertData(DateTime.UtcNow.ToString(FMT), "0");
        }

        private void Listener(string IP, int port)
        {
            // initialize a connection to the controller
            Server server = new Server(IP, port);
          
            // listen for messages sent from a host to this specific IP:port
            while (true)
            {
                Thread.Sleep(50);
                try
                {
                    // check if a new package is recieved
                    server.listen();
                    last_recieved_time = DateTime.UtcNow;

                    // parse the message which may contain u1, u2, yc1, yc2, yo1, yo2
                    ParseMessage(server.last_recieved);

                    // flag that the GUI is recieving packages from the controller
                    if (is_recieving == false) is_recieving = true;

                    // count the number of controlled states recieved
                    int counter = 0;
                    foreach (string key in recieved_packages.Keys)
                    {
                        if (key.Contains("yc"))
                        {
                            counter++;

                            // flag that full communication is present
                            if (is_connected_to_process == false) is_connected_to_process = true;
                        }
                    }
                    n_contr_states = counter;
                }
                catch (Exception ex)
                {
                    Main_form.log(Convert.ToString(ex));
                }
            }
        }

        private void Sender(string IP, int port)
        {
            // initialize a connection to the controller
            Client client = new Client(IP, port);

            // send messages to a host on this specific IP:port
            while (true)
            {
                Thread.Sleep(100);

                // attatch reference values
                string message = "";

                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(FMT) + "#");

                for (int i = 1; i <= n_contr_states; i++)
                {
                    message += "r" + i.ToString() + "_" + references["r" + i.ToString()].GetLastValue() + "#";
                }

                // attach controller parameters
                message += Convert.ToString("Kp_" + ControllerParameters.Kp + "#Ki_" + ControllerParameters.Ki + "#Kd_" + ControllerParameters.Kd);

                // if the first character is '#', then remove it
                if (message.Substring(0, 1) == "#")  message = message.Substring(1);
                
                // send message
                client.send(message);

                // flag that the GUI is sending packages to the controller
                if (is_sending == false) is_sending = true;              
            }
        }

        public void ParseMessage(string message)
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

                // if the key doesn't exist, add it
                if (recieved_packages.ContainsKey(key) == false)
                {
                    recieved_packages.Add(key, new DataContainer(n_steps));
                    if (key == "yo1" || key == "yc1") recieved_packages[key].hasResidual = true; // flag the state yc1 as having a residual  
                }

                // store the value
                recieved_packages[key].InsertData(time, value);
            }

            // update state estimator and calculate residual
            StateEstimate(time);
            if (recieved_packages.ContainsKey("yo1")) recieved_packages["yo1"].CalculateResidual(estimates["yo1_hat"].GetLastValue());  
            if (recieved_packages.ContainsKey("yc1")) recieved_packages["yc1"].CalculateResidual(estimates["yc1_hat"].GetLastValue());

            // add reference time-stamp
            references["r1"].CopyAndPushArray();
            references["r2"].CopyAndPushArray();          
        } 

        private void StateEstimate(string time)
        {
            // estimate states             
            double z = Convert.ToDouble(recieved_packages["yc1"].GetLastValue());
            double u = Convert.ToDouble(recieved_packages["u1"].GetLastValue());
            double[,] x = filter.Update(z, u);

            // store the value
            estimates["yo1_hat"].InsertData(time, x[0, 0].ToString());
            estimates["yc1_hat"].InsertData(time, x[1, 0].ToString());
        }

        public string GetStatus()
        {
            if (is_connected_to_process == true)
            {
                status = "GUI <-> Contr <-> Plant";
                if (LastRecievedStatus() == false) status = "GUI || Contr ? Plant";
            }
            else if (is_sending == true && is_recieving == true) status = "GUI <-> Contr";
            else if (is_sending == false && is_recieving == true) status = "GUI <- Contr";
            else if (is_sending == true && is_recieving == false) status = "GUI -> Contr";
            return status;
        }

        public bool LastRecievedStatus()
        {
            TimeSpan time_diff = DateTime.UtcNow - last_recieved_time;

            if (time_diff.Seconds > 3) return false;
            else return true;    
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

    public struct ConnectionParameters
    {
        public string ip_recieve, ip_send;
        public int port_recieve, port_send;

        public ConnectionParameters(string ip_recieve, string ip_send, int port_recieve, int port_send)
        {
            this.ip_recieve = ip_recieve;
            this.ip_send = ip_send;
            this.port_recieve = port_recieve;
            this.port_send = port_send;
        }
    }

    public class DataContainer
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // store the time:value pair in string arrays
        public string[] time;
        public string[] value;
        public string[] residual;

        // residual setting
        public bool hasResidual = false;

        // constructor
        public DataContainer(int size)
        {
            time = new string[size];
            value = new string[size];
            residual = new string[size];
        }

        // instert a new time:value pair data point
        public void InsertData(string time, string value)
        {
            Array.Copy(this.time, 1, this.time, 0, this.time.Length - 1);
            this.time[this.time.Length - 1] = time;

            Array.Copy(this.value, 1, this.value, 0, this.value.Length - 1);
            this.value[this.value.Length - 1] = value;
        }

        // calculate residual
        public void CalculateResidual(string value)
        {
            double resid = Convert.ToDouble(GetLastValue()) - Convert.ToDouble(value);
            Array.Copy(residual, 1, residual, 0, residual.Length - 1);
            residual[residual.Length - 1] = resid.ToString();
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
