namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Collections;
    using NOption.Extensions;

    public class OptTable
    {
        private readonly HashSet<string> prefixes = new HashSet<string>();
        private readonly List<Option> options;
        private readonly OrderedDictionary<int, Option> optionMap =
            new OrderedDictionary<int, Option>();
        private readonly char[] prefixChars;
        private readonly int unknownOptionId;
        private readonly int inputOptionId;
        private readonly int firstOptionIdx;

        /// <summary>
        ///   Initializes a new instance of the <see cref="OptTable"/> class
        ///   with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public OptTable(IEnumerable<Option> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.options = new List<Option>(options.Where(x => x != null));
            foreach (var option in this.options)
                option.InitializeOwner(this);
            VerifyOptions(this.options);

            foreach (var opt in this.options) {
                prefixes.AddRange(opt.Prefixes);
                optionMap.Add(opt.Id, opt);

                if (opt.Kind == OptionKind.Unknown)
                    unknownOptionId = opt.Id;
                else if (opt.Kind == OptionKind.Input)
                    inputOptionId = opt.Id;
            }

            this.options.StableSort(new OptionComparer());

            firstOptionIdx = 0;
            if (unknownOptionId != 0)
                ++firstOptionIdx;
            if (inputOptionId != 0)
                ++firstOptionIdx;

            var chars = new HashSet<char>();
            foreach (var prefix in prefixes)
                chars.AddRange(prefix.OfType<char>());
            prefixChars = chars.ToArray();
        }

        /// <summary>Gets the list of defined options.</summary>
        public IReadOnlyList<Option> DefinedOptions => optionMap.Values;

        /// <summary>
        ///   Gets the <see cref="Option"/> for the specified id.
        /// </summary>
        /// <param name="id">
        ///   The id of the option to get.
        /// </param>
        /// <returns>
        ///   The specified <see cref="Option"/> or <see langword="null"/> if no
        ///   such option exists.
        /// </returns>
        public Option GetOption(OptSpecifier id)
        {
            if (!id.IsValid)
                return null;
            if (!optionMap.TryGetValue(id.Id, out var opt))
                return null;
            return opt;
        }

        public IArgumentList ParseArgs(
            IReadOnlyList<string> args, out MissingArgs missing)
        {
            missing = new MissingArgs();

            var list = new ArgumentList();
            if (args == null)
                return list;

            for (int idx = 0; idx < args.Count;) {
                int prev = idx;
                Arg arg = ParseArg(args, ref idx, out int missingArgOptIndex);
                if (arg == null) {
                    missing = new MissingArgs(
                        prev,
                        idx - args.Count,
                        options[missingArgOptIndex]);
                    break;
                }

                list.Add(arg);
            }

            return list;
        }

        public void WriteHelp(IOptionHelpFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            formatter.WriteHelp(optionMap.Values);
        }

        private Arg ParseArg(
            IReadOnlyList<string> args, ref int argIndex, out int missingArgOptIndex)
        {
            Debug.Assert(argIndex >= 0 && argIndex < args.Count);

            missingArgOptIndex = 0;

            string argStr = args[argIndex];
            if (argStr == null)
                return null;

            if (IsInputArg(argStr)) {
                int id = inputOptionId > 0 ? inputOptionId : unknownOptionId;
                return new Arg(GetOption(id), argStr, argIndex++, argStr);
            }

            string name = argStr.TrimStart(prefixChars);
            int firstInfo = options.WeakPredecessor(
                firstOptionIdx, options.Count - firstOptionIdx, name,
                CompareNameIgnoreCase);
            int prevArgIndex = argIndex;

            for (int idx = firstInfo; idx < options.Count; ++idx) {
                Option opt = options[idx];
                Arg arg = opt.Accept(args, ref argIndex);
                if (arg != null)
                    return arg;

                // Check if this option is missing values.
                if (prevArgIndex != argIndex) {
                    missingArgOptIndex = idx;
                    return null;
                }

                // Restore old argIndex.
                argIndex = prevArgIndex;
            }

            return new Arg(GetOption(unknownOptionId), argStr, argIndex++, argStr);
        }

        private static int CompareNameCase(Option opt, string name)
        {
            return StringUtils.CompareNameCase(opt.Name, name);
        }

        private static int CompareNameIgnoreCase(Option opt, string name)
        {
            return StringUtils.CompareNameIgnoreCase(opt.Name, name);
        }

        private static void VerifyOptions(IEnumerable<Option> options)
        {
            int unknownId = 0;
            int inputId = 0;
            int firstRealId = 0;
            var ids = new HashSet<int>();
            var names = new HashSet<string>();
            foreach (var option in options) {
                if (option.Id <= 0)
                    throw new InvalidOptTableException($"Option id '{option.Id}' must be greater than 0");

                if (option.Kind == OptionKind.Unknown) {
                    if (unknownId != 0)
                        throw new InvalidOptTableException("Duplicate option with OptionKind.Unknown");
                    unknownId = option.Id;
                } else if (option.Kind == OptionKind.Input) {
                    if (inputId != 0)
                        throw new InvalidOptTableException("Duplicate option with OptionKind.Input");
                    inputId = option.Id;
                } else if (firstRealId == 0)
                    firstRealId = option.Id;

                if (ids.Contains(option.Id))
                    throw new InvalidOptTableException($"Duplicate option id '{option.Id}'");
                ids.Add(option.Id);

                if (names.Contains(option.Name))
                    throw new InvalidOptTableException($"Duplicate option name '{option.Name}'");
                names.Add(option.Name);
            }

            //if (firstRealId < unknownId || firstRealId < inputId)
            //    throw new OptTableException("OptKind.Unknown and OptKind.Input must precede all other options.");
        }

        private bool IsInputArg(string argStr)
        {
            return prefixes.All(p => !argStr.StartsWith(p));
        }

        private sealed class OptionComparer : IComparer<Option>
        {
            public int Compare(Option x, Option y)
            {
                if (x.Kind != y.Kind) {
                    if (x.Kind == OptionKind.Unknown)
                        return -1;
                    if (y.Kind == OptionKind.Unknown)
                        return 1;
                    if (x.Kind == OptionKind.Input)
                        return -1;
                    if (y.Kind == OptionKind.Input)
                        return 1;
                }

                return StringUtils.CompareNameIgnoreCase(x.Name, y.Name);
            }
        }
    }
}
