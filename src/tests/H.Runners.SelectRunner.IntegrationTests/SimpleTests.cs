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
    //    public static void CheckDesktop()
    //    {
    //        if (!MouseUtilities.IsDesktop())
    //        {
    //            Assert.Inconclusive("Mouse tests work only on desktop.");
    //        }
    //    }

        [TestMethod]
        public async Task SelectTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            //CheckDesktop();

            using var runner = new SelectRunner();

            var process = new Process<ICommand>();
            var task = runner.SelectAsync(process, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);

            await process.StopAsync(cancellationToken).ConfigureAwait(false);

            var rectangle = await task.ConfigureAwait(false);
            Console.WriteLine($"{rectangle.Left}, {rectangle.Top}, {rectangle.Right}, {rectangle.Bottom}");
        }
    }
}
