using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using GlobalComponents;

namespace Channel_GUI
{
    static class Helpers
    {
        public static double[] DecodeTimeSeries(string text)
        {
            // split text at each 'tab'
            string[] strings = text.Split('\t');
            double[] time_series = new double[strings.Length];

            // fill the vector
            for (int i = 0; i < strings.Length - 1; i++)
            {
                time_series[i] = Convert.ToDouble(strings[i]);
            }
            return time_series;
        }

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

        public static void ManageNumericalUpdowns(ChannelGUI Main)
        {
            Main.tbTimeSeries.Visible = false;
            Main.btnClearCustomAttack.Visible = false;
            Main.labelTimeSeries.Visible = false;

            if (Main.rbBias.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = false;
                Main.nudFrequency.Enabled = false;
            }
            else if (Main.rbTransientIncrease.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = true;
                Main.nudFrequency.Enabled = false;
            }
            else if (Main.rbTransientDecrease.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = true;
                Main.nudFrequency.Enabled = false;
            }
            else if (Main.rbSinusoid.Checked == true)
            {
                Main.nudAmplitude.Enabled = true;
                Main.nudTimeConst.Enabled = false;
                Main.nudFrequency.Enabled = true;
            }
            else if (Main.rbManual.Checked == true)
            {
                Main.nudDuration.Enabled = true;
                Main.nudAmplitude.Enabled = false;
                Main.nudTimeConst.Enabled = false;
                Main.nudFrequency.Enabled = false;
                Main.tbTimeSeries.Visible = true;
                Main.btnClearCustomAttack.Visible = true;
                Main.labelTimeSeries.Visible = true;
            }
        }
    }

    public struct AttackParameters
    {
        public string target_tag;
        public string target_ip;
        public string target_port;

        // specify attack type
        public string attack_type;
        public double[] time_series;       // empty array which is used ONLY if a manual attack is conduced
                                           // bool which determine if the attack adds a value or sets a value
        public string time_series_raw;

        public bool integrity_add;

        // attack parameters
        public double duration;
        public double amplitude;
        public double time_constant;
        public double frequency;
        public bool target_all_IPs;
        public bool target_all_Ports;

        public AttackParameters(string target_tag, string target_ip, string target_port, string attack_type, double[] time_series, string time_series_raw, bool integrity_add, double duration, double amplitude, double time_constant, double frequency, bool target_all_IPs, bool target_all_Ports)
        {
            this.target_tag = target_tag;
            this.target_ip = target_ip;
            this.target_port = target_port;
            this.attack_type = attack_type;
            this.time_series = time_series;
            this.time_series_raw = time_series_raw;
            this.integrity_add = integrity_add;
            this.duration = duration;
            this.amplitude = amplitude;
            this.time_constant = time_constant;
            this.frequency = frequency;
            this.target_all_IPs = target_all_IPs;
            this.target_all_Ports = target_all_Ports;
        }
    }
}
