using bfng.Parsing;
using bfng.Runtime;
using ReactiveUI;
using System;

namespace bfng.dbgui.ViewModels
{
    public class SourceStatementViewModel : ViewModelBase
    {
        private int x;
        private int y;
        private char expression;
        private Instruction instruction;
        private bool active;
        private bool mutated;

        public SourceStatementViewModel(ExecutionContext executionContext, int x, int y)
        {
            X = x;
            Y = y;
            Expression = executionContext.GetExpression(x, y);
            Instruction = executionContext.GetInstruction(x, y);
            Active = executionContext.InstructionPointer.X == x &&
                executionContext.InstructionPointer.Y == y;
            Mutated = executionContext.MutatedInstructions[x, y] != null;
        }

        public SourceStatementViewModel Assign(SourceStatementViewModel other)
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
            get => x;
            set => this.RaiseAndSetIfChanged(ref x, value);
        }
        public int Y {
            get => y;
            set => this.RaiseAndSetIfChanged(ref y, value);
        }
        public char Expression
        {
            get => expression;
            set => this.RaiseAndSetIfChanged(ref expression, value);
        }
        public Instruction Instruction {
            get => instruction;
            set => this.RaiseAndSetIfChanged(ref instruction, value);
        }
        public bool Active
        {
            get => active;
            set => this.RaiseAndSetIfChanged(ref active, value);
        }
        public bool Mutated
        {
            get => mutated;
            set => this.RaiseAndSetIfChanged(ref mutated, value);
        }
    }
}
