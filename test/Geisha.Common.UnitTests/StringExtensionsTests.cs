using System.IO;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.UnitTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void ToStream_ShouldCreateStreamThatIsCopyOfString()
        {
            // Arrange
            var sourceString = Utils.Random.GetString(100);

            // Act
            var stream = sourceString.ToStream();

            // Assert
            string stringFromStream;
            using (var streamReader = new StreamReader(stream))
            {
                stringFromStream = streamReader.ReadToEnd();
            }

            Assert.That(stringFromStream, Is.EqualTo(sourceString));
        }
    }
}