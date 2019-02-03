using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class Helpers
    {
        public static bool isDouble(string str)
        {
            try
            {
                Double.Parse(str);
                return true;
            }
            catch { return false; }
        }
    }
}
