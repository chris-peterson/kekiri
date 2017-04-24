using System;
using System.Reflection;
using NUnit.Common;
using NUnitLite;

namespace Kekiri.Examples.NUnit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var writter = new ExtendedTextWrapper(Console.Out);
            new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args, writter, Console.In);
        }
    }
}