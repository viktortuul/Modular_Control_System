using System;
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
using GlobalComponents;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace Model_GUI
{
    public partial class ModelGUI : Form
    {
        // logger (write to file)
        static bool FLAG_LOG = false;
        static StringBuilder sb = new StringBuilder();

        // chart settings
        public int time_chart_window = 60; // chart view time [s]

        // flag specific keys with pre-defined meanings
        static string[] DEF_control_signal = new string[] { "u1", "u2" };

        // data containers (dictionaries)
        public Dictionary<string, string> packet_last = new Dictionary<string, string>(); // recieved packet <tag, value>
        public Dictionary<string, double[]> packets = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        // data containers (dictionaries), only for plotting
        public Dictionary<string, DataContainer> states = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> perturbations = new Dictionary<string, DataContainer>();

        // channel variables
        public bool using_channel = false;
        ConnectionParameters EP_Controller = new ConnectionParameters();
        AddressEndPoint EP_Send_Controller = new AddressEndPoint();

        // initialize perturbation settings
        DisturbanceModel Disturbance = new DisturbanceModel();

        // initialize an empty Plant class
        Plant Plant = new Plant();

        // measurement noise standard deviation
        double meas_noise_std = 0.1;

        // model parameters from command line arguments
        public double[] model_parameters = new double[0];

        // folder setting for chart image save
        public string folderName = "";

        // GUI view mode
        public string GUI_view_mode = "control";

        public ModelGUI()
        {
            InitializeComponent();
        }

        private void ModelGUI_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            ParseArgs(args);

            // create a thread for listening on the physical plant
            Thread thread_listener = new Thread(() => Listener(EP_Send_Controller.IP, EP_Controller.PortThis, Plant));
            thread_listener.Start();

            // create a thread for communication with the controller
            Thread thread_sender = new Thread(() => Sender(EP_Send_Controller.IP, EP_Send_Controller.Port, Plant));
            thread_sender.Start();

            // create a thread for the simulation
            int dt = 10; // simulation update rate [ms]
            Thread thread_process = new Thread(() => Process(Plant, dt));
            thread_process.Start();

            // folder and chart settings
            InitialSettings();

            // start plotting
            timerChart.Start();

            // initialize with control view mode
            toggleViewMode("control");
        }

        public void Process(Plant Plant, int dt)
        {
            while (true)
            {
                Thread.Sleep(dt);

                // update process
                if (Disturbance.time_left > 0)
                {
                    ApplyDisturbance(Plant);
                }
                Plant.UpdateStates();
            }
        }

        private void ApplyDisturbance(Plant Plant)
        {
            Disturbance.PerturbationNext();
            Plant.ApplyDisturbance(Disturbance.target_state, Disturbance.value_disturbance);
            SampleStates(Plant);        
            if (Disturbance.type == "instant") Disturbance.Stop();
        }

        public void Listener(string IP, int port, Plant Plant)
        {
            // initialize a connection to the controller
            Server listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    listener.Listen();

                    // parse recieved message
                    ParseMessage(listener.getMessage());

                    // pre-allocate control signal vector (size = the number of received different control signals)
                    double[] u = new double[packet_last.Count];

                    int i = 0;
                    foreach (string key in packet_last.Keys)
                    {
                        u[i] = Convert.ToDouble(packet_last[key]);
                        i++;
                    }

                    // update actuators
                    Plant.set_u(u);
                    
                    // log received message (used for package delivery analysis)
                    Helpers.Log(sb, listener.getMessage(), FLAG_LOG);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
        }

        public void Sender(string IP, int port, Plant Plant)
        {
            // initialize a connection to the controller
            Client sender = new Client(IP, port);

            while (true)
            {
                Thread.Sleep(50);    

                string message = "";
                if (using_channel == true) message += Convert.ToString("EP_" + EP_Controller.IP + ":" + EP_Controller.Port + "#"); // add end-point if canal is used
                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT) + "#");

                // attach observed states measurement 
                for (int i = 0; i < Plant.get_yo().Length; i++)
                {
                    // apply measurement noise
                    var r = new GaussianRandom();
                    double noise = r.NextGaussian(0, meas_noise_std);
                    message += "yo" + (i + 1) + "_" + (Plant.get_yo()[i] + noise).ToString() + "#";
                }  
               
                // attach controlled states measurement
                for (int i = 0; i < Plant.get_yc().Length; i++)
                {
                    // apply measurement noise
                    var r = new GaussianRandom();
                    double noise = r.NextGaussian(0, meas_noise_std);
                    message += "yc" + (i + 1) + "_" + (Plant.get_yc()[i] + noise).ToString() + "#";


                    // append the last actuator state
                    message += "uc" + (i + 1) + "_" + Plant.get_uc()[i].ToString() + "#";
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

                if (DEF_control_signal.Contains(key))
                {
                    if (packet_last.ContainsKey(key) == false) packet_last.Add(key, value);
                    packet_last[key] = value;
                }     
            }
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            // sample states for plotting
            SampleStates(Plant);

            // draw chart
            UpdateChart(dataChart, states);
            UpdateChart(perturbationChart, perturbations);

            // update time axis minimum and maximum
            Charting.UpdateChartAxes(dataChart, time_chart_window);
            Charting.UpdateChartAxes(perturbationChart, time_chart_window);

            // draw animation
            Animation.DrawTanks(this);
 
            // update labels
            Helpers.UpdatePerturbationLabels(this, Disturbance);
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
                    DateTime time = DateTime.ParseExact(dict[key].time[i], Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    chart_.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].value[i]));
                }

                // remove old data points
                if (chart_.Series[key].Points.Count > Constants.n_steps) chart_.Series[key].Points.RemoveAt(0);
            }
        }

        public void SampleStates(Plant Plant) // used for plotting
        {
            // observed states
            for (int i = 0; i < Plant.get_yo().Length; i++)
            {
                Tools.AddKeyToDict(states, "yo" + (i + 1), Constants.n_steps);
                states["yo" + (i + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), Plant.get_yo()[i].ToString());
            }

            // controlled states
            for (int i = 0; i < Plant.get_yc().Length; i++)
            {
                Tools.AddKeyToDict(states, "yc" + (i + 1), Constants.n_steps);
                states["yc" + (i + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), Plant.get_yc()[i].ToString());
            }

            // control signal 
            int j = 0;
            var keys = packet_last.Keys.ToList();
            foreach (string key in keys)
            {
                Tools.AddKeyToDict(states, "u" + (j + 1), Constants.n_steps);
                states["u" + (j + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), packet_last[key]);
                j++;
            }

            // disturbances
            Tools.AddKeyToDict(perturbations, "dist", Constants.n_steps);
            perturbations["dist"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), Disturbance.value_disturbance.ToString());
            
        }

        // HELPERS BELOW ##########################################################################################################

        private void InitialSettings()
        {
            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripLabel.Text = "Dir: " + folderName;

            // folder and chart settings
            Charting.InitializeChartSettings(dataChart, title: "");
            Charting.InitializeChartSettings(perturbationChart, title: "");
        }

        public void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                List<string> arg_sep = Tools.ArgsParser(arg);
                if (arg_sep.Count() == 0) continue;
                string arg_name = arg_sep[0];

                switch (arg_name)
                {
                    case "model":
                        string arg_model_type = arg_sep[1];
                        switch (arg_model_type)
                        {
                            case "dwt":
                                model_parameters = new double[] { Convert.ToDouble(arg_sep[2]), Convert.ToDouble(arg_sep[3]), Convert.ToDouble(arg_sep[4]), Convert.ToDouble(arg_sep[5]) };
                                Plant = new Plant(new DoubleWatertank(model_parameters));
                                meas_noise_std = Convert.ToDouble(arg_sep[6]);
                                break;
                            case "qwt":
                                model_parameters = new double[] { Convert.ToDouble(arg_sep[2]), Convert.ToDouble(arg_sep[3]), Convert.ToDouble(arg_sep[4]), Convert.ToDouble(arg_sep[5]), Convert.ToDouble(arg_sep[6]), Convert.ToDouble(arg_sep[7]), Convert.ToDouble(arg_sep[8]), Convert.ToDouble(arg_sep[9]) };
                                Plant = new Plant(new QuadWatertank(model_parameters));
                                meas_noise_std = Convert.ToDouble(arg_sep[10]);
                                break;
                            case "ipsiso":
                                Plant = new Plant(new InvertedPendulumSISO());
                                break;
                            default:
                                model_parameters = new double[] { Convert.ToDouble(arg_sep[2]), Convert.ToDouble(arg_sep[3]), Convert.ToDouble(arg_sep[4]), Convert.ToDouble(arg_sep[5]) };
                                Plant = new Plant(new DoubleWatertank(model_parameters));
                                meas_noise_std = Convert.ToDouble(arg_sep[6]);
                                MessageBox.Show("Unknown model type: " + arg_model_type + ". Using default: double water tank");
                                break;
                        }
                        break;

                    case "channel_controller":
                        EP_Send_Controller = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                        using_channel = true;
                        break;
                
                    case "controller_ep":
                        EP_Controller = new ConnectionParameters(arg_sep[1], Convert.ToInt16(arg_sep[2]), Convert.ToInt16(arg_sep[3]));
                        break;
                       
                    case "log":
                        if (arg_sep[1] == "false") FLAG_LOG = false;
                        if (arg_sep[1] == "true") FLAG_LOG = true;
                        break;

                    case "ARG_INVALID":
                        break;

                    default:
                        MessageBox.Show("Unknown argument: " + arg_name);
                        break;
                }
            }
        }

        private void clbSeries_ItemCheck(object sender, ItemCheckEventArgs e)
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

        private void ModelGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            // kill all threads when the form closes
            Environment.Exit(0);
        }

        private void nudHistory_ValueChanged(object sender, EventArgs e)
        {
            time_chart_window = Convert.ToInt16(nudHistory.Value);
            
            if (time_chart_window > 0)
            {
                dataChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
                perturbationChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
            }
        }

        private void btnClearCharts_Click(object sender, EventArgs e)
        {
            foreach (var series in dataChart.Series) series.Points.Clear();
            foreach (var series in perturbationChart.Series) series.Points.Clear();
        }

        private void btnApplyDisturbance_Click(object sender, EventArgs e)
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

            Disturbance = new DisturbanceModel(target_state, type, duration, amplitude_disturbance, time_const, frequency);
        }

        private void btnStopDisturbance_Click(object sender, EventArgs e)
        {
            Disturbance.Stop();
        }

        private void timerUpdateGUI_Tick_1(object sender, EventArgs e)
        {
            // scale y-axis for the charts
            Charting.ChangeYScale(dataChart, treshold_interval: "one", grid_interval: "adaptive");
            Charting.ChangeYScale(perturbationChart, treshold_interval: "one", grid_interval: "adaptive");
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataChart.SaveImage(folderName + "\\chart_model_main.png", ChartImageFormat.Png);
            perturbationChart.SaveImage(folderName + "\\chart_model_disturbance.png", ChartImageFormat.Png);
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

        private void toggleViewMode(string mode)
        {
            if (mode == "control")
            {
                foreach (Control c in this.Controls)
                {
                    c.Visible = true; //or true.
                }

                tabControl1.Width = this.Width - 325 - pictureBox1.Width - 20;
                tabControl1.Height = this.Height - 70;
                pictureBox1.Height = this.Height - 70 - 23;
            }
            else if (mode == "observe")
            {
                string[] visible_controls = new string[] { "tabControl1", "dataChart", "pictureBox1", "statusStrip1" };
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
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            if (GUI_view_mode == "control")
            {
                toggleViewMode("observe");
                GUI_view_mode = "observe";
            }
            else
            {
                toggleViewMode("control");
                GUI_view_mode = "control";
            }
        }
    }
}

