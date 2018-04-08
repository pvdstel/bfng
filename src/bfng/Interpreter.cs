using System;
using System.Collections.Generic;
using System.Text;

namespace bfng
{
    public class Interpreter
    {
        public void Execute(Program program)
        {
            ExecutionContext context = new ExecutionContext(program);

            while (context.IsRunning)
            {
                Instruction currentInstruction = context.GetCurrentInstruction();
                currentInstruction(context);
                context.AdvanceInstructionPointer();
            }

            Console.WriteLine();
        }
    }
}
