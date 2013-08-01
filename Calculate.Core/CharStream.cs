using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core
{
    class CharStream
    {
        readonly Stack<int> _savedIndexes = new Stack<int>();
        readonly string _input;
        int _index;

        public CharStream(string input)
        {
            _input = input ?? "";
        }

        public void PushState()
        {
            _savedIndexes.Push(_index);
        }

        public void PopState()
        {
            if (_savedIndexes.Count > 0)
            {
                _index = _savedIndexes.Pop();
            }
        }

        public char? Peek()
        {
            if (_index == _input.Length)
                return null;
            return _input[_index];
        }

        public char? Read()
        {
            if (_index == _input.Length)
                return null;
            return _input[_index++];
        }
    }
}