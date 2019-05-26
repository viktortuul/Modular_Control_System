using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace GlobalComponents

{
    public struct Constants
    {
        // time formats
        public const string FMT = "yyyy-MM-dd HH:mm:ss.fff";
        const string FMT_plot = "HH:mm:ss";

        // number of items in a DataContainer object
        public const int n_datapoints_max = 3000;
        public const int n_datapoints = 3000;
        public const int n_datapoints_controller = 300;
    }

    public struct ControllerType
    {
        // controller types 
        public const string PID_STANDARD = "PID_STANDARD";
        public const string PID_PLUS = "PID_PLUS";
        public const string PID_SUPPRESS = "PID_SUPPRESS";
    }

    public struct PlantType
    {
        // plant types 
        public const string SIMULATION = "SIMULATION";
        public const string PHYSICAL = "PHYSICAL";
    }

    public struct PhysicalProcessType
    {
        // physical process types 
        public const string SEGWAY = "SEGWAY";
        public const string PLATOONING = "PLATOONING";
        public const string INVPENDULUM = "INVPENDULUM";
    }

    public struct GUIViewMode
    {
        // view modes
        public const string CONTROL = "CONTROL";
        public const string OBSERVE = "OBSERVE";
    }

    public struct DataPointSetting
    {
        // add data point modes (plotting)
        public const string ADD_LATEST = "ADD_LATEST";
        public const string LOAD_ALL = "LOAD_ALL";
    }

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

    public class DataContainer
    {
        // number of stored data points
        int size = 0;

        // store the time:value pair in string arrays
        public string[] time;
        public string[] value;
        public string[] residual;

        // residual setting (specifically for the HMI module)
        public bool has_residual = false;

        // constructor
        public DataContainer(int size)
        {
            this.size = size;
            time = new string[size];
            value = new string[size];
            residual = new string[size];
        }

        // instert a new time:value pair data point
        public void InsertData(string time, string value)
        {
            Array.Copy(this.time, 1, this.time, 0, this.time.Length - 1);
            this.time[this.time.Length - 1] = time;

            Array.Copy(this.value, 1, this.value, 0, this.value.Length - 1);
            this.value[this.value.Length - 1] = value;
        }

        // calculate residual
        public void CalculateResidual(string value)
        {
            double resid = Convert.ToDouble(GetLastValue()) - Convert.ToDouble(value);
            Array.Copy(residual, 1, residual, 0, residual.Length - 1);
            residual[residual.Length - 1] = resid.ToString();
        }

        // insert residual if it's known
        public void InsertResidual(string resid)
        {
            Array.Copy(residual, 1, residual, 0, residual.Length - 1);
            residual[residual.Length - 1] = resid.ToString();
        }

        public string GetLastTime()
        {
            return time[time.Length - 1];
        }
        public string GetLastValue()
        {
            return value[value.Length - 1];
        }

        public void CopyAndPushArray()
        {
            Array.Copy(time, 1, time, 0, time.Length - 1);
            time[time.Length - 1] = DateTime.UtcNow.ToString(Constants.FMT);

            Array.Copy(value, 1, value, 0, value.Length - 1);
            value[value.Length - 1] = value[value.Length - 2];
        }

        public void Clear()
        {
            time = new string[size];
            value = new string[size];
            residual = new string[size];
        }
    }

    public struct Tools
    {
        public static void AddKeyToDict(Dictionary<string, DataContainer> dict, string key, int n_steps)
        {
            // if the key doesn't exist in the data container, add it
            if (dict.ContainsKey(key) == false) dict.Add(key, new DataContainer(n_steps));
        }

        public static List<string> ArgsParser(string arg)
        {
            List<string> args_parsed = new List<string>();

            if (arg.Contains("="))
            {
                string[] arg_strings = arg.Split('=');
                string name = arg_strings[0]; // argument name
                string value = arg_strings[1]; // argument value
                args_parsed.Add(name);

                if (value.Contains(":")) // splits if the argument contains several values
                {
                    string[] value_strings;
                    value_strings = value.Split(':');

                    foreach (string item in value_strings)
                    {
                        args_parsed.Add(item);
                    }
                }
                else
                {
                    args_parsed.Add(value);
                }           
            }
            else
            {
                args_parsed.Add("ARG_INVALID");
            }
            return args_parsed;
        }
    }
}


