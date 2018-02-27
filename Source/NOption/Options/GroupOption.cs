namespace NOption
{
    using System;

    /// <summary>An option group.</summary>
    public sealed class GroupOption : Option
    {
        public GroupOption(
            OptSpecifier id,
            string name,
            string helpText = null,
            string metaVar = null)
            : base(id, name, helpText: helpText, metaVar: metaVar)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Kind = OptionKind.Group;
        }

        public override string GetHelpName(string defaultMetaVar)
        {
            throw new Exception("Invalid option with help text.");
        }
    }

    public static partial class OptTableBuilderExtensions
    {
        public static OptTableBuilder AddGroup(
            this OptTableBuilder builder,
            OptSpecifier id,
            string name,
            string helpText = null,
            string metaVar = null)
        {
            var option = new GroupOption(
                id.Id,
                name,
                helpText: helpText,
                metaVar: metaVar);
            builder.Add(option);
            return builder;
        }
    }
}
