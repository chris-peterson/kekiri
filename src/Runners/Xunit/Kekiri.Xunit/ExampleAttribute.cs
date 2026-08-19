using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace Kekiri.Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExampleAttribute : DataAttribute
    {
        readonly object[] _data;

        public ExampleAttribute(params object[] data)
        {
            _data = data ?? new object[] { null };
        }

        public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
            MethodInfo testMethod,
            DisposalTracker disposalTracker)
        {
            IReadOnlyCollection<ITheoryDataRow> rows = new ITheoryDataRow[]
            {
                new TheoryDataRow(_data)
                {
                    Explicit = ExplicitAsNullable,
                    Label = Label,
                    Skip = Skip,
                    TestDisplayName = TestDisplayName,
                    Timeout = TimeoutAsNullable,
                }
            };

            return new ValueTask<IReadOnlyCollection<ITheoryDataRow>>(rows);
        }

        public override bool SupportsDiscoveryEnumeration() => true;
    }
}
