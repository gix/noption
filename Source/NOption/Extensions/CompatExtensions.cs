namespace NOption.Extensions
{
    using System;
    using System.Reflection;

    internal static class CompatExtensions
    {
        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
        }
    }
}
