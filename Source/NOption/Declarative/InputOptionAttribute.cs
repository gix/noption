namespace NOption.Declarative
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    ///   An input option with a value but no prefix. This kind is used for options
    ///   like <c>file.txt</c>.
    /// </summary>
    public class InputOptionAttribute : OptionAttribute
    {
        public InputOptionAttribute()
        {
            Name = "<input>";
        }

        public override bool AcceptsMember(MemberInfo member)
        {
            Type type;
            if (member is PropertyInfo property)
                type = property.PropertyType;
            else if (member is FieldInfo field)
                type = field.FieldType;
            else
                return false;

            return
                typeof(ICollection<string>).IsAssignableFrom(type) ||
                typeof(ICollection).IsAssignableFrom(type) ||
                typeof(string).IsAssignableFrom(type) ||
                typeof(object).IsAssignableFrom(type);
        }

        public override void AddOption(int optionId, OptTableBuilder builder)
        {
            builder.AddInput(optionId);
        }

        internal override void PopulateValue(
            IMemberRef target, int optionId, IArgumentList args)
        {
            if (!args.HasArg(optionId))
                return;

            bool allowMultiple =
                typeof(ICollection<string>).IsAssignableFrom(target.ValueType) ||
                typeof(ICollection).IsAssignableFrom(target.ValueType);

            if (allowMultiple)
                target.SetValue(args.GetAllArgValues(optionId));
            else
                target.SetValue(args.GetLastArgValue(optionId));
        }
    }
}
