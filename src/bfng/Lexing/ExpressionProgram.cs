using bfng.Utils;

namespace bfng.Lexing
{
    public class ExpressionProgram
    {
        public ExpressionProgram(char[,] expressions)
        {
            Expressions = new Readonly2DArray<char>(expressions);
            Width = expressions.GetLength(0);
            Height = expressions.GetLength(1);
        }

        public Readonly2DArray<char> Expressions { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
