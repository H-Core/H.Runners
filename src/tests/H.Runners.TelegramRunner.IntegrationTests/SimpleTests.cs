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
        private static TelegramRunner CreateTelegramRunner(long userId = 0)
        {
            return new()
            {
                Token = Environment.GetEnvironmentVariable("TELEGRAM_HOMECENTER_BOT_TOKEN")
                        ?? throw new AssertInconclusiveException("TELEGRAM_HOMECENTER_BOT_TOKEN environment variable is not found."),
                DefaultUserId = userId,
            };
        }

        [TestMethod]
        public async Task SendTextTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTelegramRunner(482553595);

            await runner.SendMessageAsync(nameof(SendTextTest), null, cancellationToken);
        }

        [TestMethod]
        public async Task SendAudioTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTelegramRunner(482553595);
            using var stream = ResourcesUtilities.ReadFileAsStream("test.mp3");
            
            await runner.SendAudioAsync(stream, null, nameof(SendAudioTest), cancellationToken);
        }

        [TestMethod]
        public async Task SendTextToTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTelegramRunner();

            await runner.SendMessageAsync(nameof(SendTextToTest), "482553595", cancellationToken);
        }

        [TestMethod]
        public async Task SendAudioToTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTelegramRunner();
            using var stream = ResourcesUtilities.ReadFileAsStream("test.mp3");

            await runner.SendAudioAsync(stream, "482553595", nameof(SendAudioToTest), cancellationToken);
        }

        [TestMethod]
        public async Task ReceivingTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateTelegramRunner();
            runner.MessageReceived += (_, message) =>
            {
                Console.WriteLine($"{nameof(runner.MessageReceived)}: {message}");
            };

            await runner.InitializeAsync(cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
        }
    }
}
