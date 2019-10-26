using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalComponents
{
    public struct Tools
    {
        public static void AddKeyToDict(Dictionary<string, DataContainer> dict, string key, int n_steps)
        {
            // if the key doesn't exist in the data container, add it
            if (dict.ContainsKey(key) == false) dict.Add(key, new DataContainer(n_steps));
        }

        public static List<string> ArgsParser(string arg)
        {
            List<string> args_parsed = new List<string>();

            if (arg.Contains("="))
            {
                string[] arg_strings = arg.Split('=');
                string name = arg_strings[0]; // argument name
                string value = arg_strings[1]; // argument value
                args_parsed.Add(name);

                if (value.Contains(":")) // splits if the argument contains several values
                {
                    string[] value_strings;
                    value_strings = value.Split(':');

                    foreach (string item in value_strings)
                    {
                        args_parsed.Add(item);
                    }
                }
                else
                {
                    args_parsed.Add(value);
                }
            }
            else
            {
                args_parsed.Add("ARG_INVALID");
            }
            return args_parsed;
        }
    }
}
