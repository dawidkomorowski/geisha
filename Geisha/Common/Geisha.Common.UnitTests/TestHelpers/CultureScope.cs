using System;
using System.Globalization;
using System.Threading;

namespace Geisha.Common.UnitTests.TestHelpers
{
    public class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalCultureInfo;

        public static CultureScope Invariant => new CultureScope();

        private CultureScope()
        {
            _originalCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = _originalCultureInfo;
        }
    }
}