using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Core.Utilities;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SequenceRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SequenceRunner()
        {
            Add(new AsyncAction("sequence", async (command, cancellationToken) =>
            {
                var count = int.Parse(command.Input.Arguments.ElementAt(0), CultureInfo.InvariantCulture);
                var commands = command.Input.Arguments
                    .Skip(1)
                    .Take(count)
                    .Select(Command.Parse)
                    .Cast<ICommand>()
                    .ToArray();
                var arguments = command.Input.Arguments
                    .Skip(1 + count)
                    .ToArray();

                await SequenceAsync(commands, arguments, cancellationToken).ConfigureAwait(false);

                return Value.Empty;
            }));
            Add(SyncAction.WithoutArguments("ignore", () => {}));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="arguments"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IValue> SequenceAsync(
            ICommand[] commands, 
            string[]? arguments = null, 
            CancellationToken cancellationToken = default)
        {
            commands = commands ?? throw new ArgumentNullException(nameof(commands));

            var value = (IValue)new Value(arguments ?? EmptyArray<string>.Value);
            foreach (var command in commands)
            {
                var values = await RunAsync(command.WithMergedInput(value), cancellationToken)
                    .ConfigureAwait(false);

                value = values.FirstOrDefault() ?? Value.Empty;
            }

            return value;
        }

        #endregion
    }
}
