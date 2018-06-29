using System;
using System.Collections.Generic;
using System.Threading;
using Communication;
using System.Diagnostics;

namespace GUI
{
    public class ControllerConnection
    {
        // main form access
        FrameGUI Main_form;



        // identity parameters
        public string name = "";
        string status = "";

        // connection parameters
        public ConnectionParameters ConnectionParameters;
        public bool is_recieving = false;
        public bool is_sending = false;
        public bool is_connected_to_process = false;

        // time variables
        public DateTime last_recieved_time;

        // number of controlled states
        public int n_contr_states = 0;

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> recieved_packages = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> references = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> estimates = new Dictionary<string, DataContainer>();

        // controller parameters
        public PIDparameters ControllerParameters;

        // initialize estimator
        KalmanFilter filter = new KalmanFilter(new double[2, 1] { { 0 }, { 0 } }, 0.16 * 2.0, 0.16 * 2.5, 15.0, 100, 6.32);

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
            Thread thread_listener = new Thread(() => Listener(ConnectionParameters.ip_endpoint, ConnectionParameters.port_this));
            thread_listener.Start();

            // create a new thread for the sender
            Thread thread_sender = new Thread(() => Sender(ConnectionParameters.ip_endpoint, ConnectionParameters.port_endpoint));
            thread_sender.Start();         
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
                    foreach (string key in recieved_packages.Keys)
                    {
                        if (key.Contains("yc"))
                        {
                            // flag that full communication is present
                            if (is_connected_to_process == false) is_connected_to_process = true;
                        }
                    }
                    Helpers.ManageReferencesKeys(n_contr_states, references, Constants.n_steps);
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

                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT) + "#");

                for (int i = 1; i <= n_contr_states; i++)
                {
                    message += "r" + i + "_" + references["r" + i].GetLastValue() + "#";
                }

                // attach controller parameters
                message += Convert.ToString("Kp_" + ControllerParameters.Kp + "#Ki_" + ControllerParameters.Ki + "#Kd_" + ControllerParameters.Kd);

                // if the first character is '#', then remove it
                if (message.Substring(0, 1) == "#") message = message.Substring(1);
                
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

                // detect corrupt values
                if (Helpers.isDouble(value) == false)
                {
                    Console.WriteLine("Error: corrupt package  <" + key + "_" + value + ">");
                    continue;
                }

                // if the key doesn't exist, add it
                if (recieved_packages.ContainsKey(key) == false)
                {
                    recieved_packages.Add(key, new DataContainer(Constants.n_steps));
                    if (key == "yo1" || key == "yc1")
                    { 
                        recieved_packages[key].hasResidual = true; // flag the state yc1 as having a residual  
                        Helpers.ManageEstimatesKeys(key, estimates, Constants.n_steps);
                    }

                    if (key.Contains("yc")) n_contr_states += 1;
                }

                // store the value
                recieved_packages[key].InsertData(time, value);
            }

            // update state estimator and calculate residual
            StateEstimate(time);
            if (recieved_packages.ContainsKey("yo1")) recieved_packages["yo1"].CalculateResidual(estimates["yo1_hat"].GetLastValue());  
            if (recieved_packages.ContainsKey("yc1")) recieved_packages["yc1"].CalculateResidual(estimates["yc1_hat"].GetLastValue());

            // add reference time-stamp
            if (references.ContainsKey("r1")) references["r1"].CopyAndPushArray();
            if (references.ContainsKey("r2")) references["r2"].CopyAndPushArray();          
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
                if (trafficEstablished() == false) status = "GUI || Contr ? Plant";
            }
            else if (is_sending == true && is_recieving == true) status = "GUI <-> Contr ? Plant";
            else if (is_sending == false && is_recieving == true) status = "GUI <- Contr ? Plant";
            else if (is_sending == true && is_recieving == false) status = "GUI -> Contr ? Plant";
            return status;
        }

        public bool trafficEstablished()
        {
            TimeSpan time_diff = DateTime.UtcNow - last_recieved_time;

            if (time_diff.Seconds > 3) return false;
            else return true;    
        }
    }
}
