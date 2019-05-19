﻿using System;
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
        private double e = 0, ep = 0, ep_plus;  // current and prior error
        private double de = 0, de_plus = 0;     // error derivative

        private double u = 0;                   // control signal
        private double F_I = 0;                 // control signal filtered
        private double dt_a = 0;                // elapsed time (from last actuator update)

        private DateTime time_last_measurement = DateTime.Now;  // time stamp of prior execution
        private DateTime time_last_actuator = DateTime.Now;     // time stamp of prior execution

        // controller coefficients
        private double Kp;                      // proportional
        private double Ki;                      // integral
        private double Kd;                      // derivative
        private double q = 0.30;                // FIR smoothing factor  --> increase --> more supress
        private double T_suppress = 0.8;        // suppress time constant  --> increase --> less suppress
        private double K_suppress = 0;          // Resulting suppress factor
        private double T_reset = 3.4;           // filter reset time constant  --> increase --> less suppress

        /*
        // constant dt
        double dt_CONST = 0.1;
        */

        // actuator limitations
        private double u_max = 7.5;
        private double u_min = 0;

        // empty constructor
        public PID(string controller_type, double[] u_saturation)
        {
            this.controller_type = controller_type;
            this.u_min = u_saturation[0];
            this.u_max = u_saturation[1];
            Console.WriteLine("PID added: " + controller_type);
            Console.WriteLine("Control signal range: [" + u_min + ", " + u_max + "]");
        }

        public void ComputeControlSignal(double r, double y, bool new_actuator_flag, double actuator_position)
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

                    if (controller_type == ControllerType.PID_STANDARD)
                    {
                        //ComputePIDnormal(dt_CONST);
                        ComputePIDnormal(dt_m);
                        //Console.WriteLine("dt:" + dt_m);
                    }
                    else if (controller_type == ControllerType.PID_PLUS)
                    {
                        // calculate time since last actuator update         
                        dt_a = (time_now - time_last_actuator).TotalSeconds;
                        time_last_actuator = DateTime.Now;

                        //Console.WriteLine("new auct flag: " + new_actuator_flag);
                        // update control signal
                        if (new_actuator_flag)
                        {
                            ComputePIDplus(dt_a, dt_m, actuator_position);
                        }                            
                    }
                    else if (controller_type == ControllerType.PID_SUPPRESS)
                    {
                        // calculate time since last actuator update    
                        dt_a = (time_now - time_last_actuator).TotalSeconds;
                        if (new_actuator_flag == true)
                        {
                            time_last_actuator = DateTime.Now;
                        }
                        
                        // update control signal
                        //if (new_actuator_flag)
                        //{
                            ComputePIDsuppress(dt_a, dt_m);
                        //}                             
                    }

                    // saturation
                    if (u > u_max) u = u_max;
                    if (u < u_min) u = u_min;
                }
            }
            // update prior time
            time_last_measurement = time_now;
        }

        private void ComputePIDnormal(double dt_m)
        {
            // integral part with anti wind-up (only add intergral action if the control signal is not saturated)
            if (anti_wind_up == true)
            {
                if (u > u_min && u < u_max) I += dt_m * e;
            }
            else I += dt_m * e;

            // derivative part (with low pass)
            if (dt_m != 0.0f) de = (e - ep) / dt_m;

            // save the prior error for the next update
            ep = e;

            // resulting control signal
            u = Kp * e + Ki * I + Kd * de;

            Console.WriteLine("dt_m: " + dt_m);
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

            // derivative term for PIDplus
            if (dt_m != 0.0f) de_plus = (e - ep_plus) / dt_m;

            // save the prior error for the next update
            ep_plus = e;

            // resulting control signal
            u = Kp * e + F_I + Kd * de_plus;

            //Console.WriteLine(Math.Exp(-dt_a / T_reset).ToString());
            //Console.WriteLine("actuator_position=" + actuator_position + " F_I=" + F_I);
            //Console.WriteLine("dt_m: " + dt_m + " K_suppress:" + K_suppress + " prop:" + Kp * ep_plus + " int: " + Ki * F_I + " deriv: " + Kd * de_plus);
            Console.WriteLine("dt_m: " + dt_m + " dt_a: " + dt_a + " last_actuator_position" + actuator_position);
        }

        private void ComputePIDsuppress(double dt_a, double dt_m)
        {
            double dt_supp = Math.Max(dt_a, dt_m);

            double gamma_hat = Math.Exp(-dt_supp / T_suppress);
            if (dt_supp < 0.10) gamma_hat = 1; // flatten out the suppres up to 100ms sampling time

            if (Math.Exp(-dt_supp / T_suppress) < K_suppress) // T_suppress = 0.8
            {
                K_suppress = gamma_hat;              
            }
            else
            {
                K_suppress = (1 - q) * K_suppress + q * gamma_hat;
            }
            
            // integral part with anti wind-up 
            if (anti_wind_up == true)
            {
                if (u > u_min && u < u_max) I += dt_m * e * K_suppress;
            }
            else I += dt_m * e * K_suppress;
            
            // derivative part (with low pass)
            if (dt_m != 0.0f) de_plus = (e - ep_plus) / dt_m;

            // save the prior error for the next update
            ep_plus = e;
            
            // resulting control signal
            u = Kp * e * K_suppress + Ki * I + Kd * de_plus;

            Console.WriteLine("dt_m: " + dt_m + "dt_a: " + dt_a + " K_suppress:" + K_suppress + " prop:" + Kp * e * K_suppress + " int: " + Ki * I + " deriv: " + Kd * de_plus);
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
