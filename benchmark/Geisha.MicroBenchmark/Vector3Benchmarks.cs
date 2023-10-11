using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Engine.Core.Math;

namespace Geisha.MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Vector3Benchmarks
    {
        private readonly Vector3 _v1 = new(1, 2, 3);
        private readonly Vector3 _v2 = new(4, 5, 6);
        private readonly Vector3Baseline _v1b = new(1, 2, 3);
        private readonly Vector3Baseline _v2b = new(4, 5, 6);

        #region Add

        [Benchmark]
        [BenchmarkCategory("Add")]
        public Vector3 Add_Vector3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Add(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Add")]
        public Vector3Baseline Add_Vector3Baseline()
        {
            Vector3Baseline r = default;
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
        public Vector3 Subtract_Vector3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1.Subtract(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Subtract")]
        public Vector3Baseline Subtract_Vector3Baseline()
        {
            Vector3Baseline r = default;
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
        public double Dot_Vector3()
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
        public double Dot_Vector3Baseline()
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
        public double Distance_Vector3()
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
        public double Distance_Vector3Baseline()
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
        public Vector3 OperatorPlus_Vector3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 + _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorPlus")]
        public Vector3Baseline OperatorPlus_Vector3Baseline()
        {
            Vector3Baseline r = default;
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
        public Vector3 OperatorMinus_Vector3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 - _v2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMinus")]
        public Vector3Baseline OperatorMinus_Vector3Baseline()
        {
            Vector3Baseline r = default;
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
        public Vector3 OperatorMul_Vector3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 * 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMul")]
        public Vector3Baseline OperatorMul_Vector3Baseline()
        {
            Vector3Baseline r = default;
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
        public Vector3 OperatorDiv_Vector3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _v1 / 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorDiv")]
        public Vector3Baseline OperatorDiv_Vector3Baseline()
        {
            Vector3Baseline r = default;
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
        public Vector3 OperatorUnaryMinus_Vector3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = -_v1;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorUnaryMinus")]
        public Vector3Baseline OperatorUnaryMinus_Vector3Baseline()
        {
            Vector3Baseline r = default;
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
        public bool OperatorEq_Vector3()
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
        public bool OperatorEq_Vector3Baseline()
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
        public bool OperatorNotEq_Vector3()
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
        public bool OperatorNotEq_Vector3Baseline()
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