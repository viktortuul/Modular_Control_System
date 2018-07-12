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
