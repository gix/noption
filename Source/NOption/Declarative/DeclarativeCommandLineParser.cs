namespace NOption.Declarative
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class DeclarativeCommandLineParser
    {
        private sealed class Info
        {
            public int OptionId;
            public MemberInfo Member;
        }

        private readonly List<Info> infos = new List<Info>();
        private readonly object optionBag;
        private readonly OptTableBuilder optTableBuilder = new OptTableBuilder();
        private OptTable optTable;

        public DeclarativeCommandLineParser(object optionBag)
        {
            if (optionBag == null)
                throw new ArgumentNullException(nameof(optionBag));
            this.optionBag = optionBag;
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
                infos.Add(new Info { OptionId = id, Member = member });
            }
        }

        public void Parse(IReadOnlyList<string> args)
        {
            SealOptTable();

            IArgumentList al = OptTable.ParseArgs(args, out var _);

            foreach (var info in infos) {
                var attribute = info.Member.GetCustomAttribute<OptionAttribute>();
                if (attribute == null)
                    continue;

                object value = attribute.GetValue(info.Member, al, info.OptionId);
                attribute.SetValue(optionBag, info.Member, value);
            }
        }

        private void SealOptTable()
        {
            if (IsSealed)
                return;

            optTable = optTableBuilder.CreateTable();
            IsSealed = true;
        }
    }
}
