using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_GUI
{
    public class DisturbanceModel
    {
        public string target_state;
        public string type;
        public double duration;
        public double time_left;
        public double time_elapsed;  
        public double frequency;
        public double time_const;
        public double amplitude_disturbance;
        public double value_disturbance;
        DateTime time_stamp_last = DateTime.Now;

        public DisturbanceModel() { }

        public DisturbanceModel(string target_state, string type, double duration, double amplitude_disturbance, double time_const, double frequency)
        {
            this.target_state = target_state;
            this.type = type;
            this.duration = duration;
            this.amplitude_disturbance = amplitude_disturbance;
            this.time_const = time_const;
            this.frequency = frequency;
            Start();
        }

        public void PerturbationNext()
        {
            // time management
            double dt = (DateTime.Now - time_stamp_last).TotalMilliseconds;
            time_elapsed += Convert.ToDouble(dt) / 1000;
            time_left -= Convert.ToDouble(dt) / 1000;
            time_stamp_last = DateTime.Now;
            if (time_left <= 0) Stop();

            if (type == "constant") value_disturbance = (dt / 1000) * amplitude_disturbance;
            else if (type == "transient") value_disturbance = (dt / 1000) * amplitude_disturbance * Math.Exp(-time_elapsed / time_const);
            else if (type == "sinusoid") value_disturbance = (dt / 1000) * amplitude_disturbance * Math.Sin(frequency * time_elapsed * 2 * Math.PI);
            else if (type == "instant") value_disturbance = amplitude_disturbance;
        }

        public void Start()
        {
            time_stamp_last = DateTime.Now;
            time_left = duration;
            time_elapsed = 0;
        }

        public void Stop()
        {
            time_elapsed = 0;
            time_left = 0;
            value_disturbance = 0;
        }
    }
}
