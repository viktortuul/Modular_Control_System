using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Channel_GUI
{
    public class AttackModel
    {
        // state
        public bool active = false;

        // target EP and tag
        public string target_IP;
        public string target_port;
        public string target_tag;

        // target settings
        public bool all_IPs = false;
        public bool all_ports = false;

        // attack type
        public string type;

        // attack parameters
        public bool integrity_add;          // integrity attack setting: add/set value
        public double duration;             // attack duration
        public double amplitude_attack;     // attack amplitude
        public double time_const;           // time constant (transient)
        public double frequency;            // sinusoid frequency
        public double[] time_series;        // vector for a manual time series attack
        public string time_series_raw;      // raw time series
        public double value_attack;         // contemporary attack value

        // attack variables
        public double time_left;
        public double time_elapsed;

        // 
        DateTime time_stamp_last;

        public AttackModel(string target_IP, string target_port, bool all_IPs, bool all_ports, string target_tag, string type, bool integrity_add, double duration, double amplitude_attack, double time_const, double frequency, double[] time_series, string time_series_raw)
        {
            this.target_IP = target_IP;
            this.target_port = target_port;
            this.all_IPs = all_IPs;
            this.all_ports = all_ports;
            this.target_tag = target_tag;
            this.type = type;
            this.duration = duration;
            this.amplitude_attack = amplitude_attack;
            this.time_const = time_const;
            this.frequency = frequency;
            this.time_series = time_series;
            this.time_series_raw = time_series_raw;
            this.integrity_add = integrity_add;
        }

        public void UpdateModel(string target_IP, string target_port, bool all_IPs, bool all_ports, string type, bool integrity_add, double duration, double amplitude_attack, double time_const, double frequency, double[] time_series, string time_series_raw)
        {
            this.target_IP = target_IP;
            this.target_port = target_port;
            this.all_IPs = all_IPs;
            this.all_ports = all_ports;
            this.type = type;
            this.duration = duration;
            this.amplitude_attack = amplitude_attack;
            this.time_const = time_const;
            this.frequency = frequency;
            this.time_series = time_series;
            this.time_series_raw = time_series_raw;
            this.integrity_add = integrity_add;
        }

        public void Next()
        {
            // attack value depending on attack type
            if (type == "bias") value_attack = amplitude_attack;
            else if (type == "transient_decr") value_attack = amplitude_attack * Math.Exp(-time_elapsed / time_const);
            else if (type == "transient_incr") value_attack = amplitude_attack - amplitude_attack * Math.Exp(-time_elapsed / time_const);
            else if (type == "sinusoid") value_attack = amplitude_attack * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
            else if (type == "manual")
            {
                // linear mapping [elapsed time --> vector index]
                int idx = Math.Min(Convert.ToInt32((time_elapsed / duration) * (time_series.Length - 1)), time_series.Length - 1);
                value_attack = time_series[idx];
            }

            // time management 
            double elapsed_time = (DateTime.Now - time_stamp_last).TotalMilliseconds;
            time_elapsed += Convert.ToDouble(elapsed_time) / 1000;
            time_left -= Convert.ToDouble(elapsed_time) / 1000;
            time_stamp_last = DateTime.Now;

            if (time_left <= 0) Stop();
        }

        public void Start()
        {
            time_stamp_last = DateTime.Now;
            time_left = duration;
            time_elapsed = 0;
            active = true;
        }

        public void Stop()
        {
            time_elapsed = 0;
            time_left = 0;
            value_attack = 0;
            active = false;
        }

        public double ApplyPerturbation(string state)
        {
            double result = 0;
            if (integrity_add == true) result = Convert.ToDouble(state) + value_attack;
            else if (integrity_add == false) result = value_attack;

            return result;
        }

        public string IntegrityAttack(string IP, string Port, string key, string value)
        {
            double result = Convert.ToDouble(value);

            Next();
            if (active == true)
            {
                if (all_IPs == true && all_ports == true) // any IP, any Port
                {
                    if (key == target_tag) result = ApplyPerturbation(value);
                }
                else if (all_IPs == true && all_ports == false) // any IP, specific port
                {
                    if (Port == target_port && key == target_tag) result = ApplyPerturbation(value);
                }
                else if (all_IPs == false && all_ports == true) // specific IP, any port
                {
                    if (IP == target_IP && key == target_tag) result = ApplyPerturbation(value);
                }
                else if (all_IPs == false && all_ports == false) // specific IP, specific port
                {
                    if (IP == target_IP && Port == target_port && key == target_tag) result = ApplyPerturbation(value);
                }
            }

            return result.ToString();
        }
    }
}
