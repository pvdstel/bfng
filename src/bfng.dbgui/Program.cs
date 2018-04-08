using Avalonia;
using Avalonia.Logging.Serilog;
using bfng.dbgui.ViewModels;
using bfng.dbgui.Views;
using bfng.Lexing;
using bfng.Parsing;
using CommandLine;
using System;
using System.IO;

namespace bfng.dbgui
{
    class Program
    {
        internal static InstructionProgram BefungeProgram { get; private set; }
        internal static string BefungeProgramPath { get; private set; }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<RunOptions>(args)
               .WithParsed(Run);
        }

        static void Run(RunOptions options)
        {
            VerifyFileExists(options.File);
            BefungeProgramPath = Path.GetFullPath(options.File);

            Console.Write("Reading program source file...");
            string programString = File.ReadAllText(options.File);
            Console.WriteLine(" done.");

            Console.Write("Lexing program source...");
            Lexer lexer = new Lexer(options.ProgramWidth, options.ProgramHeight);
            ExpressionProgram expressionProgram = lexer.Lex(programString);
            Console.WriteLine(" done.");

            Console.Write("Parsing program expressions...");
            InstructionProgram instructionProgram = new InstructionProgram(expressionProgram);
            Console.WriteLine(" done.");

            BefungeProgram = instructionProgram;

            Console.Write("Launching debugging UI...");
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
            Console.WriteLine(" done. UI exited.");
        }

        static void VerifyFileExists(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("The Befunge file could not be found.");
                Environment.Exit(2);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
