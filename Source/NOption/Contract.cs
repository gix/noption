namespace NOption
{
    using System.Diagnostics;

    internal static class Contract
    {
        [Conditional("CONTRACT")]
        public static void Ensures(bool expr)
        {
        }

        public static T Result<T>()
        {
            return default;
        }

        public static T OldValue<T>(T value)
        {
            return default;
        }
    }
}
