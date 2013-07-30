using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Core.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void ParseSingleIntegerNumber()
        {
            var lexer = new Lexer("123");
            ValidateNextToken(lexer, TokenType.Number, 123m);
        }

        [TestMethod]
        public void ParseSingleAdditionExpression()
        {
            var lexer = new Lexer("123+124");
            ValidateNextToken(lexer, TokenType.Number, 123m);
            ValidateNextToken(lexer, TokenType.Plus);
            ValidateNextToken(lexer, TokenType.Number, 124m);
        }

        void ValidateNextToken(Lexer lexer, TokenType expectedType, object expectedValue = null)
        {
            Assert.IsTrue(lexer.Next());
            Assert.AreEqual(expectedType, lexer.CurrentToken.Type);
            Assert.AreEqual(expectedValue, lexer.CurrentToken.Value);
        }
    }
}