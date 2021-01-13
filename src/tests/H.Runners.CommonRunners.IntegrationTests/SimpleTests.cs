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
        public async Task SequenceMethodTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new SequenceRunner().WithLogging();

            await runner.SequenceAsync(
                new []
                {
                    new Command("clipboard-set"),
                    new Command("keyboard ^v"),
                }, 
                new []{ "123" }, 
                cancellationToken);
        }

        [TestMethod]
        public async Task SequenceCommandTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new SequenceRunner().WithLogging();
            
            await runner.CallAsync(
                new Command("sequence", "2", "clipboard-set", "keyboard ^v", "123"), 
                cancellationToken);
        }

        [TestMethod]
        public async Task AliasSequenceTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new AliasRunner(
                new Command("sequence", "2", "clipboard-set", "keyboard ^v"),
                "paste").WithLogging();

            await runner.CallAsync(new Command("paste", "123"), cancellationToken);
        }
    }
}
