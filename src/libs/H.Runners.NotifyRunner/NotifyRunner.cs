using System.Media;
using H.Core.Runners;
using H.Runners.Utilities;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NotifyRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public NotifyRunner()
        {
            Add(new SyncAction("notify", Notify));
        }

        #endregion

        #region Private methods

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
