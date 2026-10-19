using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.OutputDevice;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.OutputDevice;
using Microsoft.Testing.Platform.Requests;
using Microsoft.Testing.Platform.TestHost;
using TestSessionContext = Microsoft.Testing.Platform.TestHost.TestSessionContext;

namespace Kekiri.Tests;

/// <summary>
/// Stand-ins for the parts of Microsoft.Testing.Platform the runner talks to, so the runner can be
/// driven in-process: a message bus that keeps what was published, and an output device that keeps
/// what was displayed.
/// </summary>
static class Platform
{
    public static readonly SessionUid Session = new SessionUid("session");

    /// <summary>The platform constructs these itself; a test has to reach the internal constructor.</summary>
    public static TestSessionContext SessionContext() =>
        (TestSessionContext)Activator.CreateInstance(
            typeof(TestSessionContext),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { Session },
            culture: null);

    public static CreateTestSessionContext CreateSessionContext() =>
        (CreateTestSessionContext)Activator.CreateInstance(
            typeof(CreateTestSessionContext),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { Session, CancellationToken.None },
            culture: null);

    /// <summary>What the platform builds from a --treenode-filter argument.</summary>
    public static TreeNodeFilter TreeFilter(string filter) =>
        (TreeNodeFilter)Activator.CreateInstance(
            typeof(TreeNodeFilter),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { filter },
            culture: null);

    public sealed class Bus : IMessageBus
    {
        public List<TestNode> Published { get; } = new List<TestNode>();

        public Task PublishAsync(IDataProducer dataProducer, IData data)
        {
            Published.Add(((TestNodeUpdateMessage)data).TestNode);
            return Task.CompletedTask;
        }

        public TestNode Node(string displayName) => Published.Single(n => n.DisplayName == displayName);
    }

    public sealed class Completion : IExecuteRequestCompletionNotifier
    {
        public bool Completed { get; private set; }

        public void Complete() => Completed = true;
    }

    public sealed class Output : IOutputDevice
    {
        public List<(string Text, ConsoleColor? Color)> Lines { get; } = new List<(string, ConsoleColor?)>();

        public Task DisplayAsync(IOutputDeviceDataProducer producer, IOutputDeviceData data, CancellationToken cancellationToken)
        {
            var text = (FormattedTextOutputDeviceData)data;
            Lines.Add((text.Text, (text.ForegroundColor as SystemConsoleColor)?.ConsoleColor));
            return Task.CompletedTask;
        }

        public string Text => string.Join("\n", Lines.Select(l => l.Text));
    }

    public sealed class Producer : IOutputDeviceDataProducer
    {
        public string Uid => nameof(Producer);

        public string Version => "1";

        public string DisplayName => nameof(Producer);

        public string Description => nameof(Producer);

        public Task<bool> IsEnabledAsync() => Task.FromResult(true);
    }
}
