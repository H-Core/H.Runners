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

        /// <summary>
        /// 
        /// </summary>
        public Dispatcher? Dispatcher { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ClipboardRunner()
        {
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

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (Application.Current != null)
            {
                Dispatcher = Application.Current.Dispatcher;
                return;
            }

            var thread = new Thread(() =>
            {
                var application = new Application();
                application.Startup += (_, _) =>
                {
                    Dispatcher = application.Dispatcher;
                };
                application.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (Dispatcher == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<Dispatcher> GetDispatcherAsync(
            CancellationToken cancellationToken = default)
        {
            if (Dispatcher == null)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }

            return Dispatcher ?? throw new InvalidOperationException("Application is null.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        public async Task SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
        {
            var dispatcher = await GetDispatcherAsync(cancellationToken)
                .ConfigureAwait(false);

            await dispatcher.InvokeAsync(() => Clipboard.SetText(text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task<string> GetClipboardTextAsync(CancellationToken cancellationToken = default)
        {
            var dispatcher = await GetDispatcherAsync(cancellationToken)
                .ConfigureAwait(false);

            var text = await dispatcher.InvokeAsync(Clipboard.GetText);

            return text ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="cancellationToken"></param>
        public async Task SetClipboardImageAsync(Image image, CancellationToken cancellationToken = default)
        {
            image = image ?? throw new ArgumentNullException(nameof(image));

            var dispatcher = await GetDispatcherAsync(cancellationToken)
                .ConfigureAwait(false);

            await dispatcher.InvokeAsync(() => Clipboard.SetImage(image.ToBitmapImage()));
        }

        #endregion
    }
}