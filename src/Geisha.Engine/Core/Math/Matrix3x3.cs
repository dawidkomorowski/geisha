using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Math
{
    /// <summary>
    ///     2D transformation matrix in homogeneous coordinates. It is three rows by three columns matrix consisting nine
    ///     components.
    /// </summary>
    /// <remarks>
    ///     In computation this matrix treats vectors as column vectors therefore translation is located in last column of
    ///     the matrix.
    /// </remarks>
    // ReSharper disable once InconsistentNaming
    public readonly struct Matrix3x3 : IEquatable<Matrix3x3>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Matrix3x3" /> that has all components set to zero.
        /// </summary>
        public static Matrix3x3 Zero => new(0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        ///     Returns identity <see cref="Matrix3x3" />.
        /// </summary>
        public static Matrix3x3 Identity => new(1, 0, 0, 0, 1, 0, 0, 0, 1);

        #endregion

        #region Properties

        /// <summary>
        ///     Component in first row and first column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M11 { get; }

        /// <summary>
        ///     Component in first row and second column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M12 { get; }

        /// <summary>
        ///     Component in first row and third column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M13 { get; }

        /// <summary>
        ///     Component in second row and first column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M21 { get; }

        /// <summary>
        ///     Component in second row and second column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M22 { get; }

        /// <summary>
        ///     Component in second row and third column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M23 { get; }

        /// <summary>
        ///     Component in third row and first column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M31 { get; }

        /// <summary>
        ///     Component in third row and second column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M32 { get; }

        /// <summary>
        ///     Component in third row and third column of the <see cref="Matrix3x3" />.
        /// </summary>
        public double M33 { get; }

        /// <summary>
        ///     Returns matrix opposite to this matrix, that is matrix with all components negated.
        /// </summary>
        public Matrix3x3 Opposite => new(-M11, -M12, -M13, -M21, -M22, -M23, -M31, -M32, -M33);


        /// <summary>
        ///     Returns component of matrix at specified row and column.
        /// </summary>
        /// <param name="row">Zero based row index of component to retrieve.</param>
        /// <param name="column">Zero based column index of component to retrieve.</param>
        /// <exception cref="IndexOutOfRangeException">
        ///     <paramref name="row" /> and <paramref name="column" /> pair exceed matrix
        ///     size.
        /// </exception>
        public double this[int row, int column]
        {
            get
            {
                var index = row * 3 + column;

                return index switch
                {
                    0 => M11,
                    1 => M12,
                    2 => M13,
                    3 => M21,
                    4 => M22,
                    5 => M23,
                    6 => M31,
                    7 => M32,
                    8 => M33,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }

        // TODO Add documentation.
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public bool IsTRS => M31 == 0 && M32 == 0 && M33 == 1 && GMath.AlmostEqual(M21 * M22, -M11 * M12);

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates new instance of <see cref="Matrix3x3" /> given nine components values.
        /// </summary>
        /// <param name="m11">Component in first row and first column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m12">Component in first row and second column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m13">Component in first row and third column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m21">Component in second row and first column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m22">Component in second row and second column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m23">Component in second row and third column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m31">Component in third row and first column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m32">Component in third row and second column of the <see cref="Matrix3x3" />.</param>
        /// <param name="m33">Component in third row and third column of the <see cref="Matrix3x3" />.</param>
        public Matrix3x3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32,
            double m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;

            M21 = m21;
            M22 = m22;
            M23 = m23;

            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Matrix3x3" /> given array of size nine containing nine components values in
        ///     row-major layout.
        /// </summary>
        /// <param name="array">Array of size nine with nine components values in row-major layout.</param>
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not nine.</exception>
        public Matrix3x3(IReadOnlyList<double> array)
        {
            if (array.Count != 9) throw new ArgumentException("Array must have length of 9 elements.");

            M11 = array[0];
            M12 = array[1];
            M13 = array[2];

            M21 = array[3];
            M22 = array[4];
            M23 = array[5];

            M31 = array[6];
            M32 = array[7];
            M33 = array[8];
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds other matrix to this matrix.
        /// </summary>
        /// <param name="other">Other matrix to add.</param>
        /// <returns><see cref="Matrix3x3" /> that is sum of this matrix with the other.</returns>
        public Matrix3x3 Add(in Matrix3x3 other) =>
            new(
                M11 + other.M11,
                M12 + other.M12,
                M13 + other.M13,
                M21 + other.M21,
                M22 + other.M22,
                M23 + other.M23,
                M31 + other.M31,
                M32 + other.M32,
                M33 + other.M33
            );

        /// <summary>
        ///     Subtracts other matrix from this matrix.
        /// </summary>
        /// <param name="other">Other matrix to subtract.</param>
        /// <returns><see cref="Matrix3x3" /> that is difference between this matrix and the other.</returns>
        public Matrix3x3 Subtract(in Matrix3x3 other) =>
            new(
                M11 - other.M11,
                M12 - other.M12,
                M13 - other.M13,
                M21 - other.M21,
                M22 - other.M22,
                M23 - other.M23,
                M31 - other.M31,
                M32 - other.M32,
                M33 - other.M33
            );

        /// <summary>
        ///     Multiplies this matrix by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of matrix.</param>
        /// <returns><see cref="Matrix3x3" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Matrix3x3 Multiply(double scalar) =>
            new(
                M11 * scalar,
                M12 * scalar,
                M13 * scalar,
                M21 * scalar,
                M22 * scalar,
                M23 * scalar,
                M31 * scalar,
                M32 * scalar,
                M33 * scalar
            );

        /// <summary>
        ///     Multiplies this matrix by other matrix.
        /// </summary>
        /// <param name="other">Matrix to multiply by (the multiplier).</param>
        /// <returns><see cref="Matrix3x3" /> that is product of this matrix multiplied by the <paramref name="other" />.</returns>
        public Matrix3x3 Multiply(in Matrix3x3 other) =>
            new(
                M11 * other.M11 + M12 * other.M21 + M13 * other.M31,
                M11 * other.M12 + M12 * other.M22 + M13 * other.M32,
                M11 * other.M13 + M12 * other.M23 + M13 * other.M33,
                M21 * other.M11 + M22 * other.M21 + M23 * other.M31,
                M21 * other.M12 + M22 * other.M22 + M23 * other.M32,
                M21 * other.M13 + M22 * other.M23 + M23 * other.M33,
                M31 * other.M11 + M32 * other.M21 + M33 * other.M31,
                M31 * other.M12 + M32 * other.M22 + M33 * other.M32,
                M31 * other.M13 + M32 * other.M23 + M33 * other.M33
            );

        /// <summary>
        ///     Multiplies this matrix by given vector.
        /// </summary>
        /// <param name="vector">Vector to multiply by (the multiplier).</param>
        /// <returns><see cref="Vector3" /> that is product of this matrix multiplied by the <paramref name="vector" />.</returns>
        public Vector3 Multiply(in Vector3 vector) =>
            new(
                M11 * vector.X + M12 * vector.Y + M13 * vector.Z,
                M21 * vector.X + M22 * vector.Y + M23 * vector.Z,
                M31 * vector.X + M32 * vector.Y + M33 * vector.Z
            );

        /// <summary>
        ///     Divides this matrix by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of matrix.</param>
        /// <returns><see cref="Matrix3x3" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Matrix3x3 Divide(double scalar) =>
            new(
                M11 / scalar,
                M12 / scalar,
                M13 / scalar,
                M21 / scalar,
                M22 / scalar,
                M23 / scalar,
                M31 / scalar,
                M32 / scalar,
                M33 / scalar
            );

        /// <summary>
        ///     Returns array that contains matrix components in row-major layout.
        /// </summary>
        /// <returns>Array with matrix components in row-major layout.</returns>
        public double[] ToArray()
        {
            return new[] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };
        }

        /// <inheritdoc />
        public bool Equals(Matrix3x3 other) =>
            M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) &&
            M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) &&
            M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Matrix3x3 other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(M11);
            hashCode.Add(M12);
            hashCode.Add(M13);
            hashCode.Add(M21);
            hashCode.Add(M22);
            hashCode.Add(M23);
            hashCode.Add(M31);
            hashCode.Add(M32);
            hashCode.Add(M33);
            return hashCode.ToHashCode();
        }

        /// <summary>
        ///     Converts the value of the current <see cref="Matrix3x3" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Matrix3x3" /> object.</returns>
        public override string ToString() =>
            $"{nameof(M11)}: {M11}, {nameof(M12)}: {M12}, {nameof(M13)}: {M13}, {nameof(M21)}: {M21}, {nameof(M22)}: {M22}, {nameof(M23)}: {M23}, {nameof(M31)}: {M31}, {nameof(M32)}: {M32}, {nameof(M33)}: {M33}";

        #endregion

        #region Static methods

        /// <summary>
        ///     Returns 2D translation matrix that represents translation by specified <paramref name="translation" /> vector.
        /// </summary>
        /// <param name="translation">Translation that is applied by matrix.</param>
        /// <returns><see cref="Matrix3x3" /> that represents translation by <paramref name="translation" /> vector.</returns>
        public static Matrix3x3 CreateTranslation(in Vector2 translation) =>
            new(
                1, 0, translation.X,
                0, 1, translation.Y,
                0, 0, 1
            );

        /// <summary>
        ///     Returns 2D rotation matrix that represents counterclockwise rotation by <paramref name="angle" /> specified in
        ///     radians.
        /// </summary>
        /// <param name="angle">Rotation angle in radians that is applied by matrix.</param>
        /// <returns><see cref="Matrix3x3" /> that represents counterclockwise rotation by <paramref name="angle" /> radians.</returns>
        public static Matrix3x3 CreateRotation(double angle) =>
            new(
                System.Math.Cos(angle), -System.Math.Sin(angle), 0,
                System.Math.Sin(angle), System.Math.Cos(angle), 0,
                0, 0, 1
            );

        /// <summary>
        ///     Returns 2D scale matrix that represents scaling by <paramref name="scale" /> vector.
        /// </summary>
        /// <param name="scale">
        ///     Scale that is applied by matrix. Scale is a <see cref="Vector2" /> where X is scaling factor along X axis and Y is
        ///     scaling factor along Y axis.
        /// </param>
        /// <returns><see cref="Matrix3x3" /> that represents scaling by <paramref name="scale" /> vector.</returns>
        public static Matrix3x3 CreateScale(in Vector2 scale) =>
            new(
                scale.X, 0, 0,
                0, scale.Y, 0,
                0, 0, 1
            );

        // TODO Add documentation.
        public static Matrix3x3 CreateTRS(in Vector2 translation, double rotation, in Vector2 scale) =>
            new(
                scale.X * System.Math.Cos(rotation), -scale.Y * System.Math.Sin(rotation), translation.X,
                scale.X * System.Math.Sin(rotation), scale.Y * System.Math.Cos(rotation), translation.Y,
                0, 0, 1
            );

        // TODO Add documentation.
        public static Matrix3x3 Lerp(in Matrix3x3 m1, in Matrix3x3 m2, double alpha) =>
            new(
                GMath.Lerp(m1.M11, m2.M11, alpha),
                GMath.Lerp(m1.M12, m2.M12, alpha),
                GMath.Lerp(m1.M13, m2.M13, alpha),
                GMath.Lerp(m1.M21, m2.M21, alpha),
                GMath.Lerp(m1.M22, m2.M22, alpha),
                GMath.Lerp(m1.M23, m2.M23, alpha),
                GMath.Lerp(m1.M31, m2.M31, alpha),
                GMath.Lerp(m1.M32, m2.M32, alpha),
                GMath.Lerp(m1.M33, m2.M33, alpha)
            );

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one matrix to another.
        /// </summary>
        /// <param name="left">The first matrix to add.</param>
        /// <param name="right">The second matrix to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Matrix3x3 operator +(in Matrix3x3 left, in Matrix3x3 right) => left.Add(right);

        /// <summary>
        ///     Subtracts one matrix from another.
        /// </summary>
        /// <param name="left">Matrix to subtract from (the minuend).</param>
        /// <param name="right">Matrix to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Matrix3x3 operator -(in Matrix3x3 left, in Matrix3x3 right) => left.Subtract(right);

        /// <summary>
        ///     Multiplies specified matrix by given scalar.
        /// </summary>
        /// <param name="left">Matrix to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of matrix.</param>
        /// <returns><see cref="Matrix3x3" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Matrix3x3 operator *(in Matrix3x3 left, double right) => left.Multiply(right);

        /// <summary>
        ///     Multiplies one matrix by another.
        /// </summary>
        /// <param name="left">Matrix to be multiplied (the multiplicand).</param>
        /// <param name="right">Matrix to multiply by (the multiplier).</param>
        /// <returns>
        ///     <see cref="Matrix3x3" /> that is product of <paramref name="left" /> matrix multiplied by the
        ///     <paramref name="right" /> matrix.
        /// </returns>
        public static Matrix3x3 operator *(in Matrix3x3 left, in Matrix3x3 right) => left.Multiply(right);

        /// <summary>
        ///     Multiplies specified matrix by given vector.
        /// </summary>
        /// <param name="left">Matrix to be multiplied (the multiplicand).</param>
        /// <param name="right">Vector to multiply by (the multiplier).</param>
        /// <returns>
        ///     <see cref="Matrix3x3" /> that is product of <paramref name="left" /> matrix multiplied by the
        ///     <paramref name="right" /> vector.
        /// </returns>
        public static Vector3 operator *(in Matrix3x3 left, in Vector3 right) => left.Multiply(right);

        /// <summary>
        ///     Divides specified matrix by given scalar.
        /// </summary>
        /// <param name="left">Matrix to be divided.</param>
        /// <param name="right">Scalar value that is divisor of matrix.</param>
        /// <returns><see cref="Matrix3x3" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Matrix3x3 operator /(in Matrix3x3 left, double right) => left.Divide(right);

        /// <summary>
        ///     Returns matrix opposite to the specified matrix, that is matrix with all components negated.
        /// </summary>
        /// <param name="right">Matrix to be negated.</param>
        /// <returns>Matrix opposite to the specified matrix, that is matrix with all components negated.</returns>
        public static Matrix3x3 operator -(in Matrix3x3 right) => right.Opposite;

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Matrix3x3" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Matrix3x3" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(in Matrix3x3 left, in Matrix3x3 right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Matrix3x3" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Matrix3x3" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(in Matrix3x3 left, in Matrix3x3 right) => !left.Equals(right);

        #endregion
    }
}