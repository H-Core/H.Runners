using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
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
        public long DefaultUserId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string DefaultUsername { get; set; } = string.Empty;

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

        /// <summary>
        /// 
        /// </summary>
        public TelegramBotClient? Client { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<string>? MessageReceived;

        private void OnMessageReceived(string value)
        {
            MessageReceived?.Invoke(this, value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TelegramRunner()
        {
            AddSetting(nameof(Token), o => Token = o, TokenIsValid, Token);
            AddSetting(nameof(DefaultUserId), o => DefaultUserId = o, Any, DefaultUserId);
            AddSetting(nameof(DefaultUsername), o => DefaultUsername = o, Any, DefaultUsername);
            AddSetting(nameof(ProxyIp), o => ProxyIp = o, Any, ProxyIp);
            AddSetting(nameof(ProxyPort), o => ProxyPort = o, Any, ProxyPort);

            Add(SyncAction.WithoutArguments("telegram start-receiving", StartReceiving));
            Add(SyncAction.WithoutArguments("telegram stop-receiving", StopReceiving));

            Add(AsyncAction.WithCommand("telegram message", (command, cancellationToken) =>
            {
                var message = command.Input.Arguments.ElementAt(0);
                var to = command.Input.Arguments.ElementAtOrDefault(1);
                
                return SendMessageAsync(message, to, cancellationToken);
            }, "Arguments: text, to?"));
            Add(AsyncAction.WithCommand("telegram audio", (command, cancellationToken) =>
            {
                var bytes = command.Input.Data;
                var to = command.Input.Arguments.ElementAtOrDefault(0);
                var preview = command.Input.Arguments.ElementAtOrDefault(1);

                return SendAudioAsync(bytes, to, preview, cancellationToken);
            }, "Data: mp3 bytes. Arguments: to?, preview?"));
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

            var parts = token.Split(':');

            return parts.Length > 1 && int.TryParse(parts[0], out _);
        }
        
        #endregion

        #region Private methods

        private TelegramBotClient GetClient()
        {
            var isProxy = !string.IsNullOrWhiteSpace(ProxyIp) && Positive(ProxyPort);

            return isProxy
                ? new TelegramBotClient(Token, new WebProxy(ProxyIp, ProxyPort))
                : new TelegramBotClient(Token);
        }
        
        private ChatId GetChatId(string? to = null)
        {
            to ??= string.Empty;

            if (long.TryParse(to, out var result))
            {
                return new ChatId(result);
            }
            if (!string.IsNullOrWhiteSpace(to))
            {
                return new ChatId(to);
            }
            
            return string.IsNullOrWhiteSpace(DefaultUsername)
                ? new ChatId(DefaultUserId)
                : new ChatId(DefaultUsername);
        }
        
        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message, string? to = null, CancellationToken cancellationToken = default)
        {
            message = message ?? throw new ArgumentNullException(nameof(message));
            
            var client = GetClient();
            var chatId = GetChatId(to);

            await client.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void StartReceiving()
        {
            Client ??= GetClient();
            Client.StartReceiving();
            Client.OnMessage += (_, args) =>
            {
                var value = $"{args.Message.From.Username}: {args.Message.Text}";
                
                OnMessageReceived(value);
                this.Print(value);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void StopReceiving()
        {
            Client?.StopReceiving();
        }

        /// <summary>
        /// Sends mp3.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="to"></param>
        /// <param name="preview"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendAudioAsync(Stream stream, string? to = null, string? preview = null, CancellationToken cancellationToken = default)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            var client = GetClient();
            var chatId = GetChatId(to);

            await client.SendAudioAsync(
                    chatId, 
                    new InputOnlineFile(stream, preview ?? "Message"), 
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends mp3.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="to"></param>
        /// <param name="preview"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendAudioAsync(byte[] bytes, string? to = null, string? preview = null, CancellationToken cancellationToken = default)
        {
            bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));

            using var stream = new MemoryStream(bytes);
            
            await SendAudioAsync(stream, to, preview, cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
