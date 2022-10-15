﻿using System;
using System.Collections.Generic;

namespace MicroBenchmark
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
    public readonly struct Matrix4x4Baseline : IEquatable<Matrix4x4Baseline>
    {
        #region Static properties

        /// <summary>
        ///     Returns <see cref="Matrix4x4Baseline" /> that has all components set to zero.
        /// </summary>
        public static Matrix4x4Baseline Zero => new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        ///     Returns identity <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public static Matrix4x4Baseline Identity => new(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        #endregion

        #region Properties

        /// <summary>
        ///     Component in first row and first column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M11 { get; }

        /// <summary>
        ///     Component in first row and second column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M12 { get; }

        /// <summary>
        ///     Component in first row and third column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M13 { get; }

        /// <summary>
        ///     Component in first row and fourth column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M14 { get; }

        /// <summary>
        ///     Component in second row and first column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M21 { get; }

        /// <summary>
        ///     Component in second row and second column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M22 { get; }

        /// <summary>
        ///     Component in second row and third column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M23 { get; }

        /// <summary>
        ///     Component in second row and fourth column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M24 { get; }

        /// <summary>
        ///     Component in third row and first column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M31 { get; }

        /// <summary>
        ///     Component in third row and second column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M32 { get; }

        /// <summary>
        ///     Component in third row and third column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M33 { get; }

        /// <summary>
        ///     Component in third row and fourth column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M34 { get; }

        /// <summary>
        ///     Component in fourth row and first column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M41 { get; }

        /// <summary>
        ///     Component in fourth row and second column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M42 { get; }

        /// <summary>
        ///     Component in fourth row and third column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M43 { get; }

        /// <summary>
        ///     Component in fourth row and fourth column of the <see cref="Matrix4x4Baseline" />.
        /// </summary>
        public double M44 { get; }

        /// <summary>
        ///     Returns matrix opposite to this matrix, that is matrix with all components negated.
        /// </summary>
        public Matrix4x4Baseline Opposite
            =>
                new(-M11, -M12, -M13, -M14, -M21, -M22, -M23, -M24, -M31, -M32, -M33, -M34, -M41, -M42, -M43,
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

                return index switch
                {
                    0 => M11,
                    1 => M12,
                    2 => M13,
                    3 => M14,
                    4 => M21,
                    5 => M22,
                    6 => M23,
                    7 => M24,
                    8 => M31,
                    9 => M32,
                    10 => M33,
                    11 => M34,
                    12 => M41,
                    13 => M42,
                    14 => M43,
                    15 => M44,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates new instance of <see cref="Matrix4x4Baseline" /> given sixteen components values.
        /// </summary>
        /// <param name="m11">Component in first row and first column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m12">Component in first row and second column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m13">Component in first row and third column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m14">Component in first row and fourth column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m21">Component in second row and first column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m22">Component in second row and second column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m23">Component in second row and third column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m24">Component in second row and fourth column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m31">Component in third row and first column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m32">Component in third row and second column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m33">Component in third row and third column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m34">Component in third row and fourth column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m41">Component in fourth row and first column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m42">Component in fourth row and second column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m43">Component in fourth row and third column of the <see cref="Matrix4x4Baseline" />.</param>
        /// <param name="m44">Component in fourth row and fourth column of the <see cref="Matrix4x4Baseline" />.</param>
        public Matrix4x4Baseline(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24,
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
        ///     Creates new instance of <see cref="Matrix4x4Baseline" /> given array of size sixteen containing sixteen components values
        ///     in row-major layout.
        /// </summary>
        /// <param name="array">Array of size sixteen with sixteen components values in row-major layout.</param>
        /// <exception cref="ArgumentException">Length of <paramref name="array" /> is not sixteen.</exception>
        public Matrix4x4Baseline(IReadOnlyList<double> array)
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
        /// <returns><see cref="Matrix4x4Baseline" /> that is sum of this matrix with the other.</returns>
        public Matrix4x4Baseline Add(Matrix4x4Baseline other) =>
            new(
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

        /// <summary>
        ///     Subtracts other matrix from this matrix.
        /// </summary>
        /// <param name="other">Other matrix to subtract.</param>
        /// <returns><see cref="Matrix4x4Baseline" /> that is difference between this matrix and the other.</returns>
        public Matrix4x4Baseline Subtract(Matrix4x4Baseline other) =>
            new(
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

        /// <summary>
        ///     Multiplies this matrix by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is multiplier of matrix.</param>
        /// <returns><see cref="Matrix4x4Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public Matrix4x4Baseline Multiply(double scalar) =>
            new(
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

        /// <summary>
        ///     Multiplies this matrix by other matrix.
        /// </summary>
        /// <param name="other">Matrix to multiply by (the multiplier).</param>
        /// <returns><see cref="Matrix4x4Baseline" /> that is product of this matrix multiplied by the <paramref name="other" />.</returns>
        public Matrix4x4Baseline Multiply(Matrix4x4Baseline other) =>
            new(
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

        /// <summary>
        ///     Multiplies this matrix by given vector.
        /// </summary>
        /// <param name="vector">Vector to multiply by (the multiplier).</param>
        /// <returns><see cref="Vector4" /> that is product of this matrix multiplied by the <paramref name="vector" />.</returns>
        public Vector4Baseline Multiply(Vector4Baseline vector) =>
            new(
                M11 * vector.X + M12 * vector.Y + M13 * vector.Z + M14 * vector.W,
                M21 * vector.X + M22 * vector.Y + M23 * vector.Z + M24 * vector.W,
                M31 * vector.X + M32 * vector.Y + M33 * vector.Z + M34 * vector.W,
                M41 * vector.X + M42 * vector.Y + M43 * vector.Z + M44 * vector.W
            );

        /// <summary>
        ///     Divides this matrix by given scalar.
        /// </summary>
        /// <param name="scalar">Scalar value that is divisor of matrix.</param>
        /// <returns><see cref="Matrix4x4Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public Matrix4x4Baseline Divide(double scalar) =>
            new(
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

        /// <summary>
        ///     Returns array that contains matrix components in row-major layout.
        /// </summary>
        /// <returns>Array with matrix components in row-major layout.</returns>
        public double[] ToArray()
        {
            return new[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
        }

        /// <inheritdoc />
        public bool Equals(Matrix4x4Baseline other) =>
            M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) && M14.Equals(other.M14) &&
            M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) && M24.Equals(other.M24) &&
            M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33) && M34.Equals(other.M34) &&
            M41.Equals(other.M41) && M42.Equals(other.M42) && M43.Equals(other.M43) && M44.Equals(other.M44);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Matrix4x4Baseline other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(M11);
            hashCode.Add(M12);
            hashCode.Add(M13);
            hashCode.Add(M14);
            hashCode.Add(M21);
            hashCode.Add(M22);
            hashCode.Add(M23);
            hashCode.Add(M24);
            hashCode.Add(M31);
            hashCode.Add(M32);
            hashCode.Add(M33);
            hashCode.Add(M34);
            hashCode.Add(M41);
            hashCode.Add(M42);
            hashCode.Add(M43);
            hashCode.Add(M44);
            return hashCode.ToHashCode();
        }

        #endregion

        #region Static methods

        /// <summary>
        ///     Returns 3D translation matrix that represents translation by specified <paramref name="translation" /> vector.
        /// </summary>
        /// <param name="translation">Translation that is applied by matrix.</param>
        /// <returns><see cref="Matrix4x4Baseline" /> that represents translation by <paramref name="translation" /> vector.</returns>
        public static Matrix4x4Baseline CreateTranslation(Vector3Baseline translation) =>
            new(
                1, 0, 0, translation.X,
                0, 1, 0, translation.Y,
                0, 0, 1, translation.Z,
                0, 0, 0, 1
            );

        /// <summary>
        ///     Returns 3D rotation matrix that represents counterclockwise rotation around X axis by <paramref name="angle" />
        ///     specified in radians.
        /// </summary>
        /// <param name="angle">Rotation angle in radians that is applied by matrix.</param>
        /// <returns>
        ///     <see cref="Matrix4x4Baseline" /> that represents counterclockwise rotation around X axis by <paramref name="angle" />
        ///     radians.
        /// </returns>
        public static Matrix4x4Baseline CreateRotationX(double angle) =>
            new(
                1, 0, 0, 0,
                0, System.Math.Cos(angle), -System.Math.Sin(angle), 0,
                0, System.Math.Sin(angle), System.Math.Cos(angle), 0,
                0, 0, 0, 1
            );

        /// <summary>
        ///     Returns 3D rotation matrix that represents counterclockwise rotation around Y axis by <paramref name="angle" />
        ///     specified in radians.
        /// </summary>
        /// <param name="angle">Rotation angle in radians that is applied by matrix.</param>
        /// <returns>
        ///     <see cref="Matrix4x4Baseline" /> that represents counterclockwise rotation around Y axis by <paramref name="angle" />
        ///     radians.
        /// </returns>
        public static Matrix4x4Baseline CreateRotationY(double angle) =>
            new(
                System.Math.Cos(angle), 0, System.Math.Sin(angle), 0,
                0, 1, 0, 0,
                -System.Math.Sin(angle), 0, System.Math.Cos(angle), 0,
                0, 0, 0, 1
            );

        /// <summary>
        ///     Returns 3D rotation matrix that represents counterclockwise rotation around Z axis by <paramref name="angle" />
        ///     specified in radians.
        /// </summary>
        /// <param name="angle">Rotation angle in radians that is applied by matrix.</param>
        /// <returns>
        ///     <see cref="Matrix4x4Baseline" /> that represents counterclockwise rotation around Z axis by <paramref name="angle" />
        ///     radians.
        /// </returns>
        public static Matrix4x4Baseline CreateRotationZ(double angle) =>
            new(
                System.Math.Cos(angle), -System.Math.Sin(angle), 0, 0,
                System.Math.Sin(angle), System.Math.Cos(angle), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );


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
        ///     <see cref="Matrix4x4Baseline" /> that represents counterclockwise rotations around Z axis then around X axis and then Y
        ///     axis by <paramref name="rotation" /> angles specified in radians.
        /// </returns>
        // ReSharper disable once InconsistentNaming
        public static Matrix4x4Baseline CreateRotationZXY(Vector3Baseline rotation) => CreateRotationY(rotation.Y) * CreateRotationX(rotation.X) * CreateRotationZ(rotation.Z);

        /// <summary>
        ///     Returns 3D scale matrix that represents scaling by <paramref name="scale" /> vector.
        /// </summary>
        /// <param name="scale">
        ///     Scale that is applied by matrix. Scale is a <see cref="Vector3" /> where X is scaling factor along X axis, Y is
        ///     scaling factor along Y axis and Z is scaling factor along Z axis.
        /// </param>
        /// <returns><see cref="Matrix4x4Baseline" /> that represents scaling by <paramref name="scale" /> vector.</returns>
        public static Matrix4x4Baseline CreateScale(Vector3Baseline scale) =>
            new(
                scale.X, 0, 0, 0,
                0, scale.Y, 0, 0,
                0, 0, scale.Z, 0,
                0, 0, 0, 1
            );

        #endregion

        #region Operators

        /// <summary>
        ///     Adds one matrix to another.
        /// </summary>
        /// <param name="left">The first matrix to add.</param>
        /// <param name="right">The second matrix to add.</param>
        /// <returns>An object that is the sum of the values of <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Matrix4x4Baseline operator +(Matrix4x4Baseline left, Matrix4x4Baseline right) => left.Add(right);

        /// <summary>
        ///     Subtracts one matrix from another.
        /// </summary>
        /// <param name="left">Matrix to subtract from (the minuend).</param>
        /// <param name="right">Matrix to subtract (the subtrahend).</param>
        /// <returns>
        ///     An object that is the result of the value of <paramref name="left" /> minus the value of
        ///     <paramref name="right" />.
        /// </returns>
        public static Matrix4x4Baseline operator -(Matrix4x4Baseline left, Matrix4x4Baseline right) => left.Subtract(right);

        /// <summary>
        ///     Multiplies specified matrix by given scalar.
        /// </summary>
        /// <param name="left">Matrix to be multiplied.</param>
        /// <param name="right">Scalar value that is multiplier of matrix.</param>
        /// <returns><see cref="Matrix4x4Baseline" /> that is multiplied by scalar that is each of its components is multiplied by scalar.</returns>
        public static Matrix4x4Baseline operator *(Matrix4x4Baseline left, double right) => left.Multiply(right);

        /// <summary>
        ///     Multiplies one matrix by another.
        /// </summary>
        /// <param name="left">Matrix to be multiplied (the multiplicand).</param>
        /// <param name="right">Matrix to multiply by (the multiplier).</param>
        /// <returns>
        ///     <see cref="Matrix4x4Baseline" /> that is product of <paramref name="left" /> matrix multiplied by the
        ///     <paramref name="right" /> matrix.
        /// </returns>
        public static Matrix4x4Baseline operator *(Matrix4x4Baseline left, Matrix4x4Baseline right) => left.Multiply(right);

        /// <summary>
        ///     Multiplies specified matrix by given vector.
        /// </summary>
        /// <param name="left">Matrix to be multiplied (the multiplicand).</param>
        /// <param name="right">Vector to multiply by (the multiplier).</param>
        /// <returns>
        ///     <see cref="Matrix4x4Baseline" /> that is product of <paramref name="left" /> matrix multiplied by the
        ///     <paramref name="right" /> vector.
        /// </returns>
        public static Vector4Baseline operator *(Matrix4x4Baseline left, Vector4Baseline right) => left.Multiply(right);

        /// <summary>
        ///     Divides specified matrix by given scalar.
        /// </summary>
        /// <param name="left">Matrix to be divided.</param>
        /// <param name="right">Scalar value that is divisor of matrix.</param>
        /// <returns><see cref="Matrix4x4Baseline" /> that is divided by scalar that is each of its components is divided by scalar.</returns>
        public static Matrix4x4Baseline operator /(Matrix4x4Baseline left, double right) => left.Divide(right);

        /// <summary>
        ///     Returns matrix opposite to the specified matrix, that is matrix with all components negated.
        /// </summary>
        /// <param name="right">Matrix to be negated.</param>
        /// <returns>Matrix opposite to the specified matrix, that is matrix with all components negated.</returns>
        public static Matrix4x4Baseline operator -(Matrix4x4Baseline right) => right.Opposite;

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Matrix4x4Baseline" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="Matrix4x4Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Matrix4x4Baseline left, Matrix4x4Baseline right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="Matrix4x4Baseline" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="Matrix4x4Baseline" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Matrix4x4Baseline left, Matrix4x4Baseline right) => !left.Equals(right);

        #endregion
    }
}