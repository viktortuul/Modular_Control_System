using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using System.Globalization;
using GlobalComponents;
using System.IO;

namespace Model_GUI
{
    public static class Helpers
    {   
        public static void Log(StringBuilder sb, string message, string log_flag)
        {
            if (log_flag == "true")
            {
                sb.Append(message + "\n");
                File.AppendAllText("log_received.txt", sb.ToString());
                sb.Clear();
            }
        }
        public static void UpdatePerturbationLabels(ModelGUI GUI, DisturbanceModel Disturbance)
        {
            GUI.labelDebug.Text = "Time left: " + Math.Round(Disturbance.time_left, 1);
            GUI.labelDisturbance.Text = "Disturbances: \n" + Math.Round(Disturbance.value_disturbance, 2);
        }

        public static void ManageNumericalUpdowns(ModelGUI Main)
        {

            Main.labelAmplitude.Text = "Amplitude [cm/s]";
            if (Main.rbConstant.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = false;
                Main.nudFrequency.Enabled = false;
                Main.nudDuration.Enabled = true;
            }
            else if (Main.rbTransientDecrease.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = true;
                Main.nudFrequency.Enabled = false;
                Main.nudDuration.Enabled = true;
            }
            else if (Main.rbSinusoid.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = false;
                Main.nudFrequency.Enabled = true;
                Main.nudDuration.Enabled = true;
            }
            else if (Main.rbInstant.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = false;
                Main.nudFrequency.Enabled = false;
                Main.nudDuration.Enabled = false;
                Main.labelAmplitude.Text = "Amplitude [cm]";
            }
        }
    }
}
