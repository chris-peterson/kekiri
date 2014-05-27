namespace Kekiri.IoC.Autofac
{
    public class AutofacTest : IoCTest
    {
        public AutofacTest() : base(new AutofacContainer())
        {
        }
    }
}