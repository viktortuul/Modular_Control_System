using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Canal_GUI
{
    public class AttackModel
    {
        // state
        public bool running = false;

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
        public bool add_value;
        public double duration;
        public double amplitude_attack;
        public double time_const;
        public double frequency;
        public double[] time_series;
        public double value_attack;

        // attack variables
        public double time_left;
        public double time_elapsed;

        DateTime time_stamp;

        public AttackModel(string target_IP, string target_port, bool all_IPs, bool all_ports, string target_tag, string type, bool add_value, double duration, double amplitude, double time_const, double frequency, double[] time_series)
        {
            this.target_IP = target_IP;
            this.target_port = target_port;
            this.all_IPs = all_IPs;
            this.all_ports = all_ports;
            this.target_tag = target_tag;
            this.type = type;
            this.duration = duration;
            this.amplitude_attack = amplitude;
            this.time_const = time_const;
            this.frequency = frequency;
            this.time_series = time_series;
            this.add_value = add_value;
        }

        public void UpdateModel(string target_IP, string target_port, bool all_IPs, bool all_ports, string type, bool add_value, double duration, double amplitude, double time_const, double frequency, double[] time_series)
        {
            this.target_IP = target_IP;
            this.target_port = target_port;
            this.all_IPs = all_IPs;
            this.all_ports = all_ports;
            this.type = type;
            this.duration = duration;
            this.amplitude_attack = amplitude;
            this.time_const = time_const;
            this.frequency = frequency;
            this.time_series = time_series;
            this.add_value = add_value;
        }

        public void Next()
        {
            // attack value depending on attack type
            if (type == "bias") value_attack = amplitude_attack;
            else if (type == "transientD") value_attack = amplitude_attack * Math.Exp(-time_elapsed / time_const);
            else if (type == "transientI") value_attack = amplitude_attack - amplitude_attack * Math.Exp(-time_elapsed / time_const);
            else if (type == "sinusoid") value_attack = amplitude_attack * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
            else if (type == "manual")
            {
                int idx = Math.Min(Convert.ToInt32((time_elapsed / duration) * (time_series.Length - 1)), time_series.Length - 1);
                value_attack = time_series[idx];
            }
            else if (type == "delay") value_attack = time_const;

            double elapsed_time = (DateTime.Now - time_stamp).TotalMilliseconds;
            time_elapsed += Convert.ToDouble(elapsed_time) / 1000;
            time_left -= Convert.ToDouble(elapsed_time) / 1000;
            time_stamp = DateTime.Now;

            if (time_left <= 0) Stop();
        }

        public void Start()
        {
            time_stamp = DateTime.Now;
            time_left = duration;
            time_elapsed = 0;
            running = true;
        }

        public void Stop()
        {
            time_elapsed = 0;
            time_left = 0;
            value_attack = 0;
            running = false;
        }

        public double ApplyPerturbation(string state)
        {
            double result = 0;
            if (add_value == true) result = Convert.ToDouble(state) + value_attack;
            else if (add_value == false) result = value_attack;

            return result;
        }

        public string IntegrityAttack(string IP, string Port, string key, string value)
        {
            double result = Convert.ToDouble(value);

            Next();
            if (running == true)
            { 
                if (all_IPs == true && all_ports == true)
                {
                    if (key == target_tag) result = ApplyPerturbation(value);
                }
                else if (all_IPs == true && all_ports == false)
                {
                    if (Port == target_port && key == target_tag) result = ApplyPerturbation(value);
                }
                else if (all_IPs == false && all_ports == true)
                {
                    if (IP == target_IP && key == target_tag) result = ApplyPerturbation(value);
                }
                else if (all_IPs == false && all_ports == false)
                {
                    if (IP == target_IP && Port == target_port && key == target_tag) result = ApplyPerturbation(value);
                }
            }

            // if attack type is "delay", sleep the thread
            if (type == "delay") Thread.Sleep(Convert.ToInt16(time_const));

            return result.ToString();
        }
    }
}
