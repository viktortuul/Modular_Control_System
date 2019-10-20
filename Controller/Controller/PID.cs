using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalComponents;

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
        private double e = 0;                   // current error
        private double ep = 0;                  // previous error
        private double de = 0;                  // error derivative

        private double u = 0;                   // control signal
        private double F_I = 0;                 // control signal filtered
        private double dt_a = 0;                // elapsed time (from last actuator update)

        private DateTime time_last_measurement = DateTime.Now;  // time stamp of prior execution
        private DateTime time_last_actuator = DateTime.Now;     // time stamp of prior execution

        // PID parameters
        private double Kp;                      // proportional
        private double Ki;                      // integral
        private double Kd;                      // derivative
        
        // PIDplus parameters and variables
        private double T_reset = 3.4;           // filter reset time constant  --> increase --> less suppress

        // PIDsuppress parameters and variables
        private double T_suppress = 0.8;        // suppress time constant  --> increase --> less suppress
        private double T_flatten = 0.1;         // [s] 
        private double q = 0.30;                // FIR smoothing factor  --> increase --> more supress
        private double K_suppress = 0;          // Resulting suppress factor

        // control output saturation
        private double u_max = 7.5;
        private double u_min = 0;

        // empty constructor
        public PID(string controller_type, double[] u_saturation)
        {
            this.controller_type = controller_type;
            this.u_min = u_saturation[0];
            this.u_max = u_saturation[1];
            Console.WriteLine("PID controller added: " + controller_type);
            Console.WriteLine("Control signal range: [" + u_min + ", " + u_max + "]");
        }

        public void ComputeControlSignal(double r, double y, bool new_actuator_flag, double actuator_position)
        {
            // current time
            DateTime time_now = DateTime.Now;

            // calculate time since last actuator update    
            dt_a = (time_now - time_last_actuator).TotalSeconds;

            // calculate time since last actuator update    
            if (new_actuator_flag == true) time_last_actuator = DateTime.Now;

            if (parameters_assigned == true)
            {
                if (time_last_measurement != null)
                {
                    // calculte error
                    e = r - y;

                    // duration since the last update
                    double dt_m = (time_now - time_last_measurement).TotalSeconds;
                     
                    // update last time stamp
                    time_last_measurement = time_now;

                    // compute new controller output
                    switch (controller_type)
                    {                     
                        case ControllerType.PID_STANDARD:
                            ComputePIDnormal(dt_m);
                            break;
                        case ControllerType.PID_PLUS:
                            ComputePIDplus(dt_a, dt_m, actuator_position);
                            break;
                        case ControllerType.PID_SUPPRESS:
                            ComputePIDsuppress(dt_a, dt_m);
                            break;
                    }

                    // saturation
                    if (u > u_max) u = u_max;
                    if (u < u_min) u = u_min;
                }
            }
        }

        private void ComputePIDnormal(double dt_m)
        {
            // integral part with anti wind-up (only add intergral action if the control signal is not saturated)
            if (anti_wind_up == true)
            {
                if (u > u_min && u < u_max) I += dt_m * e;
            }
            else I += dt_m * e;

            // derivative part
            if (dt_m != 0.0f) de = (e - ep) / dt_m;

            // save the prior error for the next update
            ep = e;

            // resulting control signal
            u = Kp * e + Ki * I + Kd * de;

            Console.WriteLine(DateTime.Now.ToString() + " dt: " + Math.Round(dt_m, 3) + " u: " + u);
        }

        private void ComputePIDplus(double dt_a, double dt_m, double actuator_position)
        {
            T_reset = Ki;

            // integral part with anti wind-up 
            if (anti_wind_up == true)
            {
                // filtered integral term
                if (u > u_min && u < u_max) F_I = F_I + (actuator_position - F_I) * (1 - Math.Exp(-dt_m / T_reset)); // T_reset = 3.5
            }
            else F_I = F_I + (actuator_position - F_I) * (1 - Math.Exp(-dt_m / T_reset));

            // derivative part
            if (dt_m != 0.0f) de = (e - ep) / dt_m;

            // save the prior error for the next update
            ep = e;

            // resulting control signal
            u = Kp * e + F_I + Kd * de;

            //Console.WriteLine(Math.Exp(-dt_a / T_reset).ToString());
            //Console.WriteLine("actuator_position=" + actuator_position + " F_I=" + F_I);
            //Console.WriteLine("dt_m: " + dt_m + " K_suppress:" + K_suppress + " prop:" + Kp * ep_plus + " int: " + Ki * F_I + " deriv: " + Kd * de_plus);
            /*
            Console.WriteLine("dt_m: " + Math.Round(dt_m, 3) + 
                                " dt_a: " + Math.Round(dt_a, 3) + 
                                " last_actuator_position" + Math.Round(actuator_position, 3));
            */
        }

        private void ComputePIDsuppress(double dt_a, double dt_m)
        {
            double dt_supp = Math.Max(dt_a, dt_m);

            double gamma_hat = Math.Exp(-dt_supp / T_suppress);     // temporary suppress factor
            if (dt_supp < T_flatten) gamma_hat = 1;                 // flatten out the suppres up to 100ms sampling time

            if (Math.Exp(-dt_supp / T_suppress) < K_suppress)       // T_suppress = 0.8
                K_suppress = gamma_hat;              
            else
                K_suppress = (1 - q) * K_suppress + q * gamma_hat;
            
            // integral part with anti wind-up 
            if (anti_wind_up == true)
            {
                if (u > u_min && u < u_max) I += dt_m * e * K_suppress;
            }
            else I += dt_m * e * K_suppress;
            
            // derivative part
            if (dt_m != 0.0f) de = (e - ep) / dt_m;

            // save the prior error for the next update
            ep = e;
            
            // resulting control signal
            u = Kp * e * K_suppress + Ki * I + Kd * de;

            /*
            Console.WriteLine("dt_m: " + Math.Round(dt_m, 3) +
                                "\tdt_a: " + Math.Round(dt_a, 3) + 
                                "\tK_suppress:" + Math.Round(K_suppress, 3) + 
                                "\tP:" + Math.Round(Kp * e * K_suppress, 3) + 
                                "\tI: " + Math.Round(Ki * I, 3) + 
                                "\tD: " + Math.Round(Kd * de, 3));

        */
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
            // saturation
            if (u > u_max) u = u_max;
            if (u < u_min) u = u_min;
            return u;
        }

        public double get_u_max()
        {
            return u_max;
        }
        public double get_u_min()
        {
            return u_min;
        }
    }
}
