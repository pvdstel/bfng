using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace bfng
{
    class RunOptions
    {
        private const int DefaultProgramWidth = 80;
        private const int DefaultProgramHeight = 25;

        [Value(0, MetaName = "file", Required = true, HelpText = "Befunge file to execute.")]
        public string File { get; set; }

        [Option('w', Default = DefaultProgramWidth, HelpText = "The width of the program.", Min = 1)]
        public int ProgramWidth { get; set; }

        [Option('h', Default = DefaultProgramHeight, HelpText = "The height of the program.", Min = 1)]
        public int ProgramHeight { get; set; }
    }
}
