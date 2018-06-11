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
        public static int n_steps = 500;

        // connection count
        public int n_connections = 0;

        // container for controller module connections
        List<ControllerConnection> connections = new List<ControllerConnection>();
        ControllerConnection connection_current;

        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

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
            dataChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Days;
            dataChart.ChartAreas["ChartArea1"].AxisX.Interval = 50;

            residualChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            residualChart.ChartAreas["ChartArea1"].AxisY.Title = "Magnitude";
            residualChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            residualChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Days;
            residualChart.ChartAreas["ChartArea1"].AxisX.Interval = 50;
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

                // push each element in the data vectors one step back
                foreach (ControllerConnection controller in connections)
                {
                    controller.PushArrays();
                }

                // refresh the chart
                UpdateChart(connection_current);

                // enable or disable reference track bars
                ManageTrackbars();

                // draw simulation
                DrawTanks(connection_current);
            }
        }

        private void UpdateChart(ControllerConnection connection)
        {
            // get all keys from the current connection
            var keys = connection.packages.Keys.ToList();
            keys.Remove("time"); // don't consider the time key

            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (dataChart.Series.IndexOf(key) == -1)
                {
                    AddChartSeries(key);
                }

                dataChart.Series[key].Points.Clear();
                residualChart.Series["res1"].Points.Clear();

                for (int i = 0; i < connection.packages[key].Length; i++)
                {
                    //dataChart.Series[key].Points.AddY(connection.packages[key][i]);                   
                    if (connection.packages["time"][i] != null)
                    {
                        DateTime time = DateTime.ParseExact(connection.packages["time"][i], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                        dataChart.Series[key].Points.AddXY(time.ToString("hh:mm:ss tt"), Convert.ToDouble(connection.packages[key][i]));
                        //TimeSpan time =  DateTime.ParseExact(connection.packages["time"][i], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal) - DateTime.UtcNow;
                        //dataChart.Series[key].Points.AddXY(Math.Round(time.TotalSeconds,1), Convert.ToDouble(connection.packages[key][i]));

                        // plot residuals
                        if (key == "yc1")
                        {
                            residualChart.Series["res1"].Points.AddXY(time.ToString("hh:mm:ss tt"), Convert.ToDouble(connection.packages[key][i]) - Convert.ToDouble(connection.references["r1"][i]));
                        }
                    }
                }
            }

            // clear and update the graphs (for the reference)
            //for (int i = 0; i < connection.n_contr_states; i++)
            //{
            //    dataChart.Series["r" + (i + 1).ToString()].Points.Clear();
            //    for (int j = 0; j < connection.r_array.GetLength(1); j++)
            //    {
            //        dataChart.Series["r" + (i + 1).ToString()].Points.AddY(connection.r_array[i, j]);
            //    }
            //}

            // get all keys from the current connection
            keys = connection.references.Keys.ToList();
            keys.Remove("time");
            // clear and update the graphs (for each key)
            foreach (string key in keys)
            {
                // if series does not exist, add it
                if (dataChart.Series.IndexOf(key) == -1)
                {
                    AddChartSeries(key);
                }

                dataChart.Series[key].Points.Clear();
                for (int i = 0; i < connection.references[key].Length; i++)
                {
                    //dataChart.Series[key].Points.AddY(connection.packages[key][i]);                   
                    if (connection.references["time"][i] != null)
                    {
                        DateTime time = DateTime.ParseExact(connection.references["time"][i], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                        dataChart.Series[key].Points.AddXY(time.ToString("hh:mm:ss tt"), Convert.ToDouble(connection.references[key][i]));
                        //TimeSpan time =  DateTime.ParseExact(connection.packages["time"][i], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal) - DateTime.UtcNow;
                        //dataChart.Series[key].Points.AddXY(Math.Round(time.TotalSeconds,1), Convert.ToDouble(connection.packages[key][i]));
                    }
                }
            }

            // update time labels
            try
            {
                DateTime time_sent = DateTime.ParseExact(connection.package_last["time"], FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
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
                var keys = connection_current.packages.Keys.ToList();
                keys.Remove("time");

                for (int i = 0; i < connection_current.n_contr_states; i++)
                {
                    AddChartSeries("r" + (i+1).ToString());
                }

                // enable or disable reference track bars
                ManageTrackbars();

                // add all chart series
                foreach (string key in keys)
                {
                    AddChartSeries(key);
                }

                // update GUI values according to the connected controller
                trackBarReference.Value = Convert.ToInt16(connection_current.r[0]);
                trackBar1.Value = Convert.ToInt16(connection_current.r[1]);
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

        private void addReferenceSeries()
        {
            for (int i = 0; i < connection_current.n_contr_states; i++)
            {
                if ((dataChart.Series.IndexOf("r" + (i + 1).ToString()) == -1)) AddChartSeries("r" + (i + 1).ToString());
            }
        }

        public void ManageTrackbars()
        {
            // enable or disable trackbars
            if (connection_current.n_contr_states == 1)
            {
                trackBarReference.Enabled = true;
            }
            else if (connection_current.n_contr_states == 2)
            {
                trackBarReference.Enabled = true;
                trackBar1.Enabled = true;
            }
            else
            {
                trackBarReference.Enabled = false;
                trackBar1.Enabled = false;
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
            log(key);
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

        private void btnAllowConnection_Click_1(object sender, EventArgs e)
        {
            // enable parameter settings
            numUpDownKp.Enabled = true;
            numUpDownKi.Enabled = true;
            numUpDownKd.Enabled = true;
            
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
                if ((controller.ConnectionParameters.port_recieve == port_recieve &&
                    controller.ConnectionParameters.port_send == port_send) ||
                    controller.name == label)
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
                ConnectionParameters ConnParams = new ConnectionParameters(ip_recieve, ip_send, port_recieve, port_send);

                // create and add controller
                PIDparameters ControllerParameters = new PIDparameters(Convert.ToDouble(numUpDownKp.Value), Convert.ToDouble(numUpDownKi.Value), Convert.ToDouble(numUpDownKd.Value));
                ControllerConnection PID_1 = new ControllerConnection(this, label, ControllerParameters, ConnParams);
                connections.Add(PID_1);
                connection_current = PID_1;
                listBoxModules.Items.Add(label);

                // if no listbox item is selected, select the top one
                if (listBoxModules.SelectedIndex == -1)
                {
                    listBoxModules.SelectedIndex = 0;
                }

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
            int u = Convert.ToInt16(Convert.ToDouble(connection.package_last["u1"]));
            int y1 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.package_last["yo1"]));
            int y2 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.package_last["yc1"]));
            int reference = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.reference_last["r1"]));

            //
            double A1 = Convert.ToDouble(numUpDown_A1.Value);
            double a1 = Convert.ToDouble(numUpDown_a1a.Value);
            double A2 = Convert.ToDouble(numUpDown_A2.Value);
            double a2 = Convert.ToDouble(numUpDown_a2a.Value);

            // TANK 1 ----------------------------------------------------------------
            Point T = new Point(150, 230);

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

        private void trackBarReference_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.reference_last["r1"] = trackBarReference.Value.ToString();
                labelReference1.Text = trackBarReference.Value.ToString();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // set the reference signal value according to the track bar
            if (connections.Count > 0 && listBoxModules.SelectedIndex != -1)
            {
                connection_current.reference_last["r2"] = trackBar1.Value.ToString();
                labelReference2.Text = trackBar1.Value.ToString();
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
            UpdateTree(connection_current);
        }
    }
}
