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
    public partial class CanalGUI : Form
    {
        // attack model container
        public Dictionary<string, AttackModel> attack_model_container = new Dictionary<string, AttackModel>();
        string selected_attack_model = "";

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> attack_timeseries = new Dictionary<string, DataContainer>();

        // attack settings
            // specify target package
            string target_tag = "";
            string target_ip = "";
            string target_port = "";

            // specify attack type
            string attack_type = "";
            double[] time_series = new double[0];       // empty array which is used ONLY if a manual attack is conduced
            // bool which determine if the attack adds a value or sets a value
            bool integrity_add = true;

            // attack parameters
            double duration = 0;
            double amplitude = 0;
            double time_constant = 0;
            double frequency = 0;
            bool all_IPs = false;
            bool all_Ports = false;

        // chart settings
        public static int n_steps = 100;
        public int chart_history = 60;

        // drop out models
        Bernoulli Bernoulli;
        MarkovChain Markov;
        Object SelectedDropOutModel = null;

        // listening port
        int port_recieve;

        // initialize thread 
        Thread thread_listener;
        bool thread_started = false;

        // folder setting for chart image save
        public string folderName = "";

        public CanalGUI()
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

            // choose drop out model depending on the number of command line arguments
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

            Charting.InitialChartSettings(this);
            Helpers.ManageNumericalUpdowns(this);

            timerChart.Start();
        }

        public void StartListener()
        {
            // update listening port
            port_recieve = Convert.ToInt16(tbCanalPort.Text);

            // create a new thread for the listener
            if (thread_started == false)
            {
                thread_listener = new Thread(() => Listener("ANY_IP", port_recieve)); // listen on packages from any IP address
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
                try
                {
                    Listener.Listen();
                    ParseMessage(Listener.last_recieved);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    textBox2.Text += ex.ToString();
                }
            }
        }

        public void ParseMessage(string message)
        {
            // end point address
            string EP_IP = "";
            string EP_Port = "";

            // recieved package and send package
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
                    if (attack_model_container.ContainsKey(key) && Helpers.isDouble(value) == true)
                        value = attack_model_container[key].IntegrityAttack(EP_IP, EP_Port, key, value);

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

            // implement drop-out model
            MethodInfo isPass = SelectedDropOutModel.GetType().GetMethod("isPass");
            bool pass = (bool)isPass.Invoke(SelectedDropOutModel, null);

            if (pass == true) Sender.Send(message);
        }

        private void btnAddAttackModel_Click(object sender, EventArgs e)
        {
            // add new attack model
            GetAttackSettings();
            if ((attack_model_container.ContainsKey(target_tag) || target_tag == "") == false)
            {
                AttackModel new_attack_model = new AttackModel(target_ip, target_port, all_IPs, all_Ports, target_tag, attack_type, integrity_add, duration, amplitude, time_constant, frequency, time_series);
                attack_model_container.Add(target_tag, new_attack_model);
                clbAttackModels.Items.Add(target_tag);
                clbAttackModels.SelectedIndex = clbAttackModels.Items.Count - 1;
                selected_attack_model = clbAttackModels.SelectedItem.ToString();
                Helpers.AddKey(attack_timeseries, target_tag, n_steps);

                tbTargetTag.Text = "";
            }
        }

        private void btnUpdateAttackModel_Click(object sender, EventArgs e)
        {
            // update selected attack model
            GetAttackSettings();
            attack_model_container[selected_attack_model].UpdateModel(tbTargetIP.Text, tbTargetPort.Text, all_IPs, all_Ports, attack_type, integrity_add, duration, amplitude, time_constant, frequency, time_series);
        }

        private void UpdateDroupOutModel()
        {
            // update dropout models
            double treshold = Convert.ToDouble(nudBernoulliPass.Value);
            Bernoulli = new Bernoulli(treshold / 100);

            double P_pd = Convert.ToDouble(100 - nudStayPass.Value);
            double P_dp = Convert.ToDouble(100 - nudStayDrop.Value);
            Markov = new MarkovChain(1, P_pd / 100, P_dp / 100);

            // assign bernoulli or markov as the current drop out model
            if (rbBernoulli.Checked == true) SelectedDropOutModel = Bernoulli;
            if (rbMarkov.Checked == true) SelectedDropOutModel = Markov;
        }

        private void btnAttack_Click(object sender, EventArgs e)
        {
            // start attack on all checked items
            foreach (string item in clbAttackModels.CheckedItems)
            {
                string key = item.ToString();
                attack_model_container[key].Start();
            }

            timerStatus.Start();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // stop attack on all checked items
            foreach (string item in clbAttackModels.CheckedItems)
            {
                string key = item.ToString();
                attack_model_container[key].Stop();
            }
        }

        private void clbAttackModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update windows form values
            selected_attack_model = clbAttackModels.SelectedItem.ToString();

            // numerical up-downs
            nudDuration.Value = Convert.ToDecimal(attack_model_container[selected_attack_model].duration);
            nudAmplitude.Value = Convert.ToDecimal(attack_model_container[selected_attack_model].amplitude_attack);
            nudTimeConst.Value = Convert.ToDecimal(attack_model_container[selected_attack_model].time_const);
            nudFrequency.Value = Convert.ToDecimal(attack_model_container[selected_attack_model].frequency);

            // check boxes
            cbAllIPs.Checked = attack_model_container[selected_attack_model].all_IPs;
            cbAllPorts.Checked = attack_model_container[selected_attack_model].all_ports;

            // radio buttons
            if (attack_model_container[selected_attack_model].type == "bias") rbBias.Checked = true;
            if (attack_model_container[selected_attack_model].type == "transient_decr") rbTransientDecrease.Checked = true;
            if (attack_model_container[selected_attack_model].type == "transient_incr") rbTransientIncrease.Checked = true;
            if (attack_model_container[selected_attack_model].type == "sinusoid") rbSinusoid.Checked = true;
            if (attack_model_container[selected_attack_model].type == "manual") rbManual.Checked = true;
            if (attack_model_container[selected_attack_model].integrity_add == true) rbAddValue.Checked = true;
            else rbSetValue.Checked = true;
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            // add data-points
            foreach (string item in clbAttackModels.Items)
            {
                string key = item.ToString();
                attack_timeseries[key].InsertData(DateTime.UtcNow.ToString(Constants.FMT), attack_model_container[key].value_attack.ToString());
            }

            // draw chart
            UpdateChart(perturbationChart, attack_timeseries);

            // scale y-axis for the chart
            Charting.ChangeYScale(perturbationChart, "data_chart");

            // update time axis minimum and maximum
            Charting.UpdateChartAxes(perturbationChart, chart_history);
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
                if (chart_.Series.IndexOf(key) == -1) Charting.AddChartSeries(key, chart_);

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

        private void GetAttackSettings()
        {
            // specify target package
            target_tag = tbTargetTag.Text;
            target_ip = tbTargetIP.Text;
            target_port = tbTargetPort.Text;

            // specify attack type
            if (rbBias.Checked == true) attack_type = "bias";
            if (rbTransientDecrease.Checked == true) attack_type = "transient_decr";
            if (rbTransientIncrease.Checked == true) attack_type = "transient_incr";
            if (rbSinusoid.Checked == true) attack_type = "sinusoid";
            if (rbManual.Checked == true)
            {
                attack_type = "manual";
                time_series = Helpers.DecodeTimeSeries(tbTimeSeries.Text);
            }
            if (rbDelay.Checked == true) attack_type = "delay";

            // bool which determine if the attack adds a value or sets a value
            integrity_add = true;
            if (rbAddValue.Checked == true) integrity_add = true;
            else if (rbSetValue.Checked == true) integrity_add = false;

            // attack parameters
            duration = Convert.ToDouble(nudDuration.Value);
            amplitude = Convert.ToDouble(nudAmplitude.Value);
            time_constant = Convert.ToDouble(nudTimeConst.Value);
            frequency = Convert.ToDouble(nudFrequency.Value);
            all_IPs = cbAllIPs.Checked == true;
            all_Ports = cbAllPorts.Checked == true;


        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            labelStatus.Text = "Time left[s]: " + Math.Round(attack_model_container[selected_attack_model].time_left, 1) + Environment.NewLine +
            "Perturbation: " + Math.Round(attack_model_container[selected_attack_model].value_attack, 1);
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

        private void rbManual_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void rbDelay_CheckedChanged(object sender, EventArgs e)
        {
            Helpers.ManageNumericalUpdowns(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // kill all threads when the form closes
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbTimeSeries.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void CanalGUI_Load(object sender, EventArgs e)
        {

        }


    }
}
