using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class Helpers
    {
        public static void Log(StringBuilder sb, string message, string log_flag)
        {
            if (log_flag == "true")
            {
                sb.Append(message + "\n");
                File.AppendAllText("log_sent.txt", sb.ToString());
                sb.Clear();
            }
        }

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
