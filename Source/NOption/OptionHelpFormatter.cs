namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Collections;

    internal sealed class OptionHelpFormatter : IOptionHelpFormatter
    {
        private readonly TextWriter writer;

        public OptionHelpFormatter(TextWriter writer, WriteHelpSettings settings)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            this.writer = writer;
            Settings = settings;
        }

        public WriteHelpSettings Settings { get; }

        public void WriteHelp(IEnumerable<Option> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var groups = GroupOptions(options);
            int nameColumnWidth = GetMaxNameLength(groups);

            for (int i = 0; i < groups.Count; ++i) {
                if (i > 0)
                    writer.WriteLine();
                WriteGroup(groups[i], nameColumnWidth + 1);
            }

            writer.Flush();
        }

        private sealed class OptionHelp
        {
            public OptionHelp(string name, string helpText)
            {
                Name = name;
                HelpText = helpText;
            }

            public string Name { get; private set; }
            public string HelpText { get; }

            public void AddVariant(string name)
            {
                Name += ", " + name;
            }
        }

        private sealed class OptionGroup : List<OptionHelp>
        {
            public OptionGroup(string title)
            {
                Title = title;
            }

            public string Title { get; }
        }

        private IReadOnlyList<OptionGroup> GroupOptions(IEnumerable<Option> options)
        {
            Contract.Ensures(Contract.Result<IList<OptionGroup>>() != null);

            var groups = new OrderedDictionary<string, OptionGroup>();
            var helpMap = new Dictionary<Option, OptionHelp>();

            foreach (var opt in options) {
                if (opt.Kind == OptionKind.Group)
                    continue;

                int flags = opt.Flags;
                if (Settings.FlagsToInclude != 0 && (flags & Settings.FlagsToInclude) == 0)
                    continue;
                if ((flags & Settings.FlagsToExclude) != 0)
                    continue;

                // Ignore options without help text or which are aliases of an
                // option without help text.
                Option mainOpt;
                if (opt.HelpText != null)
                    mainOpt = opt;
                else if ((mainOpt = opt.Alias) != null && mainOpt.HelpText != null) {
                    // Empty
                } else
                    continue;

                string title = GetGroupTitle(mainOpt) ?? Settings.DefaultHelpGroup;
                if (!groups.ContainsKey(title))
                    groups.Add(title, new OptionGroup(title));

                string name = opt.GetHelpName(Settings.DefaultMetaVar);
                if (!helpMap.TryGetValue(mainOpt, out OptionHelp help)) {
                    helpMap[mainOpt] = help = new OptionHelp(name, mainOpt.HelpText);
                    groups[title].Add(help);
                } else
                    help.AddVariant(name);
            }

            return groups.Values;
        }

        private int GetMaxNameLength(IEnumerable<OptionGroup> optionGroups)
        {
            int maxLength = 0;
            foreach (var group in optionGroups) {
                foreach (var entry in group) {
                    // Skip titles.
                    if (entry.HelpText == null)
                        continue;

                    // Limit the amount of padding we are willing to give up for alignment.
                    int length = entry.Name.Length;
                    if (length <= Settings.NameColumnWidth)
                        maxLength = Math.Max(maxLength, length);
                }
            }
            return maxLength;
        }

        private static string GetGroupTitle(Option opt)
        {
            Option group = opt.Group;
            if (group == null)
                return null;

            if (group.HelpText != null)
                return group.HelpText;

            return GetGroupTitle(group);
        }

        private void WriteGroup(OptionGroup group, int nameColumnWidth)
        {
            writer.Write(group.Title);
            writer.WriteLine(":");

            foreach (OptionHelp entry in group) {
                string helpName = entry.Name;
                string helpText = entry.HelpText;

                int padding = nameColumnWidth - helpName.Length;
                writer.Write(Settings.IndentChars);
                writer.Write(helpName);

                // Break on long option names.
                if (padding < 0) {
                    writer.WriteLine();
                    padding = nameColumnWidth + Settings.IndentChars.Length;
                }

                int totalWidth = Settings.IndentChars.Length + nameColumnWidth + 1 + helpText.Length;
                if (totalWidth <= Settings.MaxLineLength) {
                    writer.Write(new string(' ', padding + 1));
                    writer.WriteLine(helpText);
                } else {
                    writer.Write(new string(' ', padding));
                    WriteTextBlock(helpText, nameColumnWidth);
                }
            }
        }

        private void WriteTextBlock(string text, int optionFieldWidth)
        {
            string[] words = text.Split(' ');
            int lineLength = optionFieldWidth;
            foreach (var word in words) {
                if (lineLength + word.Length + 1 > Settings.MaxLineLength) {
                    writer.WriteLine();
                    writer.Write(Settings.IndentChars);
                    writer.Write(new string(' ', optionFieldWidth));
                    lineLength = optionFieldWidth;
                }
                writer.Write(' ');
                writer.Write(word);
                lineLength += word.Length + 1;
            }
            writer.WriteLine();
        }
    }

    public static class OptionHelpFormatterExtensions
    {
        public static string GetHelp(this OptTable table)
        {
            var writer = new StringWriter();
            table.WriteHelp(writer);
            return writer.ToString();
        }

        public static void WriteHelp(this OptTable table, TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var formatter = new OptionHelpFormatter(writer, new WriteHelpSettings());
            table.WriteHelp(formatter);
        }

        public static void WriteHelp(this OptTable table, TextWriter writer, WriteHelpSettings settings)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var formatter = new OptionHelpFormatter(writer, settings);
            table.WriteHelp(formatter);
        }
    }
}
