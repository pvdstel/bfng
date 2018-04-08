using bfng.Lexing;
using bfng.Parsing;
using System.Collections.Generic;

namespace bfng.Runtime
{
    public class ExecutionContext
    {
        public ExecutionContext(Program program)
        {
            Program = program;
            Instructions = new Instruction[program.Width, program.Height];
            MutatedExpressions = new char?[program.Width, program.Height];
            Stack = new Stack<int>();

            IsRunning = true;
            ExecutionDirection = ExecutionDirection.Right;
            InstructionPointer = new InstructionPointer(0, 0);

            ParseExpressions();
        }

        public ExecutionContext(ExecutionContext executionContext)
        {
            Program = executionContext.Program;
            // Copy the instructions (shallow copy is fine)
            Instructions = (Instruction[,])executionContext.Instructions.Clone();
            // Copy the mutated expressions (shallow copy is fine)
            MutatedExpressions = (char?[,])executionContext.MutatedExpressions.Clone();
            Stack = new Stack<int>(executionContext.Stack);

            IsRunning = executionContext.IsRunning;
            ExecutionDirection = executionContext.ExecutionDirection;
            InstructionPointer = executionContext.InstructionPointer;
        }

        public void AdvanceInstructionPointer()
        {
            switch (ExecutionDirection)
            {
                case ExecutionDirection.Right:
                    InstructionPointer = new InstructionPointer((InstructionPointer.X + 1) % Program.Width, InstructionPointer.Y);
                    break;
                case ExecutionDirection.Left:
                    InstructionPointer = new InstructionPointer(((InstructionPointer.X - 1) + Program.Width) % Program.Width, InstructionPointer.Y);
                    break;
                case ExecutionDirection.Down:
                    InstructionPointer = new InstructionPointer(InstructionPointer.X, (InstructionPointer.Y + 1) % Program.Height);
                    break;
                case ExecutionDirection.Up:
                    InstructionPointer = new InstructionPointer(InstructionPointer.X, ((InstructionPointer.Y - 1) + Program.Height) % Program.Height);
                    break;
            }
        }

        public Instruction GetInstruction(int x, int y)
        {
            return Instructions[x, y];
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

        private void ParseExpressions()
        {
            Parser parser = new Parser();
            for (int x = 0; x < Program.Width; ++x)
            {
                for (int y = 0; y < Program.Height; ++y)
                {
                    char expression = Program.Expressions[x, y];
                    Instructions[x, y] = parser.Parse(expression);
                }
            }
        }

        public Program Program { get; }
        public Instruction[,] Instructions { get; }
        public char?[,] MutatedExpressions { get; }
        public Stack<int> Stack { get; }

        public bool IsRunning { get; set; }
        public ExecutionDirection ExecutionDirection { get; set; }
        public InstructionPointer InstructionPointer { get; set; }
    }
}
