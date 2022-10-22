using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Math.SAT;

namespace MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class CollisionBenchmarks
    {
        #region Overlaps_Circle

        private readonly Circle _c1 = new(new Vector2(0, 0), 1);
        private readonly Circle _c2 = new(new Vector2(0, 2.1), 1);

        private readonly Circle _c3 = new(new Vector2(0, 0), 1);
        private readonly Circle _c4 = new(new Vector2(2, 0), 1);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Overlaps_Circle")]
        public bool Overlaps_Circle()
        {
            var result1 = _c1.Overlaps(_c2);
            var result2 = _c3.Overlaps(_c4);

            return result1 && result2;
        }

        [Benchmark]
        [BenchmarkCategory("Overlaps_Circle")]
        public bool FastOverlaps_Circle()
        {
            var result1 = _c1.FastOverlaps(_c2);
            var result2 = _c3.FastOverlaps(_c4);

            return result1 && result2;
        }

        #endregion

        #region Contains_Circle

        private readonly Circle _c5 = new(new Vector2(0, 0), 10);
        private readonly Vector2 _p1 = new(15, 0);
        private readonly Vector2 _p2 = new(5, 0);


        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Contains_Circle")]
        public bool Contains_Circle()
        {
            var result1 = _c5.AsShape().Contains(_p1);
            var result2 = _c5.AsShape().Contains(_p2);

            return result1 && result2;
        }

        [Benchmark]
        [BenchmarkCategory("Contains_Circle")]
        public bool FastContains_Circle()
        {
            var result1 = _c5.Contains(_p1);
            var result2 = _c5.Contains(_p2);

            return result1 && result2;
        }

        #endregion
    }
}