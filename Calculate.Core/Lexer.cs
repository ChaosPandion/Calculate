using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core
{
    class Lexer
    {
        private readonly CharStream _input;
        private Token _currentToken;

        public Lexer(string input)
        {
            _input = new CharStream(input);
        }

        public Token CurrentToken
        {
            get { return _currentToken; }
        }

        internal char Current
        {
            get { return (char)_input.Peek(); }
        }

        internal bool EndOfInput
        {
            get { return _input.Peek() == null; }
        }

        public void PushState()
        {
            _input.PushState();
        }

        public void PopState()
        {
            _input.PopState();
        }

        public bool Next()
        {
            if (EndOfInput)
                return false;
            _currentToken = ParseOperator() ?? ParseNumber();
            return true;
        }

        void PassWhiteSpace()
        {
            while (!EndOfInput && char.IsWhiteSpace(Current))
                _input.Read();
        }

        Token ParseOperator()
        {
            PassWhiteSpace();
            if (EndOfInput)
                return null;

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
            _input.Read();
            return new Token(type);
        }

        Token ParseNumber()
        {
            PassWhiteSpace();

            var sb = new StringBuilder();


            while (!EndOfInput && char.IsDigit(Current))
                sb.Append(_input.Read().Value);
            var integral = int.Parse(sb.ToString());

            

            var fractional = 0m;
            if (!EndOfInput && Current == '.')
            {
                var c = _input.Read().Value;
                var offset = 0.1m;
                while (!EndOfInput && char.IsDigit(Current))
                {
                    c = _input.Read().Value;
                    fractional += (c - '0') * offset;
                    offset /= 10m;
                }
            }

            var result = integral + fractional;

            var exponent = 1m;
            if (!EndOfInput)
            {
                switch (Current)
                {
                    case 'e':
                    case 'E':
                        _input.Read();

                        var powerMod = 1;
                        switch (Current)
                        {
                            case '+':
                                _input.Read();
                                break;
                            case '-':
                                _input.Read();
                                powerMod = -1;
                                break;
                        }

                        sb.Clear();
                        while (!EndOfInput && char.IsDigit(Current))
                            sb.Append(_input.Read().Value);
                        var power = powerMod * int.Parse(sb.ToString());

                        exponent = (decimal)Math.Pow(10, power);
                        result *= exponent;
                        break;
                }
            }

            return new Token(TokenType.Number, result);
        }
    }

    public enum TokenType
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


    [DebuggerStepThrough]
    class Token
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TokenType _type;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
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

        public override string ToString()
        {
            switch (Type)
            {
                case TokenType.Number:
                case TokenType.Identifier:
                    if (Value == null)
                        goto default;
                    return Value.ToString();
                case TokenType.LeftParentheses:
                    return "(";
                case TokenType.RightParentheses:
                    return ")";
                case TokenType.Plus:
                    return "+";
                case TokenType.Dash:                    
                    return "-";
                case TokenType.Multiply:
                    return "*";
                case TokenType.Divide:
                    return "/";
                default:
                    return "";
            }
        }
    }
}