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
using GlobalComponents;

namespace Channel_GUI
{
    public partial class ChannelGUI : Form
    {
        // attack model container
        public Dictionary<string, AttackModel> attack_model_container = new Dictionary<string, AttackModel>();
        string attack_model_selected = "";

        // data containers (dictionaries)
        public Dictionary<string, DataContainer> attack_timeseries = new Dictionary<string, DataContainer>();
        public Dictionary<string, DataContainer> packet_timeseries = new Dictionary<string, DataContainer>();

        // end-point addresses
        public List<AddressEndPoint> end_points = new List<AddressEndPoint>();

        // end-point addresses --> y deviation in packet plot
        public Dictionary<string, double> end_point_ydev = new Dictionary<string, double>();
        public double y_incr = 0;

        // container for attack settings
        AttackParameters attack_parameters = new AttackParameters();

        // chart settings
        public int time_chart_window = 60;

        // drop out models
        Bernoulli Bernoulli;
        MarkovChain Markov;
        Object SelectedDropOutModel = null;

        // listening port
        int port_recieve;

        // initialize thread 
        Thread thread_listener;
        bool thread_listener_started = false;

        // folder setting for chart image save
        public string folderName = "";

        public ChannelGUI()
        {
            InitializeComponent();
        }

        private void ChannelGUI_Load(object sender, EventArgs e)
        {
            // read command line arguments
            string[] args = Environment.GetCommandLineArgs();
            ParseArgs(args);

            // update the dropout model (depending on selection and values)
            UpdateDroupOutModel();

            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripLabel.Text = "Dir: " + folderName;

            // Initial chart settings
            Charting.InitialChartSettings(this);

            // manage form controls (e.g. enable/disable)
            Helpers.ManageNumericalUpdowns(this);

            timerChart.Start();
        }

        public void StartListener()
        {
            // update listening port
            port_recieve = Convert.ToInt16(tbCanalPort.Text);

            // create a new thread for the listener
            if (thread_listener_started == false)
            {
                thread_listener = new Thread(() => Listener("ANY_IP", port_recieve)); // listen on packets from any IP address
                thread_listener.Start();
                thread_listener_started = true;
            }
        }

        public void Listener(string IP, int port)
        {
            // initialize a new listener
            Server Listener = new Server(IP, port);

            while (true)
            {
                try
                {
                    Listener.Listen();
                    ParseMessage(Listener.last_recieved, Listener.last_ip);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void ParseMessage(string message, string sender_IP)
        {
            // end point address
            string EP_IP = "";
            string EP_Port = "";

            // recieved packet and send packet
            string text = message;
            string message_reconstruction = "";

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
                    // if (attack_model_container.ContainsKey(key) && Helpers.isDouble(value) == true)
                    // if (attack_model_container.Any(kvp => kvp.Key.Contains(key)) && Helpers.isDouble(value) == true)
               
                    foreach (string dict_key in attack_model_container.Keys)
                    {
                        if (dict_key.Contains(key))
                        {
                            value = attack_model_container[dict_key].ApplyIntegrityAttack(EP_IP, EP_Port, key, value);
                        }                    
                    }
                    // reconstruct the message (possibly with modified contents)
                    message_reconstruction += "#" + key + "_" + value;
                }
            }

            if (message_reconstruction[0] == '#') message_reconstruction = message_reconstruction.Substring(1);
            SendMessage(EP_IP, Convert.ToInt16(EP_Port), message_reconstruction, sender_IP);
        }

        public void SendMessage(string EP_IP, int port, string message, string sender_IP)
        {
            // append each sender>>destination pair to the end_points container (and checked listbox)
            AddressEndPoint EP = new AddressEndPoint(sender_IP + " >> " + EP_IP, port);

            if (end_points.Contains(EP) == false)
            {
                end_points.Add(EP);
                clbDropOutTarget.Items.Add(EP);

                // manage the y-deviation in the plot
                end_point_ydev.Add(EP.ToString(), y_incr);
                y_incr += 0.04;
            }

            // initialize a sender
            Client Sender = new Client(EP_IP, port);

            // add key
            Tools.AddKeyToDict(packet_timeseries, EP.ToString(), Constants.n_steps);

            // check if the end-point is checked or not (subject to packet drop-out or not)
            if (clbDropOutTarget.GetItemCheckState(clbDropOutTarget.Items.IndexOf(EP)) == CheckState.Checked)
            {
                // implement drop-out model
                MethodInfo isPass = SelectedDropOutModel.GetType().GetMethod("isPass");
                bool pass = (bool)isPass.Invoke(SelectedDropOutModel, null);

                if (pass == true)
                {
                    Sender.Send(message);

                    // mark packet as passed
                    packet_timeseries[EP.ToString()].InsertData(DateTime.UtcNow.ToString(Constants.FMT), (1 + end_point_ydev[EP.ToString()]).ToString());
                }
                else
                {
                    // mark packet as dropped
                    packet_timeseries[EP.ToString()].InsertData(DateTime.UtcNow.ToString(Constants.FMT), (0 + end_point_ydev[EP.ToString()]).ToString());
                }
            }
            else
            {
                // if the end-point is unchecked, pass the packet automatically
                Sender.Send(message);

                // mark packet as passed
                packet_timeseries[EP.ToString()].InsertData(DateTime.UtcNow.ToString(Constants.FMT), (1 + end_point_ydev[EP.ToString()]).ToString());
            }
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
            UpdateChart(attackChart, attack_timeseries);
            UpdateChart(packetChart, packet_timeseries);

            // scale y-axis for the chart
            Charting.ChangeYScale(attackChart, "data_chart");

            // update time axis minimum and maximum
            Charting.UpdateChartAxes(attackChart, time_chart_window);
            Charting.UpdateChartAxes(packetChart, time_chart_window);
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

        private void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                List<string> arg_sep = Tools.ArgsParser(arg);
                string arg_name = arg_sep[0];

                switch (arg_name)
                {
                    case "port_receive":
                        // start listening on corresponding port
                        tbCanalPort.Text = arg_sep[1].ToString();
                        StartListener();
                        break;

                    case "bernoulli":
                        nudBernoulliPass.Value = Convert.ToInt16(arg_sep[1]);
                        rbBernoulli.Checked = true;
                        break;

                    case "markov":
                        nudStayPass.Value = Convert.ToInt16(arg_sep[1]);
                        nudStayDrop.Value = Convert.ToInt16(arg_sep[2]);
                        rbMarkov.Checked = true;
                        break;
                    case "ARG_INVALID":
                        break;
                    default:
                        MessageBox.Show("Unknown argument not used: " + arg_name);
                        break;
                }
            }
        }

