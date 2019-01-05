namespace Geisha.Engine.Core.Diagnostics
{
    // TODO Add documentation.
    public sealed class DiagnosticInfo
    {
        public DiagnosticInfo(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        // TODO Maybe it should be string and creator of instance is responsible for formatting it?
        public object Value { get; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}