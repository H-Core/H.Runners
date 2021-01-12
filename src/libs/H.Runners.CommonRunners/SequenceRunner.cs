using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;

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
                var commands = command.Input.Arguments
                    .Select(Command.Parse);

                await SequenceAsync(commands, cancellationToken).ConfigureAwait(false);

                return Value.Empty;
            }));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IValue> SequenceAsync(IEnumerable<ICommand> commands, CancellationToken cancellationToken = default)
        {
            commands = commands ?? throw new ArgumentNullException(nameof(commands));

            var value = (IValue)Value.Empty;
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
