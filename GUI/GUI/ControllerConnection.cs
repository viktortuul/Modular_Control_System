using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Communication;

namespace GUI
{
    public class ControllerConnection
    {
        // chart settings
        public static int n_steps = 500;

        // identity parameters
        public string name = "";
        string status = "waiting";

        // connection parameters
        public ConnectionParameters ConnectionParameters;
        public bool is_recieving = false;
        public bool is_sending = false;

        // time parameters
        public DateTime last_recieved_time;

        // display variables
        public int n_refs = 0;
        public double[] r = new double[] { 0, 0 }; // current reference value
        public double[,] r_array = new double[2, n_steps]; // reference value (array) 

        // data containers (dictionaries)
        public Dictionary<string, string> package_last = new Dictionary<string, string>(); // recieved values (last)
        public Dictionary<string, double[]> packages = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        // controller parameters
        public PIDparameters ControllerParameters;

        // main form access
        FrameGUI Main_form;

        public ControllerConnection(FrameGUI f, string label_, PIDparameters ControllerParameters_, ConnectionParameters ConnectionParameters_)
        {
            // main form access
            Main_form = f;

            // identity parameters
            name = label_;
            ConnectionParameters = ConnectionParameters_;

            // update controller parameters
            ControllerParameters = ControllerParameters_;

            // create a new thread for the listener
            Thread thread_listener = new Thread(() => Listener(ConnectionParameters.ip_recieve, ConnectionParameters.port_recieve));
            thread_listener.Start();

            // create a new thread for the sender
            Thread thread_sender = new Thread(() => Sender(ConnectionParameters.ip_send, ConnectionParameters.port_send));
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

                    int counter = 0;
                    foreach (string key in package_last.Keys)
                    {
                        if (key.Contains("yc")) counter++;
                    }
                    n_refs = counter;

                    // parse the message which contains time, u, y1, y2
                    ParseMessage(server.last_recieved);

                    // flag that the GUI is recieving packages from the controller
                    if (is_recieving == false)
                    {
                        is_recieving = true;
                        Main_form.UpdateTree(this);
                    }
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
                for (int i = 0; i < n_refs; i++)
                {
                    message += "r" + (i + 1).ToString() + "_" + r[i] + "#";
                }

                // attach controller parameters
                message += Convert.ToString("Kp_" + ControllerParameters.Kp + "#Ki_" + ControllerParameters.Ki + "#Kd_" + ControllerParameters.Kd);

                if (message.Substring(0, 1) == "#")
                {
                    message = message.Substring(1);
                }

                client.send(message);

                // flag that the GUI is sending packages to the controller
                if (is_sending == false)
                {
                    is_sending = true;
                    Main_form.UpdateTree(this);
                }
            }
        }

        public void ParseMessage(string message)
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

                // store the value
                if (package_last.ContainsKey(key) == false)
                {
                    package_last.Add(key, value);
                }
                else
                {
                    package_last[key] = value;
                }

            }
        }


        public void PushArrays()
        {
            var keys = package_last.Keys.ToList();
            keys.Remove("time"); // don't consider the time key

            foreach (string key in keys)
            {
                if (packages.ContainsKey(key) == false)
                {
                    packages.Add(key, new double[n_steps]);
                }

                Array.Copy(packages[key], 1, packages[key], 0, packages[key].Length - 1);
                packages[key][packages[key].Length - 1] = Convert.ToDouble(package_last[key]);
            }

            // push reference values
            for (int i = 0; i < n_refs; i++)
            {
                for (int j = 0; j < r_array.GetLength(1) - 1; j++)
                {
                    r_array[i, j] = r_array[i, j + 1];
                }
                r_array[i, n_steps - 1] = r[i];
            }
        }

        public string GetStatus()
        {
            if (is_sending == true && is_recieving == true) status = "active";
            else if (is_sending == false && is_recieving == true) status = "only recieving";
            else if (is_sending == true && is_recieving == false) status = "only sending";
            return status;
        }

    }

    public struct PIDparameters
    {
        public double Kp, Ki, Kd;

        public PIDparameters(double Kp_, double Ki_, double Kd_)
        {
            Kp = Kp_;
            Ki = Ki_;
            Kd = Kd_;
        }
    }

    public struct ConnectionParameters
    {
        public string ip_recieve, ip_send;
        public int port_recieve, port_send;

        public ConnectionParameters(string ip_recieve_, string ip_send_, int port_recieve_, int port_send_)
        {
            ip_recieve = ip_recieve_;
            ip_send = ip_send_;
            port_recieve = port_recieve_;
            port_send = port_send_;
        }
    }

}
