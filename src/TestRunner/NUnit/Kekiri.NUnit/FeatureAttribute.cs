using System;
using NUnit.Framework;

namespace Kekiri.NUnit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureAttribute : TestFixtureAttribute
    {
    }
}
