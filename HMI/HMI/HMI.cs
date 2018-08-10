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
        public int n_chart_history = 60; // chart view time [s]

        // folder setting for chart image save
        public string folderName = "";

        // canal flag
        public bool usingCanal = false;
        AddressEndPoint canalEP = new AddressEndPoint();

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
            }

            // folder and chart settings
            InitalSetting();
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
                Charting.UpdateChartAxes(dataChart, n_chart_history);
                Charting.UpdateChartAxes(residualChart, n_chart_history);
                Charting.UpdateChartAxes(securityChart, n_chart_history);

                // update time labels
                Helpers.UpdateTimeLabels(this, connection_current, Constants.FMT);

                // draw simulation
                Helpers.DrawTanks(this, connection_current);
            }
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
                            if (residualChart.Series.IndexOf(key + "_res") == -1) Charting.AddChartSeries(this, key + "_res", residualChart);
                            residualChart.Series[key + "_res"].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].residual[i]));

                            // if security metric series does not exist, add it
                            if (securityChart.Series.IndexOf("S_" + key) == -1) Charting.AddChartSeries(this, "S_" + key, securityChart);
                            securityChart.Series["S_" + key].Points.AddXY(time.ToOADate(), connection_current.filter.security_metric);
                        }
                    }
                }

                // remove old data points
                if (dataChart.Series[key].Points.Count > Constants.n_steps)
                {
                    dataChart.Series[key].Points.RemoveAt(0);
                    if (dict[key].has_residual)
                    {
                        residualChart.Series[key + "_res"].Points.RemoveAt(0);
                        securityChart.Series["S_" + key].Points.RemoveAt(0);
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

                // refresh the chart
                UpdateChart(connection_current.references, "load_all"); // reference
                UpdateChart(connection_current.recieved_packages, "load_all"); // states
                UpdateChart(connection_current.estimates, "load_all"); // kalman filter estimates

                // scale y-axis for the charts
                Charting.ChangeYScale(dataChart, "data_chart");
                Charting.ChangeYScale(residualChart, "residual_chart");

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
            }
        }

        private void timerUpdateGUI_Tick(object sender, EventArgs e)
        {
            // update tree information
            Helpers.UpdateTree(this, connection_current);

            // enable or disable reference track bars
            Helpers.ManageTrackbars(this);

            // scale y-axis for the charts
            Charting.ChangeYScale(dataChart, "round");
            Charting.ChangeYScale(residualChart, "deci");
            Charting.ChangeYScale(securityChart, "deci");

            labelSecurity.Text = "Security metric: " + Math.Round(connection_current.filter.security_metric, 1) + Environment.NewLine +
                                 "Status: " + connection_current.filter.security_status;
        }

        private void InitalSetting()
        {
            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripStatusLabel1.Text = "Dir: " + folderName;

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
            this.Invoke((MethodInvoker)delegate () { tbDebugLog.AppendText(DateTime.UtcNow + ": " + text + Environment.NewLine); });
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

        private void saveChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataChart.SaveImage(folderName + "\\chart.png", ChartImageFormat.Png);
        }

        private void setDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderName = folderBrowserDialog1.SelectedPath;
                toolStripStatusLabel1.Text = "Dir: " + folderName;
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            dataChart.SaveImage(folderName + "\\chart_main.png", ChartImageFormat.Png);
            residualChart.SaveImage(folderName + "\\chart_residual.png", ChartImageFormat.Png);
            securityChart.SaveImage(folderName + "\\chart_security.png", ChartImageFormat.Png);
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
            n_chart_history = Convert.ToInt16(nudHistory.Value);

            if (n_chart_history > 0)
            {
                dataChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(n_chart_history / 10);
                residualChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(n_chart_history / 10);
                securityChart.ChartAreas[0].AxisX.Interval = Convert.ToInt16(n_chart_history / 10);
            }
        }

        private void FrameGUI_Resize(object sender, EventArgs e)
        {
            try
            {
                int y_start = residualChart.Location.Y;
                int height_total = tabControl1.Height - y_start;
                residualChart.Height = height_total / 2 - y_start;
                securityChart.Location = new Point(6, + y_start + height_total / 2);
                securityChart.Height = height_total / 2 - y_start;
            }
            catch { }
        }
    }
}
