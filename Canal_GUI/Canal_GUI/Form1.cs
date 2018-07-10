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
using System.Reflection;
using Communication;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using System.IO;

namespace Canal_GUI
{
    public partial class Form1 : Form
    {
        bool thread_started = false; 

        // attack model
        public Dictionary<string, Attack> attack_container = new Dictionary<string, Attack>();

        // drop out models
        Bernoulli Bernoulli;
        MarkovChain Markov;
        Object DropOutModel = null;

        // listening port
        int port_recieve;

        // initialize thread 
        Thread thread_listener;

        // chart settings
        public static int n_steps = 100;
        public int chart_history = 60;

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> perturbations = new Dictionary<string, DataContainer>();

        // folder setting for chart image save
        public string folderName = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            port_recieve = Convert.ToInt16(args[1]);

            tbCanalPort.Text = port_recieve.ToString();
            StartListener();

            if (args.Length == 3)
            {
                nudBernoulliPass.Value = Convert.ToInt16(args[2]);
                rbBernoulli.Checked = true;
            }
            else if (args.Length == 4)
            {
                nudStayPass.Value = Convert.ToInt16(args[2]);
                nudStayDrop.Value = Convert.ToInt16(args[3]);
                rbMarkov.Checked = true;
            }
            UpdateDroupOutModel();

            timerChart.Start();
            initialSettings();

            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripStatusLabel1.Text = "Dir: " + folderName;

            ManageNumericalUpdowns();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string target_tag = tbTargetTag.Text;


            // initalize the attack object
            string type = "";
            if (rbBias.Checked == true)                 type = "bias";
            if (rbTransientDecrease.Checked == true)    type = "transientD";
            if (rbTransientIncrease.Checked == true)    type = "transientI";
            if (rbSinusoid.Checked == true)             type = "sinusoid";
            double duration = Convert.ToDouble(nudDuration.Value);
            double amplitude = Convert.ToDouble(nudAmplitude.Value);
            double time_constant = Convert.ToDouble(nudTimeConst.Value);
            double frequency = Convert.ToDouble(nudFrequency.Value);
            bool allIPs = cbAllIPs.Checked == true;
            bool allPorts = cbAllPorts.Checked == true;

            if (attack_container.ContainsKey(target_tag) || target_tag == "") // update attack settings
            {
                string tag = checkedListBox1.SelectedItem.ToString();
                attack_container[tag].UpdateModel(tbTargetIP.Text, tbTargetPort.Text, allIPs, allPorts, type, duration, amplitude, time_constant, frequency);
            }
            else // new attack model
            {
                Attack attack = new Attack(tbTargetIP.Text, tbTargetPort.Text, allIPs, allPorts, tbTargetTag.Text, type, duration, amplitude, time_constant, frequency);
                attack_container.Add(target_tag, attack);
                checkedListBox1.Items.Add(target_tag);
                checkedListBox1.SelectedIndex = checkedListBox1.Items.Count - 1;
                Helpers.CheckKey(perturbations, target_tag, n_steps);
            }

            tbTargetTag.Text = "";
        }

        private void UpdateDroupOutModel()
        {
            // update dropout models
            double treshold = Convert.ToDouble(nudBernoulliPass.Value);
            Bernoulli = new Bernoulli(treshold / 100);

            double P_pd = Convert.ToDouble(100 - nudStayPass.Value);
            double P_dp = Convert.ToDouble(100 - nudStayDrop.Value);
            Markov = new MarkovChain(1, P_pd / 100, P_dp / 100);

            if (rbBernoulli.Checked == true) DropOutModel = Bernoulli;
            if (rbMarkov.Checked == true) DropOutModel = Markov;
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            UpdateDroupOutModel();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            StartListener();
        }

        public void StartListener()
        {
            // update listening port
            port_recieve = Convert.ToInt16(tbCanalPort.Text);

            // create a new thread for the listener
            if (thread_started == false)
            {
                thread_listener = new Thread(() => Listener("ANY", port_recieve)); // listen on port 8000 (packages from any IP address)
                thread_listener.Start();
                thread_started = true;
            }
        }

