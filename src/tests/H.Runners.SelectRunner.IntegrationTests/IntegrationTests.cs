using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public async Task ScreenshotTest()
        {
            //SimpleTests.CheckDesktop();

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new SelectRunner();

            var process = new Process<ICommand>();
            var task = runner.SelectAsync(process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            await process.StopAsync(cancellationToken);

            var rectangle = await task;

            Console.WriteLine($"{rectangle.Left}, {rectangle.Top}, {rectangle.Right}, {rectangle.Bottom}");

            using var screenshotRunner = new ScreenshotRunner();
            var image = await screenshotRunner.ShotAsync(rectangle, cancellationToken);

            using var clipboardRunner = new ClipboardRunner();
            await clipboardRunner.SetClipboardImageAsync(image, cancellationToken);
        }
    }
}
