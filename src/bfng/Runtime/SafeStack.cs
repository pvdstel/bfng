using System.Collections.Generic;

namespace bfng.Runtime
{
    public static class SafeStack
    {
        public static T SafePop<T>(this Stack<T> stack, T orElse = default(T))
        {
            if (stack.TryPop(out T result))
            {
                return result;
            }
            return orElse;
        }

        public static T SafePeek<T>(this Stack<T> stack, T orElse = default(T))
        {
            if (stack.TryPeek(out T result))
            {
                return result;
            }
            return orElse;
        }
    }
}
