namespace NOption.Declarative
{
    using System;
    using System.Reflection;

    internal static class MemberRef
    {
        public static IMemberRef Create(MemberInfo member, object target)
        {
            switch (member) {
                case PropertyInfo property:
                    return new PropertyRef(property, target);
                case FieldInfo field:
                    return new FieldRef(field, target);
                default:
                    return null;
            }
        }
    }

    internal sealed class PropertyRef : IMemberRef
    {
        private readonly PropertyInfo property;

        public PropertyRef(PropertyInfo property, object target)
        {
            this.property = property;
            Target = target;
        }

        public object Target { get; }

        public MemberInfo MemberInfo => property;
        public Type ValueType => property.PropertyType;
        public bool CanRead => property.CanRead;
        public bool CanWrite => property.CanWrite;

        public object GetValue()
        {
            return property.GetValue(Target);
        }

        public void SetValue(object value)
        {
            property.SetValue(Target, value);
        }
    }

    internal sealed class FieldRef : IMemberRef
    {
        private readonly FieldInfo field;

        public FieldRef(FieldInfo field, object target)
        {
            this.field = field;
            Target = target;
        }

        public object Target { get; }
        public MemberInfo MemberInfo => field;
        public Type ValueType => field.FieldType;
        public bool CanRead => true;
        public bool CanWrite => !field.IsInitOnly;

        public object GetValue()
        {
            return field.GetValue(Target);
        }

        public void SetValue(object value)
        {
            field.SetValue(Target, value);
        }
    }
}
