using System;
using Kekiri.IoC.Autofac;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit
{
    [SetUpFixture]
    public class _BeforeTestRun
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("KEKIRI_OUTPUT", "console,files");

            AutofacBootstrapper.Initialize();
        }
    }
}