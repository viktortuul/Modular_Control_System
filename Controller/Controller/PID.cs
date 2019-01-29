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
        private double I = 0;                   // error integral
        private double e = 0, ep = 0, ep_plus;  // current and prior error
        private double de = 0, de_plus = 0;     // error derivative
        private double dE = 0, dE_plus = 0;     // error derivative (low passed)
        private double u = 0;                   // control signal
        private double F_I = 0;                 // control signal filtered
        private double F_D = 0;                 // control signal filtered
        private double dt_a = 0;                // elapsed time (from last actuator update)

        private DateTime time_last_measurement = DateTime.Now; // time stamp of prior execution
        private DateTime time_last_actuator = DateTime.Now; // time stamp of prior execution

        // controller coefficients
        private double Kp;                      // proportional
        private double Ki;                      // integral
        private double Kd;                      // derivative
        private double q = 0.10;                // FIR smoothing factor
        private double T_reset = 1.0;           // filter reset factor  

        // constant dt
        double dt_CONST = 0.1;

        // actuator limitations
        private double u_max = 7.5;
        private double u_min = -7.5 * 0;

        // empty constructor
        public PID(string controller_type)
        {
            this.controller_type = controller_type;
        }

        public void ComputeControlSignal(double r, double y, bool new_actuator_flag, bool new_measurement_flag, double actuator_position)
        {
            // current time
            DateTime time_now = DateTime.Now;

            if (parameters_assigned == true)
            {
                if (time_last_measurement != null)
                {
                    // calculte error
                    e = r - y;

                    // duration since the last update
                    double dt_m = (time_now - time_last_measurement).TotalSeconds;

                    if (controller_type == "PID_normal")
                    {
                        //ComputePIDnormal(dt_CONST);
                        ComputePIDnormal(dt_m);
                        Console.WriteLine("dt:" + dt_m);
                    }
                    else if (controller_type == "PID_plus")
                    {
                        Console.WriteLine("inne i pidplus");
                        // calculate time since last actuator update         
                        if (new_actuator_flag == true)
                        {
                            dt_a = (time_now - time_last_actuator).TotalSeconds;
                            time_last_actuator = DateTime.Now;

                            // update control signal
                            ComputePIDplus(dt_a, dt_m, new_actuator_flag, actuator_position);

                            Console.WriteLine("dt_a:" + dt_a + " dt_m:" + dt_m);
                        }                    
                    }

                    // saturation
                    if (u > u_max) u = u_max;
                    if (u < u_min) u = u_min;
                }
            }

            // update prior time
            time_last_measurement = time_now;
        }


        private void ComputePIDnormal(double dt)
        {
            // integral part with anti wind-up (only add intergral action if the control signal is not saturated)
            if (anti_wind_up == true)
            {
                if (u > u_min && u < u_max) I += dt * e;
            }
            else I += dt * e;

            // derivative part (with low pass)
            if (dt != 0.0f) de = (e - ep) / dt;
            //dE = (1 - q) * dE + q * de;
            dE = de;

            // save the prior error for the next update
            ep = e;

            // resulting control signal
            u = Kp * e + Ki * I + Kd * de;

            //Console.WriteLine(de);
        }

        private void ComputePIDplus(double dt_a, double dt_m, bool new_actuator_flag, double actuator_position)
        {
            // integral part with anti wind-up 
            if (anti_wind_up == true)
            {
                // only add integral action if the communication is established (both state measurement and actuator update)
                if (new_actuator_flag == true)
                {
                    // filtered integral term
                    //F_I = F_I + (I - F_I) * (1 - Math.Exp(-dt_a / T_reset));
                    F_I = I;

                    // only add intergral action if the control signal is not saturated
                    if (u > u_min && u < u_max) I += dt_m * e;
                    else if (u > u_max && e < 0) I += dt_m * e;
                    else if (u < u_min && e > 0) I += dt_m * e;
                }
            }
            else I += dt_m * e;

            // filtered derivative term
            F_D = F_D + (de_plus - F_D) * (1 - Math.Exp(-dt_a / T_reset));
            //F_D = de_plus;

            // derivative term for PIDplus
            if (new_actuator_flag == true)
            {
                // derivative part (with low pass)
                if (dt_m != 0.0f) de_plus = (e - ep_plus) / dt_m;
                //dE_plus = (1 - q) * dE_plus + q * de_plus;

                // save the prior error for the next update
                ep_plus = e;
            }

            // resulting control signal
            //u = Kp * e + Ki * F_I + Kd * F_D;
            u = Kp * e + Ki * F_I + Kd * de_plus;

            Console.WriteLine(new_actuator_flag + " int: " + Ki * F_I + " deriv: " + Kd * de_plus);
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
