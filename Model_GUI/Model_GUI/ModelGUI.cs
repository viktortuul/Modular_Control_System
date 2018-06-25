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
using System.Windows.Forms.DataVisualization.Charting;
using Communication;
using PhysicalProcesses;
using System.Globalization;
using System.Diagnostics;

namespace Model_GUI
{
    public partial class ModelGUI : Form
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // chart settings
        public static int n_steps = 100;
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

        // drawing
        Graphics g;
        Pen pen_b = new Pen(Color.Black, 4);
        Pen pen_r = new Pen(Color.Red, 3);
        SolidBrush brush_b = new SolidBrush(Color.LightBlue);
        Bitmap bm = new Bitmap(400, 800);

        public ModelGUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // string[] args = Environment.GetCommandLineArgs();
            string[] args = { "127.0.0.1", "127.0.0.1", "8400", "8300" };

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
            double dt = 0.01; // simulation update rate
            Thread thread_process = new Thread(() => process(plant, dt));
            thread_process.Start();

            // folder and chart settings
            InitialSettings();

            // start plotting
            timerChart.Start();
        }

        public void process(Plant plant, double dt_)
        {
            int dt = Convert.ToInt16(1000 * dt_); // convert s to ms
            DateTime nowTime = DateTime.Now;

            while (true)
            {
                Thread.Sleep(dt);

                // update and apply disturbance/noise/control perturbations
                ApplyDisturbance(plant);
                UpdatePerturbation(Noise);
                UpdatePerturbation(Control);


                // sample the current (true) states
                if ((DateTime.Now - nowTime).TotalMilliseconds >= 100)
                {
                    nowTime = DateTime.Now;
                    SampleStates(plant);
                }
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
                    int i = 0;
                    foreach (string key in package_last.Keys)
                    {
                        u[i] = Convert.ToDouble(package_last[key]) ;
                        u[i] += Control.value[i + 1]; // APPLY CONTROL PERTURBATION

                        //Console.WriteLine(key + ":" + u[i]);
                        //Debug.WriteLine(Control.value[i]);
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
                    message += "yo" + (i + 1) + "_" + plant.get_yo()[i].ToString() + "#";
                }

                // controlled states
                counter = -1;
                for (int i = 0; i < plant.get_yc().Length; i++)
                {
                    counter++;
                    message += "yc" + (i + 1) + "_" + (plant.get_yc()[i] + Noise.value[i + 1]).ToString() + "#"; // WITH MEASUREMENT NOISE
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
            labelDebug.Text = "time left: " + Math.Round(Disturbance.time_left, 1);

            dataChart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
            dataChart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();
            perturbationChart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
            perturbationChart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();

            // draw chart
            UpdateChart(dataChart, states);
            UpdateChart(perturbationChart, perturbations);
            //UpdateChartDisturbances();

            // draw animation
            if (package_last.ContainsKey("u1")) DrawTanks();

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
                else
                {
                    plant.update_state(Disturbance.value);
                }
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
                if (chart_.Series.IndexOf(key) == -1)
                {
                    Helpers.AddChartSeries(key, chart_);
                }

                int i = dict[key].time.Length - 1;
                if (dict[key].time[i] != null)
                {
                    DateTime time = DateTime.ParseExact(dict[key].time[i], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    chart_.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].value[i]));
                }

                // remove old data points
                if (chart_.Series[key].Points.Count > 1000 * 5) chart_.Series[key].Points.RemoveAt(0);
            }
        }

        //private void UpdateChartDisturbances()
        //{
        //    // get all keys from the current connection
        //    var keys = perturbations.Keys.ToArray();

        //    // clear and update the graphs (for each key)
        //    foreach (string key in keys)
        //    {
        //        // if series does not exist, add it
        //        if (perturbationChart.Series.IndexOf(key) == -1)
        //        {
        //            Helpers.AddChartSeries(key, perturbationChart);
        //        }

        //        int i = perturbations[key].time.Length - 1;
        //        if (perturbations[key].time[i] != null)
        //        {
        //            DateTime time = DateTime.ParseExact(perturbations[key].time[i], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
        //            perturbationChart.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(perturbations[key].value[i]));
        //        }

        //        // remove old data points
        //        if (perturbationChart.Series[key].Points.Count > 1000 * 5) perturbationChart.Series[key].Points.RemoveAt(0);
        //    }
        //}

        public void SampleStates(Plant plant)
        {
            // observed states
            for (int i = 0; i < plant.get_yo().Length; i++)
            {
                Helpers.CheckKey(states, "yo" + (i + 1), n_steps);
                states["yo" + (i + 1)].InsertData(DateTime.UtcNow.ToString(FMT), plant.get_yo()[i].ToString());
            }

            // controlled states
            for (int i = 0; i < plant.get_yc().Length; i++)
            {
                Helpers.CheckKey(states, "yc" + (i + 1), n_steps);
                states["yc" + (i + 1)].InsertData(DateTime.UtcNow.ToString(FMT), plant.get_yc()[i].ToString());
            }

            // disturbances
            for (int i = 0; i < Disturbance.value.Length; i++)
            {
                Helpers.CheckKey(perturbations, "dist." + (i + 1), n_steps);
                perturbations["dist." + (i + 1)].InsertData(DateTime.UtcNow.ToString(FMT), Disturbance.value[i].ToString());
            }

            // noise
            for (int i = 0; i < Noise.value.Length; i++)
            {
                Helpers.CheckKey(perturbations, "noise" + (i + 1), n_steps);
                perturbations["noise" + (i + 1)].InsertData(DateTime.UtcNow.ToString(FMT), Noise.value[i].ToString());
            }

            // control
            for (int i = 0; i < Control.value.Length; i++)
            {
                Helpers.CheckKey(perturbations, "control" + (i + 1), n_steps);
                perturbations["control" + (i + 1)].InsertData(DateTime.UtcNow.ToString(FMT), Control.value[i].ToString());
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

        public class DataContainer
        {
            // time format
            const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

            // store the time:value pair in string arrays
            public string[] time;
            public string[] value;

            // constructor
            public DataContainer(int size)
            {
                time = new string[size];
                value = new string[size];
            }

            // instert a new time:value pair data point
            public void InsertData(string time_, string value_)
            {
                Array.Copy(time, 1, time, 0, time.Length - 1);
                time[time.Length - 1] = time_;

                Array.Copy(value, 1, value, 0, value.Length - 1);
                value[value.Length - 1] = value_;
            }

            public string GetLastTime()
            {
                return time[time.Length - 1];
            }
            public string GetLastValue()
            {
                return value[value.Length - 1];
            }
        }

        public void DrawTanks()
        {
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);

            // map tank height in cm to pixels
            double max_height_p = pictureBox1.Height / 2.5; // max height [pixels]
            double max_inflow_width = 10;
            double max_height_r = 25; // real max height [cm]
            double cm2pix = max_height_p / max_height_r;

            // extract signals
            int u = Convert.ToInt16(Convert.ToDouble(package_last["u1"]));
            int y1 = Convert.ToInt16(cm2pix * Convert.ToDouble(states["yo1"].GetLastValue()));
            int y2 = Convert.ToInt16(cm2pix * Convert.ToDouble(states["yc1"].GetLastValue()));

            //
            double A1 = 15; //Convert.ToDouble(numUpDown_A1.Value);
            double a1 = 0.16*2; //Convert.ToDouble(numUpDown_a1a.Value);
            double A2 = 100; //Convert.ToDouble(numUpDown_A2.Value);
            double a2 = 0.16; //Convert.ToDouble(numUpDown_a2a.Value);

            // TANK 1 ----------------------------------------------------------------
            Point T = new Point(75, Convert.ToInt16(pictureBox1.Height / 2));

            // tank dimensions
            double R1_ = Math.Sqrt(A1 / Math.PI); int R1 = Convert.ToInt16(R1_ * cm2pix);
            double r1_ = Math.Sqrt(a1 / Math.PI); int r1 = Convert.ToInt16(r1_ * cm2pix);
            double h1_ = 25; int h1 = Convert.ToInt16(h1_ * cm2pix);

            // inlet
            if (u > 0)
            {
                Rectangle water_in = new Rectangle(T.X - R1 + 5, T.Y - h1 - 50, Convert.ToInt16(max_inflow_width * (u / 7.5)), h1 + 50);
                g.FillRectangle(brush_b, water_in);
            }

            // water 
            Rectangle water1 = new Rectangle(T.X - R1, T.Y - y1, 2 * R1, y1);
            g.FillRectangle(brush_b, water1);

            if (y1 > 0)
            {
                Rectangle water_fall = new Rectangle(T.X - r1, T.Y, 2 * r1, 250);
                g.FillRectangle(brush_b, water_fall);
            }

            // walls
            Point w1_top = new Point(T.X - R1, T.Y - h1); Point w1_bot = new Point(T.X - R1, T.Y);
            Point w2_top = new Point(T.X + R1, T.Y - h1); Point w2_bot = new Point(T.X + R1, T.Y);
            Point wb1_l = new Point(T.X - R1, T.Y); Point wb1_r = new Point(T.X - r1, T.Y);
            Point wb2_l = new Point(T.X + r1, T.Y); Point wb2_r = new Point(T.X + R1, T.Y);
            g.DrawLine(pen_b, w1_top, w1_bot);
            g.DrawLine(pen_b, w2_top, w2_bot);
            g.DrawLine(pen_b, wb1_l, wb1_r);
            g.DrawLine(pen_b, wb2_l, wb2_r);

            // TANK 2 ----------------------------------------------------------------
            T = new Point(T.X, Convert.ToInt16(pictureBox1.Height - 20));

            // tank dimensions
            double R2_ = Math.Sqrt(A2 / Math.PI); int R2 = Convert.ToInt16(R2_ * cm2pix);
            double r2_ = Math.Sqrt(a2 / Math.PI); int r2 = Convert.ToInt16(r2_ * cm2pix);
            double h2_ = 25; int h2 = Convert.ToInt16(h2_ * cm2pix);

            // water 
            Rectangle water2 = new Rectangle(T.X - R2, T.Y - y2, 2 * R2, y2);
            g.FillRectangle(brush_b, water2);

            if (y2 > 0)
            {
                Rectangle water_fall = new Rectangle(T.X - r2, T.Y, 2 * r2, 200);
                g.FillRectangle(brush_b, water_fall);
            }

            // walls
            w1_top = new Point(T.X - R2, T.Y - h2); w1_bot = new Point(T.X - R2, T.Y);
            w2_top = new Point(T.X + R2, T.Y - h2); w2_bot = new Point(T.X + R2, T.Y);
            wb1_l = new Point(T.X - R2, T.Y); wb1_r = new Point(T.X - r2, T.Y);
            wb2_l = new Point(T.X + r2, T.Y); wb2_r = new Point(T.X + R2, T.Y);
            g.DrawLine(pen_b, w1_top, w1_bot);
            g.DrawLine(pen_b, w2_top, w2_bot);
            g.DrawLine(pen_b, wb1_l, wb1_r);
            g.DrawLine(pen_b, wb2_l, wb2_r);

            pictureBox1.Image = bm;
        }



        

        private void InitialSettings()
        {
            // chart settings
            dataChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            dataChart.ChartAreas["ChartArea1"].AxisY.Title = "Magnitude";
            dataChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            dataChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            dataChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            dataChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);

            perturbationChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            perturbationChart.ChartAreas["ChartArea1"].AxisY.Title = "Magnitude";
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

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            // scale y-axis for the charts
            Helpers.changeYScala(dataChart, "");
            Helpers.changeYScala(perturbationChart, "");
        }


    }
}

