namespace NOption
{
    using System;

    public sealed class WriteHelpSettings
    {
        private string indentChars;
        private string defaultMetaVarName;
        private string defaultHelpGroup;
        private int maxLineLength;
        private int nameColumnWidth;

        public WriteHelpSettings()
        {
            IndentChars = "  ";
            NameColumnWidth = 30;
            MaxLineLength = 80;
            DefaultMetaVar = "<value>";
            DefaultHelpGroup = "Options";
        }

        public string IndentChars
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return indentChars;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.IndexOf('\r') != -1)
                    throw new ArgumentException("Indent must not contain any newline characters");
                if (value.IndexOf('\n') != -1)
                    throw new ArgumentException("Indent must not contain any newline characters");
                indentChars = value;
            }
        }

        public int NameColumnWidth
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return nameColumnWidth;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Contract violated: value >= 0");
                nameColumnWidth = value;
            }
        }

        public int MaxLineLength
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return maxLineLength;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Contract violated: value >= 0");
                maxLineLength = value;
            }
        }

        public string DefaultMetaVar
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return defaultMetaVarName;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                defaultMetaVarName = value;
            }
        }

        public int FlagsToInclude { get; set; }
        public int FlagsToExclude { get; set; }

        public string DefaultHelpGroup
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return defaultHelpGroup;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                defaultHelpGroup = value;
            }
        }
    }
}
