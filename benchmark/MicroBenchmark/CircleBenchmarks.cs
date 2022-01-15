using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Common.Math;

namespace MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class CircleBenchmarks
    {
        [GlobalSetup]
        public void Setup()
        {
            _circleInstances = new Circle[Instances];
            _circleBaselineInstances = new CircleBaseline[Instances];
        }

        #region FillArrayInstances

        private const int Instances = 1000;
        private Circle[] _circleInstances;
        private CircleBaseline[] _circleBaselineInstances;

        [Benchmark]
        [BenchmarkCategory("FillArrayInstances")]
        public void FillArrayWithInstances_Circle()
        {
            for (var i = 0; i < Instances; i++)
            {
                _circleInstances[i] = new Circle(10);
            }
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("FillArrayInstances")]
        public void FillArrayWithInstances_CircleBaseline()
        {
            for (var i = 0; i < Instances; i++)
            {
                _circleBaselineInstances[i] = new CircleBaseline(10);
            }
        }

        #endregion

        #region Transform

        private readonly Circle _circle = new Circle(10);
        private readonly CircleBaseline _circleBaseline = new CircleBaseline(10);
        private readonly Matrix3x3 _transform = Matrix3x3.CreateTranslation(new Vector2(20, 10));

        [Benchmark]
        [BenchmarkCategory("Transform")]
        public Circle Transform_Circle()
        {
            return _circle.Transform(_transform);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Transform")]
        public CircleBaseline Transform_CircleBaseline()
        {
            return _circleBaseline.Transform(_transform);
        }

        #endregion

        #region Overlaps

        private readonly Circle _circle1 = new Circle(new Vector2(5, 0), 10);
        private readonly Circle _circle2 = new Circle(new Vector2(10, 0), 10);
        private readonly CircleBaseline _circleBaseline1 = new CircleBaseline(new Vector2(5, 0), 10);
        private readonly CircleBaseline _circleBaseline2 = new CircleBaseline(new Vector2(10, 0), 10);

        [Benchmark]
        [BenchmarkCategory("Overlaps")]
        public bool Overlaps_Circle()
        {
            return _circle1.Overlaps(_circle2);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Overlaps")]
        public bool Overlaps_CircleBaseline()
        {
            return _circleBaseline1.Overlaps(_circleBaseline2);
        }

        #endregion
    }
}