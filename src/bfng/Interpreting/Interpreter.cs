using bfng.Parsing;
using bfng.Runtime;
using System;

namespace bfng.Interpreting
{
    public class Interpreter
    {
        private IEnvironment _environment;

        public Interpreter() : this(new ConsoleEnvironment()) { }

        public Interpreter(IEnvironment environment)
        {
            _environment = environment;
        }

        public void Execute(InstructionProgram program)
        {
            ExecutionContext context = new ExecutionContext(program);

            while (context.IsRunning)
            {
                Instruction currentInstruction = context.GetCurrentInstruction();
                currentInstruction(context, _environment);
                context.AdvanceInstructionPointer();
            }

            Console.WriteLine();
        }
    }
}
