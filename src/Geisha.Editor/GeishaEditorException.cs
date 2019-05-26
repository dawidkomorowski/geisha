using System;
using System.Runtime.Serialization;
using Geisha.Common;

namespace Geisha.Editor
{
    /// <summary>
    /// Represents general error that occur in Geisha Editor. Base exception for all Geisha Editor related components.
    /// </summary>
    public class GeishaEditorException : GeishaException
    {
        public GeishaEditorException()
        {
        }

        public GeishaEditorException(string message) : base(message)
        {
        }

        public GeishaEditorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GeishaEditorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}