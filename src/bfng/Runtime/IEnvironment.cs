using System;
using System.Collections.Generic;
using System.Text;

namespace bfng.Runtime
{
    public interface IEnvironment
    {
        char InputCharacter();

        int InputInteger();

        void OutputCharacter(char c);

        void OutputInteger(int i);
    }
}
