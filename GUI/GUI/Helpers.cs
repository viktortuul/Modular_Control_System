﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using System.Globalization;

namespace GUI
{
    public static class Helpers
    {
        public static void AddChartSeries(string key, object chart)
        {
            Chart chart_ = (Chart)chart;

            // add a new time series
            if (chart_.Series.IndexOf(key) == -1)
            {
                chart_.Series.Add(key);
                chart_.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart_.Series[key].BorderWidth = 2;

                switch (key)
                {
                    case "r1":
                        chart_.Series[key].Color = Color.Blue;
                        chart_.Series[key].BorderWidth = 1;
                        break;
                    case "r2":
                        chart_.Series[key].Color = Color.Green;
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
                    case "yo2":
                        chart_.Series[key].Color = Color.Gray;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yc1":
                        chart_.Series[key].Color = Color.Blue;
                        chart_.Series[key].BorderWidth = 3;
                        break;
                    case "yc2":
                        chart_.Series[key].Color = Color.Green;
                        chart_.Series[key].BorderWidth = 3;
                        break;
                    default:
                        break;
                }

                // set the x-axis type to DateTime
                chart_.Series[key].XValueType = ChartValueType.DateTime;
            }
        }

        public static void ManageTrackbars(FrameGUI GUI)
        {
            // enable or disable trackbars
            if (GUI.connection_current.n_contr_states == 1)
            {
                GUI.trackBarReference1.Enabled = true;
                GUI.numUpDownRef1.Enabled = true;
            }
            else if (GUI.connection_current.n_contr_states == 2)
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

        public static void AddReferenceSeries(FrameGUI GUI)
        {
            for (int i = 0; i < GUI.connection_current.n_contr_states; i++)
            {
                if (GUI.dataChart.Series.IndexOf("r" + (i + 1).ToString()) == -1)
                {
                    Helpers.AddChartSeries("r" + (i + 1).ToString(), GUI.dataChart);
                }
            }
        }

        public static void UpdateTree(FrameGUI GUI, ControllerConnection controller)
        {
            // update tree
            GUI.treeViewControllers.Nodes[controller.name].Text = controller.name + " (" + controller.GetStatus() + ")";
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kp"].Text = "Kp: " + GUI.connection_current.ControllerParameters.Kp.ToString();
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Ki"].Text = "Ki: " + GUI.connection_current.ControllerParameters.Ki.ToString();
            GUI.treeViewControllers.Nodes[controller.name].Nodes["Controller"].Nodes["Kd"].Text = "Kd: " + GUI.connection_current.ControllerParameters.Kd.ToString();
        }

        public static void UpdateChartAxes(Chart chart, int chart_history)
        {
            chart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
            chart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();
        }

        public static void UpdateTimeLabels(FrameGUI GUI, ControllerConnection connection, string FMT)
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

        public static void DrawTanks(FrameGUI GUI, ControllerConnection connection)
        {
            GUI.g = Graphics.FromImage(GUI.bm);
            GUI.g.Clear(Color.White);

            // map tank height in cm to pixels
            double max_height_p = GUI.pictureBox1.Height / 2.5; // max height [pixels]
            double max_inflow_width = 10;
            double max_height_r = 25; // real max height [cm]
            double cm2pix = max_height_p / max_height_r;

            // extract signals
            int u = Convert.ToInt16(Convert.ToDouble(connection.recieved_packages["u1"].GetLastValue()));
            int y1 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.recieved_packages["yo1"].GetLastValue()));
            int y2 = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.recieved_packages["yc1"].GetLastValue()));
            int reference = Convert.ToInt16(cm2pix * Convert.ToDouble(connection.references["r1"].GetLastValue()));

            //
            double A1 = Convert.ToDouble(GUI.numUpDown_A1.Value);
            double a1 = Convert.ToDouble(GUI.numUpDown_a1a.Value);
            double A2 = Convert.ToDouble(GUI.numUpDown_A2.Value);
            double a2 = Convert.ToDouble(GUI.numUpDown_a2a.Value);

            // TANK 1 ----------------------------------------------------------------
            Point T = new Point(75, Convert.ToInt16(GUI.pictureBox1.Height / 2));

            // tank dimensions
            double R1_ = Math.Sqrt(A1 / Math.PI); int R1 = Convert.ToInt16(R1_ * cm2pix);
            double r1_ = Math.Sqrt(a1 / Math.PI); int r1 = Convert.ToInt16(r1_ * cm2pix);
            double h1_ = 25; int h1 = Convert.ToInt16(h1_ * cm2pix);

            // inlet
            if (u > 0)
            {
                Rectangle water_in = new Rectangle(T.X - R1 + 5, T.Y - h1 - 50, Convert.ToInt16(max_inflow_width * (u / 7.5)), h1 + 50);
                GUI.g.FillRectangle(GUI.brush_b, water_in);
            }

            // water 
            Rectangle water1 = new Rectangle(T.X - R1, T.Y - y1, 2 * R1, y1);
            GUI.g.FillRectangle(GUI.brush_b, water1);

            if (y1 > 0)
            {
                Rectangle water_fall = new Rectangle(T.X - r1, T.Y, 2 * r1, 250);
                GUI.g.FillRectangle(GUI.brush_b, water_fall);
            }

            // walls
            Point w1_top = new Point(T.X - R1, T.Y - h1); Point w1_bot = new Point(T.X - R1, T.Y);
            Point w2_top = new Point(T.X + R1, T.Y - h1); Point w2_bot = new Point(T.X + R1, T.Y);
            Point wb1_l = new Point(T.X - R1, T.Y); Point wb1_r = new Point(T.X - r1, T.Y);
            Point wb2_l = new Point(T.X + r1, T.Y); Point wb2_r = new Point(T.X + R1, T.Y);
            GUI.g.DrawLine(GUI.pen_b, w1_top, w1_bot);
            GUI.g.DrawLine(GUI.pen_b, w2_top, w2_bot);
            GUI.g.DrawLine(GUI.pen_b, wb1_l, wb1_r);
            GUI.g.DrawLine(GUI.pen_b, wb2_l, wb2_r);

            // TANK 2 ----------------------------------------------------------------
            T = new Point(T.X, Convert.ToInt16(GUI.pictureBox1.Height - 20));

            // tank dimensions
            double R2_ = Math.Sqrt(A2 / Math.PI); int R2 = Convert.ToInt16(R2_ * cm2pix);
            double r2_ = Math.Sqrt(a2 / Math.PI); int r2 = Convert.ToInt16(r2_ * cm2pix);
            double h2_ = 25; int h2 = Convert.ToInt16(h2_ * cm2pix);

            // water 
            Rectangle water2 = new Rectangle(T.X - R2, T.Y - y2, 2 * R2, y2);
            GUI.g.FillRectangle(GUI.brush_b, water2);

            if (y2 > 0)
            {
                Rectangle water_fall = new Rectangle(T.X - r2, T.Y, 2 * r2, 200);
                GUI.g.FillRectangle(GUI.brush_b, water_fall);
            }

            // walls
            w1_top = new Point(T.X - R2, T.Y - h2); w1_bot = new Point(T.X - R2, T.Y);
            w2_top = new Point(T.X + R2, T.Y - h2); w2_bot = new Point(T.X + R2, T.Y);
            wb1_l = new Point(T.X - R2, T.Y); wb1_r = new Point(T.X - r2, T.Y);
            wb2_l = new Point(T.X + r2, T.Y); wb2_r = new Point(T.X + R2, T.Y);
            GUI.g.DrawLine(GUI.pen_b, w1_top, w1_bot);
            GUI.g.DrawLine(GUI.pen_b, w2_top, w2_bot);
            GUI.g.DrawLine(GUI.pen_b, wb1_l, wb1_r);
            GUI.g.DrawLine(GUI.pen_b, wb2_l, wb2_r);

            // draw reference
            Point reference_l = new Point(T.X - R2 - 15, T.Y - reference); Point reference_r = new Point(T.X - R2 - 3, T.Y - reference);
            GUI.g.DrawLine(GUI.pen_r, reference_l, reference_r);

            GUI.pictureBox1.Image = GUI.bm;
        }

        public static void ChangeYScale(object chart, string verbose)
        {
            bool points_exist = false;
            double max = Double.MinValue;
            double min = Double.MaxValue;

            Chart tmpChart = (Chart)chart;

            double leftLimit = tmpChart.ChartAreas["ChartArea1"].AxisX.Minimum;
            double rightLimit = tmpChart.ChartAreas["ChartArea1"].AxisX.Maximum;

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

            if (points_exist == true)
            {
                if (verbose == "data_chart")
                {
                    tmpChart.ChartAreas["ChartArea1"].AxisY.Maximum = Math.Ceiling(max);
                    tmpChart.ChartAreas["ChartArea1"].AxisY.Minimum = Math.Floor(min);
                    tmpChart.ChartAreas["ChartArea1"].AxisY.Interval = 1;
                }
                else if (verbose == "residual_chart")
                {
                    tmpChart.ChartAreas["ChartArea1"].AxisY.Maximum = Math.Ceiling(max * 10) / 10;
                    tmpChart.ChartAreas["ChartArea1"].AxisY.Minimum = Math.Floor(min * 10) / 10;
                }
            }
        }
    }
}

//this.Invoke((MethodInvoker)delegate ()
//{

//});