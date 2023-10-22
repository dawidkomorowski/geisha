using BenchmarkDotNet.Attributes;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class Int64HashCodeBenchmarks
{
    private const int Iterations = 1_000_000;
    private readonly ulong _ulong = ulong.MaxValue;

    [Benchmark]
    public int Ulong_GetHashCode()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _ulong.GetHashCode();
        }

        return r;
    }

    [Benchmark]
    public int ULong_Cast()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = (int)_ulong;
        }

        return r;
    }
}