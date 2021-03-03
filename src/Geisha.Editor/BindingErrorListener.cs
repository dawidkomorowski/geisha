using System;
using System.Diagnostics;
using System.Text;

namespace Geisha.Editor
{
    public sealed class BindingErrorListener : TraceListener
    {
        private readonly StringBuilder _messageBuilder = new StringBuilder();

        private BindingErrorListener()
        {
        }

        public static void EnableExceptionOnBindingError()
        {
            PresentationTraceSources.Refresh();
            var dataBindingSource = PresentationTraceSources.DataBindingSource;
            dataBindingSource.Listeners.Add(new BindingErrorListener());
            dataBindingSource.Switch.Level = SourceLevels.Error;
        }

        public override void Write(string message)
        {
            _messageBuilder.Append(message);
        }

        public override void WriteLine(string message)
        {
            _messageBuilder.AppendLine(message);

            throw new BindingErrorException(_messageBuilder.ToString());
        }
    }

    public sealed class BindingErrorException : Exception
    {
        public BindingErrorException(string message) : base(message)
        {
        }
    }
}