using System;
using System.Collections.Generic;

namespace Geisha.Common.Math
{
    /// <summary>
    ///     3D transformation matrix in homogeneous coordinates. It is four rows by four columns matrix consisting sixteen
    ///     components.
    /// </summary>
    /// <remarks>
    ///     In computation this matrix treats vectors as column vectors therefore translation is located in last column of
    ///     the matrix.
    /// </remarks>
    // ReSharper disable once InconsistentNaming
    public readonly struct Matrix4x4 : IEquatable<Matrix4x4>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Matrix4x4" /> that has all components set to zero.
        /// </summary>
        public static Matrix4x4 Zero => new Matrix4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        ///     Returns identity <see cref="Matrix4x4" />.
        /// </summary>
        public static Matrix4x4 Identity => new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        #endregion

        #region Properties

        /// <summary>
        ///     Component in first row and first column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M11 { get; }

        /// <summary>
        ///     Component in first row and second column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M12 { get; }

        /// <summary>
        ///     Component in first row and third column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M13 { get; }

        /// <summary>
        ///     Component in first row and fourth column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M14 { get; }

        /// <summary>
        ///     Component in second row and first column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M21 { get; }

        /// <summary>
        ///     Component in second row and second column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M22 { get; }

        /// <summary>
        ///     Component in second row and third column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M23 { get; }

        /// <summary>
        ///     Component in second row and fourth column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M24 { get; }

        /// <summary>
        ///     Component in third row and first column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M31 { get; }

        /// <summary>
        ///     Component in third row and second column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M32 { get; }

        /// <summary>
        ///     Component in third row and third column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M33 { get; }

        /// <summary>
        ///     Component in third row and fourth column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M34 { get; }

        /// <summary>
        ///     Component in fourth row and first column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M41 { get; }

        /// <summary>
        ///     Component in fourth row and second column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M42 { get; }

        /// <summary>
        ///     Component in fourth row and third column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M43 { get; }

        /// <summary>
        ///     Component in fourth row and fourth column of the <see cref="Matrix4x4" />.
        /// </summary>
        public double M44 { get; }

        /// <summary>
        ///     Returns matrix opposite to this matrix, that is matrix with all components negated.
        /// </summary>
        public Matrix4x4 Opposite
            =>
                new Matrix4x4(-M11, -M12, -M13, -M14, -M21, -M22, -M23, -M24, -M31, -M32, -M33, -M34, -M41, -M42, -M43,
                    -M44);

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
                var index = row * 4 + column;

