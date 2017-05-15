using System;
using System.Runtime.Serialization;

namespace Geisha.Common
{
    /// <summary>
    /// Represents general error that occur in Geisha ecosystem. Base exception for all Geisha related components.
    /// </summary>
    public class GeishaException : Exception
    {
        public GeishaException()
        {
        }

        public GeishaException(string message) : base(message)
        {
        }

        public GeishaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GeishaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}