namespace NOption.Declarative
{
    using System;
    using System.Collections.Generic;

    public abstract class PrefixedOptionAttribute : OptionAttribute
    {
        protected PrefixedOptionAttribute(string prefixedName)
        {
            if (prefixedName == null)
                throw new ArgumentNullException(nameof(prefixedName));

            if (!TryParseName(prefixedName, out var prefix, out var name))
                throw new ArgumentException("Invalid name", nameof(prefixedName));

            Name = name;
            Prefixes = new[] { prefix };
        }

        protected PrefixedOptionAttribute(string prefix, string name)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!IsValidName(name))
                throw new ArgumentException("Invalid name", nameof(name));
            Name = name;
            Prefixes = new[] { prefix };
        }

        protected PrefixedOptionAttribute(IReadOnlyList<string> prefixes, string name)
        {
            if (prefixes == null)
                throw new ArgumentNullException(nameof(prefixes));
            if (prefixes.Count == 0)
                throw new ArgumentException("Contract violated: prefixes.Count != 0");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!IsValidName(name))
                throw new ArgumentException("Invalid name", nameof(name));
            Name = name;
            Prefixes = prefixes;
        }

        public IReadOnlyList<string> Prefixes { get; }
    }
}
