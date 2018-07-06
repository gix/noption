namespace NOption.Declarative
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class CommandLineParser
    {
        private sealed class Info
        {
            public int OptionId;
            public IMemberRef Member;
        }

        private readonly List<Info> infos = new List<Info>();
        private readonly object optionBag;
        private readonly OptTableBuilder optTableBuilder = new OptTableBuilder();
        private OptTable optTable;

        public CommandLineParser(object optionBag)
        {
            this.optionBag = optionBag ?? throw new ArgumentNullException(nameof(optionBag));
            ReflectOptTable();
        }

        public OptTable OptTable
        {
            get
            {
                SealOptTable();
                return optTable;
            }
        }

        public OptTableBuilder Builder
        {
            get
            {
                if (IsSealed)
                    throw new InvalidOperationException("Object is sealed");
                return optTableBuilder;
            }
        }

        public bool IsSealed { get; private set; }

        private void ReflectOptTable()
        {
            Type type = optionBag.GetType();

            int nextOptionId = 2;
            foreach (var member in type.GetTypeInfo().DeclaredMembers) {
                var attribute = member.GetCustomAttribute<OptionAttribute>();
                if (attribute == null)
                    continue;

                if (!attribute.AcceptsMember(member))
                    throw new OptionException(member.Name + " has incompatible option attribute.");

                int id = nextOptionId++;
                attribute.AddOption(id, optTableBuilder);
                infos.Add(new Info { OptionId = id, Member = MemberRef.Create(member, optionBag) });
            }
        }

        public static IArgumentList Parse(IReadOnlyList<string> args, object optionBag)
        {
            return new CommandLineParser(optionBag).Parse(args);
        }

        public IArgumentList Parse(IReadOnlyList<string> args)
        {
            SealOptTable();

            IArgumentList al = OptTable.ParseArgs(args, out _);

            foreach (var info in infos) {
                var attribute = info.Member.MemberInfo.GetCustomAttribute<OptionAttribute>();
                if (attribute == null)
                    continue;

                attribute.PopulateValue(info.Member, info.OptionId, al);
            }

            return al;
        }

        private void SealOptTable()
        {
            if (IsSealed)
                return;

            optTable = optTableBuilder.CreateTable();
            IsSealed = true;
        }
    }

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
        private readonly object target;

        public PropertyRef(PropertyInfo property, object target)
        {
            this.property = property;
            this.target = target;
        }

        public object Target => target;
        public MemberInfo MemberInfo => property;
        public Type ValueType => property.PropertyType;
        public bool CanRead => property.CanRead;
        public bool CanWrite => property.CanWrite;

        public object GetValue()
        {
            return property.GetValue(target);
        }

        public void SetValue(object value)
        {
            property.SetValue(target, value);
        }
    }

    internal sealed class FieldRef : IMemberRef
    {
        private readonly FieldInfo field;
        private readonly object target;

        public FieldRef(FieldInfo field, object target)
        {
            this.field = field;
            this.target = target;
        }

        public object Target => target;
        public MemberInfo MemberInfo => field;
        public Type ValueType => field.FieldType;
        public bool CanRead => true;
        public bool CanWrite => !field.IsInitOnly;

        public object GetValue()
        {
            return field.GetValue(target);
        }

        public void SetValue(object value)
        {
            field.SetValue(target, value);
        }
    }
}
