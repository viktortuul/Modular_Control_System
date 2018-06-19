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

        // folder setting for chart image save
        public string folderName = "";

        // chart settings
        public static int n_steps = 1000;
        public int chart_history = 60;

        // connection count
        public int n_connections = 0;

        // container for controller module connections
        List<ControllerConnection> connections = new List<ControllerConnection>();
        ControllerConnection connection_current;

        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";
        const string FMT_plot = "HH:mm:ss";

        // drawing
        Graphics g;
        Pen pen_b = new Pen(Color.Black, 4);
        Pen pen_r = new Pen(Color.Red, 3);
        SolidBrush brush_b = new SolidBrush(Color.LightBlue);
        Bitmap bm = new Bitmap(600, 600);

        public FrameGUI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            folderName = Directory.GetCurrentDirectory();

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
            residualChart.Series.Add("res1");
            residualChart.Series["res1"].XValueType = ChartValueType.DateTime;
            residualChart.Series["res1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            residualChart.Series["res1"].BorderWidth = 2;
            residualChart.Series["res1"].Color = Color.Red;

            toolStripStatusLabel1.Text = "Dir: " + folderName;
        }

        private void timerChart_Tick(object sender, EventArgs e)
        {
            if (connection_current.is_connected_to_process == true)
            {
                // add the correct number of reference series
                addReferenceSeries();

                // clear residual chart

                // refresh the chart
                UpdateChart(connection_current.references); // reference
                UpdateChart(connection_current.recieved_packages); // states
                UpdateChart(connection_current.estimates); // kalman filter estimates

                // update time axis minimum and maximum
                dataChart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
                dataChart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();
                residualChart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
                residualChart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();

                // update time labels
                UpdateTimeLabels(connection_current);

                // draw simulation
                DrawTanks(connection_current);
            }
        }

        private void UpdateChart(Dictionary<string, DataContainer> dict)
        {
            // get all keys from the current connection
            var keys = dict.Keys.ToList();

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (dataChart.Series.IndexOf(key) == -1)
                {
                    AddChartSeries(key);
                }

                int i = dict[key].time.Length - 1;
                if (dict[key].time[i] != null)
                {
                    DateTime time = DateTime.ParseExact(dict[key].time[i], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    dataChart.Series[key].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].value[i]));

                    // plot residuals for flagged states
                    if (dict[key].hasResidual == true)
                    {
                        residualChart.Series["res1"].Points.AddXY(time.ToOADate(), Convert.ToDouble(dict[key].residual[i]));
                    }
                }

                // remove old data points
                if (dataChart.Series[key].Points.Count > 1000 * 5)
                {
                    dataChart.Series[key].Points.RemoveAt(0);
                    residualChart.Series["res1"].Points.RemoveAt(0);
                }
            }
        }

        public void UpdateTimeLabels(ControllerConnection connection)
        {
            try
            {
                DateTime time_sent = DateTime.ParseExact(connection.recieved_packages["u1"].GetLastTime(), FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = connection.last_recieved_time - time_sent;
                label_time.Text = "time now:                    " + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + "\n" +
                                  "last recieved:              " + connection.last_recieved_time.ToString("hh:mm:ss.fff tt") + "\n" +
                                  "when it was sent:        " + time_sent.ToString("hh:mm:ss.fff tt") + "\n" +
                                  "transmission duration [ms]: " + timeDiff.TotalMilliseconds;
            }
            catch { }
        }

        private void listBoxControllers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // specify the new connection
                connection_current = connections[listBoxModules.SelectedIndex];

                // clear and add keys to the new chart
                dataChart.Series.Clear();
                var keys = connection_current.recieved_packages.Keys.ToList();

                for (int i = 0; i < connection_current.n_contr_states; i++) AddChartSeries("r" + (i+1).ToString());              

                // enable or disable reference track bars
                ManageTrackbars();

                // add all chart series
                foreach (string key in keys) AddChartSeries(key);

                // update GUI values according to the connected controller
                trackBarReference1.Value = Convert.ToInt16(connection_current.references["r1"].GetLastValue());
                trackBarReference2.Value = Convert.ToInt16(connection_current.references["r2"].GetLastValue());
                numUpDownKp.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kp);
                numUpDownKi.Value = Convert.ToDecimal(connection_current.ControllerParameters.Ki);
                numUpDownKd.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kd);
                textBox_ip_send.Text = connection_current.ConnectionParameters.ip_send;
                textBox_ip_recieve.Text = connection_current.ConnectionParameters.ip_recieve;
                numericUpDown_port_send.Text = connection_current.ConnectionParameters.port_send.ToString();
                numericUpDown_port_recieve.Text = connection_current.ConnectionParameters.port_recieve.ToString();

                // select the corresponding item in the treeview
                treeViewControllers.SelectedNode = treeViewControllers.Nodes[connection_current.name];
            }
            catch (Exception ex)
            {
                log(ex.ToString());
            }
        }

        private void btnAllowConnection_Click_1(object sender, EventArgs e)
        {
            // enable parameter settings
            numUpDownKp.Enabled = true; numUpDownKi.Enabled = true; numUpDownKd.Enabled = true;
            
            // controller name and corresponding ip:port pairs
            string label = textBoxName.Text;
            string ip_send = textBox_ip_send.Text;
            string ip_recieve = textBox_ip_recieve.Text;
            int port_recieve = Convert.ToInt16(numericUpDown_port_recieve.Value);
            int port_send = Convert.ToInt16(numericUpDown_port_send.Value);

            // look for name and port conflicts with present allowed traffics
            bool already_exists = false;

            foreach (ControllerConnection controller in connections)
            {
                if ((controller.ConnectionParameters.port_recieve == port_recieve && controller.ConnectionParameters.port_send == port_send) ||
                    controller.name == label)
                {
                    already_exists = true;
                }
            }

            if (already_exists == true) log("Error: name or ip:port pair already in use");
            else
            {
                // connection parameters
                ConnectionParameters ConnParams = new ConnectionParameters(ip_recieve, ip_send, port_recieve, port_send);

                // create and add controller
                PIDparameters ControllerParameters = new PIDparameters(Convert.ToDouble(numUpDownKp.Value), Convert.ToDouble(numUpDownKi.Value), Convert.ToDouble(numUpDownKd.Value));
                ControllerConnection PID = new ControllerConnection(this, label, ControllerParameters, ConnParams);
                connections.Add(PID);
                connection_current = PID;
                listBoxModules.Items.Add(label);

                // select the added module
                listBoxModules.SelectedIndex = listBoxModules.Items.Count -1;

                timerChart.Start();
                timerTree.Start();

                // create a root node
                treeViewControllers.Nodes.Add(label, label);
                treeViewControllers.Nodes[label].Nodes.Add("Communication", "Communication");
                treeViewControllers.Nodes[label].Nodes["Communication"].Nodes.Add("ip", "IP (controller): " + ip_send.ToString());
                treeViewControllers.Nodes[label].Nodes["Communication"].Nodes.Add("port", "Port (controller): " + port_send.ToString());
                treeViewControllers.Nodes[label].Nodes["Communication"].Nodes.Add("port_gui", "Port (this): " + port_recieve.ToString());
                treeViewControllers.Nodes[label].Nodes.Add("Controller", "Controller settings");
                treeViewControllers.Nodes[label].Nodes["Controller"].Nodes.Add("Kp", "Kp: " + ControllerParameters.Kp.ToString());
                treeViewControllers.Nodes[label].Nodes["Controller"].Nodes.Add("Ki", "Ki: " + ControllerParameters.Ki.ToString());
                treeViewControllers.Nodes[label].Nodes["Controller"].Nodes.Add("Kd", "Ks: " + ControllerParameters.Kd.ToString());
                treeViewControllers.ExpandAll();

                // update counter
                n_connections += 1;
                textBoxName.Text = "Module" + (n_connections + 1);
            }
        }

        public void DrawTanks(ControllerConnection connection)
        {
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);

            // map tank height in cm to pixels
            double max_height_p = 200; // max height [pixels]
            double max_inflow_width = 10;
            double max_height_r = 25; // real max height [cm]
            double cm2pix = max_height_p / max_height_r;

            // extract signals
            int u = Convert.ToInt16(Convert.ToDouble(connection.recieved_packages["u1"].GetLastValue()));
            int y1 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.recieved_packages["yo1"].GetLastValue()));
            int y2 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.recieved_packages["yc1"].GetLastValue()));
            int reference = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.references["r1"].GetLastValue()));

            //
            double A1 = Convert.ToDouble(numUpDown_A1.Value);
            double a1 = Convert.ToDouble(numUpDown_a1a.Value);
            double A2 = Convert.ToDouble(numUpDown_A2.Value);
            double a2 = Convert.ToDouble(numUpDown_a2a.Value);

            // TANK 1 ----------------------------------------------------------------
            Point T = new Point(75, 230);

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
            T = new Point(T.X, T.Y + 250);

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

            // draw reference
            Point reference_l = new Point(T.X - R2 - 15, T.Y - reference); Point reference_r = new Point(T.X - R2 - 3, T.Y - reference);
            g.DrawLine(pen_r, reference_l, reference_r);

            pictureBox1.Image = bm;
        }

        // ------------- helpers -----------------

        private void addReferenceSeries()
        {
            for (int i = 0; i < connection_current.n_contr_states; i++)
            {
                if ((dataChart.Series.IndexOf("r" + (i + 1).ToString()) == -1)) AddChartSeries("r" + (i + 1).ToString());
            }
        }

        public void UpdateTree(ControllerConnection controller)
        {
            // update tree
            this.Invoke((MethodInvoker)delegate ()
            {
                treeViewControllers.Nodes[controller.name].Text = controller.name + " (" + controller.GetStatus() + ")";
                treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kp"].Text = "Kp: " + connection_current.ControllerParameters.Kp.ToString();
                treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Ki"].Text = "Ki: " + connection_current.ControllerParameters.Ki.ToString();
                treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kd"].Text = "Kd: " + connection_current.ControllerParameters.Kd.ToString();
            });
        }

        public void AddChartSeries(string key)
        {
            // add a new time series
            this.Invoke((MethodInvoker)delegate ()
            {
                if (dataChart.Series.IndexOf(key) == -1)
                {
                    dataChart.Series.Add(key);
                    dataChart.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    dataChart.Series[key].BorderWidth = 2;

                    switch (key)
                    {
                        case "r1":
                            dataChart.Series[key].Color = Color.Blue;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "r2":
                            dataChart.Series[key].Color = Color.Green;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "u1":
                            dataChart.Series[key].Color = Color.Orange;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "u2":
                            dataChart.Series[key].Color = Color.Magenta;
                            dataChart.Series[key].BorderWidth = 1;
                            break;
                        case "yo1":
                            dataChart.Series[key].Color = Color.Black;
                            dataChart.Series[key].BorderWidth = 2;
                            break;
                        case "yo2":
                            dataChart.Series[key].Color = Color.Gray;
                            dataChart.Series[key].BorderWidth = 2;
                            break;
                        case "yc1":
                            dataChart.Series[key].Color = Color.Blue;
                            dataChart.Series[key].BorderWidth = 3;
                            break;
                        case "yc2":
                            dataChart.Series[key].Color = Color.Green;
                            dataChart.Series[key].BorderWidth = 3;
                            break;
                        default:
                            break;
                    }

                    // set the x-axis type to DateTime
                    dataChart.Series[key].XValueType = ChartValueType.DateTime;
                }
            });
        }

        public void ManageTrackbars()
        {
            // enable or disable trackbars
            if (connection_current.n_contr_states == 1)
            {
                trackBarReference1.Enabled = true;
                numUpDownRef1.Enabled = true;
            }
            else if (connection_current.n_contr_states == 2)
            {
                trackBarReference1.Enabled = true;
                trackBarReference2.Enabled = true;
                numUpDownRef1.Enabled = true;
                numUpDownRef2.Enabled = true;
            }
            else
            {
                trackBarReference1.Enabled = false;
                trackBarReference2.Enabled = false;
                numUpDownRef1.Enabled = false;
                numUpDownRef2.Enabled = false;
            }
        }

        private void trackBarReference_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.references["r1"].InsertData(DateTime.UtcNow.ToString(FMT), trackBarReference1.Value.ToString());
                numUpDownRef1.Value = trackBarReference1.Value;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.references["r2"].InsertData(DateTime.UtcNow.ToString(FMT), trackBarReference2.Value.ToString());
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
            this.Invoke((MethodInvoker)delegate () {tbDebugLog.AppendText(DateTime.UtcNow + ": " + text + Environment.NewLine); });
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
            dataChart.SaveImage(folderName + "\\chart.png", ChartImageFormat.Png);
        }

        private void timerTree_Tick(object sender, EventArgs e)
        {
            // update tree information
            UpdateTree(connection_current);

            // enable or disable reference track bars
            ManageTrackbars();
        }

        private void numUpDownRef1_ValueChanged(object sender, EventArgs e)
        {
            // set the reference signal value according to the numeric up-down
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.references["r1"].InsertData(DateTime.UtcNow.ToString(FMT), numUpDownRef1.Value.ToString());
                trackBarReference1.Value = Convert.ToInt16(numUpDownRef1.Value);
            }
        }

        private void numUpDownRef2_ValueChanged(object sender, EventArgs e)
        {
            // set the reference signal value according to the numeric up-down
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.references["r2"].InsertData(DateTime.UtcNow.ToString(FMT), numUpDownRef2.Value.ToString());
                trackBarReference2.Value = Convert.ToInt16(numUpDownRef2.Value);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            chart_history = Convert.ToInt16(numericUpDown1.Value);

            if (chart_history > 0)
            {
                dataChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(chart_history / 10);
                residualChart.ChartAreas["ChartArea1"].AxisX.Interval = Convert.ToInt16(chart_history / 10);
            }
        }
    }
}
