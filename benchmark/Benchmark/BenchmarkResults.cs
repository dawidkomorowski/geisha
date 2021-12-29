using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Benchmark
{
    internal sealed class BenchmarkResult
    {
        public string BenchmarkName { get; set; }
        public int Frames { get; set; }
        public int FixedFrames { get; set; }
    }

    internal sealed class BenchmarkResults
    {
        private readonly List<BenchmarkResult> _results = new List<BenchmarkResult>();
        private readonly string _resultsFilePath;

        public BenchmarkResults()
        {
            _resultsFilePath = $"BenchmarkResults--{DateTime.Now:yyyy-MM-dd--HH-mm-ss}";

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