namespace NOption.Tests.Declarative
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using NOption.Declarative;
    using Xunit;

    public class OptionAttributesTest
    {
        private sealed class Options
        {
            [InputOption(HelpText = "foo")]
            public string Input { get; set; }

            [FlagOption("-flag1", HelpText = "Flag bool", DefaultValue = false)]
            public bool Flag1 { get; set; }

            [FlagOption("-flag2", HelpText = "Flag object", DefaultValue = true)]
            public object Flag2 { get; set; }

            [JoinedOption("-strjoin:", HelpText = "Joined String", DefaultValue = "DefVal")]
            public string JoinedString { get; set; }

            [JoinedOption("-intjoin=", HelpText = "Joined Int", DefaultValue = "42")]
            public int JoinedInt { get; set; }

            [CommaJoinedOption("-cjint1=", HelpText = "Comma Joined Int1")]
            public int[] CommaJoinedInt1 { get; set; }

            [CommaJoinedOption("-cjint2=", HelpText = "Comma Joined Int2")]
            public List<int> CommaJoinedInt2 { get; set; }
        }

        [Fact]
        public void CommaJoinedInt()
        {
            var args = new[] { "-cjint1=1,2,3", "-cjint2=4,5,6" };

            var options = new Options();
            var al = CommandLineParser.Parse(args, options);

            Assert.Equal(new[] { 1, 2, 3 }, options.CommaJoinedInt1);
            Assert.Equal(new[] { 4, 5, 6 }, options.CommaJoinedInt2);
            Assert.True(al.All(x => x.IsClaimed));
        }

        [Fact]
        public void Multiple()
        {
            var args = new[] { "-flag1", "-flag2", "-strjoin:abc", "-intjoin=23", "foo.txt" };

            var options = new Options();
            var al = CommandLineParser.Parse(args, options);

            Assert.Equal(true, options.Flag1);
            Assert.Equal(true, options.Flag2);
            Assert.Equal("abc", options.JoinedString);
            Assert.Equal(23, options.JoinedInt);
            Assert.Equal("foo.txt", options.Input);
            Assert.True(al.All(x => x.IsClaimed));
        }

        [Fact]
        public void Defaults()
        {
            var args = new string[0];

            var options = new Options();
            var al = CommandLineParser.Parse(args, options);

            Assert.Equal(false, options.Flag1);
            Assert.Equal(true, options.Flag2);
            Assert.Equal("DefVal", options.JoinedString);
            Assert.Equal(42, options.JoinedInt);
            Assert.Null(options.Input);
            Assert.Empty(al);
        }

        [Fact]
        public void Clobbering()
        {
            var args = new[] { "-strjoin:abc", "-strjoin:def", "-intjoin=23", "-intjoin=24", "foo.txt" };

            var options = new Options();
            var al = CommandLineParser.Parse(args, options);

            Assert.Equal("def", options.JoinedString);
            Assert.Equal(24, options.JoinedInt);
            Assert.Equal("foo.txt", options.Input);
            Assert.True(al.All(x => x.IsClaimed));
        }
    }
}
