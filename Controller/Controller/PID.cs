using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class PID
    {
        // anti wind-up
        private bool anti_wind_up = true;

        // flags
        private bool parameters_assigned = false;

        // states
        private double I = 0;           // error integral
        private double e = 0, ep = 0;   // current and prior error
        private double de = 0;          // error derivative
        private double dE = 0;          // error derivative (low passed)
        private double u = 0;           // control signal
        private DateTime time_previous = DateTime.Now; // time stamp of prior execution

        // controller coefficients
        private double Kp;              // proportional
        private double Ki;              // integral
        private double Kd;              // derivative
        private double q = 0.01;        // FIR smoothing factor

        // actuator limitations
        private double u_max = 7.5;
        private double u_min = -7.5 * 0;

        // empty constructor
        public PID() { }

        public void ComputeControlSignal(double r, double y)
        {
            // calculate the dime duration from the last update
            DateTime time_now = DateTime.Now;

            if (parameters_assigned == true)
            {
                if (time_previous != null)
                {
                    // calculte error
                    e = r - y;

                    // duration since the last update
                    double dt = (time_now - time_previous).TotalSeconds;

                    // integral part with anti wind-up (only add intergral action if the control signal is not saturated)
                    if (anti_wind_up == true)
                    {
                        if (u > u_min && u < u_max) I += dt * e;                 
                    }
                    else I += dt * e;

                    // derivative part (with low pass)
                    if (dt != 0.0f) de = (e - ep) / dt;
                    dE = (1 - q) * dE + q * de;

                    // resulting control signal
                    u = Kp * e + Ki * I + Kd * dE;

                    // save the prior error for the next update
                    ep = e; 

                    // saturation
                    if (u > u_max) u = u_max;
                    if (u < u_min) u = u_min;
                }
            }

            // update prior time
            time_previous = time_now;
        }

        public void UpdateParameters(double Kp, double Ki, double Kd)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
            parameters_assigned = true;
        }

        public double get_u()
        {
            return u;
        }
    }
}
