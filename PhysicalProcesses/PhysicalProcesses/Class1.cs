using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalProcesses
{
    public class Plant
    {
        string model_type = "";

        DoubleWatertank dWT;
        QuadWatertank qWT;

        // identify what type of plant is considered
        public Plant(DoubleWatertank watertank)
        {
            model_type = "DoubleWatertank";
            dWT = watertank;
        }

        public Plant(QuadWatertank watertank)
        {
            model_type = "QuadWatertank";
            qWT = watertank;
        }

        // update the model states
        public void update_state(double[] disturbanceVolume)
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    dWT.update_states(disturbanceVolume);
                    break;
                case "QuadWatertank":
                    qWT.update_states(disturbanceVolume);
                    break;
            }
        }

        // measure the observed states
        public double[] get_yo()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    return dWT.get_yo();
                case "QuadWatertank":
                    return qWT.get_yo();
                default:
                    return new double[2];
            }
        }

        // measure the controlled states
        public double[] get_yc()
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    return dWT.get_yc();
                case "QuadWatertank":
                    return qWT.get_yc();
                default:
                    return new double[2];
            }
        }

        // update the control input value
        public void set_u(double[] u_)
        {
            switch (model_type)
            {
                case "DoubleWatertank":
                    dWT.set_u(u_);
                    break;
                case "QuadWatertank":
                    qWT.set_u(u_);
                    break;
            }
        }
    }

    public class DoubleWatertank
    {
        // internal states
        double h1 = 0; // height tank 1 (top)
        double h2 = 0; // height tank 2 (bottom)

        // state constraints
        double h1_max = 25; // tank 1 height
        double h2_max = 25; // tank 2 height

        // signals
        double u = 0; // control signal [V]

        // parameters
        double g = 9.81 * 100;

        // operating actuator proportional constants [cm^3/(Vs)]
        double k = 6.32;

        // outlet areas [cm^2]
        double a1 = 0.16;
        double a2 = 0.16;

        // cross section areas [cm^2]
        double A1 = 15.0;
        double A2 = 100.0;

        double dt;

        // time
        private DateTime update_last = DateTime.Now; // time stamp of prior execution

        public DoubleWatertank()
        {
        }

        public void update_states(double[] disturbance_volume)
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
                h1 += dt * (1 / A1) * (k * u - q_out1 + disturbance_volume[0]); // top tank
                h2 += dt * (1 / A2) * (q_out1 - q_out2 + disturbance_volume[1]); // bottom tank

                // saturation
                if (h1 < 0) h1 = 0; // empty tank
                if (h2 < 0) h2 = 0; // empty tank
                if (h1 > h1_max) h1 = h1_max; // filled tank
                if (h2 > h2_max) h2 = h1_max; // filled tank
            }

            // update prior time
            update_last = nowTime;
        }

        public double[] get_yo()
        {
            // apply measurement noise
            var r = new GaussianRandom();
            double n = r.NextGaussian(0, 0.02);
            return new double[] { h1 + n };
        }

        public double[] get_yc()
        {
            // apply measurement noise
            var r = new GaussianRandom();
            double n = r.NextGaussian(0, 0.02);
            return new double[] { h2 + n };
        }

        public void set_u(double[] _u)
        {
            u = _u[0];
        }
    }

    public class QuadWatertank
    {
        // internal states
        double h11 = 0; // height tank 11 (top left)
        double h12 = 0; // height tank 12 (top right)
        double h21 = 0; // height tank 21 (bottom left )
        double h22 = 0; // height tank 22 (bottom right)

        // state constraints
        double h11_max = 25; // tank 11 height
        double h12_max = 25; // tank 12 height
        double h21_max = 25; // tank 21 height
        double h22_max = 25; // tank 22 height

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

        public QuadWatertank()
        {
        }

        public void update_states(double[] disturbance_volume)
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
                h11 += dt * (1 / A11) * ((1 - gam2) * k2 * u2 - q_out11 + disturbance_volume[0]); // top left tank 
                h12 += dt * (1 / A12) * ((1 - gam1) * k1 * u1 - q_out12 + disturbance_volume[1]); // top right tank
                h21 += dt * (1 / A21) * (gam1 * k1 * u1 + q_out11 - q_out21 + disturbance_volume[2]); // bottom left tank        
                h22 += dt * (1 / A22) * (gam2 * k2 * u2 + q_out12 - q_out22 + disturbance_volume[3]); // bottom right tank

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


        public double[] get_yo()
        {
            // apply measurement noise
            var r = new GaussianRandom();
            double n1 = r.NextGaussian(0, 0.02);
            double n2 = r.NextGaussian(0, 0.02);
            return new double[] { h11 + n1, h12 + n2 };
        }

        public double[] get_yc()
        {
            // apply measurement noise
            var r = new GaussianRandom();
            double n1 = r.NextGaussian(0, 0.02);
            double n2 = r.NextGaussian(0, 0.02);
            return new double[] { h21 + n1, h22 + n2 };
        }

        public void set_u(double[] u_)
        {
            u1 = u_[0];
            u2 = u_[1];
        }
    }

    public sealed class GaussianRandom
    {
        private bool _hasDeviate;
        private double _storedDeviate;
        private readonly Random _random;

        public GaussianRandom(Random random = null)
        {
            _random = random ?? new Random();
        }

        public double NextGaussian(double mu = 0, double sigma = 1)
        {
            if (sigma <= 0)
                throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

            if (_hasDeviate)
            {
                _hasDeviate = false;
                return _storedDeviate * sigma + mu;
            }

            double v1, v2, rSquared;
            do
            {
                // two random values between -1.0 and 1.0
                v1 = 2 * _random.NextDouble() - 1;
                v2 = 2 * _random.NextDouble() - 1;
                rSquared = v1 * v1 + v2 * v2;
                // ensure within the unit circle
            } while (rSquared >= 1 || rSquared == 0);

            // calculate polar tranformation for each deviate
            var polar = Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
            // store first deviate
            _storedDeviate = v2 * polar;
            _hasDeviate = true;
            // return second deviate
            return v1 * polar * sigma + mu;
        }
    }
}
