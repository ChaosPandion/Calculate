using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core
{
    static class Parser
    {
        public static Expression Parse(string input)
        {
            var lexer = new Lexer(input);
            if (!lexer.Next())
                return null;
            return ParseGrouped(lexer) ?? ParseUngrouped(lexer);
        }

        static Expression ParseUngrouped(Lexer lexer)
        {
            return ParseAdditiveExpression(lexer);
        }

        static Expression ParseAdditiveExpression(Lexer lexer)
        {
            lexer.PushState();
            var left = ParseMultiplicativeExpression(lexer);
            if (left == null)
            {
                return null;
            }
            var type = lexer.CurrentToken.Type;
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Dash:
                    break;
                default:
                    return left;
            }
            if (!lexer.Next())
            {
                lexer.PopState();
                return null;
            }
            var right = ParseAdditiveExpression(lexer);
            if (right == null)
            {
                lexer.PopState();
                return null;
            }
            Func<decimal, decimal, decimal> op = null;
            switch (type)
            {
                case TokenType.Plus:
                    op = (x, y) => x + y;
                    break;
                case TokenType.Dash:
                    op = (x, y) => x - y;
                    break;
            }
            var result = new BinaryExpression(left, right, op);
            return result;
        }

        static Expression ParseMultiplicativeExpression(Lexer lexer)
        {
            lexer.PushState();
            var left = ParseUnaryExpression(lexer);
            if (left == null)
            {
                return null;
            }

            var type = lexer.CurrentToken.Type;
            switch (type)
            {
                case TokenType.Multiply:
                case TokenType.Divide:
                    break;
                default:
                    return left;
            }
            if (!lexer.Next())
            {
                lexer.PopState();
                return null;
            }
            var right = ParseMultiplicativeExpression(lexer);
            if (right == null)
            {
                lexer.PopState();
                return null;
            }
            Func<decimal, decimal, decimal> op = null;
            switch (type)
            {
                case TokenType.Multiply:
                    op = (x, y) => x * y;
                    break;
                case TokenType.Divide:
                    op = (x, y) => x / y;
                    break;
            }
            var result = new BinaryExpression(left, right, op);
            return result;
        }

        static Expression ParseUnaryExpression(Lexer lexer)
        {
            lexer.PushState();
            var type = lexer.CurrentToken.Type;
            switch (type)
            {
                case TokenType.Negation:
                    if (!lexer.Next())
                    {
                        lexer.PopState();
                        return null;
                    }
                    break;
            }
            var operand = ParsePrimaryExpression(lexer);
            if (operand == null)
            {
                lexer.PopState();
                return null;
            }
            Func<decimal, decimal> op = null;
            switch (type)
            {
                case TokenType.Negation:
                    op = x => -x;
                    return new UnaryExpression(operand, op);
            }
            return operand;
        }

        static Expression ParsePrimaryExpression(Lexer lexer)
        {
            var result = ParseGrouped(lexer) ?? ParseConstantExpression(lexer);
            lexer.Next();
            return result;
        }

        static Expression ParseConstantExpression(Lexer lexer)
        {
            if (lexer.CurrentToken.Type != TokenType.Number)
            {
                return null;
            }
            return new ConstantExpression((decimal)lexer.CurrentToken.Value);
        }

        static Expression ParseGrouped(Lexer lexer)
        {
            lexer.PushState();

            if (lexer.CurrentToken.Type != TokenType.LeftParentheses || !lexer.Next())
            {
                lexer.PopState();
                return null;
            }

            var result = ParseUngrouped(lexer);

            if (result == null)
            {
                lexer.PopState();
                return null;
            }

            if (!lexer.Next() || lexer.CurrentToken.Type != TokenType.RightParentheses)
            {
                lexer.PopState();
                return null;
            }

            return result;
        }
    }
}