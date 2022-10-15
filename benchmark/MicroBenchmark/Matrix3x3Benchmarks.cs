using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Engine.Core.Math;

namespace MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Matrix3x3Benchmarks
    {
        private readonly Vector2 _v2 = new(1, 2);
        private readonly Vector3 _v3 = new(1, 2, 3);
        private readonly Matrix3x3 _m1 = new(1, 2, 3, 4, 5, 6, 7, 8, 9);
        private readonly Matrix3x3 _m2 = new(10, 11, 12, 13, 14, 15, 16, 17, 18);

        private readonly Vector2Baseline _v2b = new(1, 2);
        private readonly Vector3Baseline _v3b = new(1, 2, 3);
        private readonly Matrix3x3Baseline _m1b = new(1, 2, 3, 4, 5, 6, 7, 8, 9);
        private readonly Matrix3x3Baseline _m2b = new(10, 11, 12, 13, 14, 15, 16, 17, 18);

        #region Add

        [Benchmark]
        [BenchmarkCategory("Add")]
        public Matrix3x3 Add_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Add(_m2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Add")]
        public Matrix3x3Baseline Add_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b.Add(_m2b);
            }

            return r;
        }

        #endregion

        #region Subtract

        [Benchmark]
        [BenchmarkCategory("Subtract")]
        public Matrix3x3 Subtract_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Subtract(_m2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Subtract")]
        public Matrix3x3Baseline Subtract_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b.Subtract(_m2b);
            }

            return r;
        }

        #endregion

        #region Multiply

        [Benchmark]
        [BenchmarkCategory("Multiply")]
        public Matrix3x3 Multiply_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Multiply(_m2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Multiply")]
        public Matrix3x3Baseline Multiply_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b.Multiply(_m2b);
            }

            return r;
        }

        #endregion

        #region MultiplyVec

        [Benchmark]
        [BenchmarkCategory("MultiplyVec")]
        public Vector3 MultiplyVec_Matrix3x3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Multiply(_v3);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("MultiplyVec")]
        public Vector3Baseline MultiplyVec_Matrix3x3Baseline()
        {
            Vector3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b.Multiply(_v3b);
            }

            return r;
        }

        #endregion

        #region CreateTranslation

        [Benchmark]
        [BenchmarkCategory("CreateTranslation")]
        public Matrix3x3 CreateTranslation_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix3x3.CreateTranslation(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("CreateTranslation")]
        public Matrix3x3Baseline CreateTranslation_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix3x3Baseline.CreateTranslation(_v2b);
            }

            return r;
        }

        #endregion

        #region CreateScale

        [Benchmark]
        [BenchmarkCategory("CreateScale")]
        public Matrix3x3 CreateScale_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix3x3.CreateScale(_v2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("CreateScale")]
        public Matrix3x3Baseline CreateScale_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix3x3Baseline.CreateScale(_v2b);
            }

            return r;
        }

        #endregion

        #region OperatorPlus

        [Benchmark]
        [BenchmarkCategory("OperatorPlus")]
        public Matrix3x3 OperatorPlus_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 + _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorPlus")]
        public Matrix3x3Baseline OperatorPlus_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b + _m2b;
            }

            return r;
        }

        #endregion

        #region OperatorMinus

        [Benchmark]
        [BenchmarkCategory("OperatorMinus")]
        public Matrix3x3 OperatorMinus_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 - _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMinus")]
        public Matrix3x3Baseline OperatorMinus_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b - _m2b;
            }

            return r;
        }

        #endregion

        #region OperatorMul

        [Benchmark]
        [BenchmarkCategory("OperatorMul")]
        public Matrix3x3 OperatorMul_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 * _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMul")]
        public Matrix3x3Baseline OperatorMul_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b * _m2b;
            }

            return r;
        }

        #endregion

        #region OperatorMulScal

        [Benchmark]
        [BenchmarkCategory("OperatorMulScal")]
        public Matrix3x3 OperatorMulScal_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 * 10;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMulScal")]
        public Matrix3x3Baseline OperatorMulScal_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b * 10;
            }

            return r;
        }

        #endregion

        #region OperatorMulVec

        [Benchmark]
        [BenchmarkCategory("OperatorMulVec")]
        public Vector3 OperatorMulVec_Matrix3x3()
        {
            Vector3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 * _v3;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMulVec")]
        public Vector3Baseline OperatorMulVec_Matrix3x3Baseline()
        {
            Vector3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b * _v3b;
            }

            return r;
        }

        #endregion

        #region OperatorDiv

        [Benchmark]
        [BenchmarkCategory("OperatorDiv")]
        public Matrix3x3 OperatorDiv_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 / 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorDiv")]
        public Matrix3x3Baseline OperatorDiv_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b / 10d;
            }

            return r;
        }

        #endregion

        #region OperatorUnaryMinus

        [Benchmark]
        [BenchmarkCategory("OperatorUnaryMinus")]
        public Matrix3x3 OperatorUnaryMinus_Matrix3x3()
        {
            Matrix3x3 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = -_m1;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorUnaryMinus")]
        public Matrix3x3Baseline OperatorUnaryMinus_Matrix3x3Baseline()
        {
            Matrix3x3Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = -_m1b;
            }

            return r;
        }

        #endregion

        #region OperatorEq

        [Benchmark]
        [BenchmarkCategory("OperatorEq")]
        public bool OperatorEq_Matrix3x3()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 == _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorEq")]
        public bool OperatorEq_Matrix3x3Baseline()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b == _m2b;
            }

            return r;
        }

        #endregion

        #region OperatorNotEq

        [Benchmark]
        [BenchmarkCategory("OperatorNotEq")]
        public bool OperatorNotEq_Matrix3x3()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 != _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorNotEq")]
        public bool OperatorNotEq_Matrix3x3Baseline()
        {
            bool r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b != _m2b;
            }

            return r;
        }

        #endregion
    }
}