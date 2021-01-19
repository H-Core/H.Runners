using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Core.TestHelpers;
using H.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public async Task ScreenshotTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var app = await TestWpfApp.CreateAsync(cancellationToken);
            using var runner = new SelectRunner(app.Dispatcher).WithLogging();

            var process = new Process<ICommand>();
            var task = runner.SelectAsync(process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            await process.StopAsync(cancellationToken);

            var rectangle = await task;

            Console.WriteLine($"{rectangle.Left}, {rectangle.Top}, {rectangle.Right}, {rectangle.Bottom}");

            using var screenshotRunner = new ScreenshotRunner().WithLogging();
            var image = await screenshotRunner.ShotAsync(rectangle, cancellationToken);

            using var clipboardRunner = new ClipboardRunner(app.Dispatcher).WithLogging();
            await clipboardRunner.SetClipboardImageAsync(image, cancellationToken);
        }

        [TestMethod]
        public async Task ScreenshotCallTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var app = await TestWpfApp.CreateAsync(cancellationToken);
            using var selectRunner = new SelectRunner(app.Dispatcher).WithLogging();

            var process = new Process<ICommand>();
            var selectTask = selectRunner.CallAsync(new Command("select"), process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            await process.StopAsync(cancellationToken);

            var selectOutput = await selectTask;

            Console.WriteLine($"{nameof(selectOutput)}: {selectOutput.Output}");
            
            using var screenshotRunner = new ScreenshotRunner().WithLogging();

            var screenshotOutput = await screenshotRunner.CallAsync(
                new Command("screenshot", selectOutput.Output), 
                cancellationToken);

            Console.WriteLine($"{nameof(screenshotOutput)}: {screenshotOutput.Output}");

            using var clipboardRunner = new ClipboardRunner(app.Dispatcher).WithLogging();

            var clipboardOutput = await clipboardRunner.CallAsync(
                new Command("clipboard-set-image", screenshotOutput.Output),
                cancellationToken);

            Console.WriteLine($"{nameof(clipboardOutput)}: {clipboardOutput.Output}");
        }
    }
}
