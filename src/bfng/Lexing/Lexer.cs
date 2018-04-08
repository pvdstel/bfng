using System;

namespace bfng.Lexing
{
    public class Lexer
    {
        public const int DefaultBufferWidth = 80;
        public const int DefaultBufferHeight = 25;
        public const char DefaultCharacter = ' ';

        public Lexer(int bufferWidth = DefaultBufferWidth, int bufferHeight = DefaultBufferHeight)
        {
            BufferWidth = Math.Max(1, bufferWidth);
            BufferHeight = Math.Max(1, bufferHeight);
        }

        public int BufferWidth { get; }
        public int BufferHeight { get; }

        public Program Lex(string programString)
        {
            char[,] expressions = new char[BufferWidth, BufferHeight];
            string[] programLines = programString.Split(Environment.NewLine);

            for (int x = 0; x < BufferWidth; ++x)
            {
                for (int y = 0; y < BufferHeight; ++y)
                {
                    expressions[x, y] = GetCharAtPosition(programLines, x, y);
                }
            }

            return new Program(expressions);
        }

        private char GetCharAtPosition(string[] lines, int x, int y)
        {
            if (lines.Length > y)
            {
                string line = lines[y];
                if (line.Length > x)
                {
                    return line[x];
                }
            }
            return DefaultCharacter;
        }
    }
}
