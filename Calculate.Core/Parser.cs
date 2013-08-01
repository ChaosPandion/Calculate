using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculate.Core.Expressions;

namespace Calculate.Core
{
    static class Parser
    {
        public static Expression Parse(string input)
        {
            var lexer = new Lexer(input);
            if (!lexer.Next())
                return null;
            return ParseUngrouped(lexer);
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
                Debug.WriteLine("ParseAdditiveExpression: No MultiplicativeExpression was found.");
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
                Debug.WriteLine("ParseAdditiveExpression: No token following operator.");
                lexer.PopState();
                return null;
            }
            var right = ParseAdditiveExpression(lexer);
            if (right == null)
            {
                Debug.WriteLine("ParseAdditiveExpression: No expression following operator.");
                lexer.PopState();
                return null;
            }
            BinaryExpression result;
            switch (type)
            {
                case TokenType.Plus:
                    result = new BinaryExpression(left, right, BinaryOperator.Addition);
                    break;
                case TokenType.Dash:
                    result = new BinaryExpression(left, right, BinaryOperator.Subtraction);
                    break;
                default:
                    result = null;
                    break;
            }
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
            BinaryExpression result;
            switch (type)
            {
                case TokenType.Multiply:
                    result = new BinaryExpression(left, right, BinaryOperator.Multiplication);
                    break;
                case TokenType.Divide:
                    result = new BinaryExpression(left, right, BinaryOperator.Division);
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }

        static Expression ParseUnaryExpression(Lexer lexer)
        {
            lexer.PushState();
            var type = lexer.CurrentToken.Type;
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Dash:
                    if (!lexer.Next())
                    {
                        Debug.WriteLine("ParseUnaryExpression: No token following unary operator.");
                        lexer.PopState();
                        return null;
                    }
                    break;
            }
            var operand = ParsePrimaryExpression(lexer);
            if (operand == null)
            {
                Debug.WriteLine("ParseUnaryExpression: No operand following unary operator.");
                lexer.PopState();
                return null;
            }
            Expression result;
            switch (type)
            {
                case TokenType.Plus:
                    result = new UnaryExpression(operand, UnaryOperator.Plus);
                    break;
                case TokenType.Dash:
                    result = new UnaryExpression(operand, UnaryOperator.Minus);
                    break;
                default:
                    result = operand;
                    break;
            }
            return result;
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

            if (lexer.CurrentToken.Type != TokenType.RightParentheses)
            {
                lexer.PopState();
                return null;
            }

            result = new GroupedExpression(result);
            return result;
        }
    }
}