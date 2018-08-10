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

namespace Model_GUI
{
    public partial class ModelGUI : Form
    {
        string CORRUPT_STRING = "";

        // chart settings
        public int chart_history = 60;

        // data containers (dictionaries)
        public Dictionary<string, string> package_last = new Dictionary<string, string>(); // recieved package <tag, value>
        public Dictionary<string, double[]> packages = new Dictionary<string, double[]>(); // recieved values (continously added according to timer)

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> states = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> perturbations = new Dictionary<string, DataContainer>();

        // initialize perturbation settings
        Perturbation Disturbance = new Perturbation();
        Perturbation Noise = new Perturbation();
        Perturbation Control = new Perturbation();

        // initialize an empty plant class
        Plant plant = new Plant();

        // folder setting for chart image save
        public string folderName = "";

        // canal flag
        public bool using_canal = false;
        ConnectionParameters EP_Controller = new ConnectionParameters();
        AddressEndPoint EP = new AddressEndPoint();

        public ModelGUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();

            EP_Controller = new ConnectionParameters(args[1], Convert.ToInt16(args[2]), Convert.ToInt16(args[3]));
            string model_type = args[4];

            if (args.Length == 7)
            {
                EP = new AddressEndPoint(args[5], Convert.ToInt16(args[6]));
                using_canal = true;
            }
            else if (args.Length == 5)
            {
                EP = new AddressEndPoint(EP_Controller.IP, EP_Controller.Port);
            }


            // store the model in a container which generically send and access values 
            switch (model_type)
            {
                case "dwt": plant = new Plant(new DoubleWatertank()); break;
                case "qwt": plant = new Plant(new QuadWatertank()); break;
                case "ipsiso": plant = new Plant(new InvertedPendulumSISO()); break;
            }

            // create a thread for listening on the controller
            Thread thread_listener = new Thread(() => Listener(EP.IP, EP_Controller.PortThis, plant));
            thread_listener.Start();

            // create a thread for communication with the controller
            Thread thread_sender = new Thread(() => Sender(EP.IP, EP.Port, plant));
            thread_sender.Start();

            // create a thread for the simulation
            double dt = 0.01; // simulation update rate
            Thread thread_process = new Thread(() => Process(plant, dt));
            thread_process.Start();

            // folder and chart settings
            InitialSettings();

            // start plotting
            timerChart.Start();

            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripStatusLabel1.Text = "Dir: " + folderName;
        }

        public void Process(Plant plant, double dt_)
        {
            int dt = Convert.ToInt16(1000 * dt_); // convert s to ms

            while (true)
            {
                Thread.Sleep(dt);
                // update and apply disturbance/noise/control perturbations
                ApplyDisturbance(plant);
                UpdatePerturbation(Noise);
                UpdatePerturbation(Control);
            }
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
                    ParseMessage(listener.last_recieved);

                    // parse dictionary to proper input
                    double[] u = new double[package_last.Count];

                    int i = 0;
                    foreach (string key in package_last.Keys)
                    {
                        u[i] = Convert.ToDouble(package_last[key]) ;
                        u[i] += Control.value[i + 1]; // APPLY CONTROL PERTURBATION ########################################################
                        i++;
                    }
                    plant.set_u(u);
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

                // send measurements y      
                string message = "";
                if (using_canal == true) message += Convert.ToString("EP_" + EP_Controller.IP + ":" + EP_Controller.Port + "#");
                message += Convert.ToString("time_" + DateTime.UtcNow.ToString(Constants.FMT) + "#");

                // observed states
                //for (int i = 0; i < plant.get_yo().Length; i++)
                //    message += "yo" + (i + 1) + "_" + plant.get_yo()[i].ToString() + "#";      

                // controlled states
                for (int i = 0; i < plant.get_yc().Length; i++)
                {
                    // apply measurement noise
                    var r = new GaussianRandom();
                    double n = r.NextGaussian(0, 0.1);

                    message += "yc" + (i + 1) + "_" + (plant.get_yc()[i] + n + Noise.value[i + 1] + CORRUPT_STRING).ToString() + "#"; // WITH MEASUREMENT NOISE
                }
                    

                message = message.Substring(0, message.LastIndexOf('#')); // remove the redundant delimiter
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

                if (package_last.ContainsKey(key) == false) package_last.Add(key, value);
                
                package_last[key] = value;
            }
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            labelDebug.Text = "time left: " + Math.Round(Disturbance.time_left, 1);

