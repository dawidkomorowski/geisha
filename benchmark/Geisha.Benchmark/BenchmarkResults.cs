using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Benchmark
{
    internal sealed class BenchmarkResult
    {
        public string BenchmarkName { get; init; }
        public int Frames { get; init; }
        public int FixedFrames { get; init; }
        public int DrawCalls { get; init; }
        public int AvgDrawCallsPerFrame { get; init; }
    }

    internal sealed class BenchmarkResults
    {
        private readonly List<BenchmarkResult> _results = new();
        private readonly string _resultsFilePath;

        public BenchmarkResults()
        {
            _resultsFilePath = $"BenchmarkResults--{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.json";

            SaveResults();
        }

        public void AddResult(BenchmarkResult result)
        {
            _results.Add(result);

            SaveResults();
        }

        private void SaveResults()
        {
            var json = JsonSerializer.Serialize(_results, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_resultsFilePath, json);
        }
    }
}