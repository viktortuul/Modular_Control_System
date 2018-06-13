using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using Communication;
using PhysicalProcesses;

namespace Model_GUI
{
    public partial class Form1 : Form
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // chart settings
        public static int n_steps = 500;

        // measurement noise

        // data containers (dictionaries)
        public Dictionary<string, string> package_last = new Dictionary<string, string>(); // recieved package <tag, value>
        public Dictionary<string, double[]> packages = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        public Dictionary<string, double> states_last = new Dictionary<string, double>(); // recieved package <tag, value>
        public Dictionary<string, double[]> states = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        public Dictionary<string, double> disturbances_last = new Dictionary<string, double>(); // recieved package <tag, value>
        public Dictionary<string, double[]> disturbances = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        DisturbanceSettings DS = new DisturbanceSettings();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // string[] args = Environment.GetCommandLineArgs();
            string[] args = {"127.0.0.1", "127.0.0.1", "8400", "8300"};

            // parse the command line arguments
            string ip_controller_send = args[0];
            string ip_plant_recieve = args[1];
            int port_controller_send = Convert.ToInt16(args[2]);
            int port_plant_recieve = Convert.ToInt16(args[3]);

            // initialize model
            DoubleWatertank watertank = new DoubleWatertank();
            //QuadWatertank watertank = new QuadWatertank();

            // store thge model in a container which generically send and access values 
            Plant plant = new Plant(watertank);

            // create a thread for listening on the controller
            Thread thread_listener = new Thread(() => Listener(ip_plant_recieve, port_plant_recieve, plant));
            thread_listener.Start();

            // create a thread for communication with the controller
            Thread thread_sender = new Thread(() => Sender(ip_controller_send, port_controller_send, plant));
            thread_sender.Start();

            // create a thread for the simulation
            double dt = 0.01; // model update rate
            Thread thread_process = new Thread(() => process(plant, dt));
            thread_process.Start();

