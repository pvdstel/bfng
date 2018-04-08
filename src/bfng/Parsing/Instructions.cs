using bfng.Runtime;
using System;

namespace bfng.Parsing
{
    public class Instructions
    {
        private static readonly Random random = new Random();

        public static readonly Instruction Addition = (e, env) => e.Stack.Push(e.Stack.SafePop() + e.Stack.SafePop());
        public static readonly Instruction Subtraction = (e, env) =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l - r);
        };
        public static readonly Instruction Multiplication = (e, env) => e.Stack.Push(e.Stack.SafePop() * e.Stack.SafePop());
        public static readonly Instruction Division = (e, env) =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l / r);
        };
        public static readonly Instruction Modulo = (e, env) =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l % r);
        };

        public static readonly Instruction Not = (e, env) => e.Stack.Push(e.Stack.SafePop() == 0 ? 1 : 0);
        public static readonly Instruction GreaterThan = (e, env) =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l > r ? 1 : 0);
        };

        public static readonly Instruction DirectionRight = (e, env) => e.ExecutionDirection = ExecutionDirection.Right;
        public static readonly Instruction DirectionLeft = (e, env) => e.ExecutionDirection = ExecutionDirection.Left;
        public static readonly Instruction DirectionUp = (e, env) => e.ExecutionDirection = ExecutionDirection.Up;
        public static readonly Instruction DirectionDown = (e, env) => e.ExecutionDirection = ExecutionDirection.Down;
        public static readonly Instruction DirectionRandom = (e, env) => e.ExecutionDirection = (ExecutionDirection)random.Next(4);

        public static readonly Instruction IfHorizontal = (e, env) => e.ExecutionDirection = e.Stack.SafePop() == 0 ? ExecutionDirection.Right : ExecutionDirection.Left;
        public static readonly Instruction IfVertical = (e, env) => e.ExecutionDirection = e.Stack.SafePop() == 0 ? ExecutionDirection.Down : ExecutionDirection.Up;

        public static readonly Instruction StringMode = (e, env) =>
        {
            e.AdvanceInstructionPointer();
            char currentExpression = e.GetCurrentExpression();
            while (currentExpression != '"')
            {
                e.Stack.Push(currentExpression);
                e.AdvanceInstructionPointer();
                currentExpression = e.GetCurrentExpression();
            }
        };

        public static readonly Instruction DuplicateStackTop = (e, env) => e.Stack.Push(e.Stack.SafePeek());
        public static readonly Instruction SwapStackStop = (e, env) =>
        {
            int a = e.Stack.SafePop();
            int b = e.Stack.SafePop();
            e.Stack.Push(a);
            e.Stack.Push(b);
        };
        public static readonly Instruction DiscardStackTop = (e, env) => e.Stack.SafePop();

        public static readonly Instruction OutputInteger = (e, env) => env.OutputInteger(e.Stack.SafePop());
        public static readonly Instruction OutputCharacter = (e, env) => env.OutputCharacter((char)e.Stack.SafePop());

        public static readonly Instruction Bridge = (e, env) => e.AdvanceInstructionPointer();

        public static readonly Instruction GetExpression = (e, env) =>
        {
            int y = e.Stack.SafePop();
            int x = e.Stack.SafePop();
            if (x >= 0 && x < e.Program.Width && y >= 0 && y < e.Program.Height)
            {
                e.Stack.Push(e.GetExpression(x, y));
            }
            else
            {
                e.Stack.Push(0);
            }
        };
        public static readonly Instruction PutExpression = (e, env) =>
        {
            int y = e.Stack.SafePop();
            int x = e.Stack.SafePop();
            int v = e.Stack.SafePop();
            if (x >= 0 && x < e.Program.Width && y >= 0 && y < e.Program.Height)
            {
                e.MutatedExpressions[x, y] = (char)v;
                e.MutatedInstructions[x, y] = new Parser().Parse((char)v);
            }
        };

        public static readonly Instruction InputInteger = (e, env) => e.Stack.Push(env.InputInteger());
        public static readonly Instruction InputCharacter = (e, env) => e.Stack.Push(env.InputCharacter());

        public static readonly Instruction End = (e, env) => e.IsRunning = false;

        public static Instruction BuildPush(int n) => (e, env) => e.Stack.Push(n);

        public static readonly Instruction NOP = (e, env) => { };

        public static readonly Instruction Unknown = NOP;
    }
}
