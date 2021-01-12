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
        public AliasRunner(ICommand command, params string[] aliases)
        {
            foreach (var alias in aliases)
            {
                Add(SyncAction.WithCommand(alias, originalCommand =>
                {
                    Run(command.WithMergedInput(originalCommand.Input));
                }));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AliasRunner(string name, params string[] aliases) : 
            this(new Command(name), aliases)
        {
        }

        #endregion
    }
}