        private void btnAddAttackModel_Click(object sender, EventArgs e)
        {
            // add new attack model
            GetAttackSettings();
            if ((attack_model_container.ContainsKey(attack_parameters.target_tag) || attack_parameters.target_tag == "") == false)
            {
                AttackModel new_attack_model = new AttackModel(attack_parameters.target_ip, attack_parameters.target_port, attack_parameters.target_all_IPs, attack_parameters.target_all_Ports, attack_parameters.target_tag, attack_parameters.attack_type, attack_parameters.integrity_add, attack_parameters.duration, attack_parameters.amplitude, attack_parameters.time_constant, attack_parameters.frequency, attack_parameters.time_series, attack_parameters.time_series_raw);
                attack_model_container.Add(attack_parameters.target_tag + " @" + attack_parameters.target_ip + ":" + attack_parameters.target_port, new_attack_model);
                clbAttackModels.Items.Add(attack_parameters.target_tag + " @" + attack_parameters.target_ip + ":" + attack_parameters.target_port);
                clbAttackModels.SelectedIndex = clbAttackModels.Items.Count - 1;
                attack_model_selected = clbAttackModels.SelectedItem.ToString();
                Tools.AddKeyToDict(attack_timeseries, attack_parameters.target_tag + " @" + attack_parameters.target_ip + ":" + attack_parameters.target_port, Constants.n_steps);

                tbTargetTag.Text = "";
            }
        }

        private void btnUpdateAttackModel_Click(object sender, EventArgs e)
        {
            // update selected attack model
            GetAttackSettings();
            attack_model_container[attack_model_selected].UpdateModel(attack_parameters.target_ip, attack_parameters.target_port, attack_parameters.target_all_IPs, attack_parameters.target_all_Ports, attack_parameters.attack_type, attack_parameters.integrity_add, attack_parameters.duration, attack_parameters.amplitude, attack_parameters.time_constant, attack_parameters.frequency, attack_parameters.time_series, attack_parameters.time_series_raw);
        }

        private void UpdateDroupOutModel()
        {
            // bernoulli
            double treshold = Convert.ToDouble(nudBernoulliPass.Value);
            Bernoulli = new Bernoulli(treshold / 100);

            // markov
            double P_pd = Convert.ToDouble(100 - nudStayPass.Value);
            double P_dp = Convert.ToDouble(100 - nudStayDrop.Value);
            Markov = new MarkovChain(1, P_pd / 100, P_dp / 100);

            // assign bernoulli or markov as the current drop out model
            if (rbBernoulli.Checked == true) SelectedDropOutModel = Bernoulli;
            if (rbMarkov.Checked == true) SelectedDropOutModel = Markov;
        }

        private void btnStartAttack_Click(object sender, EventArgs e)
        {
            // start attack on all checked items
            foreach (string item in clbAttackModels.CheckedItems)
            {
                string key = item.ToString();
                attack_model_container[key].Start();
            }

            timerStatus.Start();
        }

        private void btnStopAttack_Click(object sender, EventArgs e)
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
            attack_model_selected = clbAttackModels.SelectedItem.ToString();

            // numerical up-downs
            nudDuration.Value = Convert.ToDecimal(attack_model_container[attack_model_selected].duration);
            nudAmplitude.Value = Convert.ToDecimal(attack_model_container[attack_model_selected].amplitude_attack);
            nudTimeConst.Value = Convert.ToDecimal(attack_model_container[attack_model_selected].time_const);
            nudFrequency.Value = Convert.ToDecimal(attack_model_container[attack_model_selected].frequency);
            tbTimeSeries.Text = attack_model_container[attack_model_selected].time_series_raw;
            tbTargetIP.Text = attack_model_container[attack_model_selected].target_IP;
            tbTargetPort.Text = attack_model_container[attack_model_selected].target_port;
            //tbTimeSeries.Text = attack_model_container[attack_model_selected].time_series;

            // check boxes
            cbAllIPs.Checked = attack_model_container[attack_model_selected].target_all_IPs;
            cbAllPorts.Checked = attack_model_container[attack_model_selected].target_all_ports;

            // radio buttons
            if (attack_model_container[attack_model_selected].type == "bias") rbBias.Checked = true;
            if (attack_model_container[attack_model_selected].type == "transient_decr") rbTransientDecrease.Checked = true;
            if (attack_model_container[attack_model_selected].type == "transient_incr") rbTransientIncrease.Checked = true;
            if (attack_model_container[attack_model_selected].type == "sinusoid") rbSinusoid.Checked = true;
            if (attack_model_container[attack_model_selected].type == "manual") rbManual.Checked = true;
            if (attack_model_container[attack_model_selected].integrity_add == true) rbAddValue.Checked = true;
            else rbSetValue.Checked = true;
        }

        private void GetAttackSettings()
        {
            // specify target packet
            string target_tag = tbTargetTag.Text;

            string target_ip = "";
            string target_port = "";

            if (cbAllIPs.Checked == true) target_ip = "<any IP>";
            else  target_ip = tbTargetIP.Text;
            if (cbAllPorts.Checked == true) target_port = "<any Port>";
            else  target_port = tbTargetPort.Text;

            // specify attack type
            string attack_type = "";
            double[] time_series = new double[0];
            if (rbBias.Checked == true) attack_type = "bias";
            if (rbTransientDecrease.Checked == true) attack_type = "transient_decr";
            if (rbTransientIncrease.Checked == true) attack_type = "transient_incr";
            if (rbSinusoid.Checked == true) attack_type = "sinusoid";
            string time_series_raw = tbTimeSeries.Text;
            if (rbManual.Checked == true)
            {
                attack_type = "manual";
                time_series = Helpers.DecodeTimeSeries(time_series_raw);
            }

            // bool which determine if the attack adds a value or sets a value
            bool integrity_add = true;
            if (rbAddValue.Checked == true) integrity_add = true;
            else if (rbSetValue.Checked == true) integrity_add = false;

            // attack parameters
            double duration = Convert.ToDouble(nudDuration.Value);
            double amplitude = Convert.ToDouble(nudAmplitude.Value);
            double time_constant = Convert.ToDouble(nudTimeConst.Value);
            double frequency = Convert.ToDouble(nudFrequency.Value);
            bool target_all_IPs = cbAllIPs.Checked;
            bool target_all_Ports = cbAllPorts.Checked;

            attack_parameters = new AttackParameters(target_tag, target_ip, target_port, attack_type, time_series, time_series_raw, integrity_add, duration, amplitude, time_constant, frequency, target_all_IPs, target_all_Ports);
        }

        private void nudHistory_ValueChanged(object sender, EventArgs e)
        {
            time_chart_window = Convert.ToInt16(nudHistory.Value);
            if (time_chart_window > 0)
            {
                attackChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
                packetChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
            }
            nudHistory1.Value = time_chart_window;
        }

        private void nudHistory1_ValueChanged(object sender, EventArgs e)
        {
            time_chart_window = Convert.ToInt16(nudHistory1.Value);
            if (time_chart_window > 0)
            {
                attackChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
                packetChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
            }
            nudHistory.Value = time_chart_window;
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            labelStatus.Text = "Time left[s]: " + Math.Round(attack_model_container[attack_model_selected].time_left, 1) + Environment.NewLine +
            "Perturbation: " + Math.Round(attack_model_container[attack_model_selected].value_attack, 1);
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
            attackChart.SaveImage(folderName + "\\chart_canal_attack.png", ChartImageFormat.Png);
            packetChart.SaveImage(folderName + "\\chart_canal_packet.png", ChartImageFormat.Png);
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

        private void btnClearCustomAttack_Click(object sender, EventArgs e)
        {
            tbTimeSeries.Text = "";
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

        private void nudStayDrop_ValueChanged(object sender, EventArgs e)
        {
            UpdateDroupOutModel();
        }

        private void nudStayPass_ValueChanged(object sender, EventArgs e)
        {
            UpdateDroupOutModel();
        }

        private void nudBernoulliPass_ValueChanged(object sender, EventArgs e)
        {
            UpdateDroupOutModel();
        }

        private void rbMarkov_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDroupOutModel();
        }

        private void rbBernoulli_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDroupOutModel();
        }
    }
}
