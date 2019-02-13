using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalProcesses
{
    public class InvertedPendulumSISO
    {
        // internal states
        double x = 0;       // cart position
        double x_d = 0;     // cart speed
        double phi = 0;     // stick angle
        double phi_d = 0;   // stick angular velocity

        // signals
        double F = 0;       // control signal [N]

        // parameters
        double g = 9.81;    // [m/s^2]
        double M = 0.1;     // cart mass [kg]
        double m = 0.1;     // stick mass [kg]
        double b = 0.1;     // friction of the cart [(N/m)/sec]
        double I = 0.06;    // inertia of the pendulum [kg*m^2]
        double L = 10.0;    // length to the pendulum's center of mass [m]

        // time
        private DateTime update_last = DateTime.Now; // time stamp of prior execution
        double dt;

        public InvertedPendulumSISO()
        {
        }

        public void UpdateStates()
        {
            // calculate the dime duration from the last update
            DateTime nowTime = DateTime.Now;

            if (update_last != null)
            {
                dt = (nowTime - update_last).TotalSeconds;

                // accelerations
                double x_dd = -(I + m * Math.Pow(L, 2)) * b / (I * (M + m) + M * m * Math.Pow(L, 2)) * x_d +
                              Math.Pow(m, 2) * g * Math.Pow(L, 2) / (I * (M + m) + M * m * Math.Pow(L, 2)) * phi +
                              (I + m * Math.Pow(L, 2)) / (I * (M + m) + M * m * Math.Pow(L, 2)) * F;

                double phi_dd = -m * L * b / (I * (M + m) + M * m * Math.Pow(L, 2)) * x_d +
                              m * g * L * (M + m) / (I * (M + m) + M * m * Math.Pow(L, 2)) * phi +
                              m * L / (I * (M + m) + M * m * Math.Pow(L, 2)) * F;

                // update states
                x_d += dt * x_dd;
                x += dt * x_d;
                phi_d += dt * phi_dd;
                phi += dt * phi_d;
            }

            // update prior time
            update_last = nowTime;
        }

        public void ApplyDisturbance(string target_state, double disturbance_magnitude)
        {
            if (target_state == "x") x += disturbance_magnitude;
            else if (target_state == "x_d") x_d += disturbance_magnitude;
            else if (target_state == "phi") phi += disturbance_magnitude;
            else if (target_state == "phi_d") phi_d += disturbance_magnitude;
        }

        public double[] get_yo()
        {
            return new double[] { 0 };
            //return new double[] { x + n1, x_d + n2 , phi_d + n3};
        }

        public double[] get_yc()
        {
            return new double[] { phi };
        }

        public void set_u(double[] u_)
        {
            F = u_[0];
        }
    }

    public class InvertedPendulumMIMO
    {
        // internal states
        double x = 0;       // cart position
        double x_d = 0;     // cart speed
        double phi = 0;     // stick angle
        double phi_d = 0;   // stick angular velocity

        // signals
        double F = 0;       // control signal [N]

        // parameters
        double g = 9.81;    // [m/s^2]
        double M = 0.1;     // cart mass [kg]
        double m = 0.1;     // stick mass [kg]
        double b = 0.1;     // friction of the cart [(N/m)/sec]
        double I = 0.06;    // inertia of the pendulum [kg*m^2]
        double L = 10.0;    // length to the pendulum's center of mass [m]

        // time
        private DateTime update_last = DateTime.Now; // time stamp of prior execution
        double dt;

        public InvertedPendulumMIMO()
        {
        }

        public void UpdateStates()
        {
            // calculate the dime duration from the last update
            DateTime nowTime = DateTime.Now;

            if (update_last != null)
            {
                dt = (nowTime - update_last).TotalSeconds;

                // accelerations
                double x_dd = -(I + m * Math.Pow(L, 2)) * b / (I * (M + m) + M * m * Math.Pow(L, 2)) * x_d +
                              Math.Pow(m, 2) * g * Math.Pow(L, 2) / (I * (M + m) + M * m * Math.Pow(L, 2)) * phi +
                              (I + m * Math.Pow(L, 2)) / (I * (M + m) + M * m * Math.Pow(L, 2)) * F;

                double phi_dd = -m * L * b / (I * (M + m) + M * m * Math.Pow(L, 2)) * x_d +
                              m * g * L * (M + m) / (I * (M + m) + M * m * Math.Pow(L, 2)) * phi +
                              m * L / (I * (M + m) + M * m * Math.Pow(L, 2)) * F;

                // update states
                x_d += dt * x_dd;
                x += dt * x_d;
                phi_d += dt * phi_dd;
                phi += dt * phi_d;
            }

            // update prior time
            update_last = nowTime;
        }

        public void ChangeState(string state, double disturbance)
        {
            if (state == "x") x += disturbance;
            else if (state == "x_d") x_d += disturbance;
            else if (state == "phi") phi += disturbance;
            else if (state == "phi_d") phi_d += disturbance;
        }

        public double[] get_yo()
        {
            return new double[] { 0 };
            //return new double[] { x_d + n2 , phi_d + n3};
        }

        public double[] get_yc()
        {
            return new double[] { x, phi };
        }

        public void set_u(double[] u_)
        {
            F = u_[0];
        }
    }
}
