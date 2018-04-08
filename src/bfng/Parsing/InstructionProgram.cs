using bfng.Lexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace bfng.Parsing
{
    public class InstructionProgram
    {
        public InstructionProgram(ExpressionProgram expressionProgram)
        {
            Expressions = expressionProgram.Expressions;
            Width = expressionProgram.Width;
            Height = expressionProgram.Height;

            Instructions = new Instruction[expressionProgram.Width, expressionProgram.Height];
            ParseExpressions();
        }

        public char[,] Expressions { get; }
        public Instruction[,] Instructions { get; }
        public int Width { get; }
        public int Height { get; }

        private void ParseExpressions()
        {
            Parser parser = new Parser();
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    char expression = Expressions[x, y];
                    Instructions[x, y] = parser.Parse(expression);
                }
            }
        }
    }
}
