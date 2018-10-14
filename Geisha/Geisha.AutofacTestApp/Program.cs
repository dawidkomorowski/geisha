using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Geisha.AutofacTestApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterType<Output>().As<IOutput>().SingleInstance();
                containerBuilder.RegisterType<Constant12>().As<IConstant>().SingleInstance();
                containerBuilder.RegisterType<LetterA>().As<ILetter>().SingleInstance();
                containerBuilder.RegisterType<LetterB>().As<ILetter>().SingleInstance();
                containerBuilder.RegisterType<LetterC>().As<ILetter>().SingleInstance();
                containerBuilder.RegisterType<UpperCase>().As<ICase>().SingleInstance();
                containerBuilder.RegisterType<GuidConstant>().As<IGuidConstant>().SingleInstance();

                using (var container = containerBuilder.Build())
                {
                    using (var lifetimeScope = container.BeginLifetimeScope())
                    {
                        var output = lifetimeScope.Resolve<IOutput>();
                        output.WriteLine("Resolved!");
                    }

                    Console.WriteLine("LifeTimeScope disposed!");
                }

                Console.WriteLine("Container disposed!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    public interface IOutput
    {
        void WriteLine(string text);
    }

    public class Output : IOutput
    {
        private readonly IConstant _constant;
        private readonly IEnumerable<ILetter> _letters;
        private readonly IGuidConstant _guidConstant;

        public Output(IConstant constant, IEnumerable<ILetter> letters, IGuidConstant guidConstant)
        {
            _constant = constant;
            _letters = letters;
            _guidConstant = guidConstant;
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
            Console.WriteLine($"Output.GuidConstant.Value: {_guidConstant.Value}");

            Console.WriteLine($"Constant value is: {_constant.Value}");

            Console.Write("Letters: ");
            foreach (var letter in _letters)
            {
                Console.Write($"{letter.Value}, ");
            }

            Console.WriteLine();
        }
    }

    public interface IConstant
    {
        int Value { get; }
    }

    public class Constant12 : IConstant
    {
        private readonly IGuidConstant _guidConstant;

        public Constant12(IGuidConstant guidConstant)
        {
            _guidConstant = guidConstant;
            Console.WriteLine($"Constant12.GuidConstant.Value: {_guidConstant.Value}");
        }

        public int Value => 12;
    }

    public interface ILetter
    {
        char Value { get; }
    }

    public class LetterA : ILetter
    {
        private readonly IGuidConstant _guidConstant;

        public LetterA(IGuidConstant guidConstant)
        {
            _guidConstant = guidConstant;
            Console.WriteLine($"LetterA.GuidConstant.Value: {_guidConstant.Value}");
        }

        public char Value => 'a';
    }

    public class LetterB : ILetter
    {
        public char Value => 'b';
    }

    public class LetterC : ILetter
    {
        private readonly ICase _case;

        public LetterC(ICase @case)
        {
            _case = @case;
        }

        public char Value => _case.DoCase('c');
    }

    public interface ICase
    {
        char DoCase(char letter);
    }

    public class UpperCase : ICase, IDisposable
    {
        public char DoCase(char letter)
        {
            return letter.ToString().ToUpper().Single();
        }

        public void Dispose()
        {
            Console.WriteLine("UpperCase disposed!");
        }
    }

    public interface IGuidConstant
    {
        Guid Value { get; }
    }

    public class GuidConstant : IGuidConstant
    {
        public GuidConstant()
        {
            Value = Guid.NewGuid();
        }

        public Guid Value { get; }
    }
}