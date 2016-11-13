using System;

namespace Geisha.ManualPerformanceTests
{
    internal struct FixedMatrix3
    {
        private readonly double _m11;
        private readonly double _m12;
        private readonly double _m13;
        private readonly double _m21;
        private readonly double _m22;
        private readonly double _m23;
        private readonly double _m31;
        private readonly double _m32;
        private readonly double _m33;

        public double this[int row, int column]
        {
            get
            {
                var index = row*3 + column;

                switch (index)
                {
                    case 0:
                        return _m11;
                    case 1:
                        return _m12;
                    case 2:
                        return _m13;
                    case 3:
                        return _m21;
                    case 4:
                        return _m22;
                    case 5:
                        return _m23;
                    case 6:
                        return _m31;
                    case 7:
                        return _m32;
                    case 8:
                        return _m33;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public FixedMatrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31,
            double m32,
            double m33)
        {
            _m11 = m11;
            _m12 = m12;
            _m13 = m13;
            _m21 = m21;
            _m22 = m22;
            _m23 = m23;
            _m31 = m31;
            _m32 = m32;
            _m33 = m33;
        }

        public FixedMatrix3(double[] matrix)
        {
            if (matrix.Length != 9)
            {
                throw new ArgumentException("Array must be the length of 9 elements.");
            }

            _m11 = matrix[0];
            _m12 = matrix[1];
            _m13 = matrix[2];
            _m21 = matrix[3];
            _m22 = matrix[4];
            _m23 = matrix[5];
            _m31 = matrix[6];
            _m32 = matrix[7];
            _m33 = matrix[8];
        }

        public FixedMatrix3 MultiplyWithIndexer(FixedMatrix3 other)
        {
            return new FixedMatrix3(
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

        public FixedMatrix3 Multiply(FixedMatrix3 other)
        {
            return new FixedMatrix3(
                _m11*other._m11 + _m12*other._m21 + _m13*other._m31,
                _m11*other._m12 + _m12*other._m22 + _m13*other._m32,
                _m11*other._m13 + _m12*other._m23 + _m13*other._m33,
                _m21*other._m11 + _m22*other._m21 + _m23*other._m31,
                _m21*other._m12 + _m22*other._m22 + _m23*other._m32,
                _m21*other._m13 + _m22*other._m23 + _m23*other._m33,
                _m31*other._m11 + _m32*other._m21 + _m33*other._m31,
                _m31*other._m12 + _m32*other._m22 + _m33*other._m32,
                _m31*other._m13 + _m32*other._m23 + _m33*other._m33
            );
        }
    }
}