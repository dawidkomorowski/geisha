using BenchmarkDotNet.Attributes;
using Geisha.Engine.Core.Math;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class VectorBenchmarks
{
    private readonly Vector2 _v2 = new(1, 2);
    private readonly Vector2 _v2o = new(3, 4);
    private readonly Vector3 _v3 = new(1, 2, 3);
    private readonly Vector3 _v3o = new(4, 5, 6);
    private readonly Vector4 _v4 = new(1, 2, 3, 4);
    private readonly Vector4 _v4o = new(5, 6, 7, 8);

    [Benchmark]
    public double Vector2_Length()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v2.Length;
        }

        return r;
    }

    [Benchmark]
    public double Vector2_LengthSquared()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v2.LengthSquared;
        }

        return r;
    }

    [Benchmark]
    public double Vector2_Distance()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v2.Distance(_v2o);
        }

        return r;
    }

    [Benchmark]
    public double Vector2_DistanceSquared()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v2.DistanceSquared(_v2o);
        }

        return r;
    }

    [Benchmark]
    public double Vector3_Length()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v3.Length;
        }

        return r;
    }

    [Benchmark]
    public double Vector3_LengthSquared()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v3.LengthSquared;
        }

        return r;
    }

    [Benchmark]
    public double Vector3_Distance()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v3.Distance(_v3o);
        }

        return r;
    }

    [Benchmark]
    public double Vector3_DistanceSquared()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v3.DistanceSquared(_v3o);
        }

        return r;
    }

    [Benchmark]
    public double Vector4_Length()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v4.Length;
        }

        return r;
    }

    [Benchmark]
    public double Vector4_LengthSquared()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v4.LengthSquared;
        }

        return r;
    }

    [Benchmark]
    public double Vector4_Distance()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v4.Distance(_v4o);
        }

        return r;
    }

    [Benchmark]
    public double Vector4_DistanceSquared()
    {
        var r = 0d;
        for (int i = 0; i < 1000; i++)
        {
            r = _v4.DistanceSquared(_v4o);
        }

        return r;
    }
}