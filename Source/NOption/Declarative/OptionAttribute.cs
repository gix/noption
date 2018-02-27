namespace NOption.Declarative
{
    using System;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public abstract class OptionAttribute : Attribute
    {
        private string name;

        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return name;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (!IsValidName(value))
                    throw new ArgumentException("Contract violated: IsValidName(value)");
                name = value;
            }
        }

        public int Id { get; set; }

        public string HelpText { get; set; }

        public static bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && !name.Any(char.IsWhiteSpace);
        }

        protected static bool TryParse(string prefixedName, out string prefix, out string name)
        {
            foreach (var p in new[] { "--", "-", "/" }) {
                if (prefixedName.Length <= p.Length ||
                    !prefixedName.StartsWith(p))
                    continue;

                prefix = p;
                name = prefixedName.Substring(p.Length);
                if (!IsValidName(name))
                    return false;
                return true;
            }

            prefix = null;
            name = null;
            return false;
        }

        public abstract bool AcceptsMember(MemberInfo member);

        public abstract void AddOption(int optionId, OptTableBuilder builder);

        public virtual object GetValue(MemberInfo member, IArgumentList args, int optionId)
        {
            Type type;
            if (member is PropertyInfo property)
                type = property.PropertyType;
            else if (member is FieldInfo field)
                type = field.FieldType;
            else
                throw new ArgumentException("Unsupported member type");

            return GetValue(type, args, optionId);
        }

        public abstract object GetValue(Type type, IArgumentList args, int optionId);

        public virtual void SetValue<T>(T obj, MemberInfo member, object value)
        {
            if (member is PropertyInfo property)
                property.SetValue(obj, value);
            else if (member is FieldInfo field)
                field.SetValue(obj, value);
            else
                throw new ArgumentException("Unsupported member type");
        }
    }
}