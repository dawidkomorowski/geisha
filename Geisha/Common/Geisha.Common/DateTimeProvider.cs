using System;
using System.ComponentModel.Composition;

namespace Geisha.Common
{
    // TODO Add documentation.
    // TODO Add unit tests?
    public interface IDateTimeProvider
    {
        DateTime Now();
    }

    [Export(typeof(IDateTimeProvider))]
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}