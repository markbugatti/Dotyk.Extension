using System;
using System.Reflection;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;

namespace Dotyk.Store.Cli
{
    class Program
    {
        private static readonly Regex PSArgRegex = new Regex(@"^-\w+$", RegexOptions.IgnoreCase);

        private static void FixArgsStyle(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (PSArgRegex.IsMatch(args[i]))
                    args[i] = "-" + args[i];
            }
        }

        static void DefineAndExecute(CommonOptions commonOptions)
        {
            commonOptions.Execute().Wait();
        }
    }
}
