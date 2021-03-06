﻿using bfng.Lexing;
using CommandLine;

namespace bfng
{
    class RunOptions
    {
        [Value(0, MetaName = "file", Required = true, HelpText = "Befunge file to execute.")]
        public string File { get; set; }

        [Option('w', Default = Lexer.DefaultBufferWidth, HelpText = "The width of the program grid.")]
        public int ProgramWidth { get; set; }

        [Option('h', Default = Lexer.DefaultBufferHeight, HelpText = "The height of the program grid.")]
        public int ProgramHeight { get; set; }

        [Option('T', Default = false, HelpText = "Whether timing debug information should be printed.")]
        public bool PrintTiming { get; set; }

        [Option(Default = false, HelpText = "Suppresses interpreter output.")]
        public bool DisableInterpreterOutput { get; set; }
    }
}
