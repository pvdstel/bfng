using System;
using System.Collections.Generic;
using System.Text;

namespace bfng.Utils
{
    public class DropOutStack<T>
    {
        private T[] items;
        private int top = 0;
        private int size = 0;

        public DropOutStack(int capacity)
        {
            items = new T[capacity];
        }

        public int Count => size;

        public void Push(T item)
        {
            ++size;
            items[top] = item;
            top = (top + 1) % items.Length;
        }

        public T Pop()
        {
            if (size == 0)
            {
                throw new InvalidOperationException("The stack is empty.");
            }
            --size;
            top = ((top - 1) + items.Length) % items.Length;
            return items[top];
        }
    }
}
