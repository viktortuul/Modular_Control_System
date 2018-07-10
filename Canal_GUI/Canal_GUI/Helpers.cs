using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace Canal_GUI
{
    class Helpers
    {
        // time format
        public const string FMT = "yyyy-MM-dd HH:mm:ss.fff";


        public static bool isDouble(string str)
        {
            try
            {
                Double.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

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

        public static void CheckKey(Dictionary<string, DataContainer> dict, string key, int n_steps)
        {
            // if the key doesn't exist, add it
            if (dict.ContainsKey(key) == false) dict.Add(key, new DataContainer(n_steps));
        }

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
}
