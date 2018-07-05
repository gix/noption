namespace NOption.Tests
{
    using System.Collections.Generic;
    using Xunit;
    using Xunit.Abstractions;

    public class VerbOptionTest
    {
        private readonly ITestOutputHelper output;

        public VerbOptionTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Verb()
        {
            var table = new OptTableBuilder()
                .Add(new VerbOption(1, "action", helpText: "Action", metaVar: "<action>"))
                .Add(new SeparateOption(2, "-", "name", helpText: "Name", metaVar: "foo"))
                .Add(new SeparateOption(3, "-", "foo", helpText: "Bar", metaVar: "foo"))
                .Add(new InputOption(4, metaVar: "foo"))
                .CreateTable();

            var al = table.ParseArgs(new[] { "bar", "-name", "n" }, out var _);

            output.WriteLine("Help: {0}", table.GetHelp());
            output.WriteLine("{0}", al.Count);
            output.WriteLine("{0}", al[0]);
            output.WriteLine("{0}", al[1]);
        }

        public class VerbOption : Option
        {
            public VerbOption(
                OptSpecifier id, string name, OptSpecifier? aliasId = null, OptSpecifier? groupId = null, string helpText = null, string metaVar = null)
                : base(id, name, aliasId, groupId, helpText, metaVar)
            {
            }

            protected override Arg AcceptCore(IReadOnlyList<string> args, ref int argIndex, int argLen)
            {
                return base.AcceptCore(args, ref argIndex, argLen);
            }
        }
    }
}
