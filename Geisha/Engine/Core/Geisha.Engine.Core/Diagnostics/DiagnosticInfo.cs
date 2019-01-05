namespace Geisha.Engine.Core.Diagnostics
{
    // TODO Add documentation.
    public sealed class DiagnosticInfo
    {
        public DiagnosticInfo(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}