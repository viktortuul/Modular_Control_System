using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using Communication;
using System.Globalization;

namespace GUI
{
    public partial class Form1 : Form
    {
        // connection count
        public int n_connections = 0;

        // chart settings
        public static int n_steps = 500;

        // container for controller module connections
        List<ControllerConnection> connections = new List<ControllerConnection>();
        ControllerConnection connection_current;

        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fffffff";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set chart axes
            dataChart.ChartAreas["ChartArea1"].AxisX.Title = "Time [steps * 100ms]";
            dataChart.ChartAreas["ChartArea1"].AxisY.Title = "Magnitude";
        }
 
        private void timerChart_Tick(object sender, EventArgs e)
        {
            // add the correct number of reference series
            for (int i = 0; i < connection_current.n_refs; i++)
            {
                if ((dataChart.Series.IndexOf("r" + (i + 1).ToString()) == -1))
                AddChartSeries("r" + (i + 1).ToString());
            }

            // push each element in the data vectors one step back
            foreach (ControllerConnection controller in connections)
            {
                var keys = controller.package_last.Keys.ToList();
                keys.Remove("time"); // don't consider the time key

                foreach (string key in keys)
                {
                    if (controller.packages.ContainsKey(key) == false)
                    {
                        controller.packages.Add(key, new double[n_steps]);
                    }
                    
                    Array.Copy(controller.packages[key], 1, controller.packages[key], 0, controller.packages[key].Length - 1);
                    controller.packages[key][controller.packages[key].Length - 1] = Convert.ToDouble(controller.package_last[key]);
                }

                // push reference values
                for (int i = 0; i < controller.n_refs; i++)
                {
                    for (int j = 0; j < controller.r_array.GetLength(1) - 1; j++)
                    {
                        controller.r_array[i, j] = controller.r_array[i, j + 1];
                    }
                    controller.r_array[i, n_steps - 1] = controller.r[i];
                }
            }

            // refresh the chart
            if (connections.Count > 0)
            {
                UpdateChart(connection_current);       
            }

            // enable or disable reference track bars
            ManageTrackbars();

        }

        public void AddChartSeries(string key)
        {
            // add a new time series
            log("<" + key + ">");

            this.Invoke((MethodInvoker)delegate ()
            {
                if (dataChart.Series.IndexOf(key) == -1)
                {
                    dataChart.Series.Add(key);
                    dataChart.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    dataChart.Series[key].BorderWidth = 2;

                    switch (key)
                    {
                        case "r1":
                            dataChart.Series[key].Color = Color.Red;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "r2":
                            dataChart.Series[key].Color = Color.Maroon;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "u1":
                            dataChart.Series[key].Color = Color.Orange;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "u2":
                            dataChart.Series[key].Color = Color.Yellow;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "yo1":
                            dataChart.Series[key].Color = Color.Blue;
                            break;
                        case "yo2":
                            dataChart.Series[key].Color = Color.Green;
                            break;
                        case "yc1":
                            dataChart.Series[key].Color = Color.Black;
                            break;
                        case "yc2":
                            dataChart.Series[key].Color = Color.Gray;
                            break;
                        default:
                            break;
                    }        
                 }
            });
        }

        private void UpdateChart(ControllerConnection controller)
        {
            var keys = controller.packages.Keys.ToArray();

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (dataChart.Series.IndexOf(key) == -1)
                {
                    AddChartSeries(key);
                }

                dataChart.Series[key].Points.Clear();
                for (int i = 0; i < controller.packages[key].Length; i++)
                {
                    dataChart.Series[key].Points.AddY(controller.packages[key][i]);              
                }      
            }

            // clear and update the graphs (for the reference)
            try
            {
                for (int i = 0; i < controller.n_refs; i++)
                {
                    dataChart.Series["r" + (i + 1).ToString()].Points.Clear();
                    for (int j = 0; j < controller.r_array.GetLength(1); j++)
                    {
                        dataChart.Series["r" + (i + 1).ToString()].Points.AddY(controller.r_array[i, j]);
                    }
                }
            }
            catch (Exception ex)
            {
                log(ex.ToString());
            }

            // update tank charts
            try
            {
                chartTankTop.Series["tank_top"].Points.Clear();
                chartTankBottom.Series["tank_bottom"].Points.Clear();
                chartTankTop.Series["tank_top"].Points.AddY(Convert.ToDouble(controller.package_last["yo1"]));
                chartTankBottom.Series["tank_bottom"].Points.AddY(Convert.ToDouble(controller.package_last["yc1"]));
            }
            catch { }

