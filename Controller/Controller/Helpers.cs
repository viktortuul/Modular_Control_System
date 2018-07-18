using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class Helpers
    {
    }

    public struct Constants
    {
        // time format
        public const string FMT = "yyyy-MM-dd HH:mm:ss.fff";

        // chart settings
        public const int n_steps = 100;
    }

    public struct PIDparameters
    {
        public double Kp, Ki, Kd;

        public PIDparameters(double Kp, double Ki, double Kd)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
        }
    }
}
