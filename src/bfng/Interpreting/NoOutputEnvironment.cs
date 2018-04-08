using System;

namespace bfng.Interpreting
{
    public class NoOutputEnvironment : IEnvironment
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
        }

        public void OutputInteger(int i)
        {
        }
    }
}
