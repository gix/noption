namespace NOption
{
    using System;
    using System.Collections.Generic;

    /// <summary>An input option with a value but no prefix.</summary>
    public sealed class InputOption : Option
    {
        public InputOption(
            OptSpecifier id,
            string helpText = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null,
            string metaVar = null)
            : base(id, "<input>", helpText: helpText, metaVar: metaVar,
                   aliasId: aliasId, groupId: groupId)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            Kind = OptionKind.Input;
        }

        protected override OptionRenderStyle RenderStyle
        {
            get
            {
                if (((OptionFlag)Flags & OptionFlag.RenderJoined) != 0)
                    return OptionRenderStyle.Joined;
                if (((OptionFlag)Flags & OptionFlag.RenderSeparate) != 0)
                    return OptionRenderStyle.Separate;

                return OptionRenderStyle.Values;
            }
        }

        public override string GetHelpName(string defaultMetaVar)
        {
            throw new Exception("Invalid option with help text.");
        }

        protected override Arg AcceptCore(
            IReadOnlyList<string> args, ref int argIndex, int argLen)
        {
            string argStr = args[argIndex];
            return new Arg(this, argStr, argIndex++, argStr);
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddInput(this OptTableBuilder builder, OptSpecifier id)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");

            builder.Add(new InputOption(id.Id));
            return builder;
        }
    }
}
