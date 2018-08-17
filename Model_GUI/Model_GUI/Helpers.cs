using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using System.Globalization;


namespace Model_GUI
{
    public struct Constants
    {
        // time format
        public const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // chart settings
        public const int n_steps = 5000;
    }

    public class DataContainer
    {
        // store the time:value pair in string arrays
        public string[] time;
        public string[] value;

        // constructor
        public DataContainer(int size)
        {
            time = new string[size];
            value = new string[size];
        }

        // instert a new time:value pair data point
        public void InsertData(string time_, string value_)
        {
            Array.Copy(time, 1, time, 0, time.Length - 1);
            time[time.Length - 1] = time_;

            Array.Copy(value, 1, value, 0, value.Length - 1);
            value[value.Length - 1] = value_;
        }

        public string GetLastTime()
        {
            return time[time.Length - 1];
        }
        public string GetLastValue()
        {
            return value[value.Length - 1];
        }
    }

    public static class Helpers
    {
        // drawing
        static Graphics g;
        static Pen pen_b = new Pen(Color.Black, 4);
        static Pen pen_r = new Pen(Color.Red, 3);
        static SolidBrush brush_b = new SolidBrush(Color.LightBlue);
        static Bitmap bm = new Bitmap(400, 800);


        public static void UpdateChartAxes(Chart chart, int chart_history)
        {
            chart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
            chart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();
        }

        public static void AddChartSeries(string key, object chart)
        {
            Chart chart_ = (Chart)chart;

            // add a new time series
            if (chart_.Series.IndexOf(key) == -1)
            {
                chart_.Series.Add(key);
                chart_.Series[key].ChartType = SeriesChartType.Line;
                chart_.Series[key].BorderWidth = 2;

                switch (key)
                {
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
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "yc2":
                        chart_.Series[key].Color = Color.Green;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    default:
                        break;
                }
                // set the x-axis type to DateTime
                chart_.Series[key].XValueType = ChartValueType.DateTime;
            }
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

            if (points_exist == true && (max > min))
            {
  
                tmpChart.ChartAreas["ChartArea1"].AxisY.Maximum = Math.Max(Math.Ceiling(max), 0);
                tmpChart.ChartAreas["ChartArea1"].AxisY.Minimum = Math.Min(Math.Floor(min), 0);
                tmpChart.ChartAreas["ChartArea1"].AxisY.Interval = 1;            
            }

        }


        public static void CheckKey(Dictionary<string, DataContainer> dict, string key, int n_steps)
        {
            // if the key doesn't exist, add it
            if (dict.ContainsKey(key) == false) dict.Add(key, new DataContainer(n_steps));
        }

        public static void UpdatePerturbationLabels(ModelGUI GUI, ModelGUI.Perturbation Disturbance)
        {

            GUI.labelDisturbance.Text = "Disturbances: \n" +
                                    Math.Round(Disturbance.value[0], 1) + "\n" +
                                    Math.Round(Disturbance.value[1], 1) + "\n" +
                                    Math.Round(Disturbance.value[2], 1) + "\n" +
                                    Math.Round(Disturbance.value[3], 1);
        }

        public static void DrawTanks(ModelGUI GUI)
        {
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);

            // map tank height in cm to pixels
            double max_height_p = GUI.pictureBox1.Height / 2.5; // max height [pixels]
            double max_inflow_width = 10;
            double max_height_r = 20; // real max height [cm]
            double cm2pix = max_height_p / max_height_r;

            // extract signals
            int u = 0;
            try { u = Convert.ToInt16(Convert.ToDouble(GUI.package_last["u1"])); } catch { };
            int y1 = Convert.ToInt16(cm2pix * Convert.ToDouble(GUI.states["yo1"].GetLastValue()));
            int y2 = Convert.ToInt16(cm2pix * Convert.ToDouble(GUI.states["yc1"].GetLastValue()));

            // tank dimensions
            double A1 = 15; //Convert.ToDouble(numUpDown_A1.Value);
            double a1 = 0.2; //Convert.ToDouble(numUpDown_a1a.Value);
            double A2 = 50; //Convert.ToDouble(numUpDown_A2.Value);
            double a2 = 0.3; //Convert.ToDouble(numUpDown_a2a.Value);

            // TANK 1 ----------------------------------------------------------------
            Point T1 = new Point(75, Convert.ToInt16(GUI.pictureBox1.Height / 2));
            Point T2 = new Point(T1.X, Convert.ToInt16(GUI.pictureBox1.Height - 20));

            // tank dimensions
            double R1_ = Math.Sqrt(A1 / Math.PI); int R1 = Convert.ToInt16(R1_ * cm2pix);
            double r1_ = Math.Sqrt(a1 / Math.PI); int r1 = Convert.ToInt16(r1_ * cm2pix);
            double h1_ = 20; int h1 = Convert.ToInt16(h1_ * cm2pix);

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

            // TANK 2 ----------------------------------------------------------------


            // tank dimensions
            double R2_ = Math.Sqrt(A2 / Math.PI); int R2 = Convert.ToInt16(R2_ * cm2pix);
            double r2_ = Math.Sqrt(a2 / Math.PI); int r2 = Convert.ToInt16(r2_ * cm2pix);
            double h2_ = 20; int h2 = Convert.ToInt16(h2_ * cm2pix);

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

            GUI.pictureBox1.Image = bm;
        }

    }
}
