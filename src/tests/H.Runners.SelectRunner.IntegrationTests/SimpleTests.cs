using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
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

            using var runner = new SelectRunner();

            var process = new Process<ICommand>();
            var task = runner.SelectAsync(process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            await process.StopAsync(cancellationToken);

            var rectangle = await task;
            Console.WriteLine($"{rectangle.Left}, {rectangle.Top}, {rectangle.Right}, {rectangle.Bottom}");
        }

        [TestMethod]
        public void GetCursorPositionTest()
        {
            var point = MouseUtilities.GetCursorPosition();

            Console.WriteLine($"{point.X}, {point.Y}");
        }
    }
}
