using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Searchers;
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
            if (!File.Exists(runner.QBitTorrentPath))
            {
                Assert.Inconclusive("QBitTorrent is not installed.");
            }
            
            runner.CommandReceived += (_, command) => Console.WriteLine($"{command}");
            runner.AsyncCommandReceived += async (_, command, token) =>
            {
                Console.WriteLine($"{command}");

                using var searcher = new YandexSearcher();
                var results = await searcher.SearchAsync(command.Input.Argument, token);

                return new Value(results.Select(result => result.Url).ToArray());
            };
            
            await runner.TorrentAsync("красотка", cancellationToken);
        }
    }
}
