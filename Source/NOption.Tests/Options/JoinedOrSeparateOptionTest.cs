namespace NOption.Tests.Options
{
    using Xunit;

    public class JoinedOrSeparateOptionTest
    {
        [Fact]
        public void ConstructMinimal()
        {
            var option = new JoinedOrSeparateOption(1, "-", "foo");

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
            var option = new JoinedOrSeparateOption(
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

            var option2 = new JoinedOrSeparateOption(2, "-", "bar");
            var option3 = new JoinedOrSeparateOption(3, "-", "qux");

            var optTable = new OptTable(new[] { option, option2, option3 });
            Assert.Same(option2, option.Alias);
            Assert.Same(option3, option.Group);
        }

        [Theory]
        [InlineData(new[] { "-foobar" }, "bar", 1)]
        [InlineData(new[] { "-foobar", "qux" }, "bar", 1)]
        [InlineData(new[] { "-foo", "bar" }, "bar", 2)]
        [InlineData(new[] { "-foo", "bar", "qux" }, "bar", 2)]
        public void Accept(string[] input, string value, int argCount)
        {
            var option = new JoinedOrSeparateOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(input, ref idx);

            Assert.Equal(argCount, idx);
            Assert.NotNull(arg);
            Assert.Same(option, arg.Option);
            Assert.Equal(0, arg.Index);
            Assert.False(arg.IsClaimed);
            Assert.Equal("-foo", arg.Spelling);
            Assert.Equal(value, arg.Value);
        }

        [Theory]
        [InlineData(new[] { "" }, 0)]
        [InlineData(new[] { "foo", "bar" }, 0)]
        [InlineData(new[] { "/foo", "bar" }, 0)]
        [InlineData(new[] { "foobar" }, 0)]
        [InlineData(new[] { "/foobar" }, 0)]
        [InlineData(new[] { "-foo" }, 2)]
        public void Accept_Returns_Null_On_UnhandledInput(string[] input, int index)
        {
            var option = new JoinedOrSeparateOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(input, ref idx);

            Assert.Equal(index, idx);
            Assert.Null(arg);
        }

        [Theory]
        [InlineData(new[] { "-foobar" }, "-foo")]
        [InlineData(new[] { "/foobar" }, "/foo")]
        [InlineData(new[] { "--foobar" }, "--foo")]
        [InlineData(new[] { "-foo", "bar" }, "-foo")]
        [InlineData(new[] { "/foo", "bar" }, "/foo")]
        [InlineData(new[] { "--foo", "bar" }, "--foo")]
        public void Alias(string[] input, string spelling)
        {
            var option = new JoinedOrSeparateOption(1, new[] { "-", "/", "--" }, "qux");
            var option2 = new JoinedOrSeparateOption(2, new[] { "-", "/", "--" }, "foo", aliasId: 1);
            var optTable = new OptTable(new[] { option, option2 });

            int idx = 0;
            var arg = option2.Accept(input, ref idx);

            Assert.NotNull(arg);
            Assert.Equal(1, arg.Option.Id);
            Assert.Equal(spelling, arg.Spelling);
            Assert.Equal("bar", arg.Value);
        }
    }
}
