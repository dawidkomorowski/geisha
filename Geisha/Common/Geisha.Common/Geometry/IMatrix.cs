using System;

namespace Geisha.Common.Geometry
{
    internal interface IMatrix<TMatrix> : IEquatable<TMatrix> where TMatrix : struct
    {
        TMatrix Opposite { get; }
        double[] Array { get; }

        TMatrix Add(TMatrix other);
        TMatrix Subtract(TMatrix other);
        TMatrix Multiply(double scalar);
        TMatrix Multiply(TMatrix other);
        TMatrix Divide(double scalar);
    }
}