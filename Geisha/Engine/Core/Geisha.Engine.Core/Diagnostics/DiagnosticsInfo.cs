namespace Geisha.Engine.Core.Diagnostics
{
    public class DiagnosticsInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}