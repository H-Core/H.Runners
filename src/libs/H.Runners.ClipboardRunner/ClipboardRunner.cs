using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        private Application? Application { get; set; }

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
                Application = Application.Current;
                return;
            }

            var thread = new Thread(() =>
            {
                Application = new Application();
                Application.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (Application == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<Application> GetApplicationAsync(
            CancellationToken cancellationToken = default)
        {
            if (Application == null)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }

            return Application ?? throw new InvalidOperationException("Application is null.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        public async Task SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
        {
            var application = await GetApplicationAsync(cancellationToken)
                .ConfigureAwait(false);

            await application.Dispatcher.InvokeAsync(() => Clipboard.SetText(text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task<string> GetClipboardTextAsync(CancellationToken cancellationToken = default)
        {
            var application = await GetApplicationAsync(cancellationToken)
                .ConfigureAwait(false);

            var text = await application.Dispatcher.InvokeAsync(Clipboard.GetText);

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

            var application = await GetApplicationAsync(cancellationToken)
                .ConfigureAwait(false);

            await application.Dispatcher.InvokeAsync(() => Clipboard.SetImage(image.ToBitmapImage()));
        }

        #endregion
    }
}