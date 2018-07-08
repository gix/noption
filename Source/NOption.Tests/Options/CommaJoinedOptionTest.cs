namespace NOption.Tests.Options
{
    using Xunit;

    public class CommaJoinedOptionTest
    {
        [Fact]
        public void ConstructMinimal()
        {
            var option = new CommaJoinedOption(1, "-", "foo");

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
            var option = new CommaJoinedOption(
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

            var option2 = new CommaJoinedOption(2, "-", "bar");
            var option3 = new CommaJoinedOption(3, "-", "qux");

            var optTable = new OptTable(new[] { option, option2, option3 });
            Assert.Same(option2, option.Alias);
            Assert.Same(option3, option.Group);
        }

        [Theory]
        [InlineData("-foo:", new[] { "" })]
        [InlineData("-foo:1,2,3", new[] { "1", "2", "3" })]
        public void Accept(string input, string[] value)
        {
            var option = new CommaJoinedOption(1, "-", "foo:");

            int idx = 0;
            var arg = option.Accept(new[] { input }, ref idx);

            Assert.Equal(1, idx);
            Assert.NotNull(arg);
            Assert.Same(option, arg.Option);
            Assert.Equal(0, arg.Index);
            Assert.False(arg.IsClaimed);
            Assert.Equal("-foo:", arg.Spelling);
            Assert.Equal(value[0], arg.Value);
            Assert.Equal(value, arg.Values);
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("/foo")]
        public void Accept_Returns_Null_On_UnhandledInput(string input)
        {
            var option = new CommaJoinedOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(new[] { input }, ref idx);

            Assert.Equal(0, idx);
            Assert.Null(arg);
        }

        [Theory]
        [InlineData("-foo=1,2,3", "-foo=")]
        [InlineData("/foo=1,2,3", "/foo=")]
        [InlineData("--foo=1,2,3", "--foo=")]
        public void Alias(string input, string spelling)
        {
            var option = new CommaJoinedOption(1, new[] { "-", "/", "--" }, "foo:");
            var option2 = new CommaJoinedOption(2, new[] { "-", "/", "--" }, "foo=", aliasId: 1);
            var optTable = new OptTable(new[] { option, option2 });

            int idx = 0;
            var arg = option2.Accept(new[] { input }, ref idx);

            Assert.NotNull(arg);
            Assert.Equal(1, arg.Option.Id);
            Assert.Equal(spelling, arg.Spelling);
            Assert.Equal("1", arg.Value);
            Assert.Equal(new[] { "1", "2", "3" }, arg.Values);
        }
    }
}
