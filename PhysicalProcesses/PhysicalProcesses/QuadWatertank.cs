using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalProcesses
{
    public class QuadWatertank
    {
        // internal states
        double h11 = 0; // height tank 11 (top left)
        double h12 = 0; // height tank 12 (top right)
        double h21 = 0; // height tank 21 (bottom left )
        double h22 = 0; // height tank 22 (bottom right)

        // state constraints
        double h11_max = 20; // tank 11 height
        double h12_max = 20; // tank 12 height
        double h21_max = 20; // tank 21 height
        double h22_max = 20; // tank 22 height

        // signals
        double u1 = 0; // control signal 1 [V]
        double u2 = 0; // control signal 2 [V]

        // parameters
        double g = 9.81 * 100; // [cm/s^2]

        // operating actuator proportional constants [cm^3/(Vs)]
        double k1 = 3.32;
        double k2 = 3.74;

        // outlet areas [cm^2]
        double a11 = 0.095;
        double a12 = 0.095;
        double a21 = 0.095;
        double a22 = 0.095;

        // valve settings
        double gam1 = 0.625;
        double gam2 = 0.625;

        // cross section areas [cm^2]
        double A11 = 15.5179;
        double A12 = 15.5179;
        double A21 = 15.5179;
        double A22 = 15.5179;

        // time
        private DateTime update_last = DateTime.Now; // time stamp of prior execution
        double dt;

        public QuadWatertank(double[] model_parameters)
        {
            if (model_parameters.Length == 8)
            {
                A11 = model_parameters[0];
                a11 = model_parameters[1];
                A12 = model_parameters[2];
                a12 = model_parameters[3];
                A21 = model_parameters[4];
                a21 = model_parameters[5];
                A22 = model_parameters[6];
                a22 = model_parameters[7];
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
                double q_out11 = a11 * Math.Sqrt(2 * h11 * g);
                double q_out12 = a12 * Math.Sqrt(2 * h12 * g);
                double q_out21 = a21 * Math.Sqrt(2 * h21 * g);
                double q_out22 = a22 * Math.Sqrt(2 * h22 * g);

                // update states
                h11 += dt * (1 / A11) * ((1 - gam2) * k2 * u2 - q_out11);       // top left tank 
                h12 += dt * (1 / A12) * ((1 - gam1) * k1 * u1 - q_out12);       // top right tank
                h21 += dt * (1 / A21) * (gam1 * k1 * u1 + q_out11 - q_out21);   // bottom left tank        
                h22 += dt * (1 / A22) * (gam2 * k2 * u2 + q_out12 - q_out22);   // bottom right tank

                // saturation
                if (h11 < 0) h11 = 0;
                if (h12 < 0) h12 = 0;
                if (h21 < 0) h21 = 0;
                if (h22 < 0) h22 = 0;
                if (h11 > h11_max) h11 = h11_max;
                if (h12 > h12_max) h12 = h12_max;
                if (h21 > h21_max) h21 = h21_max;
                if (h22 > h22_max) h22 = h22_max;
            }

            // update prior time
            update_last = nowTime;
        }

        public void ChangeState(string state, double disturbance)
        {
            if (state == "h11") h11 += disturbance;         // top left tank 
            else if (state == "h12") h12 += disturbance;    // top right tank
            else if (state == "h21") h21 += disturbance;    // bottom left tank        
            else if (state == "h22") h22 += disturbance;    // bottom right tank
        }

        public double[] get_yo()
        {
            return new double[] { h11, h12 };
        }

        public double[] get_yc()
        {
            return new double[] { h21, h22 };
        }

        public void set_u(double[] u_)
        {
            u1 = u_[0];
            u2 = u_[1];
        }
    }
}
