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

namespace HMI
{
    public partial class FrameGUI : Form
    {
        // container for controller module connections
        List<CommunicationManager> connections = new List<CommunicationManager>();
        public CommunicationManager connection_current;
        public int n_connections = 0; // connection count

        // chart settings
        public int time_chart = 60; // chart view time [s]

        // folder setting for chart image save
        public string folderName = "";

        // canal flag
        public bool usingCanal = false;
        AddressEndPoint canalEP = new AddressEndPoint();

        // tank dimensions
        public TankDimensions tankDimensions = new TankDimensions(0,0,0,0);

        // debug log
        public string debugLog = "";

        // control/obsrver mode
        public bool control_mode = true;

        public FrameGUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // two (3) command line arguments corresponds to using the canal
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 3)
            {
                canalEP = new AddressEndPoint(args[1], Convert.ToInt16(args[2]));
                usingCanal = true;
                log("Using canal: <" + args[1] + ">, <" + args[2] + ">");
            }
            else
            {
                log("Direct module communication (no canal)");
            }


            // folder and chart settings
            InitalSetting();

            // load saved settings
            double A1 = Properties.Settings.Default.BA1;
            double a1 = Properties.Settings.Default.SA1;
            double A2 = Properties.Settings.Default.BA2;
            double a2 = Properties.Settings.Default.SA2;
            tankDimensions = new TankDimensions(A1, a1, A2, a2);
            log("Settings loaded");

