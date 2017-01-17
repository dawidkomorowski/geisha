using System;
using System.Globalization;
using System.Threading;

namespace Geisha.Common.UnitTests.TestHelpers
{
    public class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalCultureInfo;

        public CultureScope()
        {
            _originalCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = _originalCultureInfo;
        }
    }
}