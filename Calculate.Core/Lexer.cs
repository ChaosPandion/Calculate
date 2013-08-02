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
            Line = 1;
            Column = 1;
        }

        public int Line { get; private set; }

        public int Column { get; private set; }

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

        public Action CreateRestorePoint()
        {
            var restore = _input.CreateRestorePoint();
            var line = Line;
            var column = Column;
            return () =>
            {
                restore();
                Line = line;
                Column = column;
            };
        }

        public bool Next()
        {
            PassWhiteSpace();
            if (EndOfInput)
                return false;
            _currentToken = ParseOperator() ?? ParseNumber();
            PassWhiteSpace();
            return true;
        }

        char? Peek()
        {
            return _input.Peek();
        }

        char? Read()
        {
            var c = _input.Read();
            if (c != null)
            {
                Column++;
            }
            if (c == null)
                _currentToken = new Token(TokenType.End);
            return c;
        }

        void PassWhiteSpace()
        {
            while (true)
            {
                var c = _input.Peek();
                var lfCount = 0;

                while (c == ' ' | c == '\t')
                {
                    Column++;
                    _input.Read();
                    c = _input.Peek();
                }

                while (c == '\n')
                {
                    lfCount++;
                    Column = 1;
                    Line++;
                    _input.Read();
                    c = _input.Peek();
                }

                if (lfCount == 0)
                    return;
            }
        }

        Token ParseOperator()
        {
            TokenType type = TokenType.End;
            switch (Peek())
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
            Read();
            return new Token(type);
        }

        BigInteger? ParseInteger()
        {
            var result = default(BigInteger?);
            while (true)
            {
                var c = Peek();
                if (c == null || !char.IsDigit(c.Value))
                    break;
                result = result ?? BigInteger.Zero;
                result *= 10;
                result += c.Value - '0';
                Read();
            }
            return result;
        }

        Token ParseNumber()
        {
            var sb = new StringBuilder();

            var integerPart = ParseInteger();
            if (integerPart == null)
                return null;

            var fractional = 0m;
            if (!EndOfInput && Current == '.')
            {
                var c = Read().Value;
                var offset = 0.1m;
                while (!EndOfInput && char.IsDigit(Current))
                {
                    c = Read().Value;
                    fractional += (c - '0') * offset;
                    offset /= 10m;
                }
            }

            var result = (Number)integerPart.Value + (Number)fractional;

            var exponent = 1m;
            if (!EndOfInput)
            {
                switch (Current)
                {
                    case 'e':
                    case 'E':
                        Read();

                        var powerMod = 1;
                        switch (Current)
                        {
                            case '+':
                                Read();
                                break;
                            case '-':
                                Read();
                                powerMod = -1;
                                break;
                        }

                        sb.Clear();
                        while (!EndOfInput && char.IsDigit(Current))
                            sb.Append(Read().Value);
                        var power = powerMod * int.Parse(sb.ToString());

                        exponent = (decimal)Math.Pow(10, power);
                        result *= (Number)exponent;
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