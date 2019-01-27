

namespace GlobalComponents
{
    public class Animation
    {

    }

    public struct Tools
    {
        /*
        public class Arg
        {
            public string name;
            public string value;

            public Arg(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
        }
        */

        /*
        public Arg argsParser(string arg)
        {
            string[] arg_string = arg.Split(':');
            string name = arg_string[0];
            string value = arg_string[1];

            Arg arg_val = new Arg(name, value);
            return arg_val;
        }
        */
        public static string[] ArgsParser(string arg)
        {
            string[] arg_string = arg.Split(':');
            string name = arg_string[0];
            string value = arg_string[1];
            return new string[] { name, value };
        }
    }
}


