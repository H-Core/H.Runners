using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Runners.Extensions;
using H.Runners.Utilities;
using Process = System.Diagnostics.Process;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultRunner : Runner
    {
        #region Properties

        private string? UserName { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public DefaultRunner()
        {
            Add(new SyncAction("notify", Notify));
            Add(new AsyncAction("start", async (command, cancellationToken) =>
            {
                var path = command.Input.Arguments.ElementAt(0);
                var arguments = command.Input.Arguments.ElementAtOrDefault(0) ?? string.Empty;

                await RunProcessAsync(path, arguments, cancellationToken).ConfigureAwait(false);

                return Value.Empty;
            }));
            //AddAsyncAction("say", sayFunc, "text");
            //AddInternalAction("print", printAction, "text");
            //AddInternalAction("warning", warningAction, "text");
            //AddInternalAction("run", command => Run(command ?? string.Empty), "other_command_key");
            //AddAction("search", async key => printAction(string.Join(Environment.NewLine, await searchFunc(key))), "key");

            //AddAsyncAction("sleep", SleepCommand, "integer");
            //AddAction("sync-sleep", command => Thread.Sleep(int.TryParse(command, out var result) ? result : 1000), "integer");

            //AddAction("say-my-name", async _ =>
            //{
            //    if (string.IsNullOrWhiteSpace(UserName))
            //    {
            //        await SayAsync("Я еще не знаю вашего имени. Пожалуйста, представьтесь");

            //        var name = await WaitNextCommand(8000);
            //        if (name == null || string.IsNullOrWhiteSpace(name))
            //        {
            //            return;
            //        }

            //        // First char to upper case
            //        name = name[0].ToString().ToUpper() + name.Substring(1);

            //        Settings.Set("username", name);
            //        SaveSettings();
            //    }

            //    Say($"Привет {UserName}");
            //});
            AddSetting("username", o => UserName = o, NoEmpty, string.Empty);

            AddVariable("$username$", () => UserName ?? string.Empty);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arguments"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RunProcessAsync(string path, string? arguments = null, CancellationToken cancellationToken = default)
        {
            path = path ?? throw new ArgumentNullException(nameof(path));

            using var process = Process.Start(new ProcessStartInfo(path, arguments ?? string.Empty)
            {
                UseShellExecute = true,
            }) ?? throw new InvalidOperationException("Process is null.");

            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Notify()
        {
            using var stream = ResourcesUtilities.ReadFileAsStream("beep.wav");
            using var player = new SoundPlayer(stream);

            player.Play();
        }

        #endregion
    }
}
