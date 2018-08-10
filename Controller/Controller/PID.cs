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
        private double I = 0; // error integral
        private double e = 0, ep = 0; // current and prior error
        private double de = 0; // error derivative
        private double de_temp = 0; // error derivative holder
        private double u = 0; // control signal
        private DateTime update_last = DateTime.Now; // time stamp of prior execution

        // controller coefficients
        private double Kp; // proportional
        private double Ki; // integral
        private double Kd; // derivative

        // controller limitations
        private double u_max = 7.5;
        private double u_min = -7.5 * 0;

        // constructor
        public PID() { }

        public void ComputeControlSignal(double r, double y)
        {
            // calculate the dime duration from the last update
            DateTime nowTime = DateTime.Now;

            if (parameters_assigned == true)
            {
                // calculte error
                e = r - y;

                if (update_last != null)
                {
                    double dt = (nowTime - update_last).TotalSeconds;

                    //integrator with anti wind-up (only add intergral action if the control signal is not saturated)
                    if (anti_wind_up == true)
                    {
                        if (u > u_min && u < u_max) I += dt * e;                 
                    }
                    else I += dt * e;

                    // derivator (with low pass)
                    //de = 1 / (dt + 1) * y - de_temp;
                    //de_temp = (y - de) / (dt + 1);

                    if (dt != 0.0f) de = (e - ep) / dt;

                    // control signal
                    u = Kp * e + Ki * I - Kd * de;

                    ep = e; // update prior error

                    // saturation
                    if (u > u_max) u = u_max;
                    if (u < u_min) u = u_min;
                }
            }

            // update prior time
            update_last = nowTime;
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
