namespace bfng.Runtime
{
    public struct InstructionPointer
    {
        public InstructionPointer(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public InstructionPointer CreateNext(ExecutionDirection executionDirection, int width, int height)
        {
            switch (executionDirection)
            {
                case ExecutionDirection.Right:
                    return new InstructionPointer((X + 1) % width, Y);
                case ExecutionDirection.Left:
                    return new InstructionPointer(((X - 1) + width) % width, Y);
                case ExecutionDirection.Down:
                    return new InstructionPointer(X, (Y + 1) % height);
                case ExecutionDirection.Up:
                    return new InstructionPointer(X, ((Y - 1) + height) % height);
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(executionDirection));
            }
        }
    }
}
