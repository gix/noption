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
    public class JoinedOrSeparateOptionAttribute : OptionAttribute
    {
        private readonly string[] prefixes;

        public JoinedOrSeparateOptionAttribute(string prefixedName)
        {
            if (prefixedName == null)
                throw new ArgumentNullException(nameof(prefixedName));

            if (!TryParse(prefixedName, out var prefix, out var name))
                throw new ArgumentException("Invalid name", nameof(prefixedName));

            Name = name;
            prefixes = new[] { prefix };
        }

        public JoinedOrSeparateOptionAttribute(string prefix, string name)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!IsValidName(name))
                throw new ArgumentException("Invalid name", nameof(name));
            Name = name;
            prefixes = new[] { prefix };
        }

        public string[] Prefixes
        {
            get
            {
                Contract.Ensures(Contract.Result<string[]>() != null);
                Contract.Ensures(Contract.Result<string[]>().Length > 0);
                return prefixes;
            }
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
            var converter = TypeDescriptor.GetConverter(target.ValueType);
            var value = args.GetLastArgValue(optionId, DefaultValue);
            target.SetValue(converter.ConvertFromInvariantString(value));
        }
    }
}
