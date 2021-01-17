using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Core.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public async Task ClipboardSetGet123Test()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new ClipboardRunner();

            await runner.SetClipboardTextAsync("123", cancellationToken);
            var text = await runner.GetClipboardTextAsync(cancellationToken);

            Assert.AreEqual("123", text);
        }

        [TestMethod]
        public async Task ClipboardSetGet123CommandTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new ClipboardRunner().WithLogging();

            await runner.CallAsync(new Command("clipboard-set-text", "123"), cancellationToken);
            var output = await runner.CallAsync(new Command("clipboard-get-text"), cancellationToken);

            Console.WriteLine($"Output: {output}");

            Assert.AreEqual("123", output.Output.Argument);
        }
    }
}
