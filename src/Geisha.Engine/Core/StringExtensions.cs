using System.IO;
using System.Text;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Provides common utility functions for <see cref="string" /> data type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Creates a <see cref="Stream" /> that is a copy of specified <see cref="string" /> using UTF8 encoding.
        /// </summary>
        /// <param name="str"><see cref="string" /> to be copied as <see cref="Stream" />.</param>
        /// <returns><see cref="Stream" /> that is a copy of specified <see cref="string" /> using UTF8 encoding.</returns>
        public static Stream ToStream(this string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str), false);
        }
    }
}