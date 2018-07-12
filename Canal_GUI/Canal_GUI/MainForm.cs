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
    public partial class MainForm : Form
    {
        // attack model container
        public Dictionary<string, Attack> attack_container = new Dictionary<string, Attack>();
        string selected_tag = "";

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> perturbations = new Dictionary<string, DataContainer>();

        // chart settings
        public static int n_steps = 100;
        public int chart_history = 60;

        // drop out models
        Bernoulli Bernoulli;
        MarkovChain Markov;
        Object DropOutModel = null;

        // listening port
        int port_recieve;

        // initialize thread 
        Thread thread_listener;
        bool thread_started = false;

        // folder setting for chart image save
        public string folderName = "";

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // read command line arguments
            string[] args = Environment.GetCommandLineArgs();
            port_recieve = Convert.ToInt16(args[1]);

            // start listening on corresponding port
            tbCanalPort.Text = port_recieve.ToString();
            StartListener();

            // choose drop out model depending on the number of arguments
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

            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripLabel.Text = "Dir: " + folderName;

            Helpers.InitialChartSettings(this);
            Helpers.ManageNumericalUpdowns(this);

            timerChart.Start();
        }

        private void btnAddAttackModel_Click(object sender, EventArgs e)
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
                attack_container[selected_tag].UpdateModel(tbTargetIP.Text, tbTargetPort.Text, allIPs, allPorts, type, duration, amplitude, time_constant, frequency);
            }
            else // new attack model
            {
                Attack attack = new Attack(tbTargetIP.Text, tbTargetPort.Text, allIPs, allPorts, tbTargetTag.Text, type, duration, amplitude, time_constant, frequency);
                attack_container.Add(target_tag, attack);
                clbAttackModels.Items.Add(target_tag);
                clbAttackModels.SelectedIndex = clbAttackModels.Items.Count - 1;
                selected_tag = clbAttackModels.SelectedItem.ToString();
                Helpers.CheckKey(perturbations, target_tag, n_steps);
            }

            tbTargetTag.Text = "";
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
                    Listener.Listen();
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
                }
                else
                {
                    // perform attack
                    if (attack_container.ContainsKey(key) && Helpers.isDouble(value) == true)
                        value = attack_container[key].IntegrityAttack(EP_IP, EP_Port, key, value);

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

            // check if pass or drop
            MethodInfo isPass = DropOutModel.GetType().GetMethod("isPass");
            bool pass = (bool)isPass.Invoke(DropOutModel, null);

            if (pass == true) Sender.Send(message);
        }

        private void UpdateDroupOutModel()
        {
            // update dropout models
            double treshold = Convert.ToDouble(nudBernoulliPass.Value);
            Bernoulli = new Bernoulli(treshold / 100);

            double P_pd = Convert.ToDouble(100 - nudStayPass.Value);
            double P_dp = Convert.ToDouble(100 - nudStayDrop.Value);
            Markov = new MarkovChain(1, P_pd / 100, P_dp / 100);

            // assign beronoulli or markov as the current drop out model
            if (rbBernoulli.Checked == true) DropOutModel = Bernoulli;
            if (rbMarkov.Checked == true) DropOutModel = Markov;
        }

        private void btnAttack_Click(object sender, EventArgs e)
        {
            foreach (string item in clbAttackModels.CheckedItems)
            {
                string key = item.ToString();
                attack_container[key].Start();
            }

            timerStatus.Start();
        }

        private void clbAttackModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            selected_tag = clbAttackModels.SelectedItem.ToString();

            // numerical up-downs
            nudDuration.Value = Convert.ToDecimal(attack_container[selected_tag].duration);
            nudAmplitude.Value = Convert.ToDecimal(attack_container[selected_tag].amplitude);
            nudTimeConst.Value = Convert.ToDecimal(attack_container[selected_tag].time_const);
            nudFrequency.Value = Convert.ToDecimal(attack_container[selected_tag].frequency);

            // check boxes
            cbAllIPs.Checked = attack_container[selected_tag].all_IPs;
            cbAllPorts.Checked = attack_container[selected_tag].all_ports;

            // radio buttons
            if (attack_container[selected_tag].type == "bias") rbBias.Checked = true;
            if (attack_container[selected_tag].type == "transientD") rbTransientDecrease.Checked = true;
            if (attack_container[selected_tag].type == "transientI") rbTransientIncrease.Checked = true;
            if (attack_container[selected_tag].type == "sinusoid") rbSinusoid.Checked = true;
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            // add data-points
            foreach (string item in clbAttackModels.Items)
            {
                string key = item.ToString();
                perturbations[key].InsertData(DateTime.UtcNow.ToString(Constants.FMT), attack_container[key].value.ToString());
            }

            // scale y-axis for the chart
            Helpers.ChangeYScale(perturbationChart, "data_chart");

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
                    DateTime time = DateTime.ParseExact(dict[key].time[i], Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    chart_.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].value[i]));
                }

                // remove old data points
                if (chart_.Series[key].Points.Count > Constants.n_steps) chart_.Series[key].Points.RemoveAt(0);
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            labelStatus.Text = "Time left[s]: " + Math.Round(attack_container[selected_tag].time_left, 1) + Environment.NewLine +
            "Perturbation: " + Math.Round(attack_container[selected_tag].value, 1);
        }

        private void btnUpdateDropoutModel_Click_1(object sender, EventArgs e)
        {
            UpdateDroupOutModel();
        }

        private void btnStartListener_Click_2(object sender, EventArgs e)
        {
            StartListener();
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            perturbationChart.SaveImage(folderName + "\\chart_canal_attack.png", ChartImageFormat.Png);
        }

        private void rbTransientDecrease_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void rbBias_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void rbTransientIncrease_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void rbSinusoid_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // kill all threads when the form closes
            Environment.Exit(0);
        }
    }
}