                switch (index)
                {
                    case 0:
                        return M11;
                    case 1:
                        return M12;
                    case 2:
                        return M13;
                    case 3:
                        return M14;
                    case 4:
                        return M21;
                    case 5:
                        return M22;
                    case 6:
                        return M23;
                    case 7:
                        return M24;
                    case 8:
                        return M31;
                    case 9:
                        return M32;
                    case 10:
                        return M33;
                    case 11:
                        return M34;
                    case 12:
                        return M41;
                    case 13:
                        return M42;
                    case 14:
                        return M43;
                    case 15:
                        return M44;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates new instance of <see cref="Matrix4x4" /> given sixteen components values.
        /// </summary>
        /// <param name="m11">Component in first row and first column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m12">Component in first row and second column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m13">Component in first row and third column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m14">Component in first row and fourth column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m21">Component in second row and first column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m22">Component in second row and second column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m23">Component in second row and third column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m24">Component in second row and fourth column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m31">Component in third row and first column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m32">Component in third row and second column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m33">Component in third row and third column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m34">Component in third row and fourth column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m41">Component in fourth row and first column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m42">Component in fourth row and second column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m43">Component in fourth row and third column of the <see cref="Matrix4x4" />.</param>
        /// <param name="m44">Component in fourth row and fourth column of the <see cref="Matrix4x4" />.</param>
        public Matrix4x4(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        /// <summary>
        ///     Creates new instance of <see cref="Matrix4x4" /> given array of size sixteen containing sixteen components values
        ///     in row-major layout.
        /// </summary>
        /// <param name="array">Array of size sixteen with sixteen components values in row-major layout.</param>
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not sixteen.</exception>
        public Matrix4x4(IReadOnlyList<double> array)
        {
            if (array.Count != 16) throw new ArgumentException("Array must have length of 16 elements.");

            M11 = array[0];
            M12 = array[1];
            M13 = array[2];
            M14 = array[3];

            M21 = array[4];
            M22 = array[5];
            M23 = array[6];
            M24 = array[7];

            M31 = array[8];
            M32 = array[9];
            M33 = array[10];
            M34 = array[11];

            M41 = array[12];
            M42 = array[13];
            M43 = array[14];
            M44 = array[15];
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds other matrix to this matrix.
        /// </summary>
        /// <param name="other">Other matrix to add.</param>
        /// <returns><see cref="Matrix4x4" /> that is sum of this matrix with the other.</returns>
        public Matrix4x4 Add(Matrix4x4 other)
        {
            return new Matrix4x4(
                M11 + other.M11,
                M12 + other.M12,
                M13 + other.M13,
                M14 + other.M14,
                M21 + other.M21,
                M22 + other.M22,
                M23 + other.M23,
                M24 + other.M24,
                M31 + other.M31,
                M32 + other.M32,
                M33 + other.M33,
                M34 + other.M34,
                M41 + other.M41,
                M42 + other.M42,
                M43 + other.M43,
                M44 + other.M44
            );
        }

        /// <summary>
        ///     Subtracts other matrix from this matrix.
        /// </summary>
        /// <param name="other">Other matrix to subtract.</param>
        /// <returns><see cref="Matrix4x4" /> that is difference between this matrix and the other.</returns>
        public Matrix4x4 Subtract(Matrix4x4 other)
        {
            return new Matrix4x4(
                M11 - other.M11,
                M12 - other.M12,
                M13 - other.M13,
                M14 - other.M14,
                M21 - other.M21,
                M22 - other.M22,
                M23 - other.M23,
                M24 - other.M24,
                M31 - other.M31,
                M32 - other.M32,
                M33 - other.M33,
                M34 - other.M34,
                M41 - other.M41,
                M42 - other.M42,
                M43 - other.M43,
                M44 - other.M44
            );
        }

        /// <summary>
        ///     Multiplies this matrix by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of matrix.</param>
        /// <returns><see cref="Matrix4x4" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Matrix4x4 Multiply(double scalar)
        {
            return new Matrix4x4(
                M11 * scalar,
                M12 * scalar,
                M13 * scalar,
                M14 * scalar,
                M21 * scalar,
                M22 * scalar,
                M23 * scalar,
                M24 * scalar,
                M31 * scalar,
                M32 * scalar,
                M33 * scalar,
                M34 * scalar,
                M41 * scalar,
                M42 * scalar,
                M43 * scalar,
                M44 * scalar
            );
        }

        /// <summary>
        ///     Multiplies this matrix by other matrix.
        /// </summary>
        /// <param name="other">Matrix to multiply by (the multiplier).</param>
        /// <returns><see cref="Matrix4x4" /> that is product of this matrix multiplied by the <paramref name="other" />.</returns>
        public Matrix4x4 Multiply(Matrix4x4 other)
        {
            return new Matrix4x4(
                M11 * other.M11 + M12 * other.M21 + M13 * other.M31 + M14 * other.M41,
                M11 * other.M12 + M12 * other.M22 + M13 * other.M32 + M14 * other.M42,
                M11 * other.M13 + M12 * other.M23 + M13 * other.M33 + M14 * other.M43,
                M11 * other.M14 + M12 * other.M24 + M13 * other.M34 + M14 * other.M44,
                M21 * other.M11 + M22 * other.M21 + M23 * other.M31 + M24 * other.M41,
                M21 * other.M12 + M22 * other.M22 + M23 * other.M32 + M24 * other.M42,
                M21 * other.M13 + M22 * other.M23 + M23 * other.M33 + M24 * other.M43,
                M21 * other.M14 + M22 * other.M24 + M23 * other.M34 + M24 * other.M44,
                M31 * other.M11 + M32 * other.M21 + M33 * other.M31 + M34 * other.M41,
                M31 * other.M12 + M32 * other.M22 + M33 * other.M32 + M34 * other.M42,
                M31 * other.M13 + M32 * other.M23 + M33 * other.M33 + M34 * other.M43,
                M31 * other.M14 + M32 * other.M24 + M33 * other.M34 + M34 * other.M44,
                M41 * other.M11 + M42 * other.M21 + M43 * other.M31 + M44 * other.M41,
                M41 * other.M12 + M42 * other.M22 + M43 * other.M32 + M44 * other.M42,
                M41 * other.M13 + M42 * other.M23 + M43 * other.M33 + M44 * other.M43,
                M41 * other.M14 + M42 * other.M24 + M43 * other.M34 + M44 * other.M44
            );
        }

        /// <summary>
        ///     Multiplies this matrix by given vector.
        /// </summary>
        /// <param name="vector">Vector to multiply by (the multiplier).</param>
        /// <returns><see cref="Vector4" /> that is product of this matrix multiplied by the <paramref name="vector" />.</returns>
        public Vector4 Multiply(Vector4 vector)
        {
            return new Vector4(
                M11 * vector.X + M12 * vector.Y + M13 * vector.Z + M14 * vector.W,
                M21 * vector.X + M22 * vector.Y + M23 * vector.Z + M24 * vector.W,
                M31 * vector.X + M32 * vector.Y + M33 * vector.Z + M34 * vector.W,
                M41 * vector.X + M42 * vector.Y + M43 * vector.Z + M44 * vector.W
            );
        }

        /// <summary>
        ///     Divides this matrix by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of matrix.</param>
        /// <returns><see cref="Matrix4x4" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Matrix4x4 Divide(double scalar)
        {
            return new Matrix4x4(
                M11 / scalar,
                M12 / scalar,
                M13 / scalar,
                M14 / scalar,
                M21 / scalar,
                M22 / scalar,
                M23 / scalar,
                M24 / scalar,
                M31 / scalar,
                M32 / scalar,
                M33 / scalar,
                M34 / scalar,
                M41 / scalar,
                M42 / scalar,
                M43 / scalar,
                M44 / scalar
            );
        }

        /// <summary>
        ///     Returns array that contains matrix components in row-major layout.
        /// </summary>
        /// <returns>Array with matrix components in row-major layout.</returns>
        public double[] ToArray()
        {
            return new[] {M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44};
        }

        /// <summary>
        ///     Returns a value indicating whether the value of this instance is equal to the value of the specified
        ///     <see cref="Matrix4x4" /> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="other" /> parameter equals the value of this instance; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(Matrix4x4 other)
        {
            return M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) && M14.Equals(other.M14) &&
                   M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) && M24.Equals(other.M24) &&
                   M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33) && M34.Equals(other.M34) &&
                   M41.Equals(other.M41) && M42.Equals(other.M42) && M43.Equals(other.M43) && M44.Equals(other.M44);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is an instance of <see cref="Matrix4x4" /> and equals the value of this
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Matrix4x4 other && Equals(other);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = M11.GetHashCode();
                hashCode = (hashCode * 397) ^ M12.GetHashCode();
                hashCode = (hashCode * 397) ^ M13.GetHashCode();
                hashCode = (hashCode * 397) ^ M14.GetHashCode();
                hashCode = (hashCode * 397) ^ M21.GetHashCode();
                hashCode = (hashCode * 397) ^ M22.GetHashCode();
                hashCode = (hashCode * 397) ^ M23.GetHashCode();
                hashCode = (hashCode * 397) ^ M24.GetHashCode();
                hashCode = (hashCode * 397) ^ M31.GetHashCode();
                hashCode = (hashCode * 397) ^ M32.GetHashCode();
                hashCode = (hashCode * 397) ^ M33.GetHashCode();
                hashCode = (hashCode * 397) ^ M34.GetHashCode();
                hashCode = (hashCode * 397) ^ M41.GetHashCode();
                hashCode = (hashCode * 397) ^ M42.GetHashCode();
                hashCode = (hashCode * 397) ^ M43.GetHashCode();
                hashCode = (hashCode * 397) ^ M44.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        ///     Returns 3D translation matrix that represents translation by specified <paramref name="translation" /> vector.
        /// </summary>
        /// <param name="translation">Translation that is applied by matrix.</param>
        /// <returns><see cref="Matrix4x4" /> that represents translation by <paramref name="translation" /> vector.</returns>
        public static Matrix4x4 CreateTranslation(Vector3 translation)
        {
            return new Matrix4x4(
                1, 0, 0, translation.X,
                0, 1, 0, translation.Y,
                0, 0, 1, translation.Z,
                0, 0, 0, 1
            );
        }

        /// <summary>
        ///     Returns 3D rotation matrix that represents counterclockwise rotation around X axis by <paramref name="angle" />
        ///     specified in radians.
        /// </summary>
        /// <param name="angle">Rotation angle in radians that is applied by matrix.</param>
        /// <returns>
        ///     <see cref="Matrix4x4" /> that represents counterclockwise rotation around X axis by <paramref name="angle" />
        ///     radians.
        /// </returns>
        public static Matrix4x4 CreateRotationX(double angle)
        {
            return new Matrix4x4(
                1, 0, 0, 0,
                0, System.Math.Cos(angle), -System.Math.Sin(angle), 0,
                0, System.Math.Sin(angle), System.Math.Cos(angle), 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        ///     Returns 3D rotation matrix that represents counterclockwise rotation around Y axis by <paramref name="angle" />
        ///     specified in radians.
        /// </summary>
        /// <param name="angle">Rotation angle in radians that is applied by matrix.</param>
        /// <returns>
        ///     <see cref="Matrix4x4" /> that represents counterclockwise rotation around Y axis by <paramref name="angle" />
        ///     radians.
        /// </returns>
        public static Matrix4x4 CreateRotationY(double angle)
        {
            return new Matrix4x4(
                System.Math.Cos(angle), 0, System.Math.Sin(angle), 0,
                0, 1, 0, 0,
                -System.Math.Sin(angle), 0, System.Math.Cos(angle), 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        ///     Returns 3D rotation matrix that represents counterclockwise rotation around Z axis by <paramref name="angle" />
        ///     specified in radians.
        /// </summary>
        /// <param name="angle">Rotation angle in radians that is applied by matrix.</param>
        /// <returns>
        ///     <see cref="Matrix4x4" /> that represents counterclockwise rotation around Z axis by <paramref name="angle" />
        ///     radians.
        /// </returns>
        public static Matrix4x4 CreateRotationZ(double angle)
        {
            return new Matrix4x4(
                System.Math.Cos(angle), -System.Math.Sin(angle), 0, 0,
                System.Math.Sin(angle), System.Math.Cos(angle), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }


        /// <summary>
        ///     Returns 3D rotation matrix that represents counterclockwise rotation around Z axis then around X axis and then Y
        ///     axis by <paramref name="rotation" /> angles specified in radians.
        /// </summary>
        /// <param name="rotation">
        ///     Rotation angles around three axes in radians that are applied by matrix. Rotation is a
        ///     <see cref="Vector3" /> where X is rotation angle around X axis, Y is rotation angle around Y axis and Z is rotation
        ///     angle around Z axis.
        /// </param>
        /// <returns>
        ///     <see cref="Matrix4x4" /> that represents counterclockwise rotations around Z axis then around X axis and then Y
        ///     axis by <paramref name="rotation" /> angles specified in radians.
        /// </returns>
        // ReSharper disable once InconsistentNaming
        public static Matrix4x4 CreateRotationZXY(Vector3 rotation)
        {
            return CreateRotationY(rotation.Y) * CreateRotationX(rotation.X) * CreateRotationZ(rotation.Z);
        }

        /// <summary>
        ///     Returns 3D scale matrix that represents scaling by <paramref name="scale" /> vector.
        /// </summary>
        /// <param name="scale">
        ///     Scale that is applied by matrix. Scale is a <see cref="Vector3" /> where X is scaling factor along X axis, Y is
        ///     scaling factor along Y axis and Z is scaling factor along Z axis.
        /// </param>
        /// <returns><see cref="Matrix4x4" /> that represents scaling by <paramref name="scale" /> vector.</returns>
        public static Matrix4x4 CreateScale(Vector3 scale)
        {
            return new Matrix4x4(
                scale.X, 0, 0, 0,
                0, scale.Y, 0, 0,
                0, 0, scale.Z, 0,
                0, 0, 0, 1
            );
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one matrix to another.
        /// </summary>
        /// <param name="left">The first matrix to add.</param>
        /// <param name="right">The second matrix to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Matrix4x4 operator +(Matrix4x4 left, Matrix4x4 right)
        {
            return left.Add(right);
        }

        /// <summary>
        ///     Subtracts one matrix from another.
        /// </summary>
        /// <param name="left">Matrix to subtract from (the minuend).</param>
        /// <param name="right">Matrix to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Matrix4x4 operator -(Matrix4x4 left, Matrix4x4 right)
        {
            return left.Subtract(right);
        }

        /// <summary>
        ///     Multiplies specified matrix by given scalar.
        /// </summary>
        /// <param name="left">Matrix to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of matrix.</param>
        /// <returns><see cref="Matrix4x4" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Matrix4x4 operator *(Matrix4x4 left, double right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        ///     Multiplies one matrix by another.
        /// </summary>
        /// <param name="left">Matrix to be multiplied (the multiplicand).</param>
        /// <param name="right">Matrix to multiply by (the multiplier).</param>
        /// <returns>
        ///     <see cref="Matrix4x4" /> that is product of <paramref name="left" /> matrix multiplied by the
        ///     <paramref name="right" /> matrix.
        /// </returns>
        public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        ///     Multiplies specified matrix by given vector.
        /// </summary>
        /// <param name="left">Matrix to be multiplied (the multiplicand).</param>
        /// <param name="right">Vector to multiply by (the multiplier).</param>
        /// <returns>
        ///     <see cref="Matrix4x4" /> that is product of <paramref name="left" /> matrix multiplied by the
        ///     <paramref name="right" /> vector.
        /// </returns>
        public static Vector4 operator *(Matrix4x4 left, Vector4 right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        ///     Divides specified matrix by given scalar.
        /// </summary>
        /// <param name="left">Matrix to be divided.</param>
        /// <param name="right">Scalar value that is divisor of matrix.</param>
        /// <returns><see cref="Matrix4x4" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Matrix4x4 operator /(Matrix4x4 left, double right)
        {
            return left.Divide(right);
        }

        /// <summary>
        ///     Returns matrix opposite to the specified matrix, that is matrix with all components negated.
        /// </summary>
        /// <param name="right">Matrix to be negated.</param>
        /// <returns>Matrix opposite to the specified matrix, that is matrix with all components negated.</returns>
        public static Matrix4x4 operator -(Matrix4x4 right)
        {
            return right.Opposite;
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Matrix4x4" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Matrix4x4" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Matrix4x4 left, Matrix4x4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Matrix4x4" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Matrix4x4" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Matrix4x4 left, Matrix4x4 right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}