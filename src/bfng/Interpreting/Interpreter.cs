using bfng.Parsing;
using bfng.Runtime;
using System;

namespace bfng.Interpreting
{
    public class Interpreter
    {
        public void Execute(InstructionProgram program)
        {
            ExecutionContext context = new ExecutionContext(program);
            ConsoleEnvironment environment = new ConsoleEnvironment();

            while (context.IsRunning)
            {
                Instruction currentInstruction = context.GetCurrentInstruction();
                currentInstruction(context, environment);
                context.AdvanceInstructionPointer();
            }

            Console.WriteLine();
        }
    }
}
