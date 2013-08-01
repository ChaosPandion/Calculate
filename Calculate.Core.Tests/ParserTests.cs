using System;
using Calculate.Core.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Core.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ExpectConstantExpressionNumber()
        {
            var e = Parser.Parse("1") as ConstantExpression;
            Assert.IsNotNull(e);
            Assert.AreEqual((Number)1m, e.Value);
        }

        [TestMethod]
        public void ExpectAdditionExpression()
        {
            var e = Parser.Parse("1+2") as BinaryExpression;
            Assert.IsNotNull(e);
            var left = e.LeftOperand as ConstantExpression;
            Assert.IsNotNull(e);
            Assert.AreEqual((Number)1m, left.Value);
            var right = e.RightOperand as ConstantExpression;
            Assert.IsNotNull(e);
            Assert.AreEqual((Number)2m, right.Value);
        }
    }
}