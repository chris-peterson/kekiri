using System.Collections.Generic;
using System.Linq;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Requests;

namespace Behavior.Internal;

/// <summary>
/// Applies the filter the platform hands to a discovery or run request. Ignoring it is what
/// makes "run this one test" run the whole suite: an IDE asks for a single scenario by sending
/// its uid, and a framework that answers with everything has told the IDE nothing it asked for.
/// </summary>
static class ScenarioFilter
{
    public static IEnumerable<Scenario> Apply(ITestExecutionFilter filter, IEnumerable<Scenario> scenarios)
    {
        switch (filter)
        {
            case TestNodeUidListFilter uids:
                var wanted = new HashSet<string>(uids.TestNodeUids.Select(u => u.Value));
                return scenarios.Where(s => wanted.Contains(s.Uid));

            // TreeNodeFilter is TPEXP: the platform ships it as evaluation-only and reserves the
            // right to change it. Handling it is still better than treating a --treenode-filter
            // as "run everything", which is a wrong answer rather than an unsupported one.
#pragma warning disable TPEXP
            case TreeNodeFilter tree:
                return scenarios.Where(s => tree.MatchesFilter(s.NodePath, s.FilterProperties));
#pragma warning restore TPEXP

            // NopFilter, and anything a later platform version adds: run everything rather than
            // silently dropping tests because the filter type wasn't recognized.
            default:
                return scenarios;
        }
    }
}