            toggleControlMode();
        }

        private void timerCharts_Tick(object sender, EventArgs e)
        {
            if (connection_current.is_connected_to_process == true)
            {
                // add the correct number of reference series
                Charting.ManageReferenceSeries(this);

                // update charts
                UpdateChart(connection_current.references, ""); // reference
                UpdateChart(connection_current.recieved_packages, ""); // states
                UpdateChart(connection_current.estimates, ""); // kalman filter estimates

                // update time axis minimum and maximum
                Charting.UpdateChartAxes(dataChart, time_chart);
                Charting.UpdateChartAxes(residualChart, time_chart);
                Charting.UpdateChartAxes(securityChart, time_chart);
            }

            // update time labels
            Helpers.UpdateTimeLabels(this, connection_current, Constants.FMT);

            // draw simulation
            Helpers.DrawTanks(this, connection_current);
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

                if (setting == "load_all")
                    index_start = 0; // draw all data points
                else
                    index_start = dict[key].time.Length - 1; // draw the last data point

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
                            securityChart.Series["Sec_" + key].Points.AddXY(time.ToOADate(), connection_current.kalman_filter.security_metric);
                        }
                    }
                }

                // remove old data points
                if (dataChart.Series[key].Points.Count > Constants.n_steps)
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
                connection_current = connections[listBoxModules.SelectedIndex];

                // enable or disable reference track bars
                Helpers.ManageTrackbars(this);

                // clear charts
                dataChart.Series.Clear();
                residualChart.Series.Clear();

                // clear checkbox series
                clbSeries.Items.Clear();

                // refresh the chart
                UpdateChart(connection_current.references, "load_all"); // reference
                UpdateChart(connection_current.recieved_packages, "load_all"); // states
                UpdateChart(connection_current.estimates, "load_all"); // kalman filter estimates

                // scale y-axis for the charts
                Charting.ChangeYScale(dataChart, treshold_interval : "one", grid_interval : "one");
                Charting.ChangeYScale(residualChart, treshold_interval: "one", grid_interval: "one");

                // update GUI values according to the connected controller
                Helpers.UpdateGuiControls(this, connection_current);

                // select the corresponding item in the treeview
                treeViewControllers.SelectedNode = treeViewControllers.Nodes[connection_current.name];
            }
            catch { }
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
            string IP_this = textBox_ip_recieve.Text;
            int port_endpoint = Convert.ToInt16(numericUpDown_port_send.Value);
            int port_this = Convert.ToInt16(numericUpDown_port_recieve.Value);

            // look for name and port conflicts with present allowed traffics
            bool already_exists = false;
            foreach (CommunicationManager controller in connections)
            {
                if ((controller.ConnectionParameters.PortThis == port_this && controller.ConnectionParameters.Port == port_endpoint) || controller.name == name)
                {
                    already_exists = true;
                }
            }

            if (already_exists == true)
            {
                log("Error: name or ip:port pair already in use");
            }
            else
            {
                // connection parameters
                ConnectionParameters connectionParameters = new ConnectionParameters(IP_endpoint, port_endpoint, port_this);

                // create and add controller
                PIDparameters controllerParameters = new PIDparameters(Convert.ToDouble(numUpDownKp.Value), Convert.ToDouble(numUpDownKi.Value), Convert.ToDouble(numUpDownKd.Value));
                CommunicationManager controller_connection = new CommunicationManager(this, name, controllerParameters, canalEP, connectionParameters);
                connections.Add(controller_connection);
                connection_current = controller_connection;
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
            }
        }

        private void timerUpdateGUI_Tick(object sender, EventArgs e)
        {
            // update tree information
            Helpers.UpdateTree(this, connection_current);

            // enable or disable reference track bars
            Helpers.ManageTrackbars(this);

            // scale y-axis for the charts
            Charting.ChangeYScale(dataChart, treshold_interval: "one", grid_interval: "one");
            Charting.ChangeYScale(residualChart, treshold_interval: "one", grid_interval: "one");
            Charting.ChangeYScale(securityChart, treshold_interval: "one", grid_interval: "adaptive");

            labelSecurity.Text = "Security metric: " + Math.Round(connection_current.kalman_filter.security_metric, 1) + Environment.NewLine +
                                 "Status: " + connection_current.kalman_filter.security_status;
        }

        private void InitalSetting()
        {
            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripLabel.Text = "Dir: " + folderName;

            Charting.AddThresholdStripLine(residualChart, 0, Color.Red);
            Charting.AddThresholdStripLine(residualChart, 0.2, Color.Red);
            Charting.AddThresholdStripLine(residualChart, -0.2, Color.Red);

            // chart settings
            Charting.ChartSettings(dataChart, "");
            Charting.ChartSettings(residualChart, "");
            Charting.ChartSettings(securityChart, "Magnitude");
        }

        private void btnClearCharts_Click(object sender, EventArgs e)
        {
            foreach (var series in dataChart.Series) series.Points.Clear();
            foreach (var series in residualChart.Series) series.Points.Clear();
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
                connection_current.references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), trackBarReference1.Value.ToString());
                numUpDownRef1.Value = trackBarReference1.Value;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.references["r2"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), trackBarReference2.Value.ToString());
                numUpDownRef2.Value = trackBarReference2.Value;
            }
        }

        private void numUpDownKp_ValueChanged(object sender, EventArgs e)
        {
            connection_current.ControllerParameters.Kp = Convert.ToDouble(numUpDownKp.Value);
        }

        private void numUpDownKi_ValueChanged(object sender, EventArgs e)
        {
            connection_current.ControllerParameters.Ki = Convert.ToDouble(numUpDownKi.Value);
        }

        private void numUpDownKd_ValueChanged(object sender, EventArgs e)
        {
            connection_current.ControllerParameters.Kd = Convert.ToDouble(numUpDownKd.Value);
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
            textBox_ip_recieve.Text = myIP;
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

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataChart.SaveImage(folderName + "\\chart_HMI_main.png", ChartImageFormat.Png);
            residualChart.SaveImage(folderName + "\\chart_HMI_residual.png", ChartImageFormat.Png);
            securityChart.SaveImage(folderName + "\\chart_HMI_security.png", ChartImageFormat.Png);
        }

        private void numUpDownRef1_ValueChanged(object sender, EventArgs e)
        {
            // set the reference signal value according to the numeric up-down
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), numUpDownRef1.Value.ToString());
                trackBarReference1.Value = Convert.ToInt16(numUpDownRef1.Value);
            }
        }

        private void numUpDownRef2_ValueChanged(object sender, EventArgs e)
        {
            // set the reference signal value according to the numeric up-down
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.references["r2"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), numUpDownRef2.Value.ToString());
                trackBarReference2.Value = Convert.ToInt16(numUpDownRef2.Value);
            }
        }

        private void nudHistory_ValueChanged(object sender, EventArgs e)
        {
            time_chart = Convert.ToInt16(nudHistory.Value);

            if (time_chart > 0)
            {
                dataChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart / 10);
                residualChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart / 10);
                securityChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(time_chart / 10);
            }
        }

        private void FrameGUI_Resize(object sender, EventArgs e)
        {
            ManageChartSize();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ManageChartSize();
        }

        private void ManageChartSize()
        {
            try
            {
                int y_start = residualChart.Location.Y;
                int height_total = tabControl1.Height - y_start;
                residualChart.Height = height_total / 2 - y_start;
                securityChart.Location = new Point(6, +y_start + height_total / 2);
                securityChart.Height = height_total / 2 - y_start;
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormSettings form_settnings = new FormSettings();
            form_settnings.Main = this;
            form_settnings.Show();

        }

        private void toggleControlMode()
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

        private void toggleObserverMode()
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

        private void clbSeries_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
