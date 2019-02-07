using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Channel_GUI
{
    class Charting
    {
        public static void UpdateChartAxes(Chart chart, int chart_history)
        {
            chart.ChartAreas["ChartArea1"].AxisX.Minimum = DateTime.UtcNow.AddSeconds(-chart_history).ToOADate();
            chart.ChartAreas["ChartArea1"].AxisX.Maximum = DateTime.UtcNow.ToOADate();
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
                tmpChart.ChartAreas["ChartArea1"].AxisY.Maximum = Math.Ceiling(max);
                tmpChart.ChartAreas["ChartArea1"].AxisY.Minimum = Math.Floor(min);
                tmpChart.ChartAreas["ChartArea1"].AxisY.Interval = 1;
            }
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

                if (chart_.Name == "packageChart")
                {
                    chart_.Series[key].ChartType = SeriesChartType.Point;
                    chart_.Series[key].MarkerSize = 7;
                    chart_.Series[key].MarkerStyle = MarkerStyle.Circle;
                }

                if (key == "u1") chart_.Series[key].Color = Color.Orange;
                else if (key == "u2") chart_.Series[key].Color = Color.Magenta;
                else if (key == "yo1") chart_.Series[key].Color = Color.Black;
                else if (key == "yo2") chart_.Series[key].Color = Color.Gray;
                else if (key == "yc1") chart_.Series[key].Color = Color.Blue;
                else if (key == "yc2") chart_.Series[key].Color = Color.Green;
                else if (key == "r1") chart_.Series[key].Color = Color.Red;

                // set the x-axis type to DateTime
                chart_.Series[key].XValueType = ChartValueType.DateTime;
            }
        }

        public static void InitialChartSettings(ChannelGUI Main)
        {
            //Main.attackChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            Main.attackChart.ChartAreas["ChartArea1"].AxisY.Title = "";
            Main.attackChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            Main.attackChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            Main.attackChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            Main.attackChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);

            //Main.packageChart.ChartAreas["ChartArea1"].AxisX.Title = "Time";
            Main.packageChart.ChartAreas["ChartArea1"].AxisY.Title = "";
            Main.packageChart.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "hh:mm:ss";
            Main.packageChart.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            Main.packageChart.ChartAreas["ChartArea1"].AxisX.Interval = 5;
            Main.packageChart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 90, 85);
        }
    }
}
