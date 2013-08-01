using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Extensions;

namespace Calculate.Core.Tests
{
    public class LexerTests
    {
        [Theory]
        [InlineData("123", 123)]
        [InlineData("123.2", 123.2)]
        [InlineData("123e-1", 123e-1)]
        [InlineData("123e+1", 123e+1)]
        [InlineData("123e1", 123e1)]
        [InlineData("123.2e-1", 123.2e-1)]
        public void Lexer_CanIdentifyNumberTokens(string input, double expectedValue)
        {
            var lexer = new Lexer(input);
            Assert.IsTrue(lexer.Next());
            Assert.AreEqual(TokenType.Number, lexer.CurrentToken.Type);
            Assert.AreEqual(expectedValue, Convert.ToDouble(lexer.CurrentToken.Value));
        }

        [Theory]
        [InlineData("(", TokenType.LeftParentheses)]
        [InlineData(")", TokenType.RightParentheses)]
        [InlineData("+", TokenType.Plus)]
        [InlineData("-", TokenType.Dash)]
        [InlineData("*", TokenType.Multiply)]
        [InlineData("/", TokenType.Divide)]
        public void Lexer_CanIdentifyOperatorTokens(string input, TokenType expectedType)
        {
            var lexer = new Lexer(input);
            Assert.IsTrue(lexer.Next());
            Assert.AreEqual(expectedType, lexer.CurrentToken.Type);
        }
    }
}