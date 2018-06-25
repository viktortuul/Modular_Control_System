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

        public static void changeYScala(object chart, string verbose)
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


        public static void CheckKey(Dictionary<string, ModelGUI.DataContainer> dict, string key, int n_steps)
        {
            // if the key doesn't exist, add it
            if (dict.ContainsKey(key) == false) dict.Add(key, new ModelGUI.DataContainer(n_steps));
        }

        public static void UpdatePerturbationLabels(ModelGUI GUI, ModelGUI.Perturbation Disturbance, ModelGUI.Perturbation Noise, ModelGUI.Perturbation Control)
        {

            GUI.labelDisturbance.Text = "Disturbances: \n" +
                                    Math.Round(Disturbance.value[0], 1) + "\n" +
                                    Math.Round(Disturbance.value[1], 1) + "\n" +
                                    Math.Round(Disturbance.value[2], 1) + "\n" +
                                    Math.Round(Disturbance.value[3], 1);

            GUI.labelNoise.Text = "Noises: \n" +
                                    Math.Round(Noise.value[0], 1) + "\n" +
                                    Math.Round(Noise.value[1], 1) + "\n" +
                                    Math.Round(Noise.value[2], 1) + "\n" +
                                    Math.Round(Noise.value[3], 1);

            GUI.labelControl.Text = "Controls: \n" +
                                    Math.Round(Control.value[0], 1) + "\n" +
                                    Math.Round(Control.value[1], 1) + "\n" +
                                    Math.Round(Control.value[2], 1) + "\n" +
                                    Math.Round(Control.value[3], 1);
        }

        //public static void AddChartSeries_d(string key)
        //{
        //    // add a new time series
        //    if (perturbationChart.Series.IndexOf(key) == -1)
        //    {
        //        perturbationChart.Series.Add(key);
        //        perturbationChart.Series[key].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
        //        perturbationChart.Series[key].BorderWidth = 2;

        //        switch (key)
        //        {
        //            case "u1":
        //                perturbationChart.Series[key].Color = Color.Orange;
        //                perturbationChart.Series[key].BorderWidth = 1;
        //                break;
        //            case "u2":
        //                perturbationChart.Series[key].Color = Color.Magenta;
        //                perturbationChart.Series[key].BorderWidth = 1;
        //                break;
        //            case "yo1":
        //                perturbationChart.Series[key].Color = Color.Black;
        //                perturbationChart.Series[key].BorderWidth = 2;
        //                break;
        //            case "yo2":
        //                perturbationChart.Series[key].Color = Color.Gray;
        //                perturbationChart.Series[key].BorderWidth = 2;
        //                break;
        //            case "yc1":
        //                perturbationChart.Series[key].Color = Color.Blue;
        //                perturbationChart.Series[key].BorderWidth = 3;
        //                break;
        //            case "yc2":
        //                perturbationChart.Series[key].Color = Color.Green;
        //                perturbationChart.Series[key].BorderWidth = 3;
        //                break;
        //            default:
        //                break;
        //        }
        //        // set the x-axis type to DateTime
        //        perturbationChart.Series[key].XValueType = ChartValueType.DateTime;
        //    }
        //}

    }
}
