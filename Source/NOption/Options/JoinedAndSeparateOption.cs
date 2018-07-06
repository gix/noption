namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   An option with a prefix and two values. The first value and prefix are
    ///   joined and the second value is separated. This kind is used for options
    ///   like <c>--opt-value1 value2</c>.
    /// </summary>
    public class JoinedAndSeparateOption : Option
    {
        public JoinedAndSeparateOption(
            OptSpecifier id,
            string prefix,
            string name,
            string helpText = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null,
            string metaVar = null)
            : base(id, prefix, name, helpText: helpText, metaVar: metaVar,
                   aliasId: aliasId, groupId: groupId)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Contract violated: !string.IsNullOrWhiteSpace(prefix)");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        public JoinedAndSeparateOption(
            OptSpecifier id,
            IReadOnlyList<string> prefixes,
            string name,
            string helpText = null,
            OptSpecifier? aliasId = null,
            OptSpecifier? groupId = null,
            string metaVar = null)
            : base(id, prefixes, name, helpText: helpText, metaVar: metaVar,
                   aliasId: aliasId, groupId: groupId)
        {
            if (!id.IsValid)
                throw new ArgumentException("Invalid id");
            if (prefixes == null)
                throw new ArgumentNullException(nameof(prefixes));
            if (prefixes.Count == 0)
                throw new ArgumentException("Contract violated: prefixes.Count != 0");
            if (prefixes.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Contract violated: !string.IsNullOrWhiteSpace(prefix)");
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        public override string GetHelpName(string defaultMetaVar)
        {
            return PrefixedName + (MetaVar ?? defaultMetaVar);
        }
    }
}
