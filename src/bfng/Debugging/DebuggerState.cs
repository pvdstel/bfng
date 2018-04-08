using bfng.Parsing;
using bfng.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace bfng.Debugging
{
    public class DebuggerState
    {
        public DebuggerState(InstructionProgram program)
        {
            Output = new StringBuilder();
            ExecutionContext = new ExecutionContext(program);
        }

        public DebuggerState(DebuggerState programState)
        {
            Output = new StringBuilder(programState.Output.ToString());
            ExecutionContext = new ExecutionContext(programState.ExecutionContext);
        }

        public StringBuilder Output { get; }
        public ExecutionContext ExecutionContext { get; }
    }
}
