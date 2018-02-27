namespace NOption
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class OptTableBuilder
    {
        private readonly List<Option> options = new List<Option>();
        private bool hasUnknown;

        public OptTableBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            options.Clear();
            hasUnknown = false;
        }

        public OptTableBuilder Add(Option option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));

            options.Add(option);
            if (option is UnknownOption)
                hasUnknown = true;
            return this;
        }

        public IList<Option> GetList()
        {
            if (!hasUnknown) {
                int maxId = options.Max(o => o.Id);
                this.AddUnknown(maxId + 1);
            }
            return options;
        }

        public OptTable CreateTable()
        {
            return new OptTable(GetList());
        }
    }
}
