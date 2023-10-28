﻿using System;
using BenchmarkDotNet.Attributes;
using Geisha.Engine.Core;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class IdComparisonBenchmarks
{
    private const int Iterations = 1_000_000;
    private readonly Guid _guid1 = Guid.Parse("DBC2342A-AACD-463B-8559-E3DC57F8B6B9");
    private readonly Guid _guid2 = Guid.Parse("DBC2342A-AACD-463B-8559-E3DC57F8B6B9");
    private readonly Guid _guid3 = Guid.Parse("19F6EE94-0091-48A9-AA5F-86E822004756");
    private readonly long _long1 = long.MaxValue;
    private readonly long _long2 = long.MaxValue;
    private readonly long _long3 = 123;
    private readonly ulong _ulong1 = ulong.MaxValue;
    private readonly ulong _ulong2 = ulong.MaxValue;
    private readonly ulong _ulong3 = 123;
    private readonly RuntimeId _runtimeId1 = new(ulong.MaxValue);
    private readonly RuntimeId _runtimeId2 = new(ulong.MaxValue);
    private readonly RuntimeId _runtimeId3 = new(123);

    [Benchmark]
    public int GuidCompare_Pessimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _guid1.CompareTo(_guid2);
        }

        return r;
    }

    [Benchmark]
    public int GuidCompare_Optimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _guid1.CompareTo(_guid3);
        }

        return r;
    }

    [Benchmark]
    public int LongCompare_Pessimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _long1.CompareTo(_long2);
        }

        return r;
    }

    [Benchmark]
    public int LongCompare_Optimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _long1.CompareTo(_long3);
        }

        return r;
    }

    [Benchmark]
    public int UlongCompare_Pessimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _ulong1.CompareTo(_ulong2);
        }

        return r;
    }

    [Benchmark]
    public int UlongCompare_Optimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _ulong1.CompareTo(_ulong3);
        }

        return r;
    }

    [Benchmark]
    public int RuntimeIdCompare_Pessimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _runtimeId1.CompareTo(_runtimeId2);
        }

        return r;
    }

    [Benchmark]
    public int RuntimeIdCompare_Optimistic()
    {
        var r = 0;

        for (var i = 0; i < Iterations; i++)
        {
            r = _runtimeId1.CompareTo(_runtimeId3);
        }

        return r;
    }
}