using System;
using System.Windows.Forms;
using H.Core.Runners;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class KeyboardRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public KeyboardRunner()
        {
            Add(SyncAction.WithSingleArgument("keyboard", Keyboard, "CONTROL+V"));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Current implementation uses <seealso cref="SendKeys.Send"/>. <br/>
        /// See https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys.send?view=net-5.0.
        /// </summary>
        /// <param name="command"></param>
        public void Keyboard(string command)
        {
            command = command ?? throw new ArgumentNullException(command);
            
            SendKeys.SendWait(command);
        }

        #endregion
    }
}
