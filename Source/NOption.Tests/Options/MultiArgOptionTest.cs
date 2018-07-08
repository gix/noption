namespace NOption.Tests.Options
{
    using Xunit;

    public class MultiArgOptionTest
    {
        [Fact]
        public void ConstructMinimal()
        {
            var option = new MultiArgOption(1, "-", "foo", 3);

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
            var option = new MultiArgOption(
                1, "-", "foo", 3, helpText: "help", aliasId: 2, groupId: 3,
                metaVar: "meta");

            Assert.Equal(1, option.Id);
            Assert.Equal("-", option.Prefix);
            Assert.Equal("foo", option.Name);
            Assert.Equal("help", option.HelpText);
            Assert.Equal("meta", option.MetaVar);
            Assert.Null(option.Alias);
            Assert.Null(option.Group);
            Assert.Same(option, option.UnaliasedOption);

            var option2 = new MultiArgOption(2, "-", "bar", 3);
            var option3 = new MultiArgOption(3, "-", "qux", 3);

            var optTable = new OptTable(new[] { option, option2, option3 });
            Assert.Same(option2, option.Alias);
            Assert.Same(option3, option.Group);
        }

        [Theory]
        [InlineData(new[] { "-foo", "1", "2", "3", "4" }, 1, new[] { "1" })]
        [InlineData(new[] { "-foo", "1", "2", "3", "4" }, 2, new[] { "1", "2" })]
        [InlineData(new[] { "-foo", "1", "2", "3", "4" }, 3, new[] { "1", "2", "3" })]
        [InlineData(new[] { "-foo", "", "2", "3", "4" }, 3, new[] { "", "2", "3" })]
        public void Accept(string[] input, int argCount, string[] values)
        {
            var option = new MultiArgOption(1, "-", "foo", argCount);

            int idx = 0;
            var arg = option.Accept(input, ref idx);

            Assert.Equal(argCount + 1, idx);
            Assert.NotNull(arg);
            Assert.Same(option, arg.Option);
            Assert.Equal(0, arg.Index);
            Assert.False(arg.IsClaimed);
            Assert.Equal("-foo", arg.Spelling);
            Assert.Equal(values[0], arg.Value);
            Assert.Equal(values, arg.Values);
        }

        [Theory]
        [InlineData(new[] { "" }, 0)]
        [InlineData(new[] { "foo", "1", "2" }, 0)]
        [InlineData(new[] { "/foo", "1", "2" }, 0)]
        [InlineData(new[] { "-foo" }, 3)]
        [InlineData(new[] { "-foo", "1" }, 3)]
        public void Accept_Returns_Null_On_UnhandledInput(string[] input, int index)
        {
            var option = new MultiArgOption(1, "-", "foo", 2);

            int idx = 0;
            var arg = option.Accept(input, ref idx);

            Assert.Equal(index, idx);
            Assert.Null(arg);
        }

        [Theory]
        [InlineData(new[] { "-foo", "1", "2" }, "-foo")]
        [InlineData(new[] { "/foo", "1", "2" }, "/foo")]
        [InlineData(new[] { "--foo", "1", "2" }, "--foo")]
        public void Alias(string[] input, string spelling)
        {
            var option = new MultiArgOption(1, new[] { "-", "/", "--" }, "qux", 2);
            var option2 = new MultiArgOption(2, new[] { "-", "/", "--" }, "foo", 2, aliasId: 1);
            var optTable = new OptTable(new[] { option, option2 });

            int idx = 0;
            var arg = option2.Accept(input, ref idx);

            Assert.NotNull(arg);
            Assert.Equal(1, arg.Option.Id);
            Assert.Equal(spelling, arg.Spelling);
            Assert.Equal("1", arg.Value);
            Assert.Equal(new[] { "1", "2" }, arg.Values);
        }
    }
}
