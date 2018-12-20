﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using Communication;
using PhysicalProcesses;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.IO;

namespace Model_GUI
{
    public partial class ModelGUI : Form
    {
        // logger
        static StringBuilder sb = new StringBuilder();

        // chart settings
        public int chart_history = 60;

        // flag specific keys with pre-defined meanings
        static string[] flag_control_signal = new string[] { "u1", "u2" };

        // data containers (dictionaries)
        public Dictionary<string, string> package_last = new Dictionary<string, string>(); // recieved package <tag, value>
        public Dictionary<string, double[]> packages = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> states = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> perturbations = new Dictionary<string, DataContainer>();

        // initialize perturbation settings
        Perturbation Disturbance = new Perturbation();

        // initialize an empty plant class
        Plant plant = new Plant();

        // folder setting for chart image save
        public string folderName = "";

        // canal flag
        public bool using_canal = false;
        ConnectionParameters EP_Controller = new ConnectionParameters();
        AddressEndPoint EP = new AddressEndPoint();

        // model parameters from command line arguments
        public double[] model_parameters = new double[0];

        // control/obsrver mode
        public bool control_mode = true;

        public ModelGUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            ProcessArguments(args);

            // create a thread for listening on the controller
            Thread thread_listener = new Thread(() => Listener(EP.IP, EP_Controller.PortThis, plant));
            thread_listener.Start();

            // create a thread for communication with the controller
            Thread thread_sender = new Thread(() => Sender(EP.IP, EP.Port, plant));
            thread_sender.Start();

            // create a thread for the simulation
            int dt = 10; // simulation update rate [ms]
            Thread thread_process = new Thread(() => Process(plant, dt));
            thread_process.Start();

            // folder and chart settings
            InitialSettings();

            // start plotting
            timerChart.Start();

            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripLabel.Text = "Dir: " + folderName;

            toggleControlMode();
        }

        public void Process(Plant plant, int dt)
        {
            while (true)
            {
                Thread.Sleep(dt);

                // update process
                if (Disturbance.time_left > 0)
                {
                    ApplyDisturbance(plant);
                }
                else
                {
                    plant.UpdateStates();
                }            
            }
        }

        private void ApplyDisturbance(Plant plant)
        {
            Disturbance.PerturbationNext();
            plant.ChangeState(Disturbance.target_state, Disturbance.value_disturbance);
            SampleStates(plant);
            this.Invoke((MethodInvoker)delegate ()
            {
                UpdateChart(perturbationChart, perturbations);
            });
            if (Disturbance.type == "instant") Disturbance.Stop();
        }

        public void Listener(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Server listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    listener.Listen();

                    // parse recieved message
                    ParseMessage(listener.last_recieved);

                    // pre-allocate control signal vector (size = the number of received different control signals)
                    double[] u = new double[package_last.Count];

                    int i = 0;
                    foreach (string key in package_last.Keys)
                    {
                        u[i] = Convert.ToDouble(package_last[key]);
                        i++;
                    }

                    // update actuators
                    plant.set_u(u);

                    // logging
                    sb.Append(listener.last_recieved + "\n");
                    File.AppendAllText("log_received.txt", sb.ToString());
                    sb.Clear();
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
        }

