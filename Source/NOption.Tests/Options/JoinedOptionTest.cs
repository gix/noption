namespace NOption.Tests.Options
{
    using Xunit;

    public class JoinedOptionTest
    {
        [Fact]
        public void ConstructMinimal()
        {
            var option = new JoinedOption(1, "-", "foo");

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
            var option = new JoinedOption(
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

            var option2 = new JoinedOption(2, "-", "bar");
            var option3 = new JoinedOption(3, "-", "qux");

            var optTable = new OptTable(new[] { option, option2, option3 });
            Assert.Same(option2, option.Alias);
            Assert.Same(option3, option.Group);
        }

        [Theory]
        [InlineData("-foo", "")]
        [InlineData("-foobar", "bar")]
        public void Accept(string input, string value)
        {
            var option = new JoinedOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(new[] { input }, ref idx);

            Assert.Equal(1, idx);
            Assert.NotNull(arg);
            Assert.Same(option, arg.Option);
            Assert.Equal(0, arg.Index);
            Assert.False(arg.IsClaimed);
            Assert.Equal("-foo", arg.Spelling);
            Assert.Equal(value, arg.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("/foo")]
        public void Accept_Returns_Null_On_UnhandledInput(string input)
        {
            var option = new JoinedOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(new[] { input }, ref idx);

            Assert.Equal(0, idx);
            Assert.Null(arg);
        }

        [Theory]
        [InlineData("-foo=bar", "-foo=")]
        [InlineData("/foo=bar", "/foo=")]
        [InlineData("--foo=bar", "--foo=")]
        public void Alias(string input, string spelling)
        {
            var option = new JoinedOption(1, new[] { "-", "/", "--" }, "foo:");
            var option2 = new JoinedOption(2, new[] { "-", "/", "--" }, "foo=", aliasId: 1);
            var optTable = new OptTable(new[] { option, option2 });

            int idx = 0;
            var arg = option2.Accept(new[] { input }, ref idx);

            Assert.NotNull(arg);
            Assert.Equal(1, arg.Option.Id);
            Assert.Equal(spelling, arg.Spelling);
            Assert.Equal("bar", arg.Value);
        }
    }
}
