using BenchmarkDotNet.Running;

namespace Geisha.MicroBenchmark
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<SortingBenchmarks>();
        }
    }
}