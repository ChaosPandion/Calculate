using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core
{
    class Lexer
    {
        private readonly Stack<int> _savedIndexes = new Stack<int>();
        private readonly string _input;
        private int _index;
        private Token _currentToken;

        public Lexer(string input)
        {
            _input = input;
        }

        public Token CurrentToken
        {
            get { return _currentToken; }
        }

        internal char Current
        {
            get { return _input[_index]; }
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

        public bool Next()
        {
            if (EndOfInput())
            {
                return false;
            }
            _currentToken = ParseOperator() ?? ParseNumber();
            return true;
        }

        void PassWhiteSpace()
        {
            if (EndOfInput())
                return;
            while (char.IsWhiteSpace(Current))
            {
                _index++;
                if (EndOfInput())
                    return;
            }
        }

        bool EndOfInput()
        {
            bool end = _index >= _input.Length;
            if (end && (_currentToken == null || _currentToken.Type != TokenType.End))
            {
                _currentToken = new Token(TokenType.End);
            }
            return end;
        }

        Token ParseOperator()
        {
            PassWhiteSpace();
            if (EndOfInput())
            {
                return null;
            } 
            TokenType type = TokenType.End;
            switch (Current)
            {
                case '+':
                    type = TokenType.Plus;
                    break;
                case '-':
                    type = TokenType.Dash;
                    break;
                case '*':
                    type = TokenType.Multiply;
                    break;
                case '/':
                    type = TokenType.Divide;
                    break;
                case '(':
                    type = TokenType.LeftParentheses;
                    break;
                case ')':
                    type = TokenType.RightParentheses;
                    break;
                default:
                    return null;
            }
            _index++;
            PassWhiteSpace();
            return new Token(type);
        }

        Token ParseNumber()
        {
            PassWhiteSpace();
            if (EndOfInput() || !char.IsDigit(Current))
            {
                return null;
            }
            var sb = new StringBuilder();
            do
            {
                sb.Append(Current);
                _index++;
                if (EndOfInput())
                    break;
            } while (char.IsDigit(Current));
            PassWhiteSpace();
            var value = decimal.Parse(sb.ToString());
            return new Token(TokenType.Number, value);
        }
    }

    enum TokenType
    {
        Number,
        Identifier,
        LeftParentheses,
        RightParentheses,
        Plus,
        Dash,
        Multiply,
        Divide,
        Negation,
        End
    }

    class Token
    {
        private readonly TokenType _type;
        private readonly object _value;

        public Token(TokenType type, object value = null)
        {
            _type = type;
            _value = value;
        }

        public TokenType Type
        {
            get { return _type; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}