using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using Newtonsoft.Json;

namespace GlobalComponents
{
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
        public static void WriteToLog(StringBuilder sb, string filename, string text)
        {
            sb.Append(text + "\n");
            File.AppendAllText(filename, sb.ToString());
            sb.Clear();
        }

        public static bool isDouble(string str)
        {
            try
            {
                Double.Parse(str);
                return true;
            }
            catch { return false; }
        }
    }
}


