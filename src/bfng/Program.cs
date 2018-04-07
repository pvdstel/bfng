﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace bfng
{
    public class Program
    {
        public Program(char[,] expressions)
        {
            Expressions = expressions;
            Width = expressions.GetLength(0);
            Height = expressions.GetLength(1);
        }

        public char[,] Expressions { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
