using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core.Runners;
using SpotifyAPI.Web;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public class SpotifyRunner : Runner
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private SpotifyClient? Client { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInitialized { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SpotifyRunner()
        {
            AddSetting(nameof(Username), o => Username = o, NoEmpty, Username);
            AddSetting(nameof(Password), o => Password = o, NoEmpty, Password);

            Add(AsyncAction.WithoutArguments("spotify-play", ResumePlaybackAsync));
            Add(AsyncAction.WithoutArguments("spotify-pause", PausePlaybackAsync));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public async Task InitializeAsync(CancellationToken _ = default)
        {
            IsInitialized = true;

            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(Username, Password);
            var authClient = new OAuthClient(config);
            var response = await authClient.RequestToken(request)
                .ConfigureAwait(false);

            Client ??= new SpotifyClient(config.WithToken(response.AccessToken));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ResumePlaybackAsync(CancellationToken cancellationToken = default)
        {
            if (!IsInitialized)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }

            Client = Client ?? throw new InvalidOperationException("Client is null.");

            await Client.Player.ResumePlayback().ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PausePlaybackAsync(CancellationToken cancellationToken = default)
        {
            if (!IsInitialized)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }

            Client = Client ?? throw new InvalidOperationException("Client is null.");

            await Client.Player.PausePlayback().ConfigureAwait(false);
        }

        #endregion
    }
}
