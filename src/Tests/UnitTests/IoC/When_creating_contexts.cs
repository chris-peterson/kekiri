using Kekiri.IoC.Autofac;
using NUnit.Framework;

namespace Kekiri.UnitTests.IoC
{
    [TestFixture]
    public class When_creating_contexts
    {
        [Test]
        public void If_class_has_default_constructor_then_use_it()
        {
            Assert.IsNotNull(new AutofacScenario<WithDefaultConstructor>().Context);
        }

        [Test]
        public void If_class_has_no_default_constructor_then_use_container()
        {
            Assert.IsNotNull(new AutofacScenario<WithNoDefaultConstructor>().Context);
        }

        #region Supporting Types
        public class WithDefaultConstructor
        {
        }

        public class WithNoDefaultConstructor
        {
            public class Dependency
            {
            }

            // ReSharper disable once UnusedParameter.Local
            public WithNoDefaultConstructor(Dependency dependency)
            {
            }
        }
        #endregion 
    }
}