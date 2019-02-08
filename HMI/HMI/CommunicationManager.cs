using System;
using System.Collections.Generic;
using System.Threading;
using Communication;
using System.Diagnostics;
using System.Linq;
using GlobalComponents;

namespace HMI
{
    public class CommunicationManager
    {
        // main form access
        FrameGUI Main;

        // identity parameters
        public string name = "";

        // connection parameters
        public AddressEndPoint Channel_EP;
        public ConnectionParameters Controller_EP;
        public bool is_recieving_packets = false; // flag that packets are being received
        public bool is_sending_packets = false; // flag that packets are being sent
        public bool is_connected_to_plant = false; // flag that packets from the plant has been received (e.g. yc1 or yo1)

        // time variables
        public DateTime time_last_recieved_packet;

        // number of controlled states
        public int n_controlled_states = 0;

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> recieved_packets = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> references = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> estimates = new Dictionary<string, DataContainer>();

        // define specific keys with pre-defined meanings
        string[] DEF_plant_states = new string[] { "yo1", "yc1", "yo2", "yc2" }; // observed and controlled states
        string[] DEF_controlled_states = new string[] { "yc1", "yc2" }; // states that can be controlled
        string[] DEF_residual_states = new string[] { "yc1" }; // states that will have residuals calculated 

        // controller parameters
        public PIDparameters ControllerParameters;

        // initialize estimator
        public KalmanFilter kalman_filter = new KalmanFilter(new double[2, 1] { { 0 }, { 0 } }, 0.3, 0.2, 15.0, 50, 6.5); // x0, a1, a2, A1, A2, k

        public CommunicationManager(FrameGUI Main, string name, PIDparameters ControllerParameters, AddressEndPoint Channel_EP, ConnectionParameters Controller_EP)
        {
            // main form access
            this.Main = Main;

            // canal address
            this.Channel_EP = Channel_EP;

            // identity parameters
            this.name = name;
            this.Controller_EP = Controller_EP;

            // update controller parameters
            this.ControllerParameters = ControllerParameters;

            // specify the endpoint (either channel or directly the controller)
            AddressEndPoint EP = new AddressEndPoint();
            if (Main.usingCanal == true)
                EP = new AddressEndPoint(Channel_EP.IP, Channel_EP.Port);
            else
                EP = new AddressEndPoint(Controller_EP.IP, Controller_EP.Port);

            // create a new thread for the sender
            Thread thread_sender = new Thread(() => Sender(EP.IP, EP.Port));
            thread_sender.Start();

            // create a new thread for the listener
            Thread thread_listener = new Thread(() => Listener(EP.IP, Controller_EP.PortThis));
            thread_listener.Start();
        }

        private void Sender(string IP, int port)
        {
            // initialize a connection to the controller
            Client sender = new Client(IP, port);

            // send messages to a host on this specific IP:port
            while (true)
            {
                Thread.Sleep(100);

                // attatch reference values
                string message = "";
                if (Main.usingCanal == true) message += Convert.ToString("EP_" + Controller_EP.IP + ":" + Controller_EP.Port + "#");
                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT) + "#");

                for (int i = 1; i <= n_controlled_states; i++)
                {
                    message += "r" + i + "_" + references["r" + i].GetLastValue() + "#";
                }

                // attach controller parameters
                message += Convert.ToString("Kp_" + ControllerParameters.Kp + "#Ki_" + ControllerParameters.Ki + "#Kd_" + ControllerParameters.Kd);

                // remove redundant delimiter
                if (message.Substring(0, 1) == "#") message = message.Substring(1);

                // send message
                sender.Send(message);

                // flag that the GUI is sending packets to the controller
                if (is_sending_packets == false) is_sending_packets = true;
            }
        }

        private void Listener(string IP, int port)
        {
            // initialize a connection to the controller
            Server listener = new Server(IP, port);
          
            // listen for messages sent from a host to this specific IP:port
            while (true)
            {
                try
                {
                    // wait for a new packet
                    listener.Listen();
                    time_last_recieved_packet = DateTime.UtcNow;

                    // parse the message 
                    ParseMessage(listener.last_recieved);

                    // flag that the GUI is recieving packets from the controller
                    if (is_recieving_packets == false) is_recieving_packets = true;
                }
                catch (Exception ex)
                {
                    Main.log(Convert.ToString(ex));
                }
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
                if (recieved_packets.ContainsKey(key) == false)
                {
                    recieved_packets.Add(key, new DataContainer(Constants.n_steps));

                    // add an eventual residual and estimate continer
                    if (DEF_residual_states.Contains(key) == true)
                    { 
                        recieved_packets[key].has_residual = true;
                        Helpers.ManageEstimatesKeys(key, estimates, Constants.n_steps);
                        if (key == "yc1") Helpers.ManageEstimatesKeys("yo1", estimates, Constants.n_steps); // also estimate yo1 (hardcoded)
                    }

                    // increment the controlled states counter if it's a controlled state
                    if (DEF_controlled_states.Contains(key))
                    {
                        n_controlled_states += 1;
                        Helpers.ManageReferencesKeys(n_controlled_states, references, Constants.n_steps);
                    }

                    // flag that the GUI is recieving packets originating from the process
                    if (DEF_plant_states.Contains(key)) is_connected_to_plant = true;             
                }

                // store the value
                recieved_packets[key].InsertData(time, value);
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
            double z = Convert.ToDouble(recieved_packets["yc1"].GetLastValue());
            double u = Convert.ToDouble(recieved_packets["u1"].GetLastValue());
            double[,] x = kalman_filter.Update(z, u);

            // store the value
            if (estimates.ContainsKey("yo1_hat")) estimates["yo1_hat"].InsertData(time, x[0, 0].ToString());
            if (estimates.ContainsKey("yc1_hat")) estimates["yc1_hat"].InsertData(time, x[1, 0].ToString());

            // store the residual
            if (recieved_packets.ContainsKey("yc1")) recieved_packets["yc1"].InsertResidual(kalman_filter.innovation.ToString());
        }

        public void UpdateKalmanFilter(double A1, double a1, double A2, double a2)
        {
            kalman_filter = new KalmanFilter(new double[2, 1] { { 0 }, { 0 } }, a1, a2, A1, A2, 6.5); // x0, a1, a2, A1, A2, k
        }

        public string GetConnectionStatus()
        {
            string status = "";
            if (is_connected_to_plant == true)
            {
                status = "GUI <-> Contr <-> Plant";
                if (isTrafficEstablished() == false) status = "GUI || Contr ? Plant";
            }
            else if (is_sending_packets == true && is_recieving_packets == true) status = "GUI <-> Contr ? Plant";
            else if (is_sending_packets == false && is_recieving_packets == true) status = "GUI <- Contr ? Plant";
            else if (is_sending_packets == true && is_recieving_packets == false) status = "GUI -> Contr ? Plant";
            return status;
        }

        public bool isTrafficEstablished()
        {
            TimeSpan time_diff = DateTime.UtcNow - time_last_recieved_packet;
            if (time_diff.Seconds > 3) return false;
            else return true;    
        }
    }
}
