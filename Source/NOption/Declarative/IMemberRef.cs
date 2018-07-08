namespace NOption.Declarative
{
    using System;
    using System.Reflection;

    public interface IMemberRef
    {
        object Target { get; }
        MemberInfo MemberInfo { get; }
        Type ValueType { get; }
        bool CanRead { get; }
        bool CanWrite { get; }
        object GetValue();
        void SetValue(object value);
    }
}
