using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.TestHelpers;
using H.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public async Task SelectTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var app = await TestWpfApp.CreateAsync(cancellationToken);
            using var runner = new SelectRunner(app.Dispatcher).WithLogging();

            var process = new Process<ICommand>();
            var task = runner.SelectAsync(process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);

            await process.StopAsync(cancellationToken).ConfigureAwait(false);

            var rectangle = await task.ConfigureAwait(false);
            Console.WriteLine($"{rectangle.Left}, {rectangle.Top}, {rectangle.Right}, {rectangle.Bottom}");
        }
    }
}
