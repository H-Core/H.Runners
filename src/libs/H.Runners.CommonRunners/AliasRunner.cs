using System.Linq;
using H.Core;
using H.Core.Runners;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AliasRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="fireAndForget"></param>
        /// <param name="aliases"></param>
        public AliasRunner(ICommand command, bool fireAndForget, params string[] aliases)
        {
            foreach (var alias in aliases)
            {
                Add(fireAndForget
                    ? new SyncAction(alias, originalCommand =>
                    {
                        Run(command.WithMergedInput(originalCommand.Input));
                    })
                    : new AsyncAction(alias, async (originalCommand, cancellationToken) =>
                    {
                        var values = await RunAsync(
                                command.WithMergedInput(originalCommand.Input), 
                                cancellationToken)
                            .ConfigureAwait(false);

                        return values.FirstOrDefault() ?? Value.Empty;
                    }));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fireAndForget"></param>
        /// <param name="aliases"></param>
        public AliasRunner(string name, bool fireAndForget, params string[] aliases) : 
            this(new Command(name), fireAndForget, aliases)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="aliases"></param>
        public AliasRunner(ICommand command, params string[] aliases) :
            this(command, false, aliases)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="aliases"></param>
        public AliasRunner(string name, params string[] aliases) :
            this(name, false, aliases)
        {
        }

        #endregion
    }
}
