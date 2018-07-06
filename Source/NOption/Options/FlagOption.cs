namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   A flag option with a prefix but no value. This kind is used for options
    ///   like <c>-x</c>, <c>--opt</c> or <c>/help</c>.
    /// </summary>
    public class FlagOption : Option
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="FlagOption"/> class.
        /// </summary>
        /// <param name="id">The option id.</param>
        /// <param name="prefix">The option prefix (e.g., "-" or "/").</param>
        /// <param name="name">The option name.</param>
        /// <param name="helpText">The option help text.</param>
        /// <param name="aliasId">Optional alias id.</param>
        /// <param name="groupId">Optional group id.</param>
        /// <param name="metaVar">Optional <see cref="Option.MetaVar"/> name.</param>
        public FlagOption(
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

        public FlagOption(
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

                return OptionRenderStyle.Separate;
            }
        }

        public override string GetHelpName(string defaultMetaVar)
        {
            return PrefixedName;
        }

        protected override Arg AcceptCore(
            IReadOnlyList<string> args, ref int argIndex, int argLen)
        {
            Option unaliasedOption = UnaliasedOption;
            string spelling = (Id == unaliasedOption.Id)
                ? args[argIndex].Substring(0, argLen)
                : unaliasedOption.PrefixedName;

            return new Arg(unaliasedOption, spelling, argIndex++);
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddFlag(
            this OptTableBuilder builder,
            OptSpecifier id,
            string prefix,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new FlagOption(
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

        public static OptTableBuilder AddFlag(
            this OptTableBuilder builder,
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new FlagOption(
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
