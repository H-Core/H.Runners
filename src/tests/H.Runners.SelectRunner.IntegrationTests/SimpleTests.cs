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
        [Ignore]
        public async Task SelectTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new SelectRunner();

            var process = new Process<ICommand>();
            var task = runner.SelectAsync(process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            await process.StopAsync(cancellationToken);

            process = new Process<ICommand>();
            var task2 = runner.SelectAsync(process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            await process.StopAsync(cancellationToken);

            var rectangle = await task;
            var rectangle2 = await task2;
            Console.WriteLine($"{rectangle.Left}, {rectangle.Top}, {rectangle.Right}, {rectangle.Bottom}");
            Console.WriteLine($"{rectangle2.Left}, {rectangle2.Top}, {rectangle2.Right}, {rectangle2.Bottom}");
        }

        [TestMethod]
        [Ignore]
        public void GetCursorPositionTest()
        {
            var point = MouseUtilities.GetCursorPosition();

            Console.WriteLine($"{point.X}, {point.Y}");
        }
    }
}
