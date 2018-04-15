using bfng.Parsing;
using bfng.Runtime;

namespace bfng.dbgui.Models
{
    public class Symbol
    {
        public Symbol(ExecutionContext executionContext, int x, int y, bool hasBreakpoint = false)
        {
            X = x;
            Y = y;
            Expression = executionContext.GetExpression(x, y);
            Instruction = executionContext.GetInstruction(x, y);
            IsActive = executionContext.IsRunning &&
                executionContext.InstructionPointer.X == x &&
                executionContext.InstructionPointer.Y == y;
            IsMutated = executionContext.MutatedInstructions[x, y] != null;
            HasBreakpoint = hasBreakpoint;
        }

        public Symbol(InstructionProgram program, int x, int y, bool hasBreakpoint = false)
        {
            X = x;
            Y = y;
            Expression = program.Expressions[x, y];
            Instruction = program.Instructions[x, y];
            IsActive = false;
            IsMutated = false;
            HasBreakpoint = hasBreakpoint;
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
        public bool IsActive
        {
            get;
            set;
        }
        public bool IsMutated
        {
            get;
            set;
        }
        public bool HasBreakpoint
        {
            get;
            set;
        }
    }
}
