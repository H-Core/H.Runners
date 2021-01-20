using System.Linq;
using WindowsInput;
using WindowsInput.Native;
using H.Core.Runners;
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
        /// Current implementation uses <seealso cref="InputSimulator"/>. <br/>
        /// </summary>
        /// <param name="values"></param>
        public void Keyboard(params Keys[] values)
        {
            var simulator = new InputSimulator();
            foreach (var keys in values)
            {
                foreach (var key in keys.Values)
                {
                    simulator.Keyboard.KeyDown((VirtualKeyCode)key);
                }
                foreach (var key in keys.Values.Reverse())
                {
                    simulator.Keyboard.KeyUp((VirtualKeyCode)key);
                }
            }
        }

        #endregion
    }
}
