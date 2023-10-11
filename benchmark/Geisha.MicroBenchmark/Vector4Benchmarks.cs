using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Engine.Core.Math;

namespace Geisha.MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Vector4Benchmarks
    {
        private readonly Vector4 _v1 = new(1, 2, 3, 4);
        private readonly Vector4 _v2 = new(5, 6, 7, 8);
        private readonly Vector4Baseline _v1b = new(1, 2, 3, 4);
        private readonly Vector4Baseline _v2b = new(5, 6, 7, 8);

        #region Add

        [Benchmark]
        [BenchmarkCategory("Add")]
        public Vector4 Add_Vector4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Add(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Add")]
        public Vector4Baseline Add_Vector4Baseline()
        {
            Vector4Baseline r = default;
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
        public Vector4 Subtract_Vector4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Subtract(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Subtract")]
        public Vector4Baseline Subtract_Vector4Baseline()
        {
            Vector4Baseline r = default;
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
        public double Dot_Vector4()
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
        public double Dot_Vector4Baseline()
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
        public double Distance_Vector4()
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
        public double Distance_Vector4Baseline()
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
        public Vector4 OperatorPlus_Vector4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 + _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorPlus")]
        public Vector4Baseline OperatorPlus_Vector4Baseline()
        {
            Vector4Baseline r = default;
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
        public Vector4 OperatorMinus_Vector4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 - _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMinus")]
        public Vector4Baseline OperatorMinus_Vector4Baseline()
        {
            Vector4Baseline r = default;
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
        public Vector4 OperatorMul_Vector4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 * 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMul")]
        public Vector4Baseline OperatorMul_Vector4Baseline()
        {
            Vector4Baseline r = default;
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
        public Vector4 OperatorDiv_Vector4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 / 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorDiv")]
        public Vector4Baseline OperatorDiv_Vector4Baseline()
        {
            Vector4Baseline r = default;
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
        public Vector4 OperatorUnaryMinus_Vector4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = -_v1;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorUnaryMinus")]
        public Vector4Baseline OperatorUnaryMinus_Vector4Baseline()
        {
            Vector4Baseline r = default;
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
        public bool OperatorEq_Vector4()
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
        public bool OperatorEq_Vector4Baseline()
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
        public bool OperatorNotEq_Vector4()
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
        public bool OperatorNotEq_Vector4Baseline()
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