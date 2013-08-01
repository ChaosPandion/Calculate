using Xunit;
using Xunit.Extensions;

namespace Calculate.Core.Tests
{
    public class CharStreamTests
    {
        [Theory]
        [InlineData("123")]
        [InlineData("123.2")]
        [InlineData("123e-1")]
        [InlineData("123.2e-1")]
        public void CharStream_CanProperlyOrderCharacters(string input)
        {
            var cs = new CharStream(input);
            foreach (var c in input)
            {
                var found = cs.Read();
                Assert.True(found.HasValue);
                Assert.Equal(c, found.Value);
            }
            var end = cs.Read();
            Assert.True(!end.HasValue);
        }

        [Theory]
        [InlineData("12345", 0)]
        [InlineData("12345", 1)]
        [InlineData("12345", 2)]
        [InlineData("12345", 3)]
        [InlineData("12345", 4)]
        public void CharStream_CanRememberStates(string input, int savedIndex)
        {
            var cs = new CharStream(input);
            for (int i = 0; i < savedIndex; i++)
                cs.Read();
            var expectedChar = cs.Peek().Value;
            cs.PushState();
            while (cs.Read() != null) ;
            cs.PopState();
            var foundChar = cs.Peek();
            Assert.True(foundChar.HasValue);
            Assert.Equal(expectedChar, foundChar.Value);
        }
    }
}