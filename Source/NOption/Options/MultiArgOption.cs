namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   An option with a prefix and a fixed number of values. The values and
    ///   prefix are separated. This kind is used for options like <c>--opt value1
    ///   value2 value3</c>.
    /// </summary>
    public class MultiArgOption : Option
    {
        public MultiArgOption(
            OptSpecifier id,
            string prefix,
            string name,
            int argCount,
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
            if (argCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(argCount), "Contract violated: argCount > 0");
            ArgCount = argCount;
        }

        public MultiArgOption(
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            int argCount,
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
            if (argCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(argCount), "Contract violated: argCount > 0");
            ArgCount = argCount;
        }

        public int ArgCount { get; }

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
            throw new Exception("Cannot print metavar for this kind of option.");
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

            argIndex += 1 + ArgCount;
            if (argIndex > args.Count)
                return null;

            return new Arg(
                unaliasedOption,
                spelling,
                argIndex - 1 - ArgCount,
                args.Skip(argIndex - ArgCount).Take(ArgCount));
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddMultiArg(
            this OptTableBuilder builder,
            OptSpecifier id,
            string prefix,
            string name,
            int argumentCount,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new MultiArgOption(
                id.Id,
                prefix,
                name,
                argumentCount,
                helpText: helpText,
                metaVar: metaVar,
                aliasId: aliasId,
                groupId: groupId);
            builder.Add(option);
            return builder;
        }

        public static OptTableBuilder AddMultiArg(
            this OptTableBuilder builder,
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            int argumentCount,
            string helpText = null,
            string metaVar = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null)
        {
            var option = new MultiArgOption(
                id.Id,
                prefixes,
                name,
                argumentCount,
                helpText: helpText,
                metaVar: metaVar,
                aliasId: aliasId,
                groupId: groupId);
            builder.Add(option);
            return builder;
        }
    }
}
