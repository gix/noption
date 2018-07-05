namespace NOption.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class ReflectionExtensions
    {
        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
        }

        public static bool IsCollectionType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeof(ICollection).IsAssignableFrom(type))
                return true;
            if (!typeInfo.IsGenericType)
                return false;

            var genericType = typeInfo.GetGenericTypeDefinition();
            return typeof(ICollection<>).IsAssignableFrom(genericType);
        }

        public static Type GetListElementType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsArray)
                return typeInfo.GetElementType();

            if (typeInfo.IsGenericType) {
                Type typeDef = typeInfo.GetGenericTypeDefinition();
                if (typeDef == typeof(IList<>))
                    return typeInfo.GenericTypeArguments[0];
            }

            Type elementType = null;
            foreach (var @interface in typeInfo.ImplementedInterfaces) {
                if (@interface.GetTypeInfo().IsGenericType) {
                    Type typeDef = @interface.GetGenericTypeDefinition();
                    if (typeDef == typeof(IList<>)) {
                        if (elementType != null)
                            return null; // Ambiguous element type because multiple lists are implemented.
                        elementType = @interface.GenericTypeArguments[0];
                    }
                }
            }

            if (elementType != null)
                return elementType;

            if (typeof(IList).IsAssignableFrom(type))
                return typeof(object);

            return null;
        }
    }
}
