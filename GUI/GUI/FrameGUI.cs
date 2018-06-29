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

namespace GUI
{
    public partial class FrameGUI : Form
    {
        // container for controller module connections
        List<ControllerConnection> connections = new List<ControllerConnection>();
        public ControllerConnection connection_current;
        public int n_connections = 0; // connection count

        // chart settings
        public int chart_history = 60; // chart view time [s]

        // folder setting for chart image save
        public string folderName = "";

        public FrameGUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // folder and chart settings
            InitalSetting();
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            if (connection_current.is_connected_to_process == true)
            {
                // add the correct number of reference series
                Helpers.ManageReferenceSeries(this);

                // update charts
                UpdateChartGraph(connection_current.references, ""); // reference
                UpdateChartGraph(connection_current.recieved_packages, ""); // states
                UpdateChartGraph(connection_current.estimates, ""); // kalman filter estimates

                // update time axis minimum and maximum
                Helpers.UpdateChartAxes(dataChart, chart_history);
                Helpers.UpdateChartAxes(residualChart, chart_history);

                // update time labels
                Helpers.UpdateTimeLabels(this, connection_current, Constants.FMT);

                // draw simulation
                Helpers.DrawTanks(this, connection_current);
            }
        }

        private void UpdateChartGraph(Dictionary<string, DataContainer> dict, string setting)
        {
            // get all keys from the current dictionary
            var keys = dict.Keys.ToList();

            // update the graphs for each key
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (dataChart.Series.IndexOf(key) == -1) Helpers.AddChartSeries(key, dataChart);

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
                        if (dict[key].hasResidual == true)
                        {
                            // if residual series does not exist, add it
                            if (residualChart.Series.IndexOf(key + "_res") == -1)  Helpers.AddChartSeries(key + "_res", residualChart);

                            residualChart.Series[key + "_res"].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].residual[i]));
                        }
                    }
                }

                // remove old data points
                if (dataChart.Series[key].Points.Count > Constants.n_steps)
                {
                    dataChart.Series[key].Points.RemoveAt(0);
                    if (dict[key].hasResidual) residualChart.Series[key + "_res"].Points.RemoveAt(0);
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
                UpdateChartGraph(connection_current.references, "load_all"); // reference
                UpdateChartGraph(connection_current.recieved_packages, "load_all"); // states
                UpdateChartGraph(connection_current.estimates, "load_all"); // kalman filter estimates

                // scale y-axis for the charts
                Helpers.ChangeYScale(dataChart, "data_chart");
                Helpers.ChangeYScale(residualChart, "residual_chart");

                // update GUI values according to the connected controller
                trackBarReference1.Value = Convert.ToInt16(connection_current.references["r1"].GetLastValue());
                trackBarReference2.Value = Convert.ToInt16(connection_current.references["r2"].GetLastValue());
                numUpDownRef1.Value = Convert.ToInt16(connection_current.references["r1"].GetLastValue());
                numUpDownRef2.Value = Convert.ToInt16(connection_current.references["r2"].GetLastValue());
                numUpDownKp.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kp);
                numUpDownKi.Value = Convert.ToDecimal(connection_current.ControllerParameters.Ki);
                numUpDownKd.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kd);
                textBox_ip_send.Text = connection_current.ConnectionParameters.ip_endpoint;
                textBox_ip_recieve.Text = connection_current.ConnectionParameters.ip_this;
                numericUpDown_port_send.Text = connection_current.ConnectionParameters.port_endpoint.ToString();
                numericUpDown_port_recieve.Text = connection_current.ConnectionParameters.port_this.ToString();

                // select the corresponding item in the treeview
                treeViewControllers.SelectedNode = treeViewControllers.Nodes[connection_current.name];
            }
            catch {}
        }

        private void btnAllowConnection_Click_1(object sender, EventArgs e)
        {
            // enable parameter settings
            numUpDownKp.Enabled = true;
            numUpDownKi.Enabled = true;
            numUpDownKd.Enabled = true;

            // controller name and corresponding ip:port pairs
            string name = textBoxName.Text;
            string IP_this = textBox_ip_recieve.Text;
            int port_this = Convert.ToInt16(numericUpDown_port_recieve.Value);
            string IP_endpoint = textBox_ip_send.Text;
            int port_endpoint = Convert.ToInt16(numericUpDown_port_send.Value);

            // look for name and port conflicts with present allowed traffics
            bool already_exists = false;
            foreach (ControllerConnection controller in connections)
            {
                if ((controller.ConnectionParameters.port_this == port_this && 
                    controller.ConnectionParameters.port_endpoint == port_endpoint) ||
                    controller.name == name)
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
                ConnectionParameters ConnParams = new ConnectionParameters(IP_this, IP_endpoint, port_this, port_endpoint);

                // create and add controller
                PIDparameters ControllerParameters = new PIDparameters(Convert.ToDouble(numUpDownKp.Value), Convert.ToDouble(numUpDownKi.Value), Convert.ToDouble(numUpDownKd.Value));
                ControllerConnection PID = new ControllerConnection(this, name, ControllerParameters, ConnParams);
                connections.Add(PID);
                connection_current = PID;
                listBoxModules.Items.Add(name);

                // select the added module
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
                treeViewControllers.Nodes[name].Nodes["Controller"].Nodes.Add("Kp", "Kp: " + ControllerParameters.Kp.ToString());
                treeViewControllers.Nodes[name].Nodes["Controller"].Nodes.Add("Ki", "Ki: " + ControllerParameters.Ki.ToString());
                treeViewControllers.Nodes[name].Nodes["Controller"].Nodes.Add("Kd", "Ks: " + ControllerParameters.Kd.ToString());
                treeViewControllers.ExpandAll();

                // update counter
                n_connections += 1;
                textBoxName.Text = "Module" + (n_connections + 1);
            }
        }

        private void timerTree_Tick(object sender, EventArgs e)
        {
            // update tree information
            Helpers.UpdateTree(this, connection_current);

            // enable or disable reference track bars
            Helpers.ManageTrackbars(this);

            // scale y-axis for the charts
            Helpers.ChangeYScale(dataChart, "data_chart");
            Helpers.ChangeYScale(residualChart, "residual_chart");
        }

        private void InitalSetting()
        {
            // application directory
            folderName = Directory.GetCurrentDirectory();
            toolStripStatusLabel1.Text = "Dir: " + folderName;

            // chart settings
            dataChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            dataChart.ChartAreas["ChartArea1"].AxisY.Title = "Magnitude";
            dataChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            dataChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            dataChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            dataChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);

            residualChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            residualChart.ChartAreas["ChartArea1"].AxisY.Title = "Magnitude";
            residualChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            residualChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            residualChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            residualChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var series in dataChart.Series) series.Points.Clear();
            foreach (var series in residualChart.Series) series.Points.Clear();
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

        private void button_thisIP_Click(object sender, EventArgs e)
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            chart_history = Convert.ToInt16(numericUpDownHistory.Value);

            if (chart_history > 0)
            {
                dataChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(chart_history / 10);
                residualChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(chart_history / 10);
            }
        }
    }
}
