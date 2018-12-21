using System;
using NUnit.Framework;

namespace Geisha.Common.UnitTests
{
    [TestFixture]
    public class CircularBufferTests
    {
        #region Constructor

        [Test]
        public void Constructor_ShouldThrowArgumentException_GivenZeroValueAsConstructorParameter()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => { new CircularBuffer<int>(0); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_ShouldThrowArgumentException_GivenNegativeValueAsConstructorParameter()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => { new CircularBuffer<int>(-1); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_ShouldCreateBufferOfSize10_Given10AsConstructorParameter()
        {
            // Arrange
            // Act
            var circularBuffer = new CircularBuffer<int>(10);

            // Assert
            Assert.That(circularBuffer.Size, Is.EqualTo(10));
        }

        [Test]
        public void Constructor_ShouldCreateBufferWith10DefaultElements_Given10AsConstructorParameterAndValueTypeAsTypeParameter()
        {
            // Arrange
            // Act
            var circularBuffer = new CircularBuffer<int>(10);

            // Assert
            Assert.That(circularBuffer[0], Is.Zero);
            Assert.That(circularBuffer[1], Is.Zero);
            Assert.That(circularBuffer[2], Is.Zero);
            Assert.That(circularBuffer[3], Is.Zero);
            Assert.That(circularBuffer[4], Is.Zero);
            Assert.That(circularBuffer[5], Is.Zero);
            Assert.That(circularBuffer[6], Is.Zero);
            Assert.That(circularBuffer[7], Is.Zero);
            Assert.That(circularBuffer[8], Is.Zero);
            Assert.That(circularBuffer[9], Is.Zero);
        }

        [Test]
        public void Constructor_ShouldCreateBufferWith10DefaultElements_Given10AsConstructorParameterAndReferenceTypeAsTypeParameter()
        {
            // Arrange
            // Act
            var circularBuffer = new CircularBuffer<object>(10);

            // Assert
            Assert.That(circularBuffer[0], Is.Null);
            Assert.That(circularBuffer[1], Is.Null);
            Assert.That(circularBuffer[2], Is.Null);
            Assert.That(circularBuffer[3], Is.Null);
            Assert.That(circularBuffer[4], Is.Null);
            Assert.That(circularBuffer[5], Is.Null);
            Assert.That(circularBuffer[6], Is.Null);
            Assert.That(circularBuffer[7], Is.Null);
            Assert.That(circularBuffer[8], Is.Null);
            Assert.That(circularBuffer[9], Is.Null);
        }

        #endregion

        #region Indexer

        [Test]
        public void Indexer_ShouldThrowIndexOutOfRangeException_GivenIndexBelowZero()
        {
            // Arrange
            // Act
            var circularBuffer = new CircularBuffer<int>(10);

            // Assert
            Assert.That(() =>
            {
                var x = circularBuffer[-1];
            }, Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void Indexer_ShouldThrowIndexOutOfRangeException_GivenIndexEqualSize()
        {
            // Arrange
            // Act
            var circularBuffer = new CircularBuffer<int>(10);

            // Assert
            Assert.That(() =>
            {
                var x = circularBuffer[10];
            }, Throws.TypeOf<IndexOutOfRangeException>());
        }

        #endregion

        #region Add

        [Test]
        public void Add_ShouldAddNewElementAsTheLastOne()
        {
            // Arrange
            var circularBuffer = new CircularBuffer<int>(10);

            // Assume
            Assume.That(circularBuffer[9], Is.EqualTo(0));

            // Act
            circularBuffer.Add(5);

            // Assert
            Assert.That(circularBuffer[9], Is.EqualTo(5));
        }

        [Test]
        public void Add_ShouldAddNewElementsAsTheLastOnesFillingWholeBufferInOrderOfAdditions()
        {
            // Arrange
            var circularBuffer = new CircularBuffer<int>(10);

            // Act
            circularBuffer.Add(1);
            circularBuffer.Add(2);
            circularBuffer.Add(3);
            circularBuffer.Add(4);
            circularBuffer.Add(5);
            circularBuffer.Add(6);
            circularBuffer.Add(7);
            circularBuffer.Add(8);
            circularBuffer.Add(9);
            circularBuffer.Add(10);

            // Assert
            Assert.That(circularBuffer[0], Is.EqualTo(1));
            Assert.That(circularBuffer[1], Is.EqualTo(2));
            Assert.That(circularBuffer[2], Is.EqualTo(3));
            Assert.That(circularBuffer[3], Is.EqualTo(4));
            Assert.That(circularBuffer[4], Is.EqualTo(5));
            Assert.That(circularBuffer[5], Is.EqualTo(6));
            Assert.That(circularBuffer[6], Is.EqualTo(7));
            Assert.That(circularBuffer[7], Is.EqualTo(8));
            Assert.That(circularBuffer[8], Is.EqualTo(9));
            Assert.That(circularBuffer[9], Is.EqualTo(10));
        }

        [Test]
        public void Add_ShouldAddNewElementAsTheLastOneReplacingTheFirstOne()
        {
            // Arrange
            var circularBuffer = new CircularBuffer<int>(10);

            circularBuffer.Add(1);
            circularBuffer.Add(2);
            circularBuffer.Add(3);
            circularBuffer.Add(4);
            circularBuffer.Add(5);
            circularBuffer.Add(6);
            circularBuffer.Add(7);
            circularBuffer.Add(8);
            circularBuffer.Add(9);
            circularBuffer.Add(10);

            // Assume
            Assume.That(circularBuffer[0], Is.EqualTo(1));
            Assume.That(circularBuffer[1], Is.EqualTo(2));
            Assume.That(circularBuffer[2], Is.EqualTo(3));
            Assume.That(circularBuffer[3], Is.EqualTo(4));
            Assume.That(circularBuffer[4], Is.EqualTo(5));
            Assume.That(circularBuffer[5], Is.EqualTo(6));
            Assume.That(circularBuffer[6], Is.EqualTo(7));
            Assume.That(circularBuffer[7], Is.EqualTo(8));
            Assume.That(circularBuffer[8], Is.EqualTo(9));
            Assume.That(circularBuffer[9], Is.EqualTo(10));

            // Act
            circularBuffer.Add(11);

            // Assert
            Assert.That(circularBuffer[0], Is.EqualTo(2));
            Assert.That(circularBuffer[1], Is.EqualTo(3));
            Assert.That(circularBuffer[2], Is.EqualTo(4));
            Assert.That(circularBuffer[3], Is.EqualTo(5));
            Assert.That(circularBuffer[4], Is.EqualTo(6));
            Assert.That(circularBuffer[5], Is.EqualTo(7));
            Assert.That(circularBuffer[6], Is.EqualTo(8));
            Assert.That(circularBuffer[7], Is.EqualTo(9));
            Assert.That(circularBuffer[8], Is.EqualTo(10));
            Assert.That(circularBuffer[9], Is.EqualTo(11));
        }

        #endregion
    }
}