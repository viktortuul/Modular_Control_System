using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canal_GUI
{
    public class Attack
    {
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
        public double duration;
        public double frequency;
        public double time_const;
        public double amplitude;
        public double value;

        // attack variables
        public double time_left;
        public double time_elapsed;

        DateTime time_stamp;

        public Attack(string target_IP, string target_port, bool all_IPs, bool all_ports, string target_tag, string type, double duration, double amplitude, double time_const, double frequency)
        {
            this.target_IP = target_IP;
            this.target_port = target_port;
            this.all_IPs = all_IPs;
            this.all_ports = all_ports;
            this.target_tag = target_tag;
            this.type = type;
            this.duration = duration;
            this.amplitude = amplitude;
            this.time_const = time_const;
            this.frequency = frequency;
        }

        public void UpdateModel(string target_IP, string target_port, bool all_IPs, bool all_ports, string type, double duration, double amplitude, double time_const, double frequency)
        {
            this.target_IP = target_IP;
            this.target_port = target_port;
            this.all_IPs = all_IPs;
            this.all_ports = all_ports;
            this.type = type;
            this.duration = duration;
            this.amplitude = amplitude;
            this.time_const = time_const;
            this.frequency = frequency;
        }

        public void Next()
        {
            if (type == "bias") value = amplitude;
            else if (type == "transientD") value = amplitude * Math.Exp(-time_elapsed / time_const);
            else if (type == "transientI") value = amplitude - amplitude * Math.Exp(-time_elapsed / time_const);
            else if(type == "sinusoid") value = amplitude * Math.Sin(frequency * time_elapsed * 2 * Math.PI);

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
        }

        public void Stop()
        {
            time_elapsed = 0;
            time_left = 0;
            value = 0;
        }

        public double ApplyPerturbation()
        {
            Next();
            return value;
        }

        public string IntegrityAttack(string IP, string Port, string key, string value)
        {
            double result = Convert.ToDouble(value);

            if (all_IPs == true && all_ports == true)
            {
                if (key == target_tag) result = Convert.ToDouble(value) + ApplyPerturbation();
            }
            else if (all_IPs == true && all_ports == false)
            {
                if (Port == target_port && key == target_tag) result = Convert.ToDouble(value) + ApplyPerturbation();
            }
            else if (all_IPs == false && all_ports == true)
            {
                if (IP == target_IP && key == target_tag) result = Convert.ToDouble(value) + ApplyPerturbation();
            }
            else if (all_IPs == false && all_ports == false)
            {
                if (IP == target_IP && Port == target_port && key == target_tag) result = Convert.ToDouble(value) + ApplyPerturbation();
            }

            return result.ToString();
        }
    }
}
