using BenchmarkDotNet.Running;

namespace Geisha.MicroBenchmark
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Matrix4x4Benchmarks>();
        }
    }
}