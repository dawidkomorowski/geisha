using System;
using System.ComponentModel.Composition;

namespace Geisha.Common
{
    /// <summary>
    ///     Interface providing date and time information.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        ///     Returns <see cref="DateTime" /> object that is set to current date and time on this computer, expressed as the
        ///     local time.
        /// </summary>
        /// <returns>
        ///     <see cref="DateTime" /> object that is set to current date and time on this computer, expressed as the local
        ///     time.
        /// </returns>
        DateTime Now();
    }

    /// <summary>
    ///     Class providing date and time information.
    /// </summary>
    /// <remarks>
    ///     This class is thin wrapper around <see cref="DateTime" /> to provide easy stubbing of date and time
    ///     information and improve testability of dependent components.
    /// </remarks>
    [Export(typeof(IDateTimeProvider))]
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc />
        /// <summary>
        ///     Returns <see cref="T:System.DateTime" /> object that is set to current date and time on this computer, expressed as
        ///     the local time.
        /// </summary>
        /// <remarks>This method is a thin wrapper around <see cref="DateTime.Now" />.</remarks>
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}