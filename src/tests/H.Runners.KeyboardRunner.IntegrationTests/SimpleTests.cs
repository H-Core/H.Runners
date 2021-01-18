using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void Keyboard123Test()
        {
            using var runner = new KeyboardRunner();
            
            runner.Keyboard(
                new Keys(Key.D1), 
                new Keys(Key.D2), 
                new Keys(Key.D3)
                );
        }

        [TestMethod]
        public async Task Keyboard123CallTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new KeyboardRunner();

            await runner.CallAsync(
                new Command("keyboard", "D1", "D2", "D3"), cancellationToken);
        }
    }
}