            timer1.Start();
        }

        private void buttonApplyDisturbance_Click(object sender, EventArgs e)
        {
            if (radioButtonConstant.Checked == true) DS.type = "constant";
            if (radioButtonTransient.Checked == true) DS.type = "transient";
            if (radioButtonSinusoid.Checked == true) DS.type = "sinusoid";

            DS.duration = Convert.ToDouble(numUpDownDuration.Value);
            DS.frequency = Convert.ToDouble(numUpDownFrequency.Value);
            DS.amplitude = new double[] { Convert.ToDouble(numUpDownAmplitude11.Value), Convert.ToDouble(numUpDownAmplitude21.Value), Convert.ToDouble(numUpDownAmplitude12.Value), Convert.ToDouble(numUpDownAmplitude22.Value) };
            DS.disturbance = new double[] { 0, 0, 0, 0 };

            DS.time_elapsed = 0;
        }


        public void process(Plant plant, double dt_)
        {
            int dt = Convert.ToInt16(1000 * dt_); // convert s to ms
            while (true)
            {
                Thread.Sleep(dt);

                if (DS.duration > 0)
                {
                    DS.DisturbanceNext(dt);              
                    plant.update_state(DS.disturbance);

                    this.Invoke((MethodInvoker)delegate () { labelDisturbance.Text = "Disturbances: \n" + Math.Round(DS.disturbance[0], 1) + "\n" + Math.Round(DS.disturbance[1], 1) + "\n" + Math.Round(DS.disturbance[2], 1) + "\n" + Math.Round(DS.disturbance[3], 1); });
                }
                else
                {
                    plant.update_state(new double[] {0,0,0,0} );
                }


                // observed states
                int counter = -1;
                for (int i = 0; i < plant.get_yo().Length; i++)
                {
                    counter++;
                    disturbances_last["yo" + (i + 1)] = plant.get_yo()[i];
                }

                // controlled states
                counter = -1;
                for (int i = 0; i < plant.get_yc().Length; i++)
                {
                    counter++;
                    disturbances_last["yc" + (i + 1)] = plant.get_yc()[i];
                }

                // disturbances
                counter = -1;
                for (int i = 0; i < DS.disturbance.Length; i++)
                {
                    counter++;
                    states_last["dist." + (i + 1)] = DS.disturbance[i];
                }

                // Console.WriteLine("y1: " + Math.Round(plant.get_y()[0], 2) + "  y2: " + Math.Round(plant.get_y()[1], 2));
            }
        }

        public class DisturbanceSettings
        {
            public string type;
            public double duration;
            public double time_elapsed;
            public double frequency;
            public double[] amplitude;
            public int[] tanks;
            public double[] disturbance = new double[] { 0, 0, 0, 0 };

            public void DisturbanceNext(int dt)
            {
                switch (type)
                {
                    case "constant":
                        disturbance = amplitude;
                        break;
                    case "transient":
                        disturbance[0] = amplitude[0] * Math.Exp(-time_elapsed);
                        disturbance[1] = amplitude[1] * Math.Exp(-time_elapsed);
                        disturbance[2] = amplitude[2] * Math.Exp(-time_elapsed);
                        disturbance[3] = amplitude[3] * Math.Exp(-time_elapsed);
                        break;
                    case "sinusoid":
                        disturbance[0] = amplitude[0] * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
                        disturbance[1] = amplitude[1] * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
                        disturbance[2] = amplitude[2] * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
                        disturbance[3] = amplitude[3] * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
                        break;
                }
                time_elapsed += Convert.ToDouble(dt) / 1000;
                duration -= Convert.ToDouble(dt) / 1000;             
            }
        }


        public void Listener(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Server server = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(5);
                try
                {
                    server.listen();
                    parse_message(server.last_recieved);

                    // parse dictionary to proper input
                    double[] u = new double[package_last.Count];
                    int i = -1;
                    foreach (string key in package_last.Keys)
                    {
                        i++;
                        u[i] = Convert.ToDouble(package_last[key]);
                        //Console.WriteLine(key + ":" + u[i]);
                    }
                    plant.set_u(u);
                }
                catch { }
            }
        }

        public void Sender(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Client client = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(10);

                // send measurements y      
                string message = "";

                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(FMT) + "#");

                // observed states
                int counter = -1;
                for (int i = 0; i < plant.get_yo().Length; i++)
                {
                    counter++;
                    message += "yo" + (i + 1) + "_" + (plant.get_yo()[i]).ToString() + "#";
                }

                // controlled states
                counter = -1;
                for (int i = 0; i < plant.get_yc().Length; i++)
                {
                    counter++;
                    message += "yc" + (i + 1) + "_" + plant.get_yc()[i].ToString() + "#";
                }

                message = message.Substring(0, message.LastIndexOf('#'));
                //Console.WriteLine("sent: " + message);
                client.send(message);
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

                if (package_last.ContainsKey(key) == false)
                {
                    package_last.Add(key, value);
                }
                package_last[key] = value;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDebug.Text = "time left: " + Math.Round(DS.duration, 1);

            PushArrays(states_last, states);
            PushArrays(disturbances_last, disturbances);
            UpdateChartStates();
            UpdateChartDisturbances();
        }

        public void PushArrays(Dictionary<string, double> dict_last, Dictionary<string, double[]> dict_all)
        {
            var keys = dict_last.Keys.ToList();
            keys.Remove("time"); // don't consider the time key

            foreach (string key in keys)
            {
                if (dict_all.ContainsKey(key) == false)
                {
                    dict_all.Add(key, new double[n_steps]);
                }

                Array.Copy(dict_all[key], 1, dict_all[key], 0, dict_all[key].Length - 1);
                dict_all[key][dict_all[key].Length - 1] = dict_last[key];
            }
        }

        private void UpdateChartStates()
        {
            // get all keys from the current connection
            var keys = states_last.Keys.ToArray();

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (dataChart.Series.IndexOf(key) == -1)
                {
                    AddChartSeries(key);
                }

                dataChart.Series[key].Points.Clear();
                for (int i = 0; i < states[key].Length; i++)
                {
                    dataChart.Series[key].Points.AddY(states[key][i]);
                }
            }
        }
        private void UpdateChartDisturbances()
        {
            // get all keys from the current connection
            var keys = disturbances_last.Keys.ToArray();

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (disturbanceChart.Series.IndexOf(key) == -1)
                {
                    AddChartSeries_d(key);
                }

                disturbanceChart.Series[key].Points.Clear();
                for (int i = 0; i < disturbances[key].Length; i++)
                {
                    disturbanceChart.Series[key].Points.AddY(disturbances[key][i]);
                }
            }
        }

        public void AddChartSeries(string key)
        {
            // add a new time series
            if (dataChart.Series.IndexOf(key) == -1)
            {
                dataChart.Series.Add(key);
                dataChart.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                dataChart.Series[key].BorderWidth = 2;

                switch (key)
                {
                    case "u1":
                        dataChart.Series[key].Color = Color.Orange;
                        dataChart.Series[key].BorderWidth = 1;
                        break;
                    case "u2":
                        dataChart.Series[key].Color = Color.Magenta;
                        dataChart.Series[key].BorderWidth = 1;
                        break;
                    case "yo1":
                        dataChart.Series[key].Color = Color.Black;
                        dataChart.Series[key].BorderWidth = 2;
                        break;
                    case "yo2":
                        dataChart.Series[key].Color = Color.Gray;
                        dataChart.Series[key].BorderWidth = 2;
                        break;
                    case "yc1":
                        dataChart.Series[key].Color = Color.Blue;
                        dataChart.Series[key].BorderWidth = 3;
                        break;
                    case "yc2":
                        dataChart.Series[key].Color = Color.Green;
                        dataChart.Series[key].BorderWidth = 3;
                        break;
                    default:
                        break;
                }
            }
        }

        public void AddChartSeries_d(string key)
        {
            // add a new time series
            if (disturbanceChart.Series.IndexOf(key) == -1)
            {
                disturbanceChart.Series.Add(key);
                disturbanceChart.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                disturbanceChart.Series[key].BorderWidth = 2;

                switch (key)
                {
                    case "u1":
                        disturbanceChart.Series[key].Color = Color.Orange;
                        disturbanceChart.Series[key].BorderWidth = 1;
                        break;
                    case "u2":
                        disturbanceChart.Series[key].Color = Color.Magenta;
                        disturbanceChart.Series[key].BorderWidth = 1;
                        break;
                    case "yo1":
                        disturbanceChart.Series[key].Color = Color.Black;
                        disturbanceChart.Series[key].BorderWidth = 2;
                        break;
                    case "yo2":
                        disturbanceChart.Series[key].Color = Color.Gray;
                        disturbanceChart.Series[key].BorderWidth = 2;
                        break;
                    case "yc1":
                        disturbanceChart.Series[key].Color = Color.Blue;
                        disturbanceChart.Series[key].BorderWidth = 3;
                        break;
                    case "yc2":
                        disturbanceChart.Series[key].Color = Color.Green;
                        disturbanceChart.Series[key].BorderWidth = 3;
                        break;
                    default:
                        break;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // kill all threads when the form closes
            Environment.Exit(0);
        }

        private void numUpDownAmplitude12_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numUpDownAmplitude22_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}

