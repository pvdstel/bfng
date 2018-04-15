using System;

namespace bfng.Runtime
{
    public struct InstructionPointer : IEquatable<InstructionPointer>
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
                    throw new ArgumentOutOfRangeException(nameof(executionDirection));
            }
        }

        public override bool Equals(object obj)
        {
            return obj is InstructionPointer ip &&
                   X == ip.X &&
                   Y == ip.Y;
        }

        public bool Equals(InstructionPointer other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(InstructionPointer pointer1, InstructionPointer pointer2)
        {
            return pointer1.Equals(pointer2);
        }

        public static bool operator !=(InstructionPointer pointer1, InstructionPointer pointer2)
        {
            return !(pointer1 == pointer2);
        }
    }
}
