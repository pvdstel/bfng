namespace bfng.Runtime
{
    public class InstructionPointer
    {
        public InstructionPointer(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}
