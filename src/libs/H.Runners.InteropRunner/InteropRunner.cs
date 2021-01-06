using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using H.Core.Runners;
using H.Runners.Utilities;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InteropRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public InteropRunner()
        {
            Add(new SyncAction("show-window", ShowWindow, "Arguments: process_name"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void ShowWindow(string name)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));

            var processes = Process
                .GetProcessesByName(name)
                .Where(i => !string.IsNullOrWhiteSpace(i.MainWindowTitle));

            var process = processes.FirstOrDefault();
            if (process == null)
            {
                throw new InvalidOperationException($@"Process: {name} is not found");
            }

            var ptr = new HandleRef(null, process.MainWindowHandle);
            const int swShow = 5;
            User32Methods.ShowWindow(ptr, swShow);
            User32Methods.SetForegroundWindow(ptr);
            User32Methods.SetFocus(ptr);
        }

        #endregion
    }
}
