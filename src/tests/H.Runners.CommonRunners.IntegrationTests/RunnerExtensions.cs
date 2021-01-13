using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;

namespace H.Runners.IntegrationTests
{
    public static class RunnerExtensions
    {
        public static async Task<ICommand> CallAsync(
            this IRunner runner, 
            ICommand command, 
            CancellationToken cancellationToken = default)
        {
            runner = runner ?? throw new ArgumentNullException(nameof(runner));
            command = command ?? throw new ArgumentNullException(nameof(command));

            var call = runner.TryPrepareCall(command) ??
                       throw new InvalidOperationException($"Command is not supported: {command.Name}.");

            return await call.RunAsync(cancellationToken);
        }
    }
}
