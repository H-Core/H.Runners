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
        public int UserId { get; set; }

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
            AddSetting(nameof(Token), o => Token = o, TokenIsValid, string.Empty);
            AddSetting(nameof(UserId), o => UserId = o, UsedIdIsValid, 0);
            AddSetting(nameof(ProxyIp), o => ProxyIp = o, Always, string.Empty);
            AddSetting(nameof(ProxyPort), o => ProxyPort = o, Always, 0);
                   
            Add(new AsyncAction("telegram text", SendMessageAsync, "message"));
            Add(new AsyncAction("telegram audio", SendAudioAsync, "bytes"));
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

        private async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            var isProxy = !string.IsNullOrWhiteSpace(ProxyIp) && Positive(ProxyPort);
            var client = isProxy
                ? new TelegramBotClient(Token, new WebProxy(ProxyIp, ProxyPort))
                : new TelegramBotClient(Token);
            
            await client.SendTextMessageAsync(new ChatId(UserId), message, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task SendAudioAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            var isProxy = !string.IsNullOrWhiteSpace(ProxyIp) && Positive(ProxyPort);
            var client = isProxy
                ? new TelegramBotClient(Token, new WebProxy(ProxyIp, ProxyPort))
                : new TelegramBotClient(Token);

            using var stream = new MemoryStream(bytes);
            await client.SendAudioAsync(new ChatId(UserId), new InputOnlineFile(stream), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion
    }
}
