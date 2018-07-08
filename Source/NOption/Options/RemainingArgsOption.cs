namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   An option with a prefix and all remaining argument values. The values
    ///   and prefix are separated. This kind is used for options like <c>-- all
    ///   remaining args</c> or <c>--args all remaining args</c>.
    /// </summary>
    public class RemainingArgsOption : Option
    {
        public RemainingArgsOption(
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
                throw new ArgumentException("Contract violated: !string.IsNullOrWhiteSpace(prefix)");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        public RemainingArgsOption(
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
                throw new ArgumentException("Invalid id");
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

        public override string GetHelpName(string defaultMetaVar)
        {
            return PrefixedName + ' ' + (MetaVar ?? defaultMetaVar);
        }

        protected override Arg AcceptCore(
            IReadOnlyList<string> args, ref int argIndex, int argLen)
        {
            string argStr = args[argIndex];

            Option unaliasedOption = UnaliasedOption;
            string spelling = argStr.Substring(0, argLen);

            // Require exact match.
            if (argLen != argStr.Length)
                return null;

            var values = args.Skip(argIndex + 1);
            var arg = new Arg(unaliasedOption, spelling, argIndex, values);
            argIndex = args.Count;
            return arg;
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddRemainingArgs(
            this OptTableBuilder builder,
            OptSpecifier id,
            string prefix,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new RemainingArgsOption(
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

        public static OptTableBuilder AddRemainingArgs(
            this OptTableBuilder builder,
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new RemainingArgsOption(
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
