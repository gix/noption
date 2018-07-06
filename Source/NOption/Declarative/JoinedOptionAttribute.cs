namespace NOption.Declarative
{
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    ///   An option with a prefix and a value. The prefix and value are joined.
    ///   This kind is used for options like <c>-Ivalue</c>, <c>--opt=value</c>
    ///   or <c>/out:value</c>.
    /// </summary>
    public class JoinedOptionAttribute : PrefixedOptionAttribute
    {
        public JoinedOptionAttribute(string prefixedName)
            : base(prefixedName)
        {
        }

        public JoinedOptionAttribute(string prefix, string name)
            : base(prefix, name)
        {
        }

        public string DefaultValue { get; set; }

        public override bool AcceptsMember(MemberInfo member)
        {
            switch (member) {
                case PropertyInfo _:
                    return true;
                case FieldInfo _:
                    return true;
                default:
                    return false;
            }
        }

        public override void AddOption(int optionId, OptTableBuilder builder)
        {
            builder.AddJoined(optionId, Prefixes, Name, HelpText);
        }

        internal override void PopulateValue(IMemberRef target, int optionId, IArgumentList args)
        {
            if (!args.HasArg(optionId))
                return;

            var converter = TypeDescriptor.GetConverter(target.ValueType);
            var value = args.GetLastArgValue(optionId, DefaultValue);
            target.SetValue(converter.ConvertFromInvariantString(value));
        }
    }
}
