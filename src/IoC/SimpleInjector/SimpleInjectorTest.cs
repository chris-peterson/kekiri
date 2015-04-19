namespace Kekiri.IoC.SimpleInjector
{
    public class SimpleInjectorTest : IoCTest
    {
        public SimpleInjectorTest() : base(new SimpleInjectorContainer())
        {
        }
    }
}