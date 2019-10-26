using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalComponents
{
    public class DataContainer
    {
        // number of stored data points
        int size = 0;

        // store the time:value pair in string arrays
        public string[] time;
        public string[] value;
        public string[] residual;
        public string[] security_metric;

        // residual setting (specifically for the HMI module)
        public bool has_residual = false;

        // constructor
        public DataContainer(int size)
        {
            this.size = size;
            time = new string[size];
            value = new string[size];
            residual = new string[size];
            security_metric = new string[size];
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

        // insert security metric if it's known
        public void InsertSecurityMetric(string sec)
        {
            Array.Copy(security_metric, 1, security_metric, 0, security_metric.Length - 1);
            security_metric[security_metric.Length - 1] = sec.ToString();
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
            security_metric = new string[size];
        }
    }
}
