using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using H.Core;
using H.Core.Runners;
using H.Runners.Extensions;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ClipboardRunner : Runner
    {
        #region Properties

        private Dispatcher Dispatcher { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ClipboardRunner(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            Add(new AsyncAction("clipboard-set-text", async (command, cancellationToken) =>
            {
                await SetClipboardTextAsync(command.Input.Argument, cancellationToken).ConfigureAwait(false);

                return Value.Empty;
            }));
            Add(new AsyncAction("clipboard-get-text", async (_, cancellationToken) =>
            {
                var text = await GetClipboardTextAsync(cancellationToken).ConfigureAwait(false);

                return new Value(text);
            }));
            Add(new AsyncAction("clipboard-set-image", async (command, cancellationToken) =>
            {
                using var stream = new MemoryStream(command.Input.Data);
                var image = Image.FromStream(stream);

                await SetClipboardImageAsync(image, cancellationToken).ConfigureAwait(false);

                return Value.Empty;
            }));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task SetClipboardTextAsync(
            string text, 
            CancellationToken cancellationToken = default)
        {
            text = text ?? throw new ArgumentNullException(nameof(text));

            await Dispatcher.InvokeAsync(
                () => Clipboard.SetText(text),
                DispatcherPriority.Normal,
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task<string> GetClipboardTextAsync(
            CancellationToken cancellationToken = default)
        {
            return await Dispatcher.InvokeAsync(
                Clipboard.GetText,
                DispatcherPriority.Normal,
                cancellationToken) ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task SetClipboardImageAsync(
            Image image, 
            CancellationToken cancellationToken = default)
        {
            image = image ?? throw new ArgumentNullException(nameof(image));

            await Dispatcher.InvokeAsync(
                () => Clipboard.SetImage(image.ToBitmapImage()),
                DispatcherPriority.Normal,
                cancellationToken);
        }

        #endregion
    }
}