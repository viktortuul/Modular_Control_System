using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canal_GUI
{
    class Bernoulli
    {
        double treshold;
        Random r = new Random();

        public Bernoulli(double treshold)
        {
            this.treshold = treshold;
        }

        public bool isPass()
        {
            return (treshold > r.NextDouble()) ? true : false;
        }
    }

    class MarkovChain
    {
        int state; // 1:pass, 0:drop
        double[] transition; // state transition probabilities
        Random r = new Random();

        public MarkovChain(int state, double P_pd, double P_dp)
        {
            this.state = state;
            transition = new double[] { P_dp, P_pd };
        }

        public bool isPass()
        {
            double p = r.NextDouble();
            if (p < transition[state]) state = (state + 1) % 2;
            return (state == 1) ? true : false;
        }
    }
}
