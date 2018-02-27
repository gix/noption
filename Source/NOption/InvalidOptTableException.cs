namespace NOption
{
    using System;

    /// <summary>
    ///   The exception that is thrown when trying to construct an invalid
    ///   <see cref="OptTable"/>.
    /// </summary>
    public sealed class InvalidOptTableException : Exception
    {
        /// <overloads>
        ///   Initializes a new instance of the <see cref="InvalidOptTableException"/>
        ///   class.
        /// </overloads>
        /// <summary>
        ///   Initializes a new instance of the <see cref="InvalidOptTableException"/>
        ///   class with default properties.
        /// </summary>
        public InvalidOptTableException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InvalidOptTableException"/>
        ///   class with a specified error message.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for this exception.
        /// </param>
        public InvalidOptTableException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="InvalidOptTableException"/>
        ///   class with a specified error message and the exception that is the
        ///   cause of this exception.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for this exception.
        /// </param>
        /// <param name="innerException">
        ///   The exception that is the cause of the current exception, or a
        ///   <see langword="null"/> reference (<c>Nothing</c> in Visual Basic)
        ///   if no inner exception is specified.
        /// </param>
        public InvalidOptTableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
