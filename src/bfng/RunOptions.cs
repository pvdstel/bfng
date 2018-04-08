using bfng.Lexing;
using CommandLine;

namespace bfng
{
    class RunOptions
    {
        [Value(0, MetaName = "file", Required = true, HelpText = "Befunge file to execute.")]
        public string File { get; set; }

        [Option('w', Default = Lexer.DefaultBufferWidth, HelpText = "The width of the program.")]
        public int ProgramWidth { get; set; }

        [Option('h', Default = Lexer.DefaultBufferHeight, HelpText = "The height of the program.")]
        public int ProgramHeight { get; set; }
    }
}
