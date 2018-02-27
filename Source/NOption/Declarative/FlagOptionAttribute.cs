namespace NOption.Declarative
{
    using System;
    using System.Reflection;

    public class FlagOptionAttribute : OptionAttribute
    {
        private readonly string[] prefixes;

        public FlagOptionAttribute(string prefixedName)
        {
            if (prefixedName == null)
                throw new ArgumentNullException(nameof(prefixedName));

            if (!TryParse(prefixedName, out var prefix, out var name))
                throw new ArgumentException("Invalid name", nameof(prefixedName));

            Name = name;
            prefixes = new[] { prefix };
        }

        public FlagOptionAttribute(string prefix, string name)
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

        public bool DefaultValue { get; set; }

        public override bool AcceptsMember(MemberInfo member)
        {
            Type type;
            if (member is PropertyInfo property)
                type = property.PropertyType;
            else if (member is FieldInfo field)
                type = field.FieldType;
            else
                return false;

            return type == typeof(bool) || type == typeof(object);
        }

        public override void AddOption(int optionId, OptTableBuilder builder)
        {
            builder.AddFlag(optionId, Prefixes, Name, HelpText);
        }

        public override object GetValue(Type type, IArgumentList args, int optionId)
        {
            bool value = args.GetFlag(optionId, DefaultValue);
            return value;
        }
    }
}
