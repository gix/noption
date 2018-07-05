namespace NOption.Tests.Options
{
    using Xunit;

    public class FlagOptionTest
    {
        [Fact]
        public void Ctor()
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
        public void Ctor2()
        {
            var option = new FlagOption(1, "-", "foo", helpText: "help");

            Assert.Equal(1, option.Id);
            Assert.Equal("-", option.Prefix);
            Assert.Equal("foo", option.Name);
            Assert.Equal("help", option.HelpText);
            Assert.Null(option.MetaVar);
            Assert.Null(option.Alias);
            Assert.Null(option.Group);
            Assert.Same(option, option.UnaliasedOption);
        }

        [Fact]
        public void Accept()
        {
            var option = new FlagOption(1, "-", "foo");

            int idx = 0;
            var arg = option.Accept(new[] { "-foo" }, ref idx);

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

            Assert.Null(arg);
        }
    }
}
