using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace MicroBenchmark
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<SampleBenchmark>();
        }
    }

    public class SampleBenchmark
    {
        [Benchmark]
        public int Add() => 1 + 2;

        [Benchmark]
        public int Add2() => 2 + 3;
    }
}