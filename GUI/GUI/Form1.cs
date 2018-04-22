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
        // chart settings
        static int n_steps = 500;


        // controller container
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
            AddChartSeries("reference");
        }
 
        private void timerChart_Tick(object sender, EventArgs e)
        {
            int index = -1;
            foreach (ControllerConnection controller in connections)
            {
                index++;

                // update listbox
                if (controller.connection_recieving == true)
                {
                    listBoxControllers.Items[index] = "PID (recieving)";
                }
                

                // push each element in the arrays one step back (don't consider the time key)
                var keys = controller.dict_last.Keys.ToList();
                keys.Remove("time");

                foreach (string key in keys)
                {
                    if (controller.dict_temporal.ContainsKey(key) == false)
                    {
                        controller.dict_temporal.Add(key, new double[n_steps]);
                    }
                    
                    Array.Copy(controller.dict_temporal[key], 1, controller.dict_temporal[key], 0, controller.dict_temporal[key].Length - 1);
                    controller.dict_temporal[key][controller.dict_temporal[key].Length - 1] = Convert.ToDouble(controller.dict_last[key]);
                }

                // reference
                Array.Copy(controller.r_array, 1, controller.r_array, 0, controller.r_array.Length - 1);
                controller.r_array[controller.r_array.Length - 1] = controller.r;

            }

            if (connections.Count > 0)
            {
                UpdateChart(connection_current);       
            }
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
                        case "reference":
                            dataChart.Series[key].Color = Color.Red;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "u":
                            dataChart.Series[key].Color = Color.Orange;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "y1":
                            dataChart.Series[key].Color = Color.Blue;
                            break;
                        case "y2":
                            dataChart.Series[key].Color = Color.Green;
                            break;
                        default:
                            break;
                    }        
                 }
            });
        }

        public void EditListBox(int index, string message)
        {
            listBoxControllers.Items[index] = message;
        }

        public void log(string text)
        {
            this.Invoke((MethodInvoker)delegate () { tbDebugLog.AppendText(text + Environment.NewLine); });
        }

        private void UpdateChart(ControllerConnection controller)
        {
            var keys = controller.dict_temporal.Keys.ToArray();

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                dataChart.Series[key].Points.Clear();
                for (int i = 0; i < controller.dict_temporal[key].Length; i++)
                {
                    dataChart.Series[key].Points.AddY(controller.dict_temporal[key][i]);              
                }      
            }

            // clear and update the graphs (for the reference)
            dataChart.Series["reference"].Points.Clear();
            for (int i = 0; i < controller.r_array.Length; i++)
            {
                dataChart.Series["reference"].Points.AddY(controller.r_array[i]);
            }


            // Tank charts
            try
            {
                chartTankTop.Series["tank_top"].Points.Clear();
                chartTankBottom.Series["tank_bottom"].Points.Clear();
                chartTankTop.Series["tank_top"].Points.AddY(Convert.ToDouble(controller.dict_last["y1"]));
                chartTankBottom.Series["tank_bottom"].Points.AddY(Convert.ToDouble(controller.dict_last["y2"]));
            }
            catch { }



            // display timers
            try
            {
                DateTime time_sent = DateTime.ParseExact(controller.dict_last["time"], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = controller.last_recieved_time - time_sent;
                label_time.Text = "time now: " + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") +
                                    "\nlast recieved: " + controller.last_recieved_time.ToString("hh:mm:ss.fff tt") +
                                    "\nwhen it was sent: " + time_sent.ToString("hh:mm:ss.fff tt") +
                                    "\ntransmission duration: " + timeDiff.TotalMilliseconds;
            }
            catch { }
        }


        public class ControllerConnection
        {
            // identity string
            public string identity = "";


            // connection parameters
            public bool connection_recieving = false;
            public bool connection_sending = false;

            // time
            public DateTime last_recieved_time;

            // display variables
            public double r = 0; // reference value
            public double[] r_array = new double[n_steps]; // reference value (array) 

            public Dictionary<string, string> dict_last = new Dictionary<string, string>(); // recieved values (last)
            public Dictionary<string, double[]> dict_temporal = new Dictionary<string, double[]>(); // recieved values (array)

            public PIDparameters PID;

            // main form access
            Form1 Main_form;

            public ControllerConnection(Form1 f1, PIDparameters parameters, string ip_recieve, string ip_send, int port_recieve, int port_send)
            {
                // create a new thread for the listener
                Thread thread_listener = new Thread(() => Listener(ip_send, port_recieve));
                thread_listener.Start();

                // create a new thread for the sender
                Thread thread_sender = new Thread(() => Sender(ip_recieve, port_send));
                thread_sender.Start();

                // update controller parameters
                PID = new PIDparameters(parameters.Kp, parameters.Ki, parameters.Kd);

                // main form access
                Main_form = f1;
            }

            private void Listener(string IP, int port)
            {
                // initialize a connection to the controller
                Server server = new Server(IP, port);

                // listen for messages sent from the client
                while (true)
                {
                    Thread.Sleep(50);
                    try
                    {
                        // check if a new package is recieved
                        server.listen();
                        last_recieved_time = DateTime.UtcNow;

                        // parse the message which contains time, u, y1, y2
                        parse_message(server.last_recieved);

                        connection_recieving = true;
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

                // listen for messages sent from the client
                while (true)
                {
                    Thread.Sleep(100);
                    // send r, kp, ki, kd
                    string message = Convert.ToString("r_" + r + "#Kp_" + PID.Kp + "#Ki_" + PID.Ki + "#Kd_" + PID.Kd);
                    client.send(message);

                    connection_sending = true;
                }
            }

            public void parse_message(string message)
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

                    if (dict_last.ContainsKey(key) == false)
                    {
                        
                        // add new chart
                        if (key != "time")
                        {
                            Main_form.AddChartSeries(key);             
                        }
                        dict_last.Add(key, value);
                    }
                    else
                    {
                        dict_last[key] = value;
                    }
  
                }
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

        private void tbReference_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            connection_current.r = tbReference.Value;
            labelReference.Text = connection_current.r.ToString();
        }

        private void numUpDownKp_ValueChanged(object sender, EventArgs e)
        {
            connection_current.PID.Kp = Convert.ToDouble(numUpDownKp.Value);
        }

        private void numUpDownKi_ValueChanged(object sender, EventArgs e)
        {
            connection_current.PID.Ki = Convert.ToDouble(nUD_Ki.Value);
        }

        private void numUpDownKd_ValueChanged(object sender, EventArgs e)
        {
            connection_current.PID.Kd = Convert.ToDouble(nUD_Kd.Value);
        }


        private void listBoxControllers_SelectedIndexChanged(object sender, EventArgs e)
        {
            connection_current = connections[listBoxControllers.SelectedIndex];
            tbReference.Value = Convert.ToInt16(connection_current.r);
            numUpDownKp.Value = Convert.ToDecimal(connection_current.PID.Kp);
            nUD_Ki.Value = Convert.ToDecimal(connection_current.PID.Ki);
            nUD_Kd.Value = Convert.ToDecimal(connection_current.PID.Kd);
            labelReference.Text = connection_current.r.ToString();

        }


        private void btnAllowConnection_Click_1(object sender, EventArgs e)
        {
            string ip_send = textBox_ip_send.Text;
            string ip_recieve = textBox_ip_recieve.Text;
            int port_recieve = Convert.ToInt32(textBox_port_recieve.Text);
            int port_send = Convert.ToInt32(textBox_port_send.Text);

            // create and add controller
            PIDparameters parameters = new PIDparameters(2, 100, 0.2);
            ControllerConnection PID_1 = new ControllerConnection(this, parameters, ip_recieve, ip_send, port_recieve, port_send);
            connections.Add(PID_1);
            connection_current = PID_1;
            listBoxControllers.Items.Add("PID (waiting)");

            timerChart.Start();
        }  
    }
}
