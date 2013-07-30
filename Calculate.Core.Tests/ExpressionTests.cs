using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Core.Tests
{
    [TestClass]
    public class ExpressionTests
    {
        private static readonly Dictionary<string, decimal> _expressions = new Dictionary<string, decimal> {
            { "123", 123m },
            { "1+2", 3m },
            { "1+2+3", 6m }
        };

        [TestMethod]
        public void RunExpressionTests()
        {
            foreach (var test in _expressions)
            {
                EvaluateExpression(test.Key, test.Value);
            }
        }

        void EvaluateExpression(string input, decimal expected)
        {
            try
            {
                var e = Expression.Parse(input);
                Assert.IsNotNull(e);
                Assert.AreEqual(expected, e.Evaluate());
            }
            catch (Exception ex)
            {
                Assert.Fail("input:{0}\nexpected:{1}\nexception:{2}", input, expected, ex);
            }
        }
    }
}
