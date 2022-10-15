using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Geisha.Engine.Core.Math;

namespace MicroBenchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Matrix4x4Benchmarks
    {
        private readonly Vector3 _v3 = new(1, 2, 3);
        private readonly Vector4 _v4 = new(1, 2, 3, 4);
        private readonly Matrix4x4 _m1 = new(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        private readonly Matrix4x4 _m2 = new(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

        private readonly Vector3Baseline _v3b = new(1, 2, 3);
        private readonly Vector4Baseline _v4b = new(1, 2, 3, 4);
        private readonly Matrix4x4Baseline _m1b = new(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        private readonly Matrix4x4Baseline _m2b = new(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

        #region Add

        [Benchmark]
        [BenchmarkCategory("Add")]
        public Matrix4x4 Add_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Add(_m2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Add")]
        public Matrix4x4Baseline Add_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Matrix4x4 Subtract_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Subtract(_m2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Subtract")]
        public Matrix4x4Baseline Subtract_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Matrix4x4 Multiply_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Multiply(_m2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Multiply")]
        public Matrix4x4Baseline Multiply_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Vector4 MultiplyVec_Matrix4x4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1.Multiply(_v4);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("MultiplyVec")]
        public Vector4Baseline MultiplyVec_Matrix4x4Baseline()
        {
            Vector4Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b.Multiply(_v4b);
            }

            return r;
        }

        #endregion

        #region CreateTranslation

        [Benchmark]
        [BenchmarkCategory("CreateTranslation")]
        public Matrix4x4 CreateTranslation_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix4x4.CreateTranslation(_v3);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("CreateTranslation")]
        public Matrix4x4Baseline CreateTranslation_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix4x4Baseline.CreateTranslation(_v3b);
            }

            return r;
        }

        #endregion

        #region CreateRotationZXY

        [Benchmark]
        [BenchmarkCategory("CreateRotationZXY")]
        public Matrix4x4 CreateRotationZXY_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix4x4.CreateRotationZXY(_v3);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("CreateRotationZXY")]
        public Matrix4x4Baseline CreateRotationZXY_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix4x4Baseline.CreateRotationZXY(_v3b);
            }

            return r;
        }

        #endregion

        #region CreateScale

        [Benchmark]
        [BenchmarkCategory("CreateScale")]
        public Matrix4x4 CreateScale_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix4x4.CreateScale(_v3);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("CreateScale")]
        public Matrix4x4Baseline CreateScale_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = Matrix4x4Baseline.CreateScale(_v3b);
            }

            return r;
        }

        #endregion

        #region OperatorPlus

        [Benchmark]
        [BenchmarkCategory("OperatorPlus")]
        public Matrix4x4 OperatorPlus_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 + _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorPlus")]
        public Matrix4x4Baseline OperatorPlus_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Matrix4x4 OperatorMinus_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 - _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMinus")]
        public Matrix4x4Baseline OperatorMinus_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Matrix4x4 OperatorMul_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 * _m2;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMul")]
        public Matrix4x4Baseline OperatorMul_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Matrix4x4 OperatorMulScal_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 * 10;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMulScal")]
        public Matrix4x4Baseline OperatorMulScal_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Vector4 OperatorMulVec_Matrix4x4()
        {
            Vector4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 * _v4;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorMulVec")]
        public Vector4Baseline OperatorMulVec_Matrix4x4Baseline()
        {
            Vector4Baseline r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1b * _v4b;
            }

            return r;
        }

        #endregion

        #region OperatorDiv

        [Benchmark]
        [BenchmarkCategory("OperatorDiv")]
        public Matrix4x4 OperatorDiv_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = _m1 / 10d;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorDiv")]
        public Matrix4x4Baseline OperatorDiv_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public Matrix4x4 OperatorUnaryMinus_Matrix4x4()
        {
            Matrix4x4 r = default;
            for (var i = 0; i < 1000; i++)
            {
                r = -_m1;
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("OperatorUnaryMinus")]
        public Matrix4x4Baseline OperatorUnaryMinus_Matrix4x4Baseline()
        {
            Matrix4x4Baseline r = default;
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
        public bool OperatorEq_Matrix4x4()
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
        public bool OperatorEq_Matrix4x4Baseline()
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
        public bool OperatorNotEq_Matrix4x4()
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
        public bool OperatorNotEq_Matrix4x4Baseline()
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