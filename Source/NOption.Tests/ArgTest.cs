namespace NOption.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ArgTest
    {
        private readonly OptTable optTable;

        public ArgTest()
        {
            optTable = new OptTable(GetOptions());
        }

        private static IEnumerable<Option> GetOptions()
        {
            return new[] { new JoinedOption(1, "-", "opt1=") };
        }

        [Fact]
        public void Properties()
        {
            var arg = new Arg(optTable.GetOption(1), "opt1=", 0, "value1");

            Assert.False(arg.IsClaimed);
            Assert.Equal(0, arg.Index);
            Assert.Equal("opt1=", arg.Spelling);
            Assert.Equal("value1", arg.Value);
            Assert.Equal(new[] { "value1" }, arg.Values.AsEnumerable());
        }

        [Fact]
        public void Claim()
        {
            var arg = new Arg(optTable.GetOption(1), "opt1=", 0, "value1");

            arg.Claim();

            Assert.True(arg.IsClaimed);
            Assert.Equal(0, arg.Index);
            Assert.Equal("opt1=", arg.Spelling);
            Assert.Equal("value1", arg.Value);
            Assert.Equal(new[] { "value1" }, arg.Values.AsEnumerable());
        }
    }
}
