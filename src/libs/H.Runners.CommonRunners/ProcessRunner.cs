using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Runners.Extensions;
using Process = System.Diagnostics.Process;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ProcessRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ProcessRunner()
        {
            Add(new AsyncAction("start-process", async (command, cancellationToken) =>
            {
                var path = command.Input.Arguments.ElementAt(0);
                var arguments = command.Input.Arguments.ElementAtOrDefault(0) ?? string.Empty;

                await StartProcessAsync(path, arguments, cancellationToken).ConfigureAwait(false);

                return Value.Empty;
            }));
            Add(AsyncAction.WithSingleArgument("explorer", ExplorerAsync, "Argument: path"));
        }

        #endregion

        #region Private methods

        private static string NormalizePath(string path)
        {
            path = path ?? throw new ArgumentNullException(nameof(path));

            return path
                .Replace("\\\\", "\\")
                .Replace("//", "\\")
                .Replace("/", "\\");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arguments"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartProcessAsync(string path, string? arguments = null, CancellationToken cancellationToken = default)
        {
            path = path ?? throw new ArgumentNullException(nameof(path));

            using var process = Process.Start(new ProcessStartInfo(path, arguments ?? string.Empty)
            {
                UseShellExecute = true,
            }) ?? throw new InvalidOperationException("Process is null.");

            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        public async Task ExplorerAsync(string path, CancellationToken cancellationToken = default)
        {
            path = path ?? throw new ArgumentNullException(nameof(path));

            await StartProcessAsync("explorer", $"\"{NormalizePath(path)}\"", cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion
    }
}
