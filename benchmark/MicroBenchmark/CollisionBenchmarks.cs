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

        #region Contains_Rectangle

        private readonly Rectangle _r5 = new(new Vector2(0, 0), new Vector2(10, 5));
        private readonly Vector2 _p3 = new(0, 5);
        private readonly Vector2 _p4 = new(5, 0);


        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Contains_Rectangle")]
        public bool Contains_Rectangle()
        {
            var result1 = _r5.AsShape().Contains(_p3);
            var result2 = _r5.AsShape().Contains(_p4);

            return result1 && result2;
        }

        [Benchmark]
        [BenchmarkCategory("Contains_Rectangle")]
        public bool FastContains_Rectangle()
        {
            var result1 = _r5.Contains(_p3);
            var result2 = _r5.Contains(_p4);

            return result1 && result2;
        }

        #endregion

        #region Overlaps_Rectangle

        private readonly Rectangle _r1 = new(new Vector2(0, 0), new Vector2(2, 1));
        private readonly Rectangle _r2 = new(new Vector2(1.6, 1.6), new Vector2(1, 2));

        private readonly Rectangle _r3 = new(new Vector2(0, 0), new Vector2(2, 1));
        private readonly Rectangle _r4 = new(new Vector2(1.5, 0), new Vector2(1, 2));

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Overlaps_Rectangle")]
        public bool Overlaps_Rectangle()
        {
            var result1 = _r1.Overlaps(_r2);
            var result2 = _r3.Overlaps(_r4);

            return result1 && result2;
        }

        [Benchmark]
        [BenchmarkCategory("Overlaps_Rectangle")]
        public bool FastOverlaps_Rectangle()
        {
            var result1 = _r1.FastOverlaps(_r2);
            var result2 = _r3.FastOverlaps(_r4);

            return result1 && result2;
        }

        #endregion

        #region Overlaps_Rectangle_And_Circle

        private readonly Rectangle _r6 = new(new Vector2(0, 0), new Vector2(2, 1));
        private readonly Circle _c6 = new(new Vector2(2, 2), 1);

        private readonly Rectangle _r7 = new(new Vector2(0, 0), new Vector2(2, 1));
        private readonly Circle _c7 = new(new Vector2(1, 1), 1);

        private readonly Rectangle _r8 = new(new Vector2(0, 0), new Vector2(2, 1));
        private readonly Circle _c8 = new(new Vector2(0, 0), 1.3);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Overlaps_Rectangle_And_Circle")]
        public bool Overlaps_Rectangle_And_Circle()
        {
            var result1 = _r6.AsShape().Overlaps(_c6.AsShape());
            var result2 = _r7.AsShape().Overlaps(_c7.AsShape());
            var result3 = _r8.AsShape().Overlaps(_c8.AsShape());

            return result1 && result2 && result3;
        }

        [Benchmark]
        [BenchmarkCategory("Overlaps_Rectangle_And_Circle")]
        public bool FastOverlaps_Rectangle_And_Circle()
        {
            var result1 = _r6.FastOverlaps(_c6);
            var result2 = _r7.FastOverlaps(_c7);
            var result3 = _r8.FastOverlaps(_c8);

            return result1 && result2 && result3;
        }

        [Benchmark]
        [BenchmarkCategory("Overlaps_Rectangle_And_Circle")]
        public bool Overlaps_Rectangle_And_Circle_Inverted()
        {
            var result1 = _c6.AsShape().Overlaps(_r6.AsShape());
            var result2 = _c7.AsShape().Overlaps(_r7.AsShape());
            var result3 = _c8.AsShape().Overlaps(_r8.AsShape());

            return result1 && result2 && result3;
        }

        [Benchmark]
        [BenchmarkCategory("Overlaps_Rectangle_And_Circle")]
        public bool FastOverlaps_Rectangle_And_Circle_Inverted()
        {
            var result1 = _c6.FastOverlaps(_r6);
            var result2 = _c7.FastOverlaps(_r7);
            var result3 = _c8.FastOverlaps(_r8);

            return result1 && result2 && result3;
        }

        #endregion
    }
}