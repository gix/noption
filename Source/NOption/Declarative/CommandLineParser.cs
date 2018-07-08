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
        private readonly OptTableBuilder builder = new OptTableBuilder();
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
                return builder;
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
                attribute.AddOption(id, builder);
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

            optTable = builder.CreateTable();
            IsSealed = true;
        }
    }
}
