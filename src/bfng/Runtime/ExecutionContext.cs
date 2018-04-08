using bfng.Parsing;
using System.Collections.Generic;

namespace bfng.Runtime
{
    public class ExecutionContext
    {
        public ExecutionContext(InstructionProgram program)
        {
            Program = program;
            MutatedInstructions = new Instruction[program.Width, program.Height];
            MutatedExpressions = new char?[program.Width, program.Height];
            Stack = new Stack<int>();

            IsRunning = true;
            ExecutionDirection = ExecutionDirection.Right;
            InstructionPointer = new InstructionPointer(0, 0);
        }

        public ExecutionContext(ExecutionContext executionContext)
        {
            Program = executionContext.Program;
            // Copy the instructions (shallow copy is fine)
            MutatedInstructions = (Instruction[,])executionContext.MutatedInstructions.Clone();
            // Copy the mutated expressions (shallow copy is fine)
            MutatedExpressions = (char?[,])executionContext.MutatedExpressions.Clone();
            Stack = new Stack<int>(executionContext.Stack);

            IsRunning = executionContext.IsRunning;
            ExecutionDirection = executionContext.ExecutionDirection;
            InstructionPointer = executionContext.InstructionPointer;
        }

        public void AdvanceInstructionPointer()
        {
            InstructionPointer = InstructionPointer.CreateNext(ExecutionDirection, Program.Width, Program.Height);
        }

        public Instruction GetInstruction(int x, int y)
        {
            return MutatedInstructions[x, y] ?? Program.Instructions[x, y];
        }

        public char GetExpression(int x, int y)
        {
            return MutatedExpressions[x, y] ?? Program.Expressions[x, y];
        }

        public Instruction GetCurrentInstruction()
        {
            return GetInstruction(InstructionPointer.X, InstructionPointer.Y);
        }

        public char GetCurrentExpression()
        {
            return GetExpression(InstructionPointer.X, InstructionPointer.Y);
        }

        public InstructionProgram Program { get; }
        public Instruction[,] MutatedInstructions { get; }
        public char?[,] MutatedExpressions { get; }
        public Stack<int> Stack { get; }

        public bool IsRunning { get; set; }
        public ExecutionDirection ExecutionDirection { get; set; }
        public InstructionPointer InstructionPointer { get; set; }
    }
}
