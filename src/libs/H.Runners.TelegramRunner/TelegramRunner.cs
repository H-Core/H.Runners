using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using H.Core.Runners;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public class TelegramRunner : Runner
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public long UserId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ProxyIp { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public int ProxyPort { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TelegramRunner()
        {
            AddSetting(nameof(Token), o => Token = o, TokenIsValid, Token);
            AddSetting(nameof(UserId), o => UserId = o, Always, UserId);
            AddSetting(nameof(Username), o => Username = o, Any, Username);
            AddSetting(nameof(ProxyIp), o => ProxyIp = o, Always, ProxyIp);
            AddSetting(nameof(ProxyPort), o => ProxyPort = o, Always, ProxyPort);
                   
            Add(new AsyncAction("telegram text", SendMessageAsync, "message"));
            Add(new AsyncAction("telegram audio", SendAudioAsync, "mp3 bytes"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TokenIsValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                var unused = new TelegramBotClient(token);

                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedId"></param>
        /// <returns></returns>
        public static bool UsedIdIsValid(int usedId) => usedId > 0;

        #endregion

        #region Private methods

        private TelegramBotClient GetClient()
        {
            var isProxy = !string.IsNullOrWhiteSpace(ProxyIp) && Positive(ProxyPort);

            return isProxy
                ? new TelegramBotClient(Token, new WebProxy(ProxyIp, ProxyPort))
                : new TelegramBotClient(Token);
        }
        
        private ChatId GetChatId()
        {
            return string.IsNullOrWhiteSpace(Username)
                ? new ChatId(UserId)
                : new ChatId(Username);
        }
        
        #endregion

        #region Public methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));
            
            var client = GetClient();
            var chatId = GetChatId();

            await client.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends mp3.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendAudioAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            var client = GetClient();
            var chatId = GetChatId();

            await client.SendAudioAsync(
                    chatId, 
                    new InputOnlineFile(stream), 
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends mp3.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendAudioAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));

            using var stream = new MemoryStream(bytes);
            
            await SendAudioAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
