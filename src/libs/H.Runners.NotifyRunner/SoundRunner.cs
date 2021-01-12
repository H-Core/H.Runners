using System.Media;
using H.Core.Runners;
using H.Runners.Utilities;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SoundRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public SoundRunner()
        {
            Add(new SyncAction("sound", Sound));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        public void Sound(string name)
        {
            using var stream = ResourcesUtilities.ReadFileAsStream($"{name}.wav");
            using var player = new SoundPlayer(stream);

            player.Play();
        }

        #endregion
    }
}