            // update time labels
            try
            {
                DateTime time_sent = DateTime.ParseExact(controller.package_last["time"], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = controller.last_recieved_time - time_sent;
                label_time.Text = "time now: " + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") +
                                    "\nlast recieved: " + controller.last_recieved_time.ToString("hh:mm:ss.fff tt") +
                                    "\nwhen it was sent: " + time_sent.ToString("hh:mm:ss.fff tt") +
                                    "\ntransmission duration: " + timeDiff.TotalMilliseconds;
            }
            catch { }
        }

        private void listBoxControllers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // specify the new connection
                connection_current = connections[listBoxModules.SelectedIndex];

                // clear and add keys to the new chart
                dataChart.Series.Clear();
                var keys = connection_current.packages.Keys.ToList();
                keys.Remove("time");

                for (int i = 0; i < connection_current.n_refs; i++)
                {
                    AddChartSeries("r" + (i+1).ToString());
                }

                // enable or disable reference track bars
                ManageTrackbars();


                foreach (string key in keys)
                {
                    AddChartSeries(key);
                }

                // update GUI values according to the connected controller
                trackBarReference.Value = Convert.ToInt16(connection_current.r[0]);
                trackBar1.Value = Convert.ToInt16(connection_current.r[1]);
                numUpDownKp.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kp);
                numUpDownKi.Value = Convert.ToDecimal(connection_current.ControllerParameters.Ki);
                numUpDownKd.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kd);
                textBox_ip_send.Text = connection_current.ConnectionParameters.ip_send;
                textBox_ip_recieve.Text = connection_current.ConnectionParameters.ip_recieve;
                numericUpDown_port_send.Text = connection_current.ConnectionParameters.port_send.ToString();
                numericUpDown_port_recieve.Text = connection_current.ConnectionParameters.port_recieve.ToString();

                // select the corresponding item in the treeview
                treeViewControllers.SelectedNode = treeViewControllers.Nodes[connection_current.name];
            }
            catch (Exception ex)
            {
                log(ex.ToString());
            }
        }

        public void ManageTrackbars()
        {
            // enable or disable trackbars
            if (connection_current.n_refs == 1)
            {
                trackBarReference.Enabled = true;
            }
            else if (connection_current.n_refs == 2)
            {
                trackBarReference.Enabled = true;
                trackBar1.Enabled = true;
            }
            else
            {
                trackBarReference.Enabled = false;
                trackBar1.Enabled = false;
            }
        }

        /*
        public void UpdateListBox(ControllerConnection controller) // renames an item in the listbox (not currently in use)
        {
            int index = 0;
            for (int i = 0; i < connections.Count(); i++)
            {
                if (connections[i].Equals(controller))
                {
                    index = i;
                    break;
                }
            }

            // update listbox
            this.Invoke((MethodInvoker)delegate ()
            {
                listBoxControllers.Items[index] = controller.Status();
            });
        }
        */

        public void UpdateTree(ControllerConnection controller)
        {
            // update tree
            this.Invoke((MethodInvoker)delegate ()
            {
                treeViewControllers.Nodes[controller.name].Text = controller.name + " (" + controller.Status() + ")";
                treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kp"].Text = connection_current.ControllerParameters.Kp.ToString();
                treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Ki"].Text = connection_current.ControllerParameters.Ki.ToString();
                treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kd"].Text = connection_current.ControllerParameters.Kd.ToString();
            });
        }

    private void btnAllowConnection_Click_1(object sender, EventArgs e)
        {
            // controller name and corresponding ip:port pairs
            string label = textBoxName.Text;
            string ip_send = textBox_ip_send.Text;
            string ip_recieve = textBox_ip_recieve.Text;
            int port_recieve = Convert.ToInt16(numericUpDown_port_recieve.Value);
            int port_send = Convert.ToInt16(numericUpDown_port_send.Text);

            // look for name and port conflicts with present allowed traffics
            bool already_exists = false;
            foreach (ControllerConnection controller in connections)
            {
                if (controller.ConnectionParameters.port_recieve == port_recieve ||
                    controller.ConnectionParameters.port_send == port_send ||
                    controller.name == label)
                {
                    already_exists = true;
                }
            }
            if (already_exists == true)
            {
                log("Error: name or port already in use");
            }
            else
            {
                // connection parameters
                ConnectionParameters ConnParams = new ConnectionParameters(ip_recieve, ip_send, port_recieve, port_send);

                // create and add controller
                PIDparameters ControllerParameters = new PIDparameters(Convert.ToDouble(numUpDownKp.Value), Convert.ToDouble(numUpDownKi.Value), Convert.ToDouble(numUpDownKd.Value));
                ControllerConnection PID_1 = new ControllerConnection(this, label, ControllerParameters, ConnParams);
                connections.Add(PID_1);
                connection_current = PID_1;
                listBoxModules.Items.Add(label);

                // if no listbox item is selected, select the top one
                if (listBoxModules.SelectedIndex == -1)
                {
                    listBoxModules.SelectedIndex = 0;
                }

                timerChart.Start();


                // create a root node
                treeViewControllers.Nodes.Add(label, label);
                treeViewControllers.Nodes[label].Nodes.Add("Connection", "Connection");
                treeViewControllers.Nodes[label].Nodes["Connection"].Nodes.Add("ip", "IP: " + ip_send.ToString());
                treeViewControllers.Nodes[label].Nodes["Connection"].Nodes.Add("port", "Port: " + port_send.ToString());
                treeViewControllers.Nodes[label].Nodes["Connection"].Nodes.Add("port_gui", "Port (gui): " + port_recieve.ToString());
                treeViewControllers.Nodes[label].Nodes.Add("Controller", "Controller (Kp, Ki, Kd)");
                treeViewControllers.Nodes[label].Nodes["Controller"].Nodes.Add("Kp", ControllerParameters.Kp.ToString());
                treeViewControllers.Nodes[label].Nodes["Controller"].Nodes.Add("Ki", ControllerParameters.Ki.ToString());
                treeViewControllers.Nodes[label].Nodes["Controller"].Nodes.Add("Kd", ControllerParameters.Kd.ToString());
                //treeView1.ExpandAll();

                // update counter
                n_connections += 1;
                textBoxName.Text = "Module" + (n_connections + 1);
            }
        }


        // ControllerConnection ###########################################################################################
        public class ControllerConnection
        {
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
            public double[] r = new double[] {0, 0}; // current reference value
            public double[,] r_array = new double[2, n_steps]; // reference value (array) 

            // data containers (dictionaries)
            public Dictionary<string, string> package_last = new Dictionary<string, string>(); // recieved values (last)
            public Dictionary<string, double[]> packages = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

            // controller parameters
            public PIDparameters ControllerParameters;

            // main form access
            Form1 Main_form;

            public ControllerConnection(Form1 f, string label_, PIDparameters ControllerParameters_, ConnectionParameters ConnectionParameters_)
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
                        message += "r" + (i+1).ToString() + "_" + r[i] + "#";
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

            public string Status()
            {
                if (is_sending == true && is_recieving == true)
                {
                    status = "active";
                }
                else if (is_sending == false && is_recieving == true)
                {
                    status = "only recieving";
                }
                else if (is_sending == true && is_recieving == false)
                {
                    status = "only sending";
                }

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

        private void trackBarReference_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.r[0] = trackBarReference.Value;
                labelReference1.Text = trackBarReference.Value.ToString();
            }
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.r[1] = trackBar1.Value;
                labelReference2.Text = trackBar1.Value.ToString();
            }
        }

        private void numUpDownKp_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                connection_current.ControllerParameters.Kp = Convert.ToDouble(numUpDownKp.Value);
                UpdateTree(connection_current);
            }
            catch { }
 
        }

        private void numUpDownKi_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                connection_current.ControllerParameters.Ki = Convert.ToDouble(numUpDownKi.Value);
                UpdateTree(connection_current);
            }
            catch { }
        }

        private void numUpDownKd_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                connection_current.ControllerParameters.Kd = Convert.ToDouble(numUpDownKd.Value);
                UpdateTree(connection_current);
            }
            catch { }
        }


        public void EditListBox(int index, string message)
        {
            listBoxModules.Items[index] = message;
        }

        public void log(string text)
        {
            this.Invoke((MethodInvoker)delegate () { tbDebugLog.AppendText(text + Environment.NewLine); });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

    }
}
