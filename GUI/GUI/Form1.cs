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
        Controller controller_current;

        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fffffff";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
 
        private void timer1_Tick(object sender, EventArgs e)
        {

            foreach (Controller controller in controllers)
            {
                // push each element in the arrays one step back
                var keys = controller.dict_last.Keys.ToList();

                // don't consider the time key
                keys.Remove("time");

                foreach (string key in keys)
                {
                    if (controller.dict_temporal.ContainsKey(key) == false)
                    {
                        controller.dict_temporal.Add(key, new double[500]);
                    }
                    controller.dict_temporal[key][controller.dict_temporal[key].Length - 1] = Convert.ToDouble(controller.dict_last[key]);
                    Array.Copy(controller.dict_temporal[key], 1, controller.dict_temporal[key], 0, controller.dict_temporal[key].Length - 1);
                }

                // reference
                controller.r_array[controller.r_array.Length - 1] = controller.r;
                Array.Copy(controller.r_array, 1, controller.r_array, 0, controller.r_array.Length - 1);

            }

            if (listBox_controllers.Items.Count > 0)
            {
                update_chart(controller_current);
            }
        }

        public void addChartKey(string key)
        {
            dataChart.Series.Add(key);
            dataChart.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            dataChart.Series[key].BorderWidth = 2;
        }

        private void update_chart(Controller controller)
        {
            var keys = controller.dict_temporal.Keys.ToArray();

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                dataChart.Series[key].Points.Clear();
                for (int i = 0; i < controller.dict_temporal[key].Length; i++)
                {
                    dataChart.Series[key].Points.AddY(controller.dict_temporal[key][i]);
                    //log(Convert.ToString(controller.dict_temporal[key][i]));
                }
            }

            // clear and update the graphs (for the reference)
            dataChart.Series["reference"].Points.Clear();
            for (int i = 0; i < controller.r_array.Length; i++)
            {
                dataChart.Series["reference"].Points.AddY(controller.r_array[i]);
            }

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


        public class Controller
        {
            // time
            public DateTime last_recieved_time;

            // display variables
            public double r = 0; // reference value
            public double[] r_array = new double[500]; // reference value (array) 

            public Dictionary<string, string> dict_last = new Dictionary<string, string>(); // recieved values (last)
            public Dictionary<string, double[]> dict_temporal = new Dictionary<string, double[]>(); // recieved values (array)

            public PIDparameters PID;

            // main form access
            Form1 Main_form;

            public Controller(Form1 f1, PIDparameters parameters, string ip_recieve, string ip_send, int port_recieve, int port_send)
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
                    }
                    catch (Exception ex)
                    {
                        //log(Convert.ToString(ex));
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
                            //add_Chart_Key = key;
                            Main_form.addChartKey(key);
                        }

                    }
                    dict_last[key] = value;
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



        private void trackBarReference_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            controller_current.r = trackBarReference.Value;
            labelReference.Text = controller_current.r.ToString();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown_Kp_ValueChanged(object sender, EventArgs e)
        {
            controller_current.PID.Kp = Convert.ToDouble(numericUpDown_Kp.Value);
        }

        private void numericUpDown_Ki_ValueChanged(object sender, EventArgs e)
        {
            controller_current.PID.Ki = Convert.ToDouble(numericUpDown_Ki.Value);
        }

        private void numericUpDown_Kd_ValueChanged(object sender, EventArgs e)
        {
            controller_current.PID.Kd = Convert.ToDouble(numericUpDown_Kd.Value);
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            controller_current = controllers[listBox_controllers.SelectedIndex];
            trackBarReference.Value = Convert.ToInt16(controller_current.r);
            numericUpDown_Kp.Value = Convert.ToDecimal(controller_current.PID.Kp);
            numericUpDown_Ki.Value = Convert.ToDecimal(controller_current.PID.Ki);
            numericUpDown_Kd.Value = Convert.ToDecimal(controller_current.PID.Kd);
            labelReference.Text = controller_current.r.ToString();

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string ip_send = textBox_ip_send.Text;
            string ip_recieve = textBox_ip_recieve.Text;
            int port_recieve = Convert.ToInt32(textBox_port_recieve.Text);
            int port_send = Convert.ToInt32(textBox_port_send.Text);

            // create and add controller
            PIDparameters parameters = new PIDparameters(2, 100, 0.2);
            Controller PID_1 = new Controller(this, parameters, ip_recieve, ip_send, port_recieve, port_send);
            controllers.Add(PID_1);
            controller_current = PID_1;
            listBox_controllers.Items.Add("PID");
        }

        
        public void log(string text)
        {
            this.Invoke((MethodInvoker)delegate () { tbDebugLog.AppendText(text + Environment.NewLine); });
        }
        
    }
}
