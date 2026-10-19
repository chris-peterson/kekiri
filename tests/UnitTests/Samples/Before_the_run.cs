using System.Collections.Generic;

namespace Kekiri.Tests.Samples.Setup;

/// <summary>Records each setup as it runs, so a test can see the order the runner called them in.</summary>
public static class Setups
{
    public static readonly List<string> Ran = new List<string>();
}

public class B_second_setup : IBeforeTestRun
{
    public void Setup() => Setups.Ran.Add(nameof(B_second_setup));
}

public class A_first_setup : IBeforeTestRun
{
    public void Setup() => Setups.Ran.Add(nameof(A_first_setup));
}

public abstract class Not_a_setup_on_its_own : IBeforeTestRun
{
    public void Setup() => Setups.Ran.Add(nameof(Not_a_setup_on_its_own));
}
