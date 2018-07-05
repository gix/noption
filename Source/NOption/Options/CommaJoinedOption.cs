namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   An option with a prefix and a variable number of values. The first
    ///   value and prefix are joined and each value is separated with a comma.
    ///   This kind is used for options like <c>--opt=value1,value2,value3</c>.
    /// </summary>
    public class CommaJoinedOption : Option
    {
        public CommaJoinedOption(
            OptSpecifier id,
            string prefix,
            string name,
            string helpText = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null,
            string metaVar = null)
            : base(id, prefix, name, helpText: helpText, metaVar: metaVar,
                   aliasId: aliasId, groupId: groupId)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Prefix must not be empty.");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        public CommaJoinedOption(
            OptSpecifier id,
            string[] prefixes,
            string name,
            string helpText = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null,
            string metaVar = null)
            : base(id, prefixes, name, helpText: helpText, metaVar: metaVar,
                   aliasId: aliasId, groupId: groupId)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            if (prefixes == null)
                throw new ArgumentNullException(nameof(prefixes));
            if (prefixes.Length == 0)
                throw new ArgumentException("At least one prefix must be specified");
            if (prefixes.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("All prefixes must be non-empty");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        public override string GetHelpName(string defaultMetaVar)
        {
            return PrefixedName + (MetaVar ?? defaultMetaVar);
        }

        protected override Arg AcceptCore(
            IReadOnlyList<string> args, ref int argIndex, int argLen)
        {
            string argStr = args[argIndex];

            Option unaliasedOption = UnaliasedOption;
            string spelling = (Id == unaliasedOption.Id)
                ? argStr.Substring(0, argLen)
                : unaliasedOption.PrefixedName;

            string[] values = argStr.Substring(argLen).Split(',');
            return new Arg(unaliasedOption, spelling, argIndex++, values);
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddCommaJoined(
            this OptTableBuilder builder,
            OptSpecifier id,
            string prefix,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new CommaJoinedOption(
                id.Id,
                prefix,
                name,
                helpText: helpText,
                metaVar: metaVar,
                aliasId: aliasId,
                groupId: groupId);
            builder.Add(option);
            return builder;
        }

        public static OptTableBuilder AddCommaJoined(
            this OptTableBuilder builder,
            OptSpecifier id,
            string[] prefixes,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new CommaJoinedOption(
                id.Id,
                prefixes,
                name,
                helpText: helpText,
                metaVar: metaVar,
                aliasId: aliasId,
                groupId: groupId);
            builder.Add(option);
            return builder;
        }
    }
}
