using System;
using System.Collections.Generic;
using Kekiri.Impl;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Kekiri.NUnit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExampleAttribute : TestCaseAttribute
    {
        public ExampleAttribute(params object [] values) : base(values)
        {
        }
    }
}