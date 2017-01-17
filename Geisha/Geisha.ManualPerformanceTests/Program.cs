using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Common.Geometry;

namespace Geisha.ManualPerformanceTests
{
    static class Program
    {
        static void Main()
        {
            var size = 1000000;

            var testArrayOfMatrix3 = CreateTestArrayOfMatrix3(size);
            var testArrayOfFixedMatrix3 = CreateTestArrayOfFixedMatrix3(size);
            var testArrayOfFixedMatrix3WithIndexer = CreateTestArrayOfFixedMatrix3(size);
            var testArrayOfMatrix3_Random = CreateTestArrayOfMatrix3(size);
            var testArrayOfFixedMatrix3_Random = CreateTestArrayOfFixedMatrix3(size);
            var testArrayOfFixedMatrix3WithIndexer_Random = CreateTestArrayOfFixedMatrix3(size);

            var runTestOfMatrix3 = RunTestOfMatrix3(testArrayOfMatrix3);
            var runTestOfFixedMatrix3 = RunTestOfFixedMatrix3(testArrayOfFixedMatrix3);
            var runTestOfFixedMatrix3WithIndexer = RunTestOfFixedMatrix3WithIndexer(testArrayOfFixedMatrix3WithIndexer);
            var runTestOfMatrix3_Random = RunTestOfMatrix3_Random(testArrayOfMatrix3_Random);
            var runTestOfFixedMatrix3_Random = RunTestOfFixedMatrix3_Random(testArrayOfFixedMatrix3_Random);
            var runTestOfFixedMatrix3WithIndexer_Random = RunTestOfFixedMatrix3WithIndexer_Random(testArrayOfFixedMatrix3WithIndexer_Random);

            Console.WriteLine(runTestOfMatrix3);
            Console.WriteLine(runTestOfFixedMatrix3);
            Console.WriteLine(runTestOfFixedMatrix3WithIndexer);
            Console.WriteLine(runTestOfMatrix3_Random);
            Console.WriteLine(runTestOfFixedMatrix3_Random);
            Console.WriteLine(runTestOfFixedMatrix3WithIndexer_Random);

            Console.ReadKey();
        }

        static Matrix3 RandomMatrix3()
        {
            var r = new Random();
            var list = new List<double>();
            for (var i = 0; i < 9; i++)
            {
                list.Add(r.NextDouble());
            }
            return new Matrix3(list.ToArray());
        }

        static FixedMatrix3 RandomFixedMatrix3()
        {
            var r = new Random();
            var list = new List<double>();
            for (var i = 0; i < 9; i++)
            {
                list.Add(r.NextDouble());
            }
            return new FixedMatrix3(list.ToArray());
        }

        static Matrix3Operation[] CreateTestArrayOfMatrix3(int size)
        {
            var array = new Matrix3Operation[size];
            for (var i = 0; i < array.Length; i++)
            {
                array[i].Left = RandomMatrix3();
                array[i].Right = RandomMatrix3();
            }
            return array;
        }

        static FixedMatrix3Operation[] CreateTestArrayOfFixedMatrix3(int size)
        {
            var array = new FixedMatrix3Operation[size];
            for (var i = 0; i < array.Length; i++)
            {
                array[i].Left = RandomFixedMatrix3();
                array[i].Right = RandomFixedMatrix3();
            }
            return array;
        }

        #region Contiguous Memory Access Tests

        static TimeSpan RunTestOfMatrix3(Matrix3Operation[] array)
        {
            var sw = Stopwatch.StartNew();
            for (var j = 0; j < 10; j++)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    array[i].Result = array[i].Left*array[i].Right;
                }
            }
            sw.Stop();
            return sw.Elapsed;
        }

        static TimeSpan RunTestOfFixedMatrix3(FixedMatrix3Operation[] array)
        {
            var sw = Stopwatch.StartNew();
            for (var j = 0; j < 10; j++)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    array[i].Result = array[i].Left.Multiply(array[i].Right);
                }
            }
            sw.Stop();
            return sw.Elapsed;
        }

        static TimeSpan RunTestOfFixedMatrix3WithIndexer(FixedMatrix3Operation[] array)
        {
            var sw = Stopwatch.StartNew();
            for (var j = 0; j < 10; j++)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    array[i].Result = array[i].Left.MultiplyWithIndexer(array[i].Right);
                }
            }
            sw.Stop();
            return sw.Elapsed;
        }

        #endregion

        #region Random Memory Access Tests

        static TimeSpan RunTestOfMatrix3_Random(Matrix3Operation[] array)
        {
            var r = new Random();
            var sw = Stopwatch.StartNew();
            for (var j = 0; j < 10; j++)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var index = r.Next(array.Length);
                    array[index].Result = array[index].Left * array[index].Right;
                }
            }
            sw.Stop();
            return sw.Elapsed;
        }

        static TimeSpan RunTestOfFixedMatrix3_Random(FixedMatrix3Operation[] array)
        {
            var r = new Random();
            var sw = Stopwatch.StartNew();
            for (var j = 0; j < 10; j++)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var index = r.Next(array.Length);
                    array[index].Result = array[index].Left.Multiply(array[index].Right);
                }
            }
            sw.Stop();
            return sw.Elapsed;
        }

        static TimeSpan RunTestOfFixedMatrix3WithIndexer_Random(FixedMatrix3Operation[] array)
        {
            var r = new Random();
            var sw = Stopwatch.StartNew();
            for (var j = 0; j < 10; j++)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var index = r.Next(array.Length);
                    array[index].Result = array[index].Left.MultiplyWithIndexer(array[index].Right);
                }
            }
            sw.Stop();
            return sw.Elapsed;
        }

        #endregion

        private struct Matrix3Operation
        {
            public Matrix3 Left;
            public Matrix3 Right;
            public Matrix3 Result;
        }

        private struct FixedMatrix3Operation
        {
            public FixedMatrix3 Left;
            public FixedMatrix3 Right;
            public FixedMatrix3 Result;
        }
    }
}