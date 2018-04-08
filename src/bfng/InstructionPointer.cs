using System;
using System.Collections.Generic;
using System.Text;

namespace bfng
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
