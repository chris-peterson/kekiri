using System.Reflection;
using System.Threading.Tasks;
using Kekiri.Mtp;
using Microsoft.Testing.Platform.Builder;

namespace Kekiri.Examples.Mtp
{
    /// <summary>
    /// The whole entry point. Written out rather than generated, so the spike shows what the
    /// platform actually needs.
    /// </summary>
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = await TestApplication.CreateBuilderAsync(args);

            builder.AddKekiri(Assembly.GetExecutingAssembly());

            using var app = await builder.BuildAsync();
            return await app.RunAsync();
        }
    }
}