        public void Sender(string IP, int port, Plant plant)
        {
            // initialize a connection to the controller
            Client sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(50);    

                string message = "";
                if (using_canal == true) message += Convert.ToString("EP_" + EP_Controller.IP + ":" + EP_Controller.Port + "#"); // add end-point if canal is used
                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT) + "#");

                // attach observed states measurement 
                for (int i = 0; i < plant.get_yo().Length; i++)
                {
                    // apply measurement noise
                    var r = new GaussianRandom();
                    double noise = r.NextGaussian(0, 0.1);
                    message += "yo" + (i + 1) + "_" + (plant.get_yo()[i] + noise).ToString() + "#";
                }  

                // attach controlled states measurement
                for (int i = 0; i < plant.get_yc().Length; i++)
                {
                    // apply measurement noise
                    var r = new GaussianRandom();
                    double noise = r.NextGaussian(0, 0.1);
                    message += "yc" + (i + 1) + "_" + (plant.get_yc()[i] + noise).ToString() + "#";

                    // append the last actuator state
                    message += "uc" + (i + 1) + "_" + plant.get_uc()[i].ToString() + "#";
                }

                // remove the redundant delimiter
                message = message.Substring(0, message.LastIndexOf('#')); 
                sender.Send(message);
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

                if (flag_control_signal.Contains(key))
                {
                    if (package_last.ContainsKey(key) == false) package_last.Add(key, value);
                    package_last[key] = value;
                }     
            }
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            // sample states for plotting
            SampleStates(plant);

            // draw chart
            UpdateChart(dataChart, states);
            UpdateChart(perturbationChart, perturbations);

            // update time axis minimum and maximum
            Charting.UpdateChartAxes(dataChart, chart_history);
            Charting.UpdateChartAxes(perturbationChart, chart_history);

            // draw animation
            Helpers.DrawTanks(this);

            // update labels
            Helpers.UpdatePerturbationLabels(this, Disturbance);
        }

        private void UpdatePerturbation(Perturbation perturbation)
        {
            if (perturbation.time_left > 0) perturbation.PerturbationNext();
        }

        private void UpdateChart(object chart, Dictionary<string, DataContainer> dict)
        {
            Chart chart_ = (Chart)chart;

            // get all keys from the current connection
            var keys = dict.Keys.ToArray();

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (chart_.Series.IndexOf(key) == -1) Charting.AddChartSeries(this, key, chart_);

                int i = dict[key].time.Length - 1;
                if (dict[key].time[i] != null)
                {
                    Console.WriteLine(dict[key].value[i]);
                    DateTime time = DateTime.ParseExact(dict[key].time[i], Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    chart_.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].value[i]));
                }

                // remove old data points
                if (chart_.Series[key].Points.Count > Constants.n_steps) chart_.Series[key].Points.RemoveAt(0);
            }
        }

        public void SampleStates(Plant plant)
        {
            // observed states
            for (int i = 0; i < plant.get_yo().Length; i++)
            {
                Helpers.CheckKey(states, "yo" + (i + 1), Constants.n_steps);
                states["yo" + (i + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), plant.get_yo()[i].ToString());
            }

            // controlled states
            for (int i = 0; i < plant.get_yc().Length; i++)
            {
                Helpers.CheckKey(states, "yc" + (i + 1), Constants.n_steps);
                states["yc" + (i + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), plant.get_yc()[i].ToString());
            }

            // control signal 
            int j = 0;
            var keys = package_last.Keys.ToList();
            foreach (string key in keys)
            {
                Helpers.CheckKey(states, "u" + (j + 1), Constants.n_steps);
                states["u" + (j + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), package_last[key]);
                j++;
            }

            // disturbances
            Helpers.CheckKey(perturbations, "dist", Constants.n_steps);
            perturbations["dist"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), Disturbance.value_disturbance.ToString());
            
        }

        // HELPERS BELOW ##########################################################################################################

        public void ProcessArguments(string[] args)
        {
            // controller end-point (controller IP, controller Port, listening Port on this module)
            EP_Controller = new ConnectionParameters(args[1], Convert.ToInt16(args[2]), Convert.ToInt16(args[3]));

            // model type (e.g. dwt)
            string model_type = args[4];

            if (args.Length == 5 || args.Length == 5 + 4)
            {
                // 5 or 9 arguments --> direct communication
                EP = new AddressEndPoint(EP_Controller.IP, EP_Controller.Port);
            }
            else if (args.Length == 7 || args.Length == 7 + 4)
            {
                // 7 or 11 arguments --> canal is used
                EP = new AddressEndPoint(args[5], Convert.ToInt16(args[6]));
                using_canal = true;
            }

            // store the model in a container which generically send and access values 
            switch (model_type)
            {
                case "dwt":
                    model_parameters = new double[] { 15, 0.3, 50, 0.2 };
                    if (args.Length == 9) model_parameters = new double[] { Convert.ToDouble(args[5]), Convert.ToDouble(args[6]), Convert.ToDouble(args[7]), Convert.ToDouble(args[8]) };
                    if (args.Length == 11) model_parameters = new double[] { Convert.ToDouble(args[7]), Convert.ToDouble(args[8]), Convert.ToDouble(args[9]), Convert.ToDouble(args[10]) };
                    plant = new Plant(new DoubleWatertank(model_parameters));
                    break;
                case "qwt":
                    model_parameters = new double[] { 15, 0.3, 50, 0.2, 15, 0.3, 50, 0.2 };
                    if (args.Length == 15) model_parameters = new double[] { Convert.ToDouble(args[7]), Convert.ToDouble(args[8]), Convert.ToDouble(args[9]), Convert.ToDouble(args[10]), Convert.ToDouble(args[11]), Convert.ToDouble(args[12]), Convert.ToDouble(args[13]), Convert.ToDouble(args[14]) };
                    if (args.Length == 15) model_parameters = new double[] { Convert.ToDouble(args[7]), Convert.ToDouble(args[8]), Convert.ToDouble(args[9]), Convert.ToDouble(args[10]), Convert.ToDouble(args[11]), Convert.ToDouble(args[12]), Convert.ToDouble(args[13]), Convert.ToDouble(args[14]) };
                    plant = new Plant(new QuadWatertank(model_parameters));
                    break;
                case "ipsiso": plant = new Plant(new InvertedPendulumSISO()); break;
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // list all checked items
            List<string> checkedItems = new List<string>();
            foreach (var item in clbSeries.CheckedItems) checkedItems.Add(item.ToString());

            // also consider the new state of the checked item
            if (e.NewValue == CheckState.Checked)
                checkedItems.Add(clbSeries.Items[e.Index].ToString());
            else
                checkedItems.Remove(clbSeries.Items[e.Index].ToString());

            // enable or disable series
            foreach (var series in dataChart.Series)
            {
                if (checkedItems.Contains(series.Name)) dataChart.Series[series.Name].Enabled = true;
                else dataChart.Series[series.Name].Enabled = false;
            }
        }

        private void InitialSettings()
        {
            // chart settings
            //dataChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            dataChart.ChartAreas["ChartArea1"].AxisY.Title = "";
            dataChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            dataChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            dataChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            dataChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);

            //perturbationChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            perturbationChart.ChartAreas["ChartArea1"].AxisY.Title = "";
            perturbationChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            perturbationChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            perturbationChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            perturbationChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // kill all threads when the form closes
            Environment.Exit(0);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            chart_history = Convert.ToInt16(numericUpDown1.Value);
            
            if (chart_history > 0)
            {
                dataChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(chart_history / 10);
                perturbationChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(chart_history / 10);
            }
        }

        private void buttonApplyDisturbance_Click(object sender, EventArgs e)
        {
            string type = "";
            if (rbConstant.Checked == true) type = "constant";
            if (rbTransientDecrease.Checked == true) type = "transient";
            if (rbSinusoid.Checked == true) type = "sinusoid";
            if (rbInstant.Checked == true) type = "instant";

            double duration = Convert.ToDouble(nudDuration.Value);
            double frequency = Convert.ToDouble(nudFrequency.Value);
            double amplitude_disturbance = Convert.ToDouble(nudAmplitude.Value);
            double time_const = Convert.ToDouble(nudTimeConst.Value);
            string target_state = tbTargetState.Text;

            Disturbance = new Perturbation(target_state, type, duration, amplitude_disturbance, time_const, frequency);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var series in dataChart.Series) series.Points.Clear();
            foreach (var series in perturbationChart.Series) series.Points.Clear();
        }

        private void timerUpdateGUI_Tick_1(object sender, EventArgs e)
        {
            // scale y-axis for the charts
            Charting.ChangeYScale(dataChart, "");
            Charting.ChangeYScale(perturbationChart, "");
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataChart.SaveImage(folderName + "\\chart_model_main.png", ChartImageFormat.Png);
            perturbationChart.SaveImage(folderName + "\\chart_model_disturbance.png", ChartImageFormat.Png);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Disturbance.Stop();
        }

        private void rBtnDisturbanceConstant_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void rBtnDisturbanceSinusoid_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void rBtnDisturbanceTransient_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void rBtnDisturbanceInstant_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void setDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderName = folderBrowserDialog1.SelectedPath;
                toolStripLabel.Text = "Dir: " + folderName;
            }
        }

        private void toggleControlMode()
        {
            foreach (Control c in this.Controls)
            {
                c.Visible = true; //or true.
            }

            tabControl1.Width = this.Width - 325 - pictureBox1.Width - 20;
            tabControl1.Height = this.Height - 70;
            pictureBox1.Height = this.Height - 70 - 23;
        }

        private void toggleObserverMode()
        {
            string[] visible_controls = new string[] {"tabControl1", "dataChart", "pictureBox1", "statusStrip1" };
            foreach (Control c in this.Controls)
            {
                if (visible_controls.Contains(c.Name) == false)
                {
                    c.Visible = false; //or true.
                }
            }

            tabControl1.Width = this.Width - pictureBox1.Width - 25;
            tabControl1.Height = this.Height - 70;
            pictureBox1.Height = this.Height - 70 - 23;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            if (control_mode == true)
            {
                toggleObserverMode();
                control_mode = false;
            }
            else
            {
                toggleControlMode();
                control_mode = true;
            }
        }
    }
}

