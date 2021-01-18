using System.Linq;
using System.Windows.Forms;
using H.Core.Runners;
using H.Runners.Extensions;
using Keys = H.Core.Keys;

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
            Add(SyncAction.WithArguments("keyboard", arguments =>
            {
                var values = arguments
                    .Select(Keys.Parse)
                    .ToArray();

                Keyboard(values);
            }, "CONTROL+V"));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Current implementation uses <seealso cref="SendKeys.Send"/>. <br/>
        /// See https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys.send?view=net-5.0.
        /// </summary>
        /// <param name="values"></param>
        public void Keyboard(params Keys[] values)
        {
            foreach (var keys in values)
            {
                SendKeys.SendWait(keys.ToSendKeys());
            }
        }

        #endregion
    }
}
