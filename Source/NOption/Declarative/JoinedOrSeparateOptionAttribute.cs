namespace NOption.Declarative
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    ///   An option with a prefix and a value. The value and prefix are either
    ///   joined or separated. This kind is used for options that can behave
    ///   either like <see cref="JoinedOption"/> or <see cref="SeparateOption"/>
    ///   (e.g., <c>-Ipath</c> or <c>-I path</c>).
    /// </summary>
    public class JoinedOrSeparateOptionAttribute : PrefixedOptionAttribute
    {
        public JoinedOrSeparateOptionAttribute(string prefixedName)
            : base(prefixedName)
        {
        }

        public JoinedOrSeparateOptionAttribute(string prefix, string name)
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
            builder.AddJoinedOrSeparate(optionId, Prefixes, Name, HelpText);
        }

        internal override void PopulateValue(
            IMemberRef target, int optionId, IArgumentList args)
        {
            if (!args.HasArg(optionId))
                return;

            var converter = TypeDescriptor.GetConverter(target.ValueType);
            var value = args.GetLastArgValue(optionId, DefaultValue);
            target.SetValue(converter.ConvertFromInvariantString(value));
        }
    }
}
