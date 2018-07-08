namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   An option with a prefix and a value. The value and prefix are either
    ///   joined or separated. This kind is used for options that can behave
    ///   either like <see cref="JoinedOption"/> or <see cref="SeparateOption"/>
    ///   (e.g., <c>-Ipath</c> or <c>-I path</c>).
    /// </summary>
    public class JoinedOrSeparateOption : SeparateOption
    {
        public JoinedOrSeparateOption(
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

        public JoinedOrSeparateOption(
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
                throw new ArgumentException("Contract violated: !string.IsNullOrWhiteSpace(prefix)");
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

                return OptionRenderStyle.Joined;
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
            if (argStr.Length == argLen)
                return base.AcceptCore(args, ref argIndex, argLen);

            Option unaliasedOption = UnaliasedOption;
            string spelling = argStr.Substring(0, argLen);
            string value = argStr.Substring(argLen);

            return new Arg(unaliasedOption, spelling, argIndex++, value);
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddJoinedOrSeparate(
            this OptTableBuilder builder,
            OptSpecifier id,
            string prefix,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new JoinedOrSeparateOption(
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

        public static OptTableBuilder AddJoinedOrSeparate(
            this OptTableBuilder builder,
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new JoinedOrSeparateOption(
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
