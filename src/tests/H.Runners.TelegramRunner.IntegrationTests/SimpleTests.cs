using System;
using System.Threading;
using System.Threading.Tasks;
using H.IO.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class SimpleTests
    {
        private static TelegramRunner CreateTelegramRunner()
        {
            return new()
            {
                Token = Environment.GetEnvironmentVariable("TELEGRAM_HOMECENTER_BOT_TOKEN")
                        ?? throw new AssertInconclusiveException("TELEGRAM_HOMECENTER_BOT_TOKEN environment variable is not found."),
                UserId = 482553595,
            };
        }

        [TestMethod]
        public async Task SendTextTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTelegramRunner();

            await runner.SendMessageAsync(nameof(SendTextTest), cancellationToken);
        }

        [TestMethod]
        public async Task SendAudioTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTelegramRunner();
            using var stream = ResourcesUtilities.ReadFileAsStream("test.mp3");
            
            await runner.SendAudioAsync(stream, cancellationToken);
        }
    }
}
