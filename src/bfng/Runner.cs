﻿using bfng.Interpreting;
using bfng.Lexing;
using bfng.Parsing;
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

            string programString = File.ReadAllText(options.File);
            Lexer lexer = new Lexer(options.ProgramWidth, options.ProgramHeight);

            ExpressionProgram expressionProgram = lexer.Lex(programString);
            InstructionProgram instructionProgram = new InstructionProgram(expressionProgram);

            Interpreter interpreter = new Interpreter();
            interpreter.Execute(instructionProgram);
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