            // sample states
            SampleStates(plant);

            // draw chart
            UpdateChart(dataChart, states);
            UpdateChart(perturbationChart, perturbations);

            // update time axis minimum and maximum
            Helpers.UpdateChartAxes(dataChart, chart_history);
            Helpers.UpdateChartAxes(perturbationChart, chart_history);

            // draw animation
            Helpers.DrawTanks(this);

            // update labels
            Helpers.UpdatePerturbationLabels(this, Disturbance, Noise, Control);
        }

        private void ApplyDisturbance(Plant plant)
        {
            if (Disturbance.time_left > 0)
            {
                Disturbance.PerturbationNext();

                if (checkBoxInstant.Checked == true)
                {
                    plant.change_state(Disturbance.value);
                    Disturbance.Stop();
                }
                else plant.update_state(Disturbance.value);
            }
            else plant.update_state(new double[] { 0, 0, 0, 0 });

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
                if (chart_.Series.IndexOf(key) == -1) Helpers.AddChartSeries(key, chart_);

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

            // disturbances
            for (int i = 0; i < Disturbance.value.Length; i++)
            {
                Helpers.CheckKey(perturbations, "dist." + (i + 1), Constants.n_steps);
                perturbations["dist." + (i + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), Disturbance.value[i].ToString());
            }

            // noise
            for (int i = 0; i < Noise.value.Length; i++)
            {
                Helpers.CheckKey(perturbations, "noise" + (i + 1), Constants.n_steps);
                perturbations["noise" + (i + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), Noise.value[i].ToString());
            }

            // control
            for (int i = 0; i < Control.value.Length; i++)
            {
                Helpers.CheckKey(perturbations, "control" + (i + 1), Constants.n_steps);
                perturbations["control" + (i + 1)].InsertData(DateTime.UtcNow.ToString(Constants.FMT), Control.value[i].ToString());
            }
        }

        public class Perturbation
        {
            public string type;
            public double time_left;
            public double time_elapsed;
            public double frequency;
            public double time_const;
            public double[] amplitude;
            public int[] tanks;
            public double[] value = new double[] { 0, 0, 0, 0 };
            DateTime time_stamp = DateTime.Now;

            public void PerturbationNext()
            {
                switch (type)
                {
                    case "constant":
                        value = amplitude;
                        break;
                    case "transient":
                        for (int i = 0; i < value.Length; i++) value[i] = amplitude[i] * Math.Exp(-time_elapsed / time_const);
                        break;
                    case "sinusoid":
                        for (int i = 0; i < value.Length; i++) value[i] = amplitude[i] * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
                        break;
                }

                double elapsed_time = (DateTime.Now - time_stamp).TotalMilliseconds;
                time_elapsed += Convert.ToDouble(elapsed_time) / 1000;
                time_left -= Convert.ToDouble(elapsed_time) / 1000;
                time_stamp = DateTime.Now;

                if (time_left <= 0) Stop();
            }

            public void Stop()
            {
                time_elapsed = 0;
                time_left = 0;
                value = new double[] { 0, 0, 0, 0 };
            }
        }

        private void InitialSettings()
        {
            // chart settings
            dataChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            dataChart.ChartAreas["ChartArea1"].AxisY.Title = "";
            dataChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            dataChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            dataChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            dataChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);

            perturbationChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxInstant.Checked == true)
            {
                numUpDownDisturbanceDuration.Enabled = false;
                numUpDownDisturbanceFrequency.Enabled = false;
                numUpDownDisturbanceTimeConst.Enabled = false;
                label3.Text = "Amplitude [cm]";
            }
            else
            {
                numUpDownDisturbanceDuration.Enabled = true;
                numUpDownDisturbanceFrequency.Enabled = true;
                numUpDownDisturbanceTimeConst.Enabled = true;
                label3.Text = "Amplitude [cm3/s]";
            }
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
            if (rBtnDisturbanceConstant.Checked == true) Disturbance.type = "constant";
            if (rBtnDisturbanceTransient.Checked == true) Disturbance.type = "transient";
            if (rBtnDisturbanceSinusoid.Checked == true) Disturbance.type = "sinusoid";

            Disturbance.time_left = Convert.ToDouble(numUpDownDisturbanceDuration.Value);
            Disturbance.frequency = Convert.ToDouble(numUpDownDisturbanceFrequency.Value);
            Disturbance.amplitude = new double[] { Convert.ToDouble(numUpDownDisturbanceAmplitude11.Value), Convert.ToDouble(numUpDownDisturbanceAmplitude21.Value), Convert.ToDouble(numUpDownDisturbanceAmplitude12.Value), Convert.ToDouble(numUpDownDisturbanceAmplitude22.Value) };
            Disturbance.time_const = Convert.ToDouble(numUpDownDisturbanceTimeConst.Value);
            Disturbance.value = new double[] { 0, 0, 0, 0 };

            Disturbance.time_elapsed = 0;
        }

        private void buttonApplyNoise_Click(object sender, EventArgs e)
        {
            if (rBtnNoiseConstant.Checked == true) Noise.type = "constant";
            if (rBtnNoiseTransient.Checked == true) Noise.type = "transient";
            if (rBtnNoiseSinusoid.Checked == true) Noise.type = "sinusoid";

            Noise.time_left = Convert.ToDouble(numUpDownNoiseDuration.Value);
            Noise.frequency = Convert.ToDouble(numUpDownNoiseFrequency.Value);
            Noise.amplitude = new double[] { Convert.ToDouble(numUpDownNoiseAmplitude11.Value), Convert.ToDouble(numUpDownNoiseAmplitude21.Value), Convert.ToDouble(numUpDownNoiseAmplitude12.Value), Convert.ToDouble(numUpDownNoiseAmplitude22.Value) };
            Noise.time_const = Convert.ToDouble(numUpDownNoiseTimeConst.Value);
            Noise.value = new double[] { 0, 0, 0, 0 };

            Noise.time_elapsed = 0;
        }
        private void buttonApplyControl_Click(object sender, EventArgs e)
        {
            if (rBtnControlConstant.Checked == true) Control.type = "constant";
            if (rBtnControlTransient.Checked == true) Control.type = "transient";
            if (rBtnControlSinusoid.Checked == true) Control.type = "sinusoid";

            Control.time_left = Convert.ToDouble(numUpDownControlDuration.Value);
            Control.frequency = Convert.ToDouble(numUpDownControlFrequency.Value);
            Control.amplitude = new double[] { Convert.ToDouble(numUpDownControlAmplitude11.Value), Convert.ToDouble(numUpDownControlAmplitude21.Value), Convert.ToDouble(numUpDownControlAmplitude12.Value), Convert.ToDouble(numUpDownControlAmplitude22.Value) };
            Control.time_const = Convert.ToDouble(numUpDownControlTimeConst.Value);
            Control.value = new double[] { 0, 0, 0, 0 };

            Control.time_elapsed = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var series in dataChart.Series) series.Points.Clear();
            foreach (var series in perturbationChart.Series) series.Points.Clear();
        }

        private void timerUpdateGUI_Tick_1(object sender, EventArgs e)
        {
            // scale y-axis for the charts
            Helpers.ChangeYScale(dataChart, "");
            Helpers.ChangeYScale(perturbationChart, "");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CORRUPT_STRING = textBoxAppendCorrupt.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CORRUPT_STRING = "";
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataChart.SaveImage(folderName + "\\chart_model_main.png", ChartImageFormat.Png);
            perturbationChart.SaveImage(folderName + "\\chart_model_disturbance.png", ChartImageFormat.Png);
        }

        private void ModelGUI_Resize(object sender, EventArgs e)
        {
            //int y_start = dataChart.Location.Y;
            //int height_total = groupBox5.Height - y_start;
            //dataChart.Height = height_total / 2 - y_start;
            //perturbationChart.Location = new Point(3, + y_start + height_total / 2);
            //perturbationChart.Height = height_total / 2 - y_start;
        }
    }
}

