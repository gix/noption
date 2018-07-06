namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   An option with a prefix and a value. The prefix and value are separated.
    ///   This kind is used for options like <c>-I value</c>, <c>--opt value</c>,
    ///   <c>/out value</c>.
    /// </summary>
    public class SeparateOption : Option
    {
        public SeparateOption(
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
                throw new ArgumentException("Contract violated: id.IsValid");
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Contract violated: !string.IsNullOrWhiteSpace(prefix)");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        public SeparateOption(
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            string helpText = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null,
            string metaVar = null)
            : base(id, prefixes, name, helpText: helpText, metaVar: metaVar,
                   aliasId: aliasId, groupId: groupId)
        {
            if (!id.IsValid)
                throw new ArgumentException("Contract violated: id.IsValid");
            if (prefixes == null)
                throw new ArgumentNullException(nameof(prefixes));
            if (prefixes.Count == 0)
                throw new ArgumentException("Contract violated: prefixes.Count != 0");
            if (prefixes.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Contract violated: !prefixes.Any(string.IsNullOrWhiteSpace)");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        protected override OptionRenderStyle RenderStyle
        {
            get
            {
                if (((OptionFlag)Flags & OptionFlag.RenderJoined) != 0)
                    return OptionRenderStyle.Joined;
                if (((OptionFlag)Flags & OptionFlag.RenderSeparate) != 0)
                    return OptionRenderStyle.Separate;

                return OptionRenderStyle.Separate;
            }
        }

        protected override Arg AcceptCore(
            IReadOnlyList<string> args, ref int argIndex, int argLen)
        {
            string argStr = args[argIndex];

            Option unaliasedOption = UnaliasedOption;
            string spelling = (Id == unaliasedOption.Id)
                ? argStr.Substring(0, argLen)
                : unaliasedOption.PrefixedName;

            // Require exact match.
            if (argLen != argStr.Length)
                return null;

            argIndex += 2;
            if (argIndex > args.Count)
                return null;

            return new Arg(unaliasedOption, spelling, argIndex - 2, args[argIndex - 1]);
        }

        public override string GetHelpName(string defaultMetaVar)
        {
            return PrefixedName + ' ' + (MetaVar ?? defaultMetaVar);
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddSeparate(
            this OptTableBuilder builder,
            OptSpecifier id,
            string prefix,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Contract violated: !string.IsNullOrWhiteSpace(prefix)");
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var option = new SeparateOption(
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

        public static OptTableBuilder AddSeparate(
            this OptTableBuilder builder,
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            if (prefixes == null)
                throw new ArgumentNullException(nameof(prefixes));
            if (prefixes.Count == 0)
                throw new ArgumentException("Contract violated: prefixes.Count != 0");
            if (prefixes.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Contract violated: !string.IsNullOrWhiteSpace(prefix)");
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var option = new SeparateOption(
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
