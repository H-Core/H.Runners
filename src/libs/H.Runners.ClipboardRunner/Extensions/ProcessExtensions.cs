using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace H.Runners.Extensions
{
    /// <summary>
    /// Extensions that work with <see cref="Process"/> <br/>
    /// <![CDATA[Version: 1.0.0.0]]> <br/>
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Waits to complete the process.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static async Task<int> WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            process = process ?? throw new ArgumentNullException(nameof(process));
            process.EnableRaisingEvents = true;

            var completionSource = new TaskCompletionSource<int>();

            process.Exited += (_, _) =>
            {
                try
                {
                    completionSource.TrySetResult(process.ExitCode);
                }
                catch (InvalidOperationException)
                {
                    completionSource.TrySetResult(0);
                }
            };
            if (process.HasExited)
            {
                return process.ExitCode;
            }

            using var registration = cancellationToken.Register(
                () => completionSource.TrySetCanceled(cancellationToken));

            return await completionSource.Task.ConfigureAwait(false);
        }
    }
}
