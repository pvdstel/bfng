using bfng.Interpreting;
using bfng.Lexing;
using bfng.Parsing;
using CommandLine;
using System;
using System.Diagnostics;
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

            string programString = null;
            TimeAction(() => programString = File.ReadAllText(options.File), options.PrintTiming, "Read source file in {0} ms.");

            Lexer lexer = new Lexer(options.ProgramWidth, options.ProgramHeight);
            ExpressionProgram expressionProgram = null;
            TimeAction(() => expressionProgram = lexer.Lex(programString), options.PrintTiming, "Performed lexing in {0} ms.");

            InstructionProgram instructionProgram = null;
            TimeAction(() => instructionProgram = new InstructionProgram(expressionProgram), options.PrintTiming, "Performed parsing in {0} ms.");

            Interpreter interpreter = new Interpreter();
            TimeAction(() => interpreter.Execute(instructionProgram), options.PrintTiming, "Executed program in {0} ms.");
        }

        static void TimeAction(Action action, bool print, string message)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            action();

            sw.Stop();
            if (print) Console.WriteLine(string.Format(message, sw.ElapsedMilliseconds));

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
