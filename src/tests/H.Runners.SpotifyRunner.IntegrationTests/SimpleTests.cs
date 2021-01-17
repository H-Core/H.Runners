using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Runners.IntegrationTests
{
    [TestClass]
    public class SimpleTests
    {
        private static SpotifyRunner CreateSpotifyRunner()
        {
            return new()
            {
                Username = Environment.GetEnvironmentVariable("SPOTIFY_USERNAME") ?? 
                           throw new AssertInconclusiveException("SPOTIFY_USERNAME environment variable is not found."),
                Password = Environment.GetEnvironmentVariable("SPOTIFY_PASSWORD") ??
                           throw new AssertInconclusiveException("SPOTIFY_PASSWORD environment variable is not found."),
            };
        }

        [TestMethod]
        public async Task ResumePlaybackTest()
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cancellationToken = cancellationTokenSource.Token;

            using var runner = CreateSpotifyRunner();

            await runner.ResumePlaybackAsync(cancellationToken);
        }
    }
}
