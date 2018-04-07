using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace bfng
{
    class RunOptions
    {
        [Value(0, MetaName = "file", Required = true, HelpText = "Befunge file to execute.")]
        public string File { get; set; }
    }
}
