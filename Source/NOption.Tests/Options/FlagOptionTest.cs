namespace NOption.Tests.Options
{
    using Xunit;

    public class FlagOptionTest
    {
        [Fact]
        public void ConstructMinimal()
        {
            var option = new FlagOption(1, "-", "foo");

            Assert.Equal(1, option.Id);
            Assert.Equal("-", option.Prefix);
            Assert.Equal("foo", option.Name);
            Assert.Null(option.HelpText);
            Assert.Null(option.MetaVar);
            Assert.Null(option.Alias);
            Assert.Null(option.Group);
            Assert.Same(option, option.UnaliasedOption);
        }

        [Fact]
        public void ConstructOptional()
        {
            var option = new FlagOption(
                1, "-", "foo", helpText: "help", aliasId: 2, groupId: 3,
                metaVar: "meta");

            Assert.Equal(1, option.Id);
            Assert.Equal("-", option.Prefix);
            Assert.Equal("foo", option.Name);
            Assert.Equal("help", option.HelpText);
            Assert.Equal("meta", option.MetaVar);
            Assert.Null(option.Alias);
            Assert.Null(option.Group);
            Assert.Same(option, option.UnaliasedOption);

            var option2 = new FlagOption(2, "-", "bar");
            var option3 = new FlagOption(3, "-", "qux");

            var optTable = new OptTable(new[] { option, option2, option3 });
            Assert.Same(option2, option.Alias);
            Assert.Same(option3, option.Group);
        }

        [Fact]
        public void Accept()
        {
            var option = new FlagOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(new[] { "-foo" }, ref idx);

            Assert.Equal(1, idx);
            Assert.NotNull(arg);
            Assert.Same(option, arg.Option);
            Assert.Equal(0, arg.Index);
            Assert.False(arg.IsClaimed);
            Assert.Equal("-foo", arg.Spelling);
            Assert.Null(arg.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("/foo")]
        public void Accept_Returns_Null_On_UnhandledInput(string input)
        {
            var option = new FlagOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(new[] { input }, ref idx);

            Assert.Equal(0, idx);
            Assert.Null(arg);
        }

        [Theory]
        [InlineData("-bar", "-bar")]
        [InlineData("/bar", "/bar")]
        [InlineData("--bar", "--bar")]
        public void Alias(string input, string spelling)
        {
            var option = new FlagOption(1, new[] { "-", "/", "--" }, "foo");
            var option2 = new FlagOption(2, new[] { "-", "/", "--" }, "bar", aliasId: 1);
            var optTable = new OptTable(new[] { option, option2 });

            int idx = 0;
            var arg = option2.Accept(new[] { input }, ref idx);

            Assert.NotNull(arg);
            Assert.Equal(1, arg.Option.Id);
            Assert.Equal(spelling, arg.Spelling);
            Assert.Null(arg.Value);
        }
    }
}
