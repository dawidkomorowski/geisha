using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Geisha.MicroBenchmark;

[MemoryDiagnoser]
public class SortingBenchmarks
{
    private Data[] _sourceData = null!;
    private readonly List<Data> _flat = new();
    private readonly List<List<Data>> _layered = new();

    [Params(10_000, 100_000, 1_000_000)]
    public int Elements { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _sourceData = new Data[Elements];
        var random = new Random(0);

        if (_sourceData is null) throw new InvalidOperationException("Bugged");

        for (var i = 0; i < _sourceData.Length; i++)
        {
            var data = new Data
            {
                Layer = random.Next(0, 2),
                OrderInLayer = random.Next(0, 20)
            };
            _sourceData[i] = data;
        }
    }

    [IterationSetup(Target = nameof(Sorting_Flat))]
    public void IterationSetup_Flat()
    {
        _flat.Clear();
        _flat.AddRange(_sourceData);
    }

    [IterationSetup(Target = nameof(Sorting_Layered))]
    public void IterationSetup_Layered()
    {
        _layered.Clear();
        foreach (var data in _sourceData)
        {
            var requiredCount = data.Layer + 1;
            while (_layered.Count < requiredCount)
            {
                _layered.Add(new List<Data>());
            }

            _layered[data.Layer].Add(data);
        }
    }

    [Benchmark(Baseline = true)]
    public void Sorting_Flat()
    {
        for (int i = 0; i < 10; i++)
        {
            _flat.Sort((d1, d2) =>
            {
                var layersComparison = d1.Layer.CompareTo(d2.Layer);

                if (layersComparison != 0) return layersComparison;

                return d1.OrderInLayer.CompareTo(d2.OrderInLayer);
            });
        }
    }

    [Benchmark]
    public void Sorting_Layered()
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (var layer in _layered)
            {
                layer.Sort((d1, d2) => d1.OrderInLayer.CompareTo(d2.OrderInLayer));
            }
        }
    }
}

public class Data
{
    public int Layer { get; set; }
    public int OrderInLayer { get; set; }
}