using CommandLine;
using System;
using System.IO;
using System.Linq;

namespace bfng
{
    class Runner
    {
        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<RunOptions>(args)
                .WithParsed(Run);
        }

        static void Run(RunOptions options)
        {
            Console.WriteLine(options.File);
        }
    }
}
