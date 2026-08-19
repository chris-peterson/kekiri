using System.Reflection;
using Autofac.Core.Activators.Reflection;

namespace Kekiri.IoC.Autofac
{
    /// <summary>
    /// Finds every instance constructor, public or not.
    /// Adapted from NonPublicConstructorFinder at https://github.com/autofac/Autofac/issues/907.
    /// </summary>
    public class NonStaticConstructorsFinder : DefaultConstructorFinder
    {
        public NonStaticConstructorsFinder()
            : base(type => type.GetConstructors(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
        }
    }
}
