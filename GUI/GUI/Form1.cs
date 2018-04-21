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
        // controller container
        List<Controller> controllers = new List<Controller>();

        // controller parameters
        PIDparameters PID = new PIDparameters(0, 0, 0);

        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fffffff";
        DateTime last_recieved_time;

        // display variables
        double r = 0; // reference value
        double[] r_array = new double[300]; // reference value (array) 

        Dictionary<string, string> dict_last = new Dictionary<string, string>(); // recieved values (last)
        Dictionary<string, double[]> dict_temporal = new Dictionary<string, double[]>(); // recieved values (array)

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // create a new thread for the listener
            Thread thread_listener = new Thread(() => Listener("127.0.0.1", 8885));
            thread_listener.Start();

            // create a new thread for the sender
            Thread thread_sender = new Thread(() => Sender("127.0.0.1", 8886));
            thread_sender.Start();

            // update controller parameters
            PID.Kp = Convert.ToDouble(numericUpDown_Kp.Value);
            PID.Ki = Convert.ToDouble(numericUpDown_Ki.Value);
            PID.Kd = Convert.ToDouble(numericUpDown_Kd.Value);
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
                }
                catch (Exception ex)
                {
                    log(Convert.ToString(ex));
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
                string message =  Convert.ToString("r_" + r + "#Kp_" + PID.Kp + "#Ki_" + PID.Ki + "#Kd_" + PID.Kd);
                client.send(message);
            }
        }

        public void parse_message(string message)
        {
            string text = message;
            // split the message with the delimiter '#'
            string[] container = text.Split('#');

            // log(message);

            foreach (string item in container)
            {
                // split each subtext (key and value)
                string[] subitem = item.Split('_');

                // extract key and value
                string key = subitem[0];
                string value = subitem[1];

                if (dict_last.ContainsKey(key) == false)
                {
                    dict_last.Add(key, value);

                    // add new chart
                    if (key != "time")
                    {
                        this.Invoke((MethodInvoker)delegate () {
                            dataChart.Series.Add(key);
                            dataChart.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                            dataChart.Series[key].BorderWidth = 2;
                        });
                        
                    }

                }
                dict_last[key] = value;
            }
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            // push each element in the arrays one step back
            var keys = dict_last.Keys.ToList();
            keys.Remove("time");

            foreach (string key in keys)
            {
                    if (dict_temporal.ContainsKey(key) == false)
                    {
                        dict_temporal.Add(key, new double[300]);
                    }
                    dict_temporal[key][dict_temporal[key].Length - 1] = Convert.ToDouble(dict_last[key]);
                Array.Copy(dict_temporal[key], 1, dict_temporal[key], 0, dict_temporal[key].Length - 1);
            }

            // reference
            r_array[r_array.Length - 1] = r;
            Array.Copy(r_array, 1, r_array, 0, r_array.Length - 1);
              
            update_chart();


            // display times
            try
            {
                DateTime time_sent = DateTime.ParseExact(dict_last["time"], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = last_recieved_time - time_sent;
                label_time.Text = "time now: " + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") +
                                    "\nlast recieved: " + last_recieved_time.ToString("hh:mm:ss.fff tt") +
                                    "\nwhen it was sent: " + time_sent.ToString("hh:mm:ss.fff tt") +
                                    "\ntransmission duration: " + timeDiff.TotalMilliseconds;
            }
            catch { }

        }

        private void update_chart()
        {
            var keys = dict_temporal.Keys.ToArray();

            // clear and update the graphs
            foreach (string key in keys)
            {
                dataChart.Series[key].Points.Clear();
                for (int i = 0; i < dict_temporal[key].Length; i++)
                {
                    dataChart.Series[key].Points.AddY(dict_temporal[key][i]);
                }
            }

            dataChart.Series["reference"].Points.Clear();
            for (int i = 0; i < r_array.Length; i++)
            {
                dataChart.Series["reference"].Points.AddY(r_array[i]);
            }
            
        }


        public class Controller
        {

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


        private void log(string text)
        {
            this.Invoke((MethodInvoker)delegate () { tbDebugLog.AppendText(text + Environment.NewLine); });
        }

        private void trackBarReference_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            r = trackBarReference.Value;
            labelReference.Text = r.ToString();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown_Kp_ValueChanged(object sender, EventArgs e)
        {
            PID.Kp = Convert.ToDouble(numericUpDown_Kp.Value);
        }

        private void numericUpDown_Ki_ValueChanged(object sender, EventArgs e)
        {
            PID.Ki = Convert.ToDouble(numericUpDown_Ki.Value);
        }

        private void numericUpDown_Kd_ValueChanged(object sender, EventArgs e)
        {
            PID.Kd = Convert.ToDouble(numericUpDown_Kd.Value);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
