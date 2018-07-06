namespace NOption.Declarative
{
    using System;
    using System.Reflection;

    /// <summary>
    ///   A flag option with a prefix but no value. This kind is used for options
    ///   like <c>-x</c>, <c>--opt</c> or <c>/help</c>.
    /// </summary>
    public class FlagOptionAttribute : PrefixedOptionAttribute
    {
        public FlagOptionAttribute(string prefixedName)
            : base(prefixedName)
        {
        }

        public FlagOptionAttribute(string prefix, string name)
            : base(prefix, name)
        {
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

        internal override void PopulateValue(
            IMemberRef target, int optionId, IArgumentList args)
        {
            if (!args.HasArg(optionId))
                return;

            bool value = args.GetFlag(optionId, DefaultValue);
            target.SetValue(value);
        }
    }
}
