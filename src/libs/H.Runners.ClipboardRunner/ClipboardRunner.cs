using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using H.Core.Runners;

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
            Add(new AsyncAction("clipboard-set", async (command, cancellationToken) =>
            {
                await SetClipboardTextAsync(command.Input.Argument, cancellationToken).ConfigureAwait(false);

                return command.Input;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        public async Task SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
        {
            if (Application == null)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }
            Application = Application ?? throw new InvalidOperationException("Application is null.");

            Application.Dispatcher?.Invoke(() => Clipboard.SetText(text, TextDataFormat.Text));
        }

        #endregion
    }
}
