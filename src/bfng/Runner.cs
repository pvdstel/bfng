using CommandLine;
using System;
using System.IO;

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
            VerifyFileExists(options.File);
        }

        static void VerifyFileExists(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("The Befunge file could not be found.");
                Environment.Exit(2);
            }
        }
    }
}
