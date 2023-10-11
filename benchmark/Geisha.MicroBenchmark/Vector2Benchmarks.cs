using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Engine.Core.Math;

namespace Geisha.MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Vector2Benchmarks
    {
        private readonly Vector2 _v1 = new(1, 2);
        private readonly Vector2 _v2 = new(3, 4);
        private readonly Vector2Baseline _v1b = new(1, 2);
        private readonly Vector2Baseline _v2b = new(3, 4);

        #region Add

        [Benchmark]
        [BenchmarkCategory("Add")]
        public Vector2 Add_Vector2()
        {
            Vector2 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Add(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Add")]
        public Vector2Baseline Add_Vector2Baseline()
        {
            Vector2Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b.Add(_v2b);
            }

            return r;
        }

        #endregion

        #region Subtract

        [Benchmark]
        [BenchmarkCategory("Subtract")]
        public Vector2 Subtract_Vector2()
        {
            Vector2 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Subtract(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Subtract")]
        public Vector2Baseline Subtract_Vector2Baseline()
        {
            Vector2Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b.Subtract(_v2b);
            }

            return r;
        }

        #endregion

        #region Dot

        [Benchmark]
        [BenchmarkCategory("Dot")]
        public double Dot_Vector2()
        {
            double r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Dot(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Dot")]
        public double Dot_Vector2Baseline()
        {
            double r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b.Dot(_v2b);
            }

            return r;
        }

        #endregion

        #region Distance

        [Benchmark]
        [BenchmarkCategory("Distance")]
        public double Distance_Vector2()
        {
            double r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Distance(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Distance")]
        public double Distance_Vector2Baseline()
        {
            double r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b.Distance(_v2b);
            }

            return r;
        }

        #endregion

        #region OperatorPlus

        [Benchmark]
        [BenchmarkCategory("OperatorPlus")]
        public Vector2 OperatorPlus_Vector2()
        {
            Vector2 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 + _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorPlus")]
        public Vector2Baseline OperatorPlus_Vector2Baseline()
        {
            Vector2Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b + _v2b;
            }

            return r;
        }

        #endregion

        #region OperatorMinus

        [Benchmark]
        [BenchmarkCategory("OperatorMinus")]
        public Vector2 OperatorMinus_Vector2()
        {
            Vector2 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 - _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMinus")]
        public Vector2Baseline OperatorMinus_Vector2Baseline()
        {
            Vector2Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b - _v2b;
            }

            return r;
        }

        #endregion

        #region OperatorMul

        [Benchmark]
        [BenchmarkCategory("OperatorMul")]
        public Vector2 OperatorMul_Vector2()
        {
            Vector2 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 * 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMul")]
        public Vector2Baseline OperatorMul_Vector2Baseline()
        {
            Vector2Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b * 10d;
            }

            return r;
        }

        #endregion

        #region OperatorDiv

        [Benchmark]
        [BenchmarkCategory("OperatorDiv")]
        public Vector2 OperatorDiv_Vector2()
        {
            Vector2 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 / 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorDiv")]
        public Vector2Baseline OperatorDiv_Vector2Baseline()
        {
            Vector2Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b / 10d;
            }

            return r;
        }

        #endregion

        #region OperatorUnaryMinus

        [Benchmark]
        [BenchmarkCategory("OperatorUnaryMinus")]
        public Vector2 OperatorUnaryMinus_Vector2()
        {
            Vector2 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = -_v1;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorUnaryMinus")]
        public Vector2Baseline OperatorUnaryMinus_Vector2Baseline()
        {
            Vector2Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = -_v1b;
            }

            return r;
        }

        #endregion

        #region OperatorEq

        [Benchmark]
        [BenchmarkCategory("OperatorEq")]
        public bool OperatorEq_Vector2()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 == _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorEq")]
        public bool OperatorEq_Vector2Baseline()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b == _v2b;
            }

            return r;
        }

        #endregion

        #region OperatorNotEq

        [Benchmark]
        [BenchmarkCategory("OperatorNotEq")]
        public bool OperatorNotEq_Vector2()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 != _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorNotEq")]
        public bool OperatorNotEq_Vector2Baseline()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1b != _v2b;
            }

            return r;
        }

        #endregion
    }
}