using System;
using NUnit.Framework;

namespace Kekiri
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TagAttribute : CategoryAttribute
    {
        public TagAttribute(string name)
            : base(name)
        {
        }
    }
}