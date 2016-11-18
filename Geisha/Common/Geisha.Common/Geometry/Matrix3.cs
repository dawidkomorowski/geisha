using System;

namespace Geisha.Common.Geometry
{
    public struct Matrix3 : IMatrix<Matrix3>
    {
        public static Matrix3 Zero => new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static Matrix3 Identity => new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public double M11 { get; }
        public double M12 { get; }
        public double M13 { get; }
        public double M21 { get; }
        public double M22 { get; }
        public double M23 { get; }
        public double M31 { get; }
        public double M32 { get; }
        public double M33 { get; }

        public Matrix3 Opposite => new Matrix3(-M11, -M12, -M13, -M21, -M22, -M23, -M31, -M32, -M33);
        public double[] Array => new[] {M11, M12, M13, M21, M22, M23, M31, M32, M33};

        public double this[int row, int column]
        {
            get
            {
                var index = row*3 + column;

                switch (index)
                {
                    case 0:
                        return M11;
                    case 1:
                        return M12;
                    case 2:
                        return M13;
                    case 3:
                        return M21;
                    case 4:
                        return M22;
                    case 5:
                        return M23;
                    case 6:
                        return M31;
                    case 7:
                        return M32;
                    case 8:
                        return M33;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Matrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32,
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

        public Matrix3(double[] array)
        {
            if (array.Length != 9)
            {
                throw new ArgumentException("Array must be the length of 9 elements.");
            }

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

        public Matrix3 Add(Matrix3 other)
        {
            return new Matrix3(
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
        }

        public Matrix3 Subtract(Matrix3 other)
        {
            return new Matrix3(
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
        }

        public Matrix3 Multiply(double scalar)
        {
            return new Matrix3(
                M11*scalar,
                M12*scalar,
                M13*scalar,
                M21*scalar,
                M22*scalar,
                M23*scalar,
                M31*scalar,
                M32*scalar,
                M33*scalar
            );
        }

        public Matrix3 Multiply(Matrix3 other)
        {
            return new Matrix3(
                M11*other.M11 + M12*other.M21 + M13*other.M31,
                M11*other.M12 + M12*other.M22 + M13*other.M32,
                M11*other.M13 + M12*other.M23 + M13*other.M33,
                M21*other.M11 + M22*other.M21 + M23*other.M31,
                M21*other.M12 + M22*other.M22 + M23*other.M32,
                M21*other.M13 + M22*other.M23 + M23*other.M33,
                M31*other.M11 + M32*other.M21 + M33*other.M31,
                M31*other.M12 + M32*other.M22 + M33*other.M32,
                M31*other.M13 + M32*other.M23 + M33*other.M33
            );
        }

        public Matrix3 Divide(double scalar)
        {
            return new Matrix3(
                M11/scalar,
                M12/scalar,
                M13/scalar,
                M21/scalar,
                M22/scalar,
                M23/scalar,
                M31/scalar,
                M32/scalar,
                M33/scalar
            );
        }

        public bool Equals(Matrix3 other)
        {
            return M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13) &&
                   M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23) &&
                   M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Matrix3 && Equals((Matrix3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = M11.GetHashCode();
                hashCode = (hashCode*397) ^ M12.GetHashCode();
                hashCode = (hashCode*397) ^ M13.GetHashCode();
                hashCode = (hashCode*397) ^ M21.GetHashCode();
                hashCode = (hashCode*397) ^ M22.GetHashCode();
                hashCode = (hashCode*397) ^ M23.GetHashCode();
                hashCode = (hashCode*397) ^ M31.GetHashCode();
                hashCode = (hashCode*397) ^ M32.GetHashCode();
                hashCode = (hashCode*397) ^ M33.GetHashCode();
                return hashCode;
            }
        }

        public static Matrix3 operator +(Matrix3 left, Matrix3 right)
        {
            return left.Add(right);
        }

        public static Matrix3 operator -(Matrix3 left, Matrix3 right)
        {
            return left.Subtract(right);
        }

        public static Matrix3 operator *(Matrix3 left, double right)
        {
            return left.Multiply(right);
        }

        public static Matrix3 operator *(Matrix3 left, Matrix3 right)
        {
            return left.Multiply(right);
        }

        public static Matrix3 operator /(Matrix3 left, double right)
        {
            return left.Divide(right);
        }

        public static Matrix3 operator -(Matrix3 right)
        {
            return right.Opposite;
        }

        public static bool operator ==(Matrix3 left, Matrix3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix3 left, Matrix3 right)
        {
            return !left.Equals(right);
        }
    }
}