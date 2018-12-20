using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class PID
    {
        // controller type
        private string controller_type = ""; // PID_normal; PID_plus

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
        private double F = 0;           // control signal filtered
        private double F_prev = 0;      // control signal filtered previous calculation

        private DateTime time_previous = DateTime.Now; // time stamp of prior execution

        // controller coefficients
        private double Kp;              // proportional
        private double Ki;              // integral
        private double Kd;              // derivative
        private double q = 0.05;         // FIR smoothing factor
        private double T_reset = 0.05;   // filter reset factor  

        // actuator limitations
        private double u_max = 7.5;
        private double u_min = -7.5 * 0;

        // empty constructor
        public PID(string controller_type)
        {
            this.controller_type = controller_type;
        }

        public void ComputeControlSignal(double r, double y, bool new_value_flag)
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
                        if (controller_type == "PID_normal")
                        {
                            if (u > u_min && u < u_max) I += dt * e;
                        }
                        else if (controller_type == "PID_plus")
                        {
                            // only add integral action if the communication is established
                            if (new_value_flag == true)
                            {
                                if (u > u_min && u < u_max) I += dt * e;
                            }
                        }
                    }
                    else I += dt * e;

                    // derivative part (with low pass)
                    if (dt != 0.0f) de = (e - ep) / dt;
                    dE = (1 - q) * dE + q * de;

                    // resulting control signal
                    u = Kp * e + Ki * I + Kd * dE;
                    
                    if (controller_type == "PIDplus")
                    {
                        // if communication is lost, don't include the derivative term
                        if (new_value_flag == false)
                        {
                            u = Kp * e + Ki * I;
                        }
                    }

                    // save the prior error for the next update
                    ep = e; 

                    // saturation
                    if (u > u_max) u = u_max;
                    if (u < u_min) u = u_min;

                    // PIDplus filter
                    if (controller_type == "PID_plus")
                    {
                        Console.WriteLine(dt);
                        F = F_prev + (u - F_prev) * (1 - Math.Exp(-dt / T_reset));
                        F_prev = F;
                    }
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
            if (controller_type == "PID_normal")
            {
                return u;
            }
            else if (controller_type == "PID_plus")
            {
                return F;
            }  
            else
            {
                return u;
            }       
        }
    }
}
