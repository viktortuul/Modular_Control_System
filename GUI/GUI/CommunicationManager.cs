using System;
using System.Collections.Generic;
using System.Threading;
using Communication;
using System.Diagnostics;

namespace GUI
{
    public class CommunicationManager
    {
        // main form access
        FrameGUI Main;

        // identity parameters
        public string name = "";
        string status = "";

        // connection parameters
        public AddressEndPoint CanalEP;
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
        public KalmanFilter filter = new KalmanFilter(new double[2, 1] { { 0 }, { 0 } }, 0.16 * 2.5, 0.16 * 2.0, 15.0, 30, 6.5);

        public CommunicationManager(FrameGUI Main, string name, PIDparameters ControllerParameters, AddressEndPoint CanalEP, ConnectionParameters ConnectionParameters)
        {
            // main form access
            this.Main = Main;

            // canal address
            this.CanalEP = CanalEP;

            // identity parameters
            this.name = name;
            this.ConnectionParameters = ConnectionParameters;

            // update controller parameters
            this.ControllerParameters = ControllerParameters;

            // specify the endpoint
            AddressEndPoint EP = new AddressEndPoint();
            if (Main.usingCanal == true)
                EP = new AddressEndPoint(CanalEP.IP, CanalEP.Port);
            else
                EP = new AddressEndPoint(ConnectionParameters.IP, ConnectionParameters.Port);
            
            // create a new thread for the listener
            Thread thread_listener = new Thread(() => Listener(EP.IP, ConnectionParameters.PortThis));
            thread_listener.Start();

            // create a new thread for the sender
            Thread thread_sender = new Thread(() => Sender(EP.IP, EP.Port));
            thread_sender.Start();
        }

        private void Listener(string IP, int port)
        {
            // initialize a connection to the controller
            Server Listener = new Server(IP, port);
          
            // listen for messages sent from a host to this specific IP:port
            while (true)
            {
                Thread.Sleep(1);
                try
                {
                    // check if a new package is recieved
                    Listener.Listen();
                    last_recieved_time = DateTime.UtcNow;

                    // parse the message which may contain u1, u2, yc1, yc2, yo1, yo2
                    ParseMessage(Listener.last_recieved);

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
                }
                catch (Exception ex)
                {
                    Main.log(Convert.ToString(ex));
                }
            }
        }

        private void Sender(string IP, int port)
        {
            // initialize a connection to the controller
            Client Sender = new Client(IP, port);

            // send messages to a host on this specific IP:port
            while (true)
            {
                Thread.Sleep(100);

                // attatch reference values
                string message = "";
                if (Main.usingCanal == true) message += Convert.ToString("EP_" + ConnectionParameters.IP + ":" + ConnectionParameters.Port + "#");
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
                Sender.Send(message);

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
                if (key == "time") { time = value; continue; }

                // if the key doesn't exist, add it
                if (recieved_packages.ContainsKey(key) == false)
                {
                    recieved_packages.Add(key, new DataContainer(Constants.n_steps));

                    // flag if it has a residual or not
                    if (key == "yc1")
                    { 
                        recieved_packages[key].hasResidual = true; // flag the state yc1 as having a residual  
                        Helpers.ManageEstimatesKeys(key, estimates, Constants.n_steps);
                        Helpers.ManageEstimatesKeys("yo1", estimates, Constants.n_steps); // also add an estimate data set
                    }

                    // increment  the controlled states counter
                    if (key.Contains("yc"))
                    {
                        n_contr_states += 1;
                        Helpers.ManageReferencesKeys(n_contr_states, references, Constants.n_steps);
                    }
                }

                // store the value
                recieved_packages[key].InsertData(time, value);
            }

            // update state estimator and calculate residual
            StateEstimate(time);

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
            if (estimates.ContainsKey("yo1_hat")) estimates["yo1_hat"].InsertData(time, x[0, 0].ToString());
            if (estimates.ContainsKey("yc1_hat")) estimates["yc1_hat"].InsertData(time, x[1, 0].ToString());

            // store the residual
            if (recieved_packages.ContainsKey("yc1")) recieved_packages["yc1"].InsertResidual(filter.innovation.ToString());
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
