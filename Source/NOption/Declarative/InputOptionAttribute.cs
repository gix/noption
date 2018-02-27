namespace NOption.Declarative
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;

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

        public override object GetValue(Type type, IArgumentList args, int optionId)
        {
            bool allowMultiple =
                typeof(ICollection<string>).IsAssignableFrom(type) ||
                typeof(ICollection).IsAssignableFrom(type);

            if (allowMultiple)
                return args.GetAllArgValues(optionId);

            if (args.Matching(optionId).Count() > 1)
                throw new OptionException();

            return args.GetLastArgValue(optionId);
        }
    }
}