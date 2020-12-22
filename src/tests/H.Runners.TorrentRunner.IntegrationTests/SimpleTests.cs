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
        private static TorrentRunner CreateTorrentRunner()
        {
            return new();
        }

        [TestMethod]
        public async Task TorrentTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTorrentRunner();
            runner.CommandReceived += (_, value) => Console.WriteLine($"{value}");
            // Search command.
            runner.AsyncCommandReceived += (_, _, _) => Task.FromResult<IValue>(new Value(
                "https://kinotorrs.me/melodrama/krasotka_1990_skachat_torrent"));
            
            await runner.TorrentAsync("красотка", cancellationToken);
        }
    }
}
