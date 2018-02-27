namespace NOption
{
    using System.Collections.Generic;

    public interface IOptionHelpFormatter
    {
        void WriteHelp(IEnumerable<Option> options);
    }
}
