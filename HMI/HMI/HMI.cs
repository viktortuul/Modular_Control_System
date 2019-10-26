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
using System.Globalization;
using System.Net;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using Communication;
using GlobalComponents;
using Newtonsoft.Json;

/* TODO
 * Only adjust max/min axis values for time-series that are checked
 * Save "checked time-series" settings when switching between processes
*/

namespace HMI
{
    public partial class FrameGUI : Form
    {
        // controller module connection
        List<CommunicationManager> connections = new List<CommunicationManager>(); // container for controller module connections
        public CommunicationManager connection_selected;    // selected controller module
        public int n_connections = 0;                       // connection count (number of connected controller modules/plants)

        // chart settings
        public int time_chart_window = 60;                  // chart view time [s]

        // folder setting for chart image save
        public string file_path = "";

        // channel settings
        public bool using_channel = false;
        AddressEndPoint Channel_EP = new AddressEndPoint();

        // tank dimensions (for visualization)
        public string plant_visualization = PlantVisualization.DOUBLE_WATERTANK;

        // debug log
        public string debugLog = "";

        // control/observer mode
        public string GUI_view_mode = GUIViewMode.CONTROL;

        // configuration
        private string config_file = "";
        public Configuration config = new Configuration();

        public FrameGUI()
        {
            InitializeComponent();
        }

        private void FrameGUI_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            ParseArgs(args);
     
            // load config from json
            config = loadJson();

            // folder and chart settings
            InitialSettings();

            // toggle view (control as default)
            toggleViewMode(GUIViewMode.CONTROL);
        }

        private Configuration loadJson()
        {
            Configuration config = new Configuration();
            using (StreamReader r = new StreamReader(config_file))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Configuration>(json);
            }

            numUpDownKp.Value = Convert.ToDecimal(config.pidParameters.P);
            numUpDownKi.Value = Convert.ToDecimal(config.pidParameters.I);
            numUpDownKd.Value = Convert.ToDecimal(config.pidParameters.D);

