using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Engine.Core.Math;

namespace MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class RectangleBenchmarks
    {
        [GlobalSetup]
        public void Setup()
        {
            _rectangleInstances = new Rectangle[Instances];
            _rectangleBaselineInstances = new RectangleBaseline[Instances];
        }

        #region FillArrayInstances

        private const int Instances = 1000;
        private Rectangle[] _rectangleInstances;
        private RectangleBaseline[] _rectangleBaselineInstances;

        [Benchmark]
        [BenchmarkCategory("FillArrayInstances")]
        public void FillArrayWithInstances_Rectangle()
        {
            for (var i = 0; i < Instances; i++)
            {
                _rectangleInstances[i] = new Rectangle(new Vector2(40, 20));
            }
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("FillArrayInstances")]
        public void FillArrayWithInstances_RectangleBaseline()
        {
            for (var i = 0; i < Instances; i++)
            {
                _rectangleBaselineInstances[i] = new RectangleBaseline(new Vector2(40, 20));
            }
        }

        #endregion

        #region Transform

        private readonly Rectangle _rectangle = new Rectangle(new Vector2(40, 20));
        private readonly RectangleBaseline _rectangleBaseline = new RectangleBaseline(new Vector2(40, 20));
        private readonly Matrix3x3 _transform = Matrix3x3.CreateTranslation(new Vector2(20, 10));

        [Benchmark]
        [BenchmarkCategory("Transform")]
        public Rectangle Transform_Rectangle()
        {
            return _rectangle.Transform(_transform);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Transform")]
        public RectangleBaseline Transform_RectangleBaseline()
        {
            return _rectangleBaseline.Transform(_transform);
        }

        #endregion

        #region Overlaps

        private readonly Rectangle _rectangle1 = new Rectangle(new Vector2(5, 0), new Vector2(40, 20));
        private readonly Rectangle _rectangle2 = new Rectangle(new Vector2(10, 0), new Vector2(20, 40));
        private readonly RectangleBaseline _rectangleBaseline1 = new RectangleBaseline(new Vector2(5, 0), new Vector2(40, 20));
        private readonly RectangleBaseline _rectangleBaseline2 = new RectangleBaseline(new Vector2(10, 0), new Vector2(20, 40));

        [Benchmark]
        [BenchmarkCategory("Overlaps")]
        public bool Overlaps_Rectangle()
        {
            return _rectangle1.Overlaps(_rectangle2);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Overlaps")]
        public bool Overlaps_RectangleBaseline()
        {
            return _rectangleBaseline1.Overlaps(_rectangleBaseline2);
        }

        #endregion
    }
}