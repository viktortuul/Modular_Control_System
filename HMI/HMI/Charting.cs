using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace HMI
{
    class Charting
    {
        public static void AddChartSeries(FrameGUI Main, string key, object chart)
        {
            string[] unchecked_keys = new string[] { "yo1", "yo2" };

            Chart chart_ = (Chart)chart;

            // add a new time series
            if (chart_.Series.IndexOf(key) == -1)
            {
                // add to the checked list box
                if (chart_.Name == "dataChart")
                {
                    Main.clbSeries.Items.Add(key);

                    if (unchecked_keys.Contains(key) == false)
                        Main.clbSeries.SetItemChecked(Main.clbSeries.Items.Count - 1, true);
                    else
                        Main.clbSeries.SetItemChecked(Main.clbSeries.Items.Count - 1, false);
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
                        chart_.Series[key].Color = Color.Gray;
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
                    case "Res_yc1":
                        chart_.Series[key].Color = Color.Blue;
                        chart_.Series[key].BorderWidth = 2;
                        break;
                    case "Sec_yc1":
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
            for (int i = 0; i < Main.connection_selected.n_controlled_states; i++)
            {
                if (Main.dataChart.Series.IndexOf("r" + (i + 1).ToString()) == -1) AddChartSeries(Main, "r" + (i + 1).ToString(), Main.dataChart);
            }
        }

        public static void UpdateChartAxes(Chart chart, int chart_history)
        {
            chart.ChartAreas[0].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
            chart.ChartAreas[0].AxisX.Maximum = DateTime.UtcNow.ToOADate();
        }

        public static void ChangeYScale(object chart, string treshold_interval, string grid_interval)
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
                if (treshold_interval == "one")
                {
                    tmpChart.ChartAreas[0].AxisY.Maximum = Math.Max(Math.Ceiling(max), 0);
                    tmpChart.ChartAreas[0].AxisY.Minimum = Math.Min(Math.Floor(min), 0);
                }
                else if (treshold_interval == "deci")
                {
                    tmpChart.ChartAreas[0].AxisY.Maximum = Math.Max(Math.Ceiling(max * 10) / 10, 0);
                    tmpChart.ChartAreas[0].AxisY.Minimum = Math.Min(Math.Floor(min * 10) / 10, 0);
                }

                if (grid_interval == "one")
                {

                    tmpChart.ChartAreas[0].AxisY.Interval = 1;
                }
                else if (grid_interval == "adaptive")
                {
                    tmpChart.ChartAreas[0].AxisY.Interval = Math.Max(Math.Ceiling(max / 10), 1);
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

        public static void InitializeChartSettings(Chart chart, string title)
        {
            chart.ChartAreas[0].AxisY.Title = title;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "hh:mm:ss";
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            chart.ChartAreas[0].AxisX.Interval = 5;
            chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);
        }

        public static void ManageChartSize(FrameGUI Main)
        {
            try
            {
                int y_start = Main.residualChart.Location.Y;
                int height_total = Main.tabControl1.Height - y_start;
                Main.residualChart.Height = height_total / 2 - y_start;
                Main.securityChart.Location = new Point(6, + y_start + height_total / 2);
                Main.securityChart.Height = height_total / 2 - y_start;
            }
            catch { }
        }
    }
}
