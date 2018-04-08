using bfng.Lexing;
using bfng.Utils;
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

            Instructions = new Readonly2DArray<Instruction>(ParseExpressions());
        }

        public Readonly2DArray<char> Expressions { get; }
        public Readonly2DArray<Instruction> Instructions { get; }
        public int Width { get; }
        public int Height { get; }

        private Instruction[,] ParseExpressions()
        {
            Instruction[,] instructions = new Instruction[Width, Height];
            Parser parser = new Parser();
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    char expression = Expressions[x, y];
                    instructions[x, y] = parser.Parse(expression);
                }
            }
            return instructions;
        }
    }
}
