namespace bfng
{
    public interface IEnvironment
    {
        char InputCharacter();

        int InputInteger();

        void OutputCharacter(char c);

        void OutputInteger(int i);
    }
}
