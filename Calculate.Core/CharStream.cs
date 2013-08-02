using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Calculate.Core
{
    [DebuggerStepThrough]
    internal sealed class CharStream
    {
        public CharStream(string input)
        {
            Input = input ?? "";
            Length = Input.Length;
            Index = 0;
        }

        public string Input { get; private set; }

        public int Length { get; private set; }

        public int Index { get; private set; }

        public Action CreateRestorePoint()
        {
            var index = Index;
            return () =>
            {
                Index = index;
            };
        }

        public char? Peek()
        {
            if (Index == Length)
                return null;
            return Input[Index];
        }

        public char? Read()
        {
            if (Index == Length)
                return null;
            return Input[Index++];
        }
    }
}