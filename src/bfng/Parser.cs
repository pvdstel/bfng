using static bfng.Instructions;

namespace bfng
{
    public class Parser
    {
        public Instruction Parse(char expression, bool strict = false)
        {
            switch (expression)
            {
                case '+': return Addition;
                case '-': return Subtraction;
                case '*': return Multiplication;
                case '/': return Division;
                case '%': return Modulo;

                case '!': return Not;
                case '`': return GreaterThan;

                case '>': return DirectionRight;
                case '<': return DirectionLeft;
                case 'v': return DirectionDown;
                case '^': return DirectionUp;
                case '?': return DirectionRandom;

                case '_': return IfHorizontal;
                case '|': return IfVertical;

                case '"': return StringMode;

                case ':': return DuplicateStackTop;
                case '\\': return SwapStackStop;
                case '$': return DiscardStackTop;

                case '.': return OutputInteger;
                case ',': return OutputCharacter;

                case '#': return Bridge;

                case 'g': return GetExpression;
                case 'p': return PutExpression;

                case '&': return InputInteger;
                case '~': return InputCharacter;

                case '@': return End;

                case char n when char.IsNumber(n): return BuildPush(n - '0');
                case char n when char.IsWhiteSpace(n): return NOP;

                default: return Unknown;
            }
        }
    }
}
