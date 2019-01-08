namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Represents single diagnostic information composed of <see cref="Name" /> and <see cref="Value" />.
    /// </summary>
    /// <remarks>Example of internal usage of this class inside the engine is FPS that can be shown on the screen.</remarks>
    public sealed class DiagnosticInfo
    {
        /// <summary>
        ///     Creates new instance of <see cref="DiagnosticInfo" /> given diagnostic info name and its value.
        /// </summary>
        /// <param name="name">Name of diagnostic info.</param>
        /// <param name="value">
        ///     Value of diagnostic info. Creator of instance is responsible for proper formatting (converting to
        ///     string) the value.
        /// </param>
        public DiagnosticInfo(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Name of diagnostic info.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Value of diagnostic info.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Converts the value of the current <see cref="DiagnosticInfo" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="DiagnosticInfo" /> object.</returns>
        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}