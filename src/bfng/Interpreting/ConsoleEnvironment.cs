using bfng.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace bfng.Interpreting
{
    public class ConsoleEnvironment : IEnvironment
    {
        public char InputCharacter()
        {
            return (char)Console.Read();
        }

        public int InputInteger()
        {
            return int.Parse(Console.ReadLine());
        }

        public void OutputCharacter(char c)
        {
            Console.Write(c);
        }

        public void OutputInteger(int i)
        {
            Console.Write(i);
        }
    }
}
