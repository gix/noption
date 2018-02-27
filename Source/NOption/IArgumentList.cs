namespace NOption
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Represents an ordered collection of command-line arguments.
    /// </summary>
    public interface IArgumentList : IList<Arg>, IReadOnlyList<Arg>
    {
        /// <summary>
        ///   Returns all arguments matching the specified id.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <returns>
        ///   An IEnumerable {Arg} that contains all arguments matching the specified
        ///   id.
        /// </returns>
        IEnumerable<Arg> Matching(OptSpecifier id);

        /// <summary>
        ///   Determines whether the argument list has any arguments matching the
        ///   specified <paramref name="id"/> and claims all such arguments.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if any argument matches <paramref name="id"/>,
        ///   otherwise <see langword="false"/>.
        /// </returns>
        bool HasArg(OptSpecifier id);

        /// <summary>
        ///   Determines whether the argument list has any arguments matching
        ///   the specified <paramref name="id"/> without claiming any.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if any argument matches <paramref name="id"/>,
        ///   otherwise <see langword="false"/>.
        /// </returns>
        bool HasArgNoClaim(OptSpecifier id);

        /// <summary>
        ///   Finds the last argument matching the specified id and returns
        ///   <see langword="true"/> if found, or a default value if the option
        ///   is unmatched. Claims all matching arguments.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <param name="defaultValue">
        ///   The value to return if no matching argument is found.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the option is matched; otherwise
        ///   <paramref name="defaultValue"/>.
        /// </returns>
        bool GetFlag(OptSpecifier id, bool defaultValue = false);

        /// <summary>
        ///   Finds the last argument matching any of the specified ids and returns
        ///   <see langword="true"/> if it matches the positive option,
        ///   <see langword="false"/> if it matches the negation, or a default
        ///   value if neither option is matched. Claims all matching arguments.
        /// </summary>
        /// <param name="positiveId">
        ///   The positive option id.
        /// </param>
        /// <param name="negativeId">
        ///   The negative option id.
        /// </param>
        /// <param name="defaultValue">
        ///   The value to return if no matching argument is found.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the positive option is matched,
        ///   <see langword="false"/> if its negation is matched, and
        ///   <paramref name="defaultValue"/> if neither option is matched. If
        ///   both the option and its negation are matched, the last one is used.
        /// </returns>
        bool GetFlag(OptSpecifier positiveId, OptSpecifier negativeId, bool defaultValue = true);

        /// <summary>
        ///   Finds the last argument matching any of the specified ids and returns
        ///   <see langword="true"/> if it matches the positive option,
        ///   <see langword="false"/> if it matches the negation, or a default
        ///   value if neither option is matched. Claims no arguments.
        /// </summary>
        /// <param name="positiveId">
        ///   The positive option id.
        /// </param>
        /// <param name="negativeId">
        ///   The negative option id.
        /// </param>
        /// <param name="defaultValue">
        ///   The value to return if no matching argument is found.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the positive option is matched,
        ///   <see langword="false"/> if its negation is matched, and
        ///   <paramref name="defaultValue"/> if neither option is matched. If
        ///   both the option and its negation are matched, the last one is used.
        /// </returns>
        bool GetFlagNoClaim(OptSpecifier positiveId, OptSpecifier negativeId, bool defaultValue = true);

        /// <summary>
        ///   Gets the last argument matching the specified <paramref name="id"/>.
        ///   Claims all matching arguments.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <returns>
        ///   The last argument or <see langword="null"/> if no argument matches
        ///   <paramref name="id"/>.
        /// </returns>
        Arg GetLastArg(OptSpecifier id);

        /// <summary>
        ///   Gets the last argument matching any of the specified ids. Claims
        ///   all matching arguments.
        /// </summary>
        /// <param name="id1">
        ///   The first option id to look for.
        /// </param>
        /// <param name="id2">
        ///   The second option id to look for.
        /// </param>
        /// <returns>
        ///   The last argument or <see langword="null"/> if no argument matches
        ///   any id.
        /// </returns>
        Arg GetLastArg(OptSpecifier id1, OptSpecifier id2);

        /// <summary>
        ///   Gets the last argument matching any of the specified ids. Claims
        ///   all matching arguments.
        /// </summary>
        /// <param name="ids">
        ///   The option ids to look for.
        /// </param>
        /// <returns>
        ///   The last argument or <see langword="null"/> if no argument matches
        ///   any id.
        /// </returns>
        Arg GetLastArg(params OptSpecifier[] ids);

        /// <summary>
        ///   Gets the last argument matching the specified <paramref name="id"/>
        ///   without claiming any arguments.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <returns>
        ///   The last matching argument or <see langword="null"/> if no argument
        ///   matches <paramref name="id"/>.
        /// </returns>
        Arg GetLastArgNoClaim(OptSpecifier id);

        /// <summary>
        ///   Gets the last argument matching any of the specified ids without
        ///   claiming it.
        /// </summary>
        /// <param name="id1">
        ///   The first option id to look for.
        /// </param>
        /// <param name="id2">
        ///   The second option id to look for.
        /// </param>
        /// <returns>
        ///   The last matching argument or <see langword="null"/> if no argument
        ///   matches.
        /// </returns>
        Arg GetLastArgNoClaim(OptSpecifier id1, OptSpecifier id2);

        /// <summary>
        ///   Gets the value of the last argument matching the specified id, or
        ///   a default value if no argument is found.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <param name="defaultValue">
        ///   The value to return if no matching argument is found.
        /// </param>
        /// <returns>
        ///   The value of the last matching argument; otherwise,
        ///   <paramref name="defaultValue"/>.
        /// </returns>
        string GetLastArgValue(OptSpecifier id, string defaultValue = null);

        /// <summary>
        ///   Gets the values of all arguments matching the specified id.
        /// </summary>
        /// <param name="id">
        ///   The option id to look for.
        /// </param>
        /// <returns>
        ///   A list of values of all matching arguments.
        /// </returns>
        IList<string> GetAllArgValues(OptSpecifier id);

        /// <summary>Claims all arguments.</summary>
        void ClaimAllArgs();

        /// <summary>Claims all arguments matching the specified id.</summary>
        /// <param name="id">The option id to claim.</param>
        void ClaimAllArgs(OptSpecifier id);

        /// <summary>Removes all arguments matching the specified id.</summary>
        /// <param name="id">The option id to remove.</param>
        void RemoveAllArgs(OptSpecifier id);

        /// <summary>
        ///   Gets the number of arguments contained in the <see cref="IArgumentList"/>.
        /// </summary>
        /// <returns>
        ///   The number of arguments contained in the <see cref="IArgumentList"/>.
        /// </returns>
        new int Count { get; }

        /// <summary>
        ///   Gets or sets the argument at the specified index.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index of the argument to get or set.
        /// </param>
        /// <returns>
        ///   The argument at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not a valid index in the
        ///   <see cref="IArgumentList"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        new Arg this[int index] { get; set; }

        /// <summary>
        ///   Adds the specified argument to the list.
        /// </summary>
        /// <param name="arg">
        ///   The argument to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="arg"/> is <see langword="null"/>.
        /// </exception>
        new void Add(Arg arg);
    }
}
