using BenchmarkDotNet.Attributes;
using Geisha.Common.Math;

namespace MicroBenchmark
{
    [MemoryDiagnoser]
    public class CircleInstantiationBenchmark
    {
        private const int Instances = 1000;
        private Circle[] _circleInstances;
        private CircleBaseline[] _circleBaselineInstances;

        [GlobalSetup]
        public void Setup()
        {
            _circleInstances = new Circle[Instances];
            _circleBaselineInstances = new CircleBaseline[Instances];
        }

        [Benchmark]
        public void FillArrayWithInstances_Circle()
        {
            for (var i = 0; i < Instances; i++)
            {
                _circleInstances[i] = new Circle(10);
            }
        }

        [Benchmark(Baseline = true)]
        public void FillArrayWithInstances_CircleBaseline()
        {
            for (var i = 0; i < Instances; i++)
            {
                _circleBaselineInstances[i] = new CircleBaseline(10);
            }
        }
    }

    [MemoryDiagnoser]
    public class CircleTransformBenchmark
    {
        private readonly Circle _circle = new Circle(10);
        private readonly CircleBaseline _circleBaseline = new CircleBaseline(10);
        private readonly Matrix3x3 _transform = Matrix3x3.CreateTranslation(new Vector2(20, 10));

        [Benchmark]
        public Circle Transform_Circle()
        {
            return _circle.Transform(_transform);
        }

        [Benchmark(Baseline = true)]
        public CircleBaseline Transform_CircleBaseline()
        {
            return _circleBaseline.Transform(_transform);
        }
    }

    [MemoryDiagnoser]
    public class CircleOverlapsBenchmark
    {
        private readonly Circle _circle1 = new Circle(new Vector2(5, 0), 10);
        private readonly Circle _circle2 = new Circle(new Vector2(10, 0), 10);
        private readonly CircleBaseline _circleBaseline1 = new CircleBaseline(new Vector2(5, 0), 10);
        private readonly CircleBaseline _circleBaseline2 = new CircleBaseline(new Vector2(10, 0), 10);

        [Benchmark]
        public bool Overlaps_Circle()
        {
            return _circle1.Overlaps(_circle2);
        }

        [Benchmark(Baseline = true)]
        public bool Overlaps_CircleBaseline()
        {
            return _circleBaseline1.Overlaps(_circleBaseline2);
        }
    }
}