using System;

namespace Geisha.Common.Geometry
{
    internal interface IVector<TVector> : IEquatable<TVector> where TVector : struct
    {
        double Length { get; }
        TVector Unit { get; }
        TVector Opposite { get; }
        double[] Array { get; }

        TVector Add(TVector other);
        TVector Subtract(TVector other);
        TVector Multiply(double scalar);
        TVector Divide(double scalar);
        double Dot(TVector other);
        double Distance(TVector other);
    }
}