using System;
using System.Collections.Generic;
using System.Text;

namespace bfng
{
    public class Instructions
    {
        private static Random _random = new Random();

        public static readonly Instruction Addition = e => e.Stack.Push(e.Stack.SafePop() + e.Stack.SafePop());
        public static readonly Instruction Subtraction = e =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l - r);
        };
        public static readonly Instruction Multiplication = e => e.Stack.Push(e.Stack.SafePop() * e.Stack.SafePop());
        public static readonly Instruction Division = e =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l / r);
        };
        public static readonly Instruction Modulo = e =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l % r);
        };

        public static readonly Instruction Not = e => e.Stack.Push(e.Stack.SafePop() == 0 ? 1 : 0);
        public static readonly Instruction GreaterThan = e =>
        {
            int r = e.Stack.SafePop();
            int l = e.Stack.SafePop();
            e.Stack.Push(l > r ? 1 : 0);
        };

        public static readonly Instruction DirectionRight = e => e.ExecutionDirection = ExecutionDirection.Right;
        public static readonly Instruction DirectionLeft = e => e.ExecutionDirection = ExecutionDirection.Left;
        public static readonly Instruction DirectionUp = e => e.ExecutionDirection = ExecutionDirection.Up;
        public static readonly Instruction DirectionDown = e => e.ExecutionDirection = ExecutionDirection.Down;
        public static readonly Instruction DirectionRandom = e => e.ExecutionDirection = (ExecutionDirection)_random.Next(4);

        public static readonly Instruction IfHorizontal = e => e.ExecutionDirection = e.Stack.SafePop() == 0 ? ExecutionDirection.Right : ExecutionDirection.Left;
        public static readonly Instruction IfVertical = e => e.ExecutionDirection = e.Stack.SafePop() == 0 ? ExecutionDirection.Down : ExecutionDirection.Up;

        public static readonly Instruction StringMode = e =>
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

        public static readonly Instruction DuplicateStackTop = e => e.Stack.Push(e.Stack.SafePeek());
        public static readonly Instruction SwapStackStop = e =>
        {
            int a = e.Stack.SafePop();
            int b = e.Stack.SafePop();
            e.Stack.Push(a);
            e.Stack.Push(b);
        };
        public static readonly Instruction DiscardStackTop = e => e.Stack.SafePop();

        public static readonly Instruction OutputInteger = e => Console.Write(e.Stack.SafePop());
        public static readonly Instruction OutputCharacter = e => Console.Write((char)e.Stack.SafePop());

        public static readonly Instruction Bridge = e => e.AdvanceInstructionPointer();

        public static readonly Instruction GetExpression = e =>
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
        public static readonly Instruction PutExpression = e =>
        {
            int y = e.Stack.SafePop();
            int x = e.Stack.SafePop();
            int v = e.Stack.SafePop();
            if (x >= 0 && x < e.Program.Width && y >= 0 && y < e.Program.Height)
            {
                e.MutatedExpressions[x, y] = (char)v;
                e.Instructions[x, y] = new Parser().Parse((char)v);
            }
        };

        public static readonly Instruction InputInteger = e => e.Stack.Push(int.Parse(Console.ReadLine()));
        public static readonly Instruction InputCharacter = e => e.Stack.Push(Console.Read());

        public static readonly Instruction End = e => e.IsRunning = false;

        public static Instruction BuildPush(int n) => e => e.Stack.Push(n);

        public static readonly Instruction NOP = e => { };

        public static readonly Instruction Unknown = NOP;
    }
}
