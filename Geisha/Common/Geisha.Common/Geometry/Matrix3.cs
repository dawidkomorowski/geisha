using System;

namespace Geisha.Common.Geometry
{
    // TODO Default parameterless constructor does not work correctly because it cannot create internal array
    public struct Matrix3 : IMatrix<Matrix3>
    {
        // TODO Refactor to be nine double readonly fields, but test the performance first
        private readonly double[] _matrix;

        public static Matrix3 Zero => new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static Matrix3 Identity => new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public Matrix3 Opposite
            =>
            new Matrix3(-this[0, 0], -this[0, 1], -this[0, 2], -this[1, 0], -this[1, 1], -this[1, 2], -this[2, 0],
                -this[2, 1], -this[2, 2]);

        public double[] Array
            =>
            new[]
            {
                _matrix[0], _matrix[1], _matrix[2],
                _matrix[3], _matrix[4], _matrix[5],
                _matrix[6], _matrix[7], _matrix[8]
            };

        public double this[int row, int column]
        {
            get { return _matrix[row*3 + column]; }
            private set { _matrix[row*3 + column] = value; }
        }

        public Matrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32,
            double m33)
        {
            _matrix = new double[9];

            this[0, 0] = m11;
            this[0, 1] = m12;
            this[0, 2] = m13;

            this[1, 0] = m21;
            this[1, 1] = m22;
            this[1, 2] = m23;

            this[2, 0] = m31;
            this[2, 1] = m32;
            this[2, 2] = m33;
        }

        public Matrix3(double[] matrix)
        {
            if (matrix.Length != 9)
            {
                throw new ArgumentException("Array must be the length of 9 elements.");
            }

            _matrix = new double[9];

            this[0, 0] = matrix[0];
            this[0, 1] = matrix[1];
            this[0, 2] = matrix[2];

            this[1, 0] = matrix[3];
            this[1, 1] = matrix[4];
            this[1, 2] = matrix[5];

            this[2, 0] = matrix[6];
            this[2, 1] = matrix[7];
            this[2, 2] = matrix[8];
        }

        public Matrix3 Add(Matrix3 other)
        {
            return new Matrix3(
                this[0, 0] + other[0, 0],
                this[0, 1] + other[0, 1],
                this[0, 2] + other[0, 2],
                this[1, 0] + other[1, 0],
                this[1, 1] + other[1, 1],
                this[1, 2] + other[1, 2],
                this[2, 0] + other[2, 0],
                this[2, 1] + other[2, 1],
                this[2, 2] + other[2, 2]
            );
        }

        public Matrix3 Subtract(Matrix3 other)
        {
            return new Matrix3(
                this[0, 0] - other[0, 0],
                this[0, 1] - other[0, 1],
                this[0, 2] - other[0, 2],
                this[1, 0] - other[1, 0],
                this[1, 1] - other[1, 1],
                this[1, 2] - other[1, 2],
                this[2, 0] - other[2, 0],
                this[2, 1] - other[2, 1],
                this[2, 2] - other[2, 2]
            );
        }

        public Matrix3 Multiply(double scalar)
        {
            return new Matrix3(
                this[0, 0]*scalar,
                this[0, 1]*scalar,
                this[0, 2]*scalar,
                this[1, 0]*scalar,
                this[1, 1]*scalar,
                this[1, 2]*scalar,
                this[2, 0]*scalar,
                this[2, 1]*scalar,
                this[2, 2]*scalar
            );
        }

        public Matrix3 Multiply(Matrix3 other)
        {
            return new Matrix3(
                this[0, 0]*other[0, 0] + this[0, 1]*other[1, 0] + this[0, 2]*other[2, 0],
                this[0, 0]*other[0, 1] + this[0, 1]*other[1, 1] + this[0, 2]*other[2, 1],
                this[0, 0]*other[0, 2] + this[0, 1]*other[1, 2] + this[0, 2]*other[2, 2],
                this[1, 0]*other[0, 0] + this[1, 1]*other[1, 0] + this[1, 2]*other[2, 0],
                this[1, 0]*other[0, 1] + this[1, 1]*other[1, 1] + this[1, 2]*other[2, 1],
                this[1, 0]*other[0, 2] + this[1, 1]*other[1, 2] + this[1, 2]*other[2, 2],
                this[2, 0]*other[0, 0] + this[2, 1]*other[1, 0] + this[2, 2]*other[2, 0],
                this[2, 0]*other[0, 1] + this[2, 1]*other[1, 1] + this[2, 2]*other[2, 1],
                this[2, 0]*other[0, 2] + this[2, 1]*other[1, 2] + this[2, 2]*other[2, 2]
            );
        }

        public Matrix3 Divide(double scalar)
        {
            return new Matrix3(
                this[0, 0]/scalar,
                this[0, 1]/scalar,
                this[0, 2]/scalar,
                this[1, 0]/scalar,
                this[1, 1]/scalar,
                this[1, 2]/scalar,
                this[2, 0]/scalar,
                this[2, 1]/scalar,
                this[2, 2]/scalar
            );
        }

        public bool Equals(Matrix3 other)
        {
            return this[0, 0].Equals(other[0, 0]) && this[0, 1].Equals(other[0, 1]) && this[0, 2].Equals(other[0, 2]) &&
                   this[1, 0].Equals(other[1, 0]) && this[1, 1].Equals(other[1, 1]) && this[1, 2].Equals(other[1, 2]) &&
                   this[2, 0].Equals(other[2, 0]) && this[2, 1].Equals(other[2, 1]) && this[2, 2].Equals(other[2, 2]);
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
                var hashCode = this[0, 0].GetHashCode();
                hashCode = (hashCode*397) ^ this[0, 1].GetHashCode();
                hashCode = (hashCode*397) ^ this[0, 2].GetHashCode();
                hashCode = (hashCode*397) ^ this[1, 0].GetHashCode();
                hashCode = (hashCode*397) ^ this[1, 1].GetHashCode();
                hashCode = (hashCode*397) ^ this[1, 2].GetHashCode();
                hashCode = (hashCode*397) ^ this[2, 0].GetHashCode();
                hashCode = (hashCode*397) ^ this[2, 1].GetHashCode();
                hashCode = (hashCode*397) ^ this[2, 2].GetHashCode();
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