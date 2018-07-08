namespace NOption.Tests.Declarative
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Xunit;

    public class OptionSetTest
    {
        [Fact]
        public void Test1()
        {
            bool? help = null;
            string output = null;

            var opts = new OptionSet {
                { "?|h|help", "displays this message", v => { help = true; } },
                { "o=|out=", "output base name", v => output = v },
            };

            var args = new[] { "-unknown", "-?", "-out=foo" };

            List<string> extra = opts.Parse(args);

            Assert.True(help);
            Assert.Equal("foo", output);
            Assert.Equal(new[] { "-unknown" }, extra.AsEnumerable());
        }
    }

    public class OptionSet : KeyedCollection<string, Option>
    {
        private const int UnknownId = 1;
        private const int InputId = 2;

        private readonly OptTableBuilder builder;
        private readonly Dictionary<int, Tuple<Action<string>>> actions =
            new Dictionary<int, Tuple<Action<string>>>();
        private int nextOptionId = 3;

        public OptionSet()
        {
            builder = new OptTableBuilder();
            builder.AddUnknown(UnknownId);
            builder.AddInput(InputId);
        }

        protected override string GetKeyForItem(Option item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return item.Name;
        }

        public OptionSet Add(string prototype, string description, Action<string> action)
        {
            return Add(prototype, description, action, false);
        }

        public OptionSet Add(string prototype, string description, Action<string> action, bool hidden)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            string[] names = prototype.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int? mainId = null;
            foreach (var name in names) {
                int id = nextOptionId++;

                if (name.EndsWith("="))
                    builder.AddJoined(id, "-", name, helpText: description, aliasId: mainId);
                else
                    builder.AddFlag(id, "-", name, helpText: description, aliasId: mainId);

                mainId = mainId ?? id;
                actions.Add(id, Tuple.Create(action));
            }

            return this;
        }

        public List<string> Parse(IReadOnlyList<string> arguments)
        {
            var optTable = builder.CreateTable();

            IArgumentList al = optTable.ParseArgs(arguments, out _);

            foreach (var arg in al) {
                if (!actions.TryGetValue(arg.Option.Id, out var tuple))
                    continue;

                tuple.Item1(arg.Value);
            }

            return al.Matching(UnknownId).Select(a => a.Value).ToList();
        }
    }
}
