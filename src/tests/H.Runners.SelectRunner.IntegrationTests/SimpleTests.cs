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
        public async Task SelectTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = new SelectRunner();

            runner.SelectAsync(TimeSpan.FromSeconds(5), cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }

        [TestMethod]
        public void GetCursorPositionTest()
        {
            var point = MouseUtilities.GetCursorPosition();

            Console.WriteLine($"{point.X}, {point.Y}");
        }
    }
}
