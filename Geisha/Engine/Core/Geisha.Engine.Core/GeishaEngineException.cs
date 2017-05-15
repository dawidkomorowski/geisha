using System;
using System.Runtime.Serialization;
using Geisha.Common;

namespace Geisha.Engine.Core
{
    /// <summary>
    /// Represents general error that occur in Geisha Engine. Base exception for all Geisha Engine related components.
    /// </summary>
    public class GeishaEngineException : GeishaException
    {
        public GeishaEngineException()
        {
        }

        public GeishaEngineException(string message) : base(message)
        {
        }

        public GeishaEngineException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GeishaEngineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}