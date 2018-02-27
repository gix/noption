namespace NOption
{
    public struct MissingArgs
    {
        public MissingArgs(int argIndex, int argCount, Option option)
            : this()
        {
            ArgIndex = argIndex;
            ArgCount = argCount;
            Option = option;
        }

        public int ArgIndex { get; }
        public int ArgCount { get; }
        public Option Option { get; }

        public override string ToString()
        {
            if (Option == null)
                return "No args missing";
            return "Option " + Option.Id + " is missing " + ArgCount + " arg(s) after arg " + ArgIndex;
        }
    }
}
