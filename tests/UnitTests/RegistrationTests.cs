using System;
using System.Threading.Tasks;
using Kekiri.Mtp;
using Microsoft.Testing.Platform.Builder;

namespace Kekiri.Tests;

/// <summary>
/// What a scenario project's generated entry point does: register Kekiri on a platform builder.
/// Building the application is what makes the platform call the factories it was handed.
/// </summary>
public class RegistrationTests
{
    [Test]
    public async Task The_generated_entry_point_registers_kekiri()
    {
        var builder = await TestApplication.CreateBuilderAsync(Array.Empty<string>());

        TestingPlatformBuilderHook.AddExtensions(builder, Array.Empty<string>());

        using var app = await builder.BuildAsync();
        await Assert.That(app).IsNotNull();
    }

    [Test]
    public async Task A_hand_written_entry_point_registers_kekiri_for_its_own_assembly()
    {
        var builder = await TestApplication.CreateBuilderAsync(Array.Empty<string>());

        await Assert.That(builder.AddKekiri()).IsSameReferenceAs(builder);

        using var app = await builder.BuildAsync();
    }
}
