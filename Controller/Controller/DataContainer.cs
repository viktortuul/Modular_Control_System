using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using GlobalComponents;

namespace Controller
{
    // Note: very similar to the standard DataController, but with some extra features for the control module
    public class DataContainer
    {
        // number of stored data points
        int size = 0;

        // store the time:value pair in string arrays
        public string[] time;
        public string[] value;

        // added data points counter
        int counter = 0;

        // constraint
        double max_delay = 300; // [ms] 

        // constructor
        public DataContainer(int size)
        {
            this.size = size;
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
                    counter += 1;

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
            DateTime t_new = DateTime.ParseExact(time, Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            DateTime t_prev = DateTime.ParseExact(GetLastTime(), Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            TimeSpan timeDiff = t_new - t_prev;

            if (timeDiff.TotalMilliseconds > 0) return true;
            else return false;
        }

        public bool isUpToDate()
        {
            // check if the last data point was added withing a specfic time
            if (GetLastTime() != null)
            {
                DateTime t_now = DateTime.ParseExact(DateTime.UtcNow.ToString(Constants.FMT), Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                DateTime t_last = DateTime.ParseExact(GetLastTime(), Constants.FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                TimeSpan timeDiff = t_now - t_last;

                if (timeDiff.TotalMilliseconds <= max_delay)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public void CopyAndPushArray()
        {
            Array.Copy(time, 1, time, 0, time.Length - 1);
            time[time.Length - 1] = DateTime.UtcNow.ToString(Constants.FMT);

            Array.Copy(value, 1, value, 0, value.Length - 1);
            value[value.Length - 1] = value[value.Length - 2];
        }

        public bool hasChanged(int idx1, int idx2)
        {
            // checks wether the values of the two items are the same or not
            return value[value.Length - 1 - idx1] != value[value.Length - 1 - idx2];
        }

        public string GetLastTime()
        {
            return time[time.Length - 1];
        }
        public string GetLastValue()
        {
            return value[value.Length - 1];
        }

        public int getCounter()
        {
            return counter;
        }

        public void Clear()
        {
            time = new string[size];
            value = new string[size];
        }
    }
}
