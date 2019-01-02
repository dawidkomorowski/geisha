namespace Geisha.Engine.Core.Diagnostics
{
    // TODO Add documentation.
    public sealed class DiagnosticInfo
    {
        // TODO Why mutable properties instead of immutable and constructor?
        public string Name { get; set; }

        // TODO Maybe it should be string and creator of instance is responsible for formatting it?
        public object Value { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}