using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using System.Globalization;

namespace HMI
{
    public struct Constants
    {
        // time format
        public const string FMT = "yyyy-MM-dd HH:mm:ss.fff";
        const string FMT_plot = "HH:mm:ss";

        // chart settings
        public const int n_steps = 5000;
    }

    public struct PIDparameters
    {
        public double Kp, Ki, Kd;

        public PIDparameters(double Kp, double Ki, double Kd)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
        }
    }

    public static class Helpers
    {
        public static void ManageReferencesKeys(int n_contr_states, Dictionary<string, DataContainer> references, int n_steps)
        {
            // add reference key
            if (n_contr_states >= 1)
            {
                if (references.ContainsKey("r1") == false)
                {
                    references.Add("r1", new DataContainer(n_steps));
                    references["r1"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), "0");
                }
            }
            if (n_contr_states >= 2)
            {
                if (references.ContainsKey("r2") == false)
                {
                    references.Add("r2", new DataContainer(n_steps));
                    references["r2"].InsertData(DateTime.UtcNow.ToString(Constants.FMT), "0");
                }
            }
        }

        public static void ManageEstimatesKeys(string key, Dictionary<string, DataContainer> estimates, int n_steps)
        {
            estimates.Add(key + "_hat", new DataContainer(n_steps)); 
        }


        public static void ManageTrackbars(FrameGUI GUI)
        {
            // enable or disable trackbars
            if (GUI.connection_current.n_controlled_states == 1)
            {
                GUI.trackBarReference1.Enabled = true;
                GUI.numUpDownRef1.Enabled = true;
            }
            else if (GUI.connection_current.n_controlled_states == 2)
            {
                GUI.trackBarReference1.Enabled = true;
                GUI.trackBarReference2.Enabled = true;
                GUI.numUpDownRef1.Enabled = true;
                GUI.numUpDownRef2.Enabled = true;
            }
            else
            {
                GUI.trackBarReference1.Enabled = false;
                GUI.trackBarReference2.Enabled = false;
                GUI.numUpDownRef1.Enabled = false;
                GUI.numUpDownRef2.Enabled = false;
            }
        }

        public static void UpdateTree(FrameGUI GUI, CommunicationManager controller)
        {
            // update tree
            GUI.treeViewControllers.Nodes[controller.name].Text = controller.name + " (" + controller.GetStatus() + ")";
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kp"].Text = "Kp: " + GUI.connection_current.ControllerParameters.Kp.ToString();
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Ki"].Text = "Ki: " + GUI.connection_current.ControllerParameters.Ki.ToString();
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kd"].Text = "Kd: " + GUI.connection_current.ControllerParameters.Kd.ToString();
        }

        public static void UpdateTimeLabels(FrameGUI GUI, CommunicationManager connection, string FMT)
        {
            try
            {
                DateTime time_sent = DateTime.ParseExact(connection.recieved_packages["u1"].GetLastTime(), FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = connection.last_recieved_time - time_sent;
                GUI.label_time.Text = "time now:                    " + DateTime.UtcNow.ToString("hh:mm:ss.fff tt") + "\n" +
                                  "last recieved:              " + connection.last_recieved_time.ToString("hh:mm:ss.fff tt") + "\n" +
                                  "when it was sent:        " + time_sent.ToString("hh:mm:ss.fff tt") + "\n" +
                                  "transmission duration [ms]: " + timeDiff.TotalMilliseconds;
            }
            catch { }
        }

        // drawing settings
        public static Graphics g;
        public static Pen pen_b = new Pen(Color.Black, 4);
        public static Pen pen_r = new Pen(Color.Red, 3);
        public static SolidBrush brush_b = new SolidBrush(Color.LightBlue);
        public static Bitmap bm = new Bitmap(400, 800);

        public static void DrawTanks(FrameGUI GUI, CommunicationManager connection)
        {
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);

            // map tank height in cm to pixels
            double max_height_p = GUI.pictureBox1.Height / 2.5; // max height [pixels]
            double max_inflow_width = 10;
            double max_height_r = 25; // real max height [cm]
            double cm2pix = max_height_p / max_height_r;

            // if there are no estimates, there is nothing to animate 
            if (connection.estimates.ContainsKey("yo1_hat") == false) return;
            if (connection.estimates.ContainsKey("yc1_hat") == false) return;

            // extract signals
            int u = Convert.ToInt16(Convert.ToDouble(connection.recieved_packages["u1"].GetLastValue()));
            int y1 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.estimates["yo1_hat"].GetLastValue()));
            int y2 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.estimates["yc1_hat"].GetLastValue()));

            int reference = 0;

            try { reference = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.references["r1"].GetLastValue())); }
            catch { }

            // dimensions
            double A1 = Convert.ToDouble(GUI.numUpDown_A1.Value);
            double a1 = Convert.ToDouble(GUI.numUpDown_a1a.Value);
            double A2 = Convert.ToDouble(GUI.numUpDown_A2.Value);
            double a2 = Convert.ToDouble(GUI.numUpDown_a2a.Value);

            // TANK 1 ----------------------------------------------------------------
            Point T1 = new Point(75, Convert.ToInt16(GUI.pictureBox1.Height / 2));
            Point T2 = new Point(T1.X, Convert.ToInt16(GUI.pictureBox1.Height - 20));

            // tank dimensions
            double R1_ = Math.Sqrt(A1 / Math.PI); int R1 = Convert.ToInt16(R1_ * cm2pix);
            double r1_ = Math.Sqrt(a1 / Math.PI); int r1 = Convert.ToInt16(r1_ * cm2pix);
            double h1_ = 25; int h1 = Convert.ToInt16(h1_ * cm2pix);

            // inlet
            if (u > 0)
            {
                Rectangle water_in = new Rectangle(T1.X - R1 + 5, T1.Y - h1 - 50, Convert.ToInt16(max_inflow_width * (u / 7.5)), h1 + 50);
                g.FillRectangle(brush_b, water_in);
            }

            // water 
            Rectangle water1 = new Rectangle(T1.X - R1, T1.Y - y1, 2 * R1, y1);
            g.FillRectangle(brush_b, water1);

            if (y1 > 5)
            {
                Rectangle water_fall = new Rectangle(T1.X - r1, T1.Y, 2 * r1, T2.Y - T1.Y);
                g.FillRectangle(brush_b, water_fall);
            }

            // walls
            Point w1_top = new Point(T1.X - R1, T1.Y - h1); Point w1_bot = new Point(T1.X - R1, T1.Y);
            Point w2_top = new Point(T1.X + R1, T1.Y - h1); Point w2_bot = new Point(T1.X + R1, T1.Y);
            Point wb1_l = new Point(T1.X - R1, T1.Y); Point wb1_r = new Point(T1.X - r1, T1.Y);
            Point wb2_l = new Point(T1.X + r1, T1.Y); Point wb2_r = new Point(T1.X + R1, T1.Y);
            g.DrawLine(pen_b, w1_top, w1_bot);
            g.DrawLine(pen_b, w2_top, w2_bot);
            g.DrawLine(pen_b, wb1_l, wb1_r);
            g.DrawLine(pen_b, wb2_l, wb2_r);

            // TANK 2 ---------------------------------------------------------------

            // tank dimensions
            double R2_ = Math.Sqrt(A2 / Math.PI); int R2 = Convert.ToInt16(R2_ * cm2pix);
            double r2_ = Math.Sqrt(a2 / Math.PI); int r2 = Convert.ToInt16(r2_ * cm2pix);
            double h2_ = 25; int h2 = Convert.ToInt16(h2_ * cm2pix);

            // water 
            Rectangle water2 = new Rectangle(T2.X - R2, T2.Y - y2, 2 * R2, y2);
            g.FillRectangle(brush_b, water2);

            if (y2 > 5)
            {
                Rectangle water_fall = new Rectangle(T2.X - r2, T2.Y, 2 * r2, 200);
                g.FillRectangle(brush_b, water_fall);
            }

            // walls
            w1_top = new Point(T2.X - R2, T2.Y - h2); w1_bot = new Point(T2.X - R2, T2.Y);
            w2_top = new Point(T2.X + R2, T2.Y - h2); w2_bot = new Point(T2.X + R2, T2.Y);
            wb1_l = new Point(T2.X - R2, T2.Y); wb1_r = new Point(T2.X - r2, T2.Y);
            wb2_l = new Point(T2.X + r2, T2.Y); wb2_r = new Point(T2.X + R2, T2.Y);

            g.DrawLine(pen_b, w1_top, w1_bot);
            g.DrawLine(pen_b, w2_top, w2_bot);
            g.DrawLine(pen_b, wb1_l, wb1_r);
            g.DrawLine(pen_b, wb2_l, wb2_r);

            // draw reference
            Point reference_l = new Point(T2.X - R2 - 15, T2.Y - reference); Point reference_r = new Point(T2.X - R2 - 3, T2.Y - reference);
            g.DrawLine(pen_r, reference_l, reference_r);

            GUI.pictureBox1.Image = bm;
        }

        public static void UpdateGuiControls(FrameGUI GUI, CommunicationManager connection_current)
        {
            GUI.trackBarReference1.Value = Convert.ToInt16(connection_current.references["r1"].GetLastValue());
            GUI.trackBarReference2.Value = Convert.ToInt16(connection_current.references["r2"].GetLastValue());
            GUI.numUpDownRef1.Value = Convert.ToInt16(connection_current.references["r1"].GetLastValue());
            GUI.numUpDownRef2.Value = Convert.ToInt16(connection_current.references["r2"].GetLastValue());
            GUI.numUpDownKp.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kp);
            GUI.numUpDownKi.Value = Convert.ToDecimal(connection_current.ControllerParameters.Ki);
            GUI.numUpDownKd.Value = Convert.ToDecimal(connection_current.ControllerParameters.Kd);
            GUI.textBox_ip_send.Text = connection_current.ConnectionParameters.IP;
            GUI.numericUpDown_port_send.Text = connection_current.ConnectionParameters.Port.ToString();
            GUI.numericUpDown_port_recieve.Text = connection_current.ConnectionParameters.PortThis.ToString();
        }
    }

    public static class Charting
    {
        public static void AddChartSeries(FrameGUI Main, string key, object chart)
        {
            Chart chart_ = (Chart)chart;

            // add a new time series
            if (chart_.Series.IndexOf(key) == -1)
            {
                // add to the checked list box
                if (chart_.Name == "dataChart")
                {
                    Main.clbSeries.Items.Add(key);
                    Main.clbSeries.SetItemChecked(Main.clbSeries.Items.Count - 1, true);
                }


                // add the to the chart
                chart_.Series.Add(key);
                chart_.Series[key].ChartType = SeriesChartType.Line;
                chart_.Series[key].BorderWidth = 1;

                switch (key)
                {
                    case "r1":
                        chart_.Series[key].Color = Color.Red;
                        chart_.Series[key].BorderWidth = 1;
                        break;
                    case "r2":
                        chart_.Series[key].Color = Color.OrangeRed;
                        chart_.Series[key].BorderWidth = 1;
                        break;
                    case "u1":
                        chart_.Series[key].Color = Color.Orange;
                        chart_.Series[key].BorderWidth = 1;
                        break;
                    case "u2":
                        chart_.Series[key].Color = Color.Magenta;
                        chart_.Series[key].BorderWidth = 1;
                        break;
                    case "yo1":
                        chart_.Series[key].Color = Color.Black;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yo1_hat":
                        chart_.Series[key].Color = Color.Black;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yo2":
                        chart_.Series[key].Color = Color.Gray;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yc1":
                        chart_.Series[key].Color = Color.Blue;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yc1_hat":
                        chart_.Series[key].Color = Color.Magenta;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yc2":
                        chart_.Series[key].Color = Color.Green;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yc1_res":
                        chart_.Series[key].Color = Color.Blue;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "S_yc1":
                        chart_.Series[key].Color = Color.Blue;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    default:
                        break;
                }

                // set the x-axis type to DateTime
                chart_.Series[key].XValueType = ChartValueType.DateTime;
            }
        }

        public static void ManageReferenceSeries(FrameGUI Main)
        {
            for (int i = 0; i < Main.connection_current.n_controlled_states; i++)
            {
                if (Main.dataChart.Series.IndexOf("r" + (i + 1).ToString()) == -1) AddChartSeries(Main, "r" + (i + 1).ToString(), Main.dataChart);
            }
        }

        public static void UpdateChartAxes(Chart chart, int chart_history)
        {
            chart.ChartAreas[0].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
            chart.ChartAreas[0].AxisX.Maximum = DateTime.UtcNow.ToOADate();
        }

        public static void ChangeYScale(object chart, string verbose)
        {
            bool points_exist = false;
            double max = Double.MinValue;
            double min = Double.MaxValue;

            Chart tmpChart = (Chart)chart;

            double leftLimit = tmpChart.ChartAreas[0].AxisX.Minimum;
            double rightLimit = tmpChart.ChartAreas[0].AxisX.Maximum;

            for (int s = 0; s < tmpChart.Series.Count(); s++)
            {
                foreach (DataPoint dp in tmpChart.Series[s].Points)
                {
                    if (dp.XValue >= leftLimit && dp.XValue <= rightLimit)
                    {
                        min = Math.Min(min, dp.YValues[0]);
                        max = Math.Max(max, dp.YValues[0]);
                        points_exist = true;
                    }
                }
            }

            if (points_exist == true && (max > min))
            {
                if (verbose == "round")
                {
                    tmpChart.ChartAreas[0].AxisY.Maximum = Math.Max(Math.Ceiling(max), 0);
                    tmpChart.ChartAreas[0].AxisY.Minimum = Math.Min(Math.Floor(min), 0);
                    tmpChart.ChartAreas[0].AxisY.Interval = 1;
                }
                else if (verbose == "deci")
                {
                    tmpChart.ChartAreas[0].AxisY.Maximum = Math.Max(Math.Ceiling(max * 10) / 10, 0);
                    tmpChart.ChartAreas[0].AxisY.Minimum = Math.Min(Math.Floor(min * 10) / 10, 0);
                }
            }
        }

        public static void AddThresholdStripLine(Chart chart, double offset, Color color)
        {
            StripLine stripLine3 = new StripLine();

            // Set threshold line so that it is only shown once
            stripLine3.Interval = 0;

            // Set the threshold line to be drawn at the calculated mean of the first series
            stripLine3.IntervalOffset = offset;

            stripLine3.BackColor = color;
            stripLine3.StripWidth = 0.001;

            // Add strip line to the chart
            chart.ChartAreas[0].AxisY.StripLines.Add(stripLine3);
        }

        public static void ChartSettings(Chart chart, string title)
        {
            chart.ChartAreas[0].AxisX.Title = "Time";
            chart.ChartAreas[0].AxisY.Title = title;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss";
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            chart.ChartAreas[0].AxisX.Interval = 5;
            chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);
        }
    }

}

//this.Invoke((MethodInvoker)delegate ()
//{

//});