using bfng.Parsing;
using bfng.Runtime;
using ReactiveUI;
using System;

namespace bfng.dbgui.Models
{
    public class Symbol
    {
        public Symbol(ExecutionContext executionContext, int x, int y)
        {
            X = x;
            Y = y;
            Expression = executionContext.GetExpression(x, y);
            Instruction = executionContext.GetInstruction(x, y);
            Active = executionContext.IsRunning &&
                executionContext.InstructionPointer.X == x &&
                executionContext.InstructionPointer.Y == y;
            Mutated = executionContext.MutatedInstructions[x, y] != null;
        }

        public Symbol Assign(Symbol other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            X = other.X;
            Y = other.Y;
            Expression = other.Expression;
            Instruction = other.Instruction;
            Active = other.Active;
            Mutated = other.Mutated;

            return this;
        }

        public int X
        {
            get;
            set;
        }
        public int Y
        {
            get;
            set;
        }
        public char Expression
        {
            get;
            set;
        }
        public Instruction Instruction
        {
            get;
            set;
        }
        public bool Active
        {
            get;
            set;
        }
        public bool Mutated
        {
            get;
            set;
        }
    }
}
