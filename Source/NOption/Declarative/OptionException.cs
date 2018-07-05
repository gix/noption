namespace NOption.Declarative
{
    using System;

    /// <summary>
    ///   The exception thrown when option parsing fails.
    /// </summary>
    public sealed class OptionException : Exception
    {
        /// <overloads>
        ///   Initializes a new instance of the <see cref="OptionException"/> class.
        /// </overloads>
        /// <summary>
        ///   Initializes a new instance of the <see cref="OptionException"/> class
        ///   with default properties.
        /// </summary>
        public OptionException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="OptionException"/> class
        ///   with a specified error message.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for this exception.
        /// </param>
        public OptionException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="OptionException"/> class
        ///   with a specified error message and the exception that is the cause
        ///   of this exception.
        /// </summary>
        /// <param name="message">
        ///   The error message that explains the reason for this exception.
        /// </param>
        /// <param name="innerException">
        ///   The exception that is the cause of the current exception, or a
        ///   <see langword="null"/> reference (<c>Nothing</c> in Visual Basic)
        ///   if no inner exception is specified.
        /// </param>
        public OptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
