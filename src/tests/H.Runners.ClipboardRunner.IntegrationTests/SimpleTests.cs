using System;
using System.Threading;
using System.Threading.Tasks;
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
    }
}
