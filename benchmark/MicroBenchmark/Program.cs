using BenchmarkDotNet.Running;

namespace MicroBenchmark
{
    internal static class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}