        public void Listener(string IP, int port)
        {
            // initialize a connection to the GUI
            Server Listener = new Server(IP, port);

            while (true)
            {
                Thread.Sleep(1);
                try
                {
                    Listener.listen();
                    ParseMessage(Listener.last_recieved);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void ParseMessage(string message)
        {
            string EP_IP = "";
            string EP_Port = "";

            string text = message;
            string reconstruction = "";

            // split the message with the delimiter '#'
            string[] container = text.Split('#');

            foreach (string item in container)
            {
                // split each subtext (key and value)
                string[] subitem = item.Split('_');

                // extract key and value
                string key = subitem[0];
                string value = subitem[1];

                // detect the end point address
                if (key == "EP")
                {
                    // extract key and value
                    string[] EP = value.Split(':');
                    EP_IP = EP[0];
                    EP_Port = EP[1];
                    //string submessage = message.Substring((key + "_" + EP).Length); // remove the EP part
                }
                else
                {
                    // perform attack
                    if (attack_container.ContainsKey(key) && Helpers.isDouble(value) == true)
                        value = attack_container[key].IntegrityAttack(EP_IP, EP_Port, key, value);

                    //if (attack != null && Helpers.isDouble(value) == true) value = attack.IntegrityAttack(EP_IP, EP_Port, key, value);
                    reconstruction += "#" + key + "_" + value;
                }
            }

            if (reconstruction[0] == '#') reconstruction = reconstruction.Substring(1);
            SendMessage(EP_IP, Convert.ToInt16(EP_Port), reconstruction);
        }

        public void SendMessage(string IP, int port, string message)
        {
            // initialize a sender
            Client Sender = new Client(IP, port);

            // drop out
            MethodInfo isPass = DropOutModel.GetType().GetMethod("isPass");
            bool pass = (bool)isPass.Invoke(DropOutModel, null);

            if (pass == true)
            {
                Sender.send(message);
                Console.WriteLine("Pass: " + message);
            }
            else
                Console.WriteLine("Drop - " + DropOutModel.ToString());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // kill all threads when the form closes
            Environment.Exit(0);
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            //labelStatus.Text = "Time left[s]: " + Math.Round(attack.time_left, 1) + Environment.NewLine +
                               //"Perturbation: " + Math.Round(attack.value, 1);
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            foreach (string item in checkedListBox1.Items)
            {
                string key = item.ToString();
                perturbations[key].InsertData(DateTime.UtcNow.ToString(Helpers.FMT), attack_container[key].value.ToString());
            }

            // update time axis minimum and maximum
            Helpers.UpdateChartAxes(perturbationChart, chart_history);

            // draw chart
            UpdateChart(perturbationChart, perturbations);
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
                    DateTime time = DateTime.ParseExact(dict[key].time[i], Helpers.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    chart_.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].value[i]));
                }

                // remove old data points
                if (chart_.Series[key].Points.Count > 1000 * 5) chart_.Series[key].Points.RemoveAt(0);
            }
        }

        public void initialSettings()
        {
            perturbationChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            perturbationChart.ChartAreas["ChartArea1"].AxisY.Title = "Magnitude";
            perturbationChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            perturbationChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            perturbationChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            perturbationChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            perturbationChart.SaveImage(folderName + "\\chart_canal_attack.png", ChartImageFormat.Png);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (string item in checkedListBox1.CheckedItems)
            {
                string key = item.ToString();
                attack_container[key].Start();
            }

            timerStatus.Start();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tag = checkedListBox1.SelectedItem.ToString();

            // numerical up-downs
            nudDuration.Value = Convert.ToDecimal(attack_container[tag].duration);
            nudAmplitude.Value = Convert.ToDecimal(attack_container[tag].amplitude);
            nudTimeConst.Value = Convert.ToDecimal(attack_container[tag].time_const);
            nudFrequency.Value = Convert.ToDecimal(attack_container[tag].frequency);

            // check boxes
            cbAllIPs.Checked = attack_container[tag].all_IPs;
            cbAllPorts.Checked = attack_container[tag].all_ports;

            // radio buttons
            if (attack_container[tag].type == "bias") rbBias.Checked = true;
            if (attack_container[tag].type == "transientD") rbTransientDecrease.Checked = true;
            if (attack_container[tag].type == "transientI") rbTransientIncrease.Checked = true;
            if (attack_container[tag].type == "sinusoid") rbSinusoid.Checked = true;
        }

        private void ManageNumericalUpdowns()
        {
            if (rbBias.Checked == true)
            {
                nudTimeConst.Enabled = false;
                nudFrequency.Enabled = false;
            }
            else if (rbTransientIncrease.Checked == true)
            {
                nudTimeConst.Enabled = true;
                nudFrequency.Enabled = false;
            }
            else if (rbTransientDecrease.Checked == true)
            {
                nudTimeConst.Enabled = true;
                nudFrequency.Enabled = false;
            }
            else if (rbSinusoid.Checked == true)
            {
                nudTimeConst.Enabled = false;
                nudFrequency.Enabled = true;
            }

        }

        private void rbTransientDecrease_CheckedChanged(object sender, EventArgs e)
        {
            ManageNumericalUpdowns();
        }

        private void rbBias_CheckedChanged(object sender, EventArgs e)
        {
            ManageNumericalUpdowns();
        }

        private void rbTransientIncrease_CheckedChanged(object sender, EventArgs e)
        {
            ManageNumericalUpdowns();
        }

        private void rbSinusoid_CheckedChanged(object sender, EventArgs e)
        {
            ManageNumericalUpdowns();
        }
    }

    
}
