using System;
using System.Collections.Generic;
using System.Text;

namespace bfng.Debugging
{
    public class DebuggerEnvironment : IEnvironment
    {
        private Debugger _debugger;

        public DebuggerEnvironment(Debugger debugger)
        {
            _debugger = debugger ?? throw new ArgumentNullException(nameof(debugger));
        }

        public DebuggerState DebuggerState { get; set; }

        public char InputCharacter()
        {
            throw new NotImplementedException();
        }

        public int InputInteger()
        {
            throw new NotImplementedException();
        }

        public void OutputCharacter(char c)
        {
            DebuggerState.Output.Append(c);
        }

        public void OutputInteger(int i)
        {
            DebuggerState.Output.Append(i);
        }
    }
}
