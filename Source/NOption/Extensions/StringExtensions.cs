namespace NOption.Extensions
{
    using System;

    internal static class StringExtensions
    {
        /// <summary>
        ///   Determines whether the substring of this <paramref name="str"/>
        ///   instance starting at a specified <paramref name="index"/> matches
        ///   the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="str">
        ///   The string to compare against.
        /// </param>
        /// <param name="prefix">
        ///   The string to compare.
        /// </param>
        /// <param name="index">
        ///   The index of <paramref name="str"/> at which to start the comparison.
        /// </param>
        /// <param name="comparisonType">
        ///   One of the enumeration values that determines how <paramref name="str"/>
        ///   and <paramref name="prefix"/> are compared.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="prefix"/> matches the
        ///   string beginning at <paramref name="index"/>; otherwise,
        ///   <see langword="false"/>.
        /// </returns>
        public static bool StartsWith(
            this string str, string prefix, int index, StringComparison comparisonType)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (prefix == null)
                return str.Length >= index;
            return
                str.Length >= prefix.Length + index &&
                string.Compare(str, index, prefix, 0, prefix.Length, comparisonType) == 0;
        }
    }
}
