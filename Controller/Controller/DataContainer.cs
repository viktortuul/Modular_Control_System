using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Controller
{
    public class DataContainer
    {
        // time format
        const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // store the time:value pair in string arrays
        public string[] time;
        public string[] value;

        // constraints
        double max_delay = 90000000; // [ms] 

        // constructor
        public DataContainer(int size)
        {
            time = new string[size];
            value = new string[size];
        }

        // instert a new time:value pair data point
        public void InsertData(string time, string value)
        { 
            if (GetLastTime() != null)
            {
                // check if the new data is up to date
                if (isMostRecent(time) == true)
                {
                    Array.Copy(this.time, 1, this.time, 0, this.time.Length - 1);
                    this.time[this.time.Length - 1] = time;

                    Array.Copy(this.value, 1, this.value, 0, this.value.Length - 1);
                    this.value[this.value.Length - 1] = value;
                }
                else
                {
                    Console.WriteLine(DateTime.UtcNow.ToString() + " > ignoring measurement data (wrong order)");
                }
            }
            else
            {
                this.time[this.time.Length - 1] = time;
                this.value[this.value.Length - 1] = value;
            }
        }

        public bool isMostRecent(string time)
        {
            // compare a time-stamp with the current most recent
            DateTime t_new = DateTime.ParseExact(time, FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            DateTime t_prev = DateTime.ParseExact(GetLastTime(), FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            TimeSpan timeDiff = t_new - t_prev;

            if (timeDiff.TotalMilliseconds > 0) return true;
            else return false;
        }

        public bool isUpToDate()
        {
            // check if the last data point was added withing a specfic time
            if (GetLastTime() != null)
            {
                DateTime t_now = DateTime.ParseExact(DateTime.UtcNow.ToString(FMT), FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                DateTime t_last = DateTime.ParseExact(GetLastTime(), FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = t_now - t_last;

                if (timeDiff.TotalMilliseconds <= max_delay) return true;
                else return false;
            }
            else return false;
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
            time[time.Length - 1] = DateTime.UtcNow.ToString(FMT);

            Array.Copy(value, 1, value, 0, value.Length - 1);
            value[value.Length - 1] = value[value.Length - 2];
        }
    }
}
