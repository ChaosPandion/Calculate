using Xunit;
using Xunit.Extensions;

namespace Calculate.Core.Tests
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1+2", 3.0)]
        [InlineData("1-2", -1.0)]
        [InlineData("1*2", 2.0)]
        [InlineData("1/4", 0.25)]
        public void Evaluator_CanEvaluateSimpleBinaryExpressions(string input, double expected)
        {
            var expression = Parser.Parse(input);
            Assert.NotNull(expression);
            var expectedNumber = (Number)expected;
            var actualNumber = Evaluator.Run(expression);
            Assert.Equal(expectedNumber, actualNumber);
        }
    }
}