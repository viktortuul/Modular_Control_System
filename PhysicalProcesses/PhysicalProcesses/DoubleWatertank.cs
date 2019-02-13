using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalProcesses
{
    public class DoubleWatertank
    {
        // internal states
        double h1 = 0; // height tank 1 (top)
        double h2 = 0; // height tank 2 (bottom)

        // state constraints
        double h1_max = 20; // tank 1 height
        double h2_max = 20; // tank 2 height

        // signals
        double u = 0; // control signal [V]

        // parameters
        double g = 9.81 * 100;

        // operating actuator proportional constants [cm^3/(Vs)]
        double k = 6.5;

        // outlet areas [cm^2]
        double a1 = 0.3;
        double a2 = 0.2;

        // cross section areas [cm^2]
        double A1 = 15.0;
        double A2 = 50.0;

        double dt;

        // time
        private DateTime update_last = DateTime.Now; // time stamp of prior execution

        public DoubleWatertank(double[] model_parameters)
        {
            if (model_parameters.Length == 4)
            {
                A1 = model_parameters[0];
                a1 = model_parameters[1];
                A2 = model_parameters[2];
                a2 = model_parameters[3];

            }
        }

        public void UpdateStates()
        {
            // calculate the dime duration from the last update
            DateTime nowTime = DateTime.Now;

            if (update_last != null)
            {
                dt = (nowTime - update_last).TotalSeconds;

                // outflows
                double q_out1 = a1 * Math.Sqrt(2 * h1 * g);
                double q_out2 = a2 * Math.Sqrt(2 * h2 * g);

                // update states
                h1 += dt * (1 / A1) * (k * u - q_out1);     // top tank
                h2 += dt * (1 / A2) * (q_out1 - q_out2);    // bottom tank

                // saturation
                if (h1 < 0) h1 = 0; // empty tank
                if (h2 < 0) h2 = 0; // empty tank
                if (h1 > h1_max) h1 = h1_max; // filled tank
                if (h2 > h2_max) h2 = h1_max; // filled tank
            }

            // update prior time
            update_last = DateTime.Now;
        }

        public void ApplyDisturbance(string target_state, double disturbance_magnitude)
        {
            if (target_state == "h1") h1 += disturbance_magnitude; // top tank 
            else if (target_state == "h2") h2 += disturbance_magnitude; // bottom tank
        }

        public void set_model_parameters(double[] model_parameters)
        {

        }

        public double[] get_yo()
        {
            return new double[] { h1 };
        }

        public double[] get_yc()
        {
            return new double[] { h2 };
        }

        public double[] get_uc()
        {
            return new double[] { u };
        }

        public void set_u(double[] _u)
        {
            u = _u[0];
        }
    }
}
