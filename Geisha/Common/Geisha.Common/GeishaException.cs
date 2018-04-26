using System;
using System.Runtime.Serialization;

namespace Geisha.Common
{
    /// <summary>
    ///     Represents general error that occur in Geisha ecosystem. Base exception for all Geisha related components.
    /// </summary>
    public class GeishaException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GeishaException" /> class.
        /// </summary>
        public GeishaException()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Geisha.Common.GeishaException" /> class with a specified error
        ///     message.
        /// </summary>
        public GeishaException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Geisha.Common.GeishaException" /> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public GeishaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Geisha.Common.GeishaException" /> class with serialized data.
        /// </summary>
        protected GeishaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}