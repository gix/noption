namespace NOption.Declarative
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using NOption.Extensions;

    /// <summary>
    ///   An option with a prefix and a variable number of values. The first
    ///   value and prefix are joined and each value is separated with a comma.
    ///   This kind is used for options like <c>--opt=value1,value2,value3</c>.
    /// </summary>
    public class CommaJoinedOptionAttribute : OptionAttribute
    {
        private readonly string[] prefixes;

        public CommaJoinedOptionAttribute(string prefixedName)
        {
            if (prefixedName == null)
                throw new ArgumentNullException(nameof(prefixedName));

            if (!TryParse(prefixedName, out var prefix, out var name))
                throw new ArgumentException("Invalid name", nameof(prefixedName));

            Name = name;
            prefixes = new[] { prefix };
        }

        public CommaJoinedOptionAttribute(string prefix, string name)
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

        public override bool AcceptsMember(MemberInfo member)
        {
            Type type;
            if (member is PropertyInfo property)
                type = property.PropertyType;
            else if (member is FieldInfo field)
                type = field.FieldType;
            else
                return false;

            return type.IsCollectionType();
        }

        public override void AddOption(int optionId, OptTableBuilder builder)
        {
            builder.AddCommaJoined(optionId, Prefixes, Name, HelpText);
        }

        internal override void PopulateValue(IMemberRef target, int optionId, IArgumentList args)
        {
            var values = args.GetAllArgValues(optionId);

            if (target.CanWrite) {
                target.SetValue(ConvertCollection(values, target.ValueType));
            } else if (target.CanRead) {
                throw new NotImplementedException();
            }
        }

        private object ConvertCollection(IList<string> values, Type targetType)
        {
            if (targetType.IsAssignableFrom(values.GetType()))
                return values;

            var elementType = targetType.GetListElementType();
            if (elementType == null)
                throw new OptionException($"Cannot convert string values to target collection type ({targetType})");

            TypeConverter converter = null;
            if (elementType != typeof(string))
                converter = TypeDescriptor.GetConverter(elementType);

            if (targetType.IsArray) {
                var array = Array.CreateInstance(targetType.GetElementType(), values.Count);
                for (int i = 0; i < values.Count; ++i)
                    array.SetValue(converter != null ? converter.ConvertFromInvariantString(values[i]) : values[i], i);

                return array;
            }

            if (targetType.GetTypeInfo().IsInterface) {
                throw new NotImplementedException();
            }

            var list = (IList)Activator.CreateInstance(targetType);
            foreach (var value in values)
                list.Add(converter != null ? converter.ConvertFromInvariantString(value) : value);

            return list;
        }
    }
}
