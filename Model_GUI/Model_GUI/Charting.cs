using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Model_GUI
{
    class Charting
    {
        public static void UpdateChartAxes(Chart chart, int time_chart_window)
        {
            chart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-time_chart_window).ToOADate();
            chart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();
        }

        public static void InitializeChartSettings(Chart chart, string title)
        {
            chart.ChartAreas["ChartArea1"].AxisY.Title = title;
            chart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            chart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            chart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);
        }

        public static void AddChartSeries(ModelGUI Main, string key, object chart)
        {
            Chart chart_ = (Chart)chart;

            // add a new time series
            if (chart_.Series.IndexOf(key) == -1)
            {
                if (chart_.Name == "dataChart")
                {
                    Main.clbSeries.Items.Add(key);
                    Main.clbSeries.SetItemChecked(Main.clbSeries.Items.Count - 1, true);
                }

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

        public static void ChangeYScale(object chart, string treshold_interval, string grid_interval)
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
                tmpChart.ChartAreas[0].AxisY.Maximum = Math.Max(Math.Ceiling(max), 0);
                tmpChart.ChartAreas[0].AxisY.Minimum = Math.Min(Math.Floor(min), 0);
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
}
