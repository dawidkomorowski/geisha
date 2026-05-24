using BenchmarkDotNet.Attributes;
using Geisha.Engine.Core.Math;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class MatrixBenchmarks
{
    private readonly double[] _angles = new double[10000];

    public MatrixBenchmarks()
    {
        for (int i = 0; i < _angles.Length; i++)
        {
            _angles[i] = i * 0.001d;
        }
    }

    // To use this baseline, introduce a copy of the benchmarked method as Matrix3x3.CreateRotation_Legacy for side-by-side comparison.
    //[Benchmark(Baseline = true)]
    //public Matrix3x3 Matrix3x3_CreateRotation_Legacy()
    //{
    //    var r = Matrix3x3.Identity;
    //    for (int i = 0; i < _angles.Length; i++)
    //    {
    //        r = Matrix3x3.CreateRotation_Legacy(_angles[i]);
    //    }

    //    return r;
    //}

    [Benchmark]
    public Matrix3x3 Matrix3x3_CreateRotation()
    {
        var r = Matrix3x3.Identity;
        for (int i = 0; i < _angles.Length; i++)
        {
            r = Matrix3x3.CreateRotation(_angles[i]);
        }

        return r;
    }
}