            return config;
        }

        private void timerCharts_Tick(object sender, EventArgs e)
        {
            if (connection_selected.is_connected_to_plant == true)
            {
                // add the correct number of reference series
                Charting.ManageReferenceSeries(this);

                // update charts
                UpdateChart(connection_selected.references, setting : DataPointSetting.ADD_LATEST); // reference
                UpdateChart(connection_selected.received_packets, setting : DataPointSetting.ADD_LATEST); // states
                UpdateChart(connection_selected.estimates, setting : DataPointSetting.ADD_LATEST); // kalman filter estimates

                // update time axis minimum and maximum
                Charting.UpdateChartAxes(dataChart, time_chart_window);
                Charting.UpdateChartAxes(residualChart, time_chart_window);
                Charting.UpdateChartAxes(securityChart, time_chart_window);
            }

            // update time labels
            Helpers.UpdateTimeLabels(this, connection_selected, Constants.FMT);

            // draw simulation (currently only supporing double watertank)
            if (plant_visualization == PlantVisualization.DOUBLE_WATERTANK)
                Animation.DrawTanks(this, connection_selected);
        }

        private void UpdateChart(Dictionary<string, DataContainer> dict, string setting)
        {
            // get all keys from the current dictionary
            var keys = dict.Keys.ToList();

            // update the graphs for each key
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (dataChart.Series.IndexOf(key) == -1) Charting.AddChartSeries(this, key, dataChart);

                // data load setting
                int index_start;

                if (setting == DataPointSetting.LOAD_ALL)
                    index_start = 0;                                // draw all data points
                else if (setting == DataPointSetting.ADD_LATEST)
                    index_start = dict[key].time.Length - 1;        // draw the last data point
                else
                    index_start = dict[key].time.Length - 1;        // draw the last data point

                // add data point(s)
                for (int i = index_start; i < dict[key].time.Length; i++)
                {
                    if (dict[key].time[i] != null)
                    {
                        DateTime time = DateTime.ParseExact(dict[key].time[i], Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                        dataChart.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].value[i]));

                        // plot residuals for flagged states
                        if (dict[key].has_residual == true)
                        {
                            // if residual series does not exist, add it
                            if (residualChart.Series.IndexOf("Res_" + key) == -1) Charting.AddChartSeries(this, "Res_" + key, residualChart);
                            residualChart.Series["Res_" + key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].residual[i]));

                            // if security metric series does not exist, add it
                            if (securityChart.Series.IndexOf("Sec_" + key) == -1) Charting.AddChartSeries(this, "Sec_" + key, securityChart);
                            securityChart.Series["Sec_" + key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].security_metric[i]));
                        }
                    }
                }

                // remove old data points
                if (dataChart.Series[key].Points.Count > Constants.n_datapoints_max)
                {
                    dataChart.Series[key].Points.RemoveAt(0);
                    if (dict[key].has_residual)
                    {
                        residualChart.Series["Res_" + key].Points.RemoveAt(0);
                        securityChart.Series["Sec_" + key].Points.RemoveAt(0);
                    }
                }
            }
        }

        private void listBoxControllers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // specify the new connection as the currently selected one
                connection_selected = connections[listBoxModules.SelectedIndex];

                // enable or disable reference track bars
                Helpers.ManageTrackbars(this);

                // update GUI values according to the connected controller
                Helpers.UpdateGuiControls(this, connection_selected);

                // clear charts
                dataChart.Series.Clear();
                residualChart.Series.Clear();
                securityChart.Series.Clear();

                // clear checkbox series
                clbSeries.Items.Clear();

                // refresh the chart, load all data points
                UpdateChart(connection_selected.references, setting : DataPointSetting.LOAD_ALL); // reference
                UpdateChart(connection_selected.received_packets, setting: DataPointSetting.LOAD_ALL); // states
                UpdateChart(connection_selected.estimates, setting: DataPointSetting.LOAD_ALL); // kalman filter estimates

                // scale y-axis for the charts
                Charting.ChangeYScale(dataChart, treshold_interval : "one", grid_interval : "one");
                Charting.ChangeYScale(residualChart, treshold_interval: "one", grid_interval: "one");
                Charting.ChangeYScale(securityChart, treshold_interval: "one", grid_interval: "one");

                // update treshold strip lines
                updateThresholdStripLines();

                // select the corresponding item in the treeview
                treeViewControllers.SelectedNode = treeViewControllers.Nodes[connection_selected.name];
            }
            catch { }
        }

        public void updateThresholdStripLines()
        {
            Charting.ClearThresholdStripLines(residualChart);
            Charting.AddThresholdStripLine(residualChart, offset: 0, color: Color.Red);
            Charting.AddThresholdStripLine(residualChart, offset: connection_selected.kalman_filter.getDelta(), color: Color.Red);
            Charting.AddThresholdStripLine(residualChart, offset: -connection_selected.kalman_filter.getDelta(), color: Color.Red);
        }

        private void timerUpdateGUI_Tick(object sender, EventArgs e)
        {
            // update tree information
            Helpers.UpdateTree(this, connection_selected);

            // enable or disable reference track bars
            Helpers.ManageTrackbars(this);

            // scale y-axis for the charts
            Charting.ChangeYScale(dataChart, treshold_interval: "one", grid_interval: "adaptive");
            Charting.ChangeYScale(residualChart, treshold_interval: "one", grid_interval: "adaptive");
            Charting.ChangeYScale(securityChart, treshold_interval: "one", grid_interval: "adaptive");

            labelSecurity.Text = "Security metric: " + Math.Round(connection_selected.kalman_filter.security_metric, 1) + Environment.NewLine +
                                 "Status: " + connection_selected.kalman_filter.security_status;
        }

        private void btnAllowConnection_Click_1(object sender, EventArgs e)
        {
            // enable parameter settings
            numUpDownKp.Enabled = true;
            numUpDownKi.Enabled = true;
            numUpDownKd.Enabled = true;

            // control module name and corresponding ip:port pairs
            string name = textBoxName.Text;
            string IP_endpoint = textBox_ip_send.Text;
            string IP_this = textBox_ip_receive.Text;
            int port_endpoint = Convert.ToInt16(numericUpDown_port_send.Value);
            int port_this = Convert.ToInt16(numericUpDown_port_receive.Value);

            // look for name and port conflicts with present allowed traffics
            bool connection_already_exists = false;
            foreach (CommunicationManager controller in connections)
            {
                if ((controller.Controller_EP.PortThis == port_this && controller.Controller_EP.Port == port_endpoint) || controller.name == name)
                {
                    connection_already_exists = true;
                }
            }

            if (connection_already_exists == true)
            {
                log("Error: name or ip:port pair already in use");
            }
            else
            {
                // connection parameters
                ConnectionParameters connectionParameters = new ConnectionParameters(IP_endpoint, port_endpoint, port_this);

                // create and add controller
                PIDparameters controllerParameters = new PIDparameters(Convert.ToDouble(numUpDownKp.Value), Convert.ToDouble(numUpDownKi.Value), Convert.ToDouble(numUpDownKd.Value));
                CommunicationManager controller_connection = new CommunicationManager(this, name, controllerParameters, Channel_EP, connectionParameters);
                connections.Add(controller_connection);
                connection_selected = controller_connection;
                listBoxModules.Items.Add(name);

                // select the added module in the listbox
                listBoxModules.SelectedIndex = listBoxModules.Items.Count - 1;

                // start timers
                timerCharts.Start();
                timerUpdateGUI.Start();

                // update tree
                treeViewControllers.Nodes.Add(name, name);
                treeViewControllers.Nodes[name].Nodes.Add("Communication", "Communication");
                treeViewControllers.Nodes[name].Nodes["Communication"].Nodes.Add("ip", "IP (endpoint): " + IP_endpoint.ToString());
                treeViewControllers.Nodes[name].Nodes["Communication"].Nodes.Add("port", "Port (endpoint): " + port_endpoint.ToString());
                treeViewControllers.Nodes[name].Nodes["Communication"].Nodes.Add("port_gui", "Port (this): " + port_this.ToString());
                treeViewControllers.Nodes[name].Nodes.Add("Controller", "Controller settings");
                treeViewControllers.Nodes[name].Nodes["Controller"].Nodes.Add("Kp", "Kp: " + controllerParameters.Kp.ToString());
                treeViewControllers.Nodes[name].Nodes["Controller"].Nodes.Add("Ki", "Ki: " + controllerParameters.Ki.ToString());
                treeViewControllers.Nodes[name].Nodes["Controller"].Nodes.Add("Kd", "Ks: " + controllerParameters.Kd.ToString());
                treeViewControllers.ExpandAll();

                // update counter
                n_connections += 1;
                textBoxName.Text = "Module" + (n_connections + 1);
                log("Communication to a control module enabled");

                numericUpDown_port_receive.Value += 1;
                numericUpDown_port_send.Value += 1;
            }
        }

        private void InitialSettings()
        {
            // application directory
            file_path = Directory.GetCurrentDirectory();
            toolStripLabel.Text = "Dir: " + file_path;

            // chart settings
            Charting.ClearThresholdStripLines(residualChart);
            Charting.AddThresholdStripLine(residualChart, offset : 0, color : Color.Red);
            Charting.AddThresholdStripLine(residualChart, offset : config.anomalyDetector.cusumDelta, color: Color.Red);
            Charting.AddThresholdStripLine(residualChart, offset : -config.anomalyDetector.cusumDelta, color: Color.Red);
            
            Charting.InitializeChartSettings(dataChart, title : "");
            Charting.InitializeChartSettings(residualChart, title : "");
            Charting.InitializeChartSettings(securityChart, title : "Magnitude");
        }

        private void ParseArgs(string[] args)
        {
            if (args.Length > 1)
            {
                foreach (string arg in args)
                {
                    List<string> arg_sep = Tools.ArgsParser(arg);
                    string arg_name = arg_sep[0];

                    switch (arg_name)
                    {
                        case "config_file":
                            config_file = arg_sep[1];
                            break;
                        case "channel_controller":
                            Channel_EP = new AddressEndPoint(arg_sep[1], Convert.ToInt16(arg_sep[2]));
                            using_channel = true;
                            log("Using channel: <" + arg_sep[1] + ">, <" + arg_sep[2] + ">");
                            break;
                        case "plant_animation":
                            if (arg_sep[1] == "dwt") plant_visualization = PlantVisualization.DOUBLE_WATERTANK;
                            break;
                        case "ARG_INVALID":
                            break;
                        default:
                            MessageBox.Show("Unknown argument: " + arg_name);
                            break;
                    }
                }
            }
        }

        private void btnClearCharts_Click(object sender, EventArgs e)
        {
            foreach (var series in dataChart.Series) series.Points.Clear();
            foreach (var series in residualChart.Series) series.Points.Clear();

            foreach (string key in connection_selected.received_packets.Keys) connection_selected.received_packets[key].Clear();
            foreach (string key in connection_selected.estimates.Keys) connection_selected.estimates[key].Clear();
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

        private void trackBarReference_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                if (cbApplyRefToAll.Checked == true)
                {
                    foreach (CommunicationManager controller in connections)
                    {
                        controller.references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), trackBarReference1.Value.ToString());
                        numUpDownRef1.Value = trackBarReference1.Value;
                    }
                }
                else
                {
                    connection_selected.references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), trackBarReference1.Value.ToString());
                    numUpDownRef1.Value = trackBarReference1.Value;
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_selected.references["r2"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), trackBarReference2.Value.ToString());
                numUpDownRef2.Value = trackBarReference2.Value;
            }
        }

        private void numUpDownRef1_ValueChanged(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                if (cbApplyRefToAll.Checked == true)
                {
                    foreach (CommunicationManager controller in connections)
                    {
                        controller.references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), numUpDownRef1.Value.ToString());
                        trackBarReference1.Value = Convert.ToInt16(numUpDownRef1.Value);
                    }
                }
                else
                {
                    connection_selected.references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), numUpDownRef1.Value.ToString());
                    trackBarReference1.Value = Convert.ToInt16(numUpDownRef1.Value);
                }
            }
        }

        private void numUpDownRef2_ValueChanged(object sender, EventArgs e)
        {
            // set the reference signal value according to the numeric up-down
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_selected.references["r2"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), numUpDownRef2.Value.ToString());
                trackBarReference2.Value = Convert.ToInt16(numUpDownRef2.Value);
            }
        }

        private void numUpDownKp_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                connection_selected.ControllerParameters.Kp = Convert.ToDouble(numUpDownKp.Value);
            }
            catch { }  
        }

        private void numUpDownKi_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                connection_selected.ControllerParameters.Ki = Convert.ToDouble(numUpDownKi.Value);
                if (Convert.ToDouble(numUpDownKi.Value) <= 0.1)
                {
                    numUpDownKi.DecimalPlaces = 2;
                    numUpDownKi.Increment = Convert.ToDecimal(0.01);
                }
                else
                {
                    numUpDownKi.DecimalPlaces = 2;
                    numUpDownKi.Increment = Convert.ToDecimal(0.1);
                }
            }
            catch { }     
        }

        private void numUpDownKd_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                connection_selected.ControllerParameters.Kd = Convert.ToDouble(numUpDownKd.Value);
            }
            catch { }
        }

        public void log(string text)
        {
            this.Invoke((MethodInvoker)delegate () { debugLog += (DateTime.UtcNow + ": " + text + Environment.NewLine); });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // kill all threads when the form closes
            Environment.Exit(0);
        }

        private void btnThisIP_Click(object sender, EventArgs e)
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST   
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            textBox_ip_receive.Text = myIP;
        }

        private void setDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                file_path = folderBrowserDialog1.SelectedPath;
                toolStripLabel.Text = "Dir: " + file_path;
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataChart.SaveImage(file_path + "\\chart_HMI_main.png", ChartImageFormat.Png);
            residualChart.SaveImage(file_path + "\\chart_HMI_residual.png", ChartImageFormat.Png);
            securityChart.SaveImage(file_path + "\\chart_HMI_security.png", ChartImageFormat.Png);
        }

        private void nudHistory_ValueChanged(object sender, EventArgs e)
        {
            time_chart_window = Convert.ToInt16(nudHistory.Value);

            if (time_chart_window > 0)
            {
                dataChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
                residualChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
                securityChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart_window / 10);
            }
        }

        private void FrameGUI_Resize(object sender, EventArgs e)
        {
            Charting.ManageChartSize(this);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Charting.ManageChartSize(this);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            FormSettings form_settnings = new FormSettings();
            form_settnings.Main = this;
            form_settnings.Show();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            if (GUI_view_mode ==  GUIViewMode.CONTROL)
            {
                toggleViewMode(GUIViewMode.OBSERVE);
                GUI_view_mode = GUIViewMode.OBSERVE;
            }
            else
            {
                toggleViewMode(GUIViewMode.CONTROL);
                GUI_view_mode = GUIViewMode.CONTROL;
            }
        }

        private void toggleViewMode(string mode)
        {
            if (mode == GUIViewMode.CONTROL)
            {
                foreach (Control c in this.Controls)
                {
                    c.Visible = true; //or true.
                }

                tabControl1.Location = new Point(228, 136);
                tabControl1.Width = this.Width - 228 - pictureBox1.Width - 20;
                tabControl1.Height = this.Height - 136 - 70;
                pictureBox1.Location = new Point(tabControl1.Location.X + tabControl1.Width - 3, 157);
                pictureBox1.Height = tabControl1.Height - 23;
            }
            else if (mode == GUIViewMode.OBSERVE)
            {
                string[] visible_controls = new string[] { "tabControl1", "dataChart", "pictureBox1", "statusStrip1" };
                foreach (Control c in this.Controls)
                {
                    if (visible_controls.Contains(c.Name) == false)
                    {
                        c.Visible = false; //or true.
                    }
                }

                tabControl1.Location = new Point(3, 3);
                tabControl1.Width = this.Width - pictureBox1.Width - 25;
                tabControl1.Height = this.Height - 70;
                pictureBox1.Location = new Point(tabControl1.Location.X + tabControl1.Width - 3, 3 + 21);
                pictureBox1.Height = this.Height - 70 - 23;
            }
        }
    }
}
