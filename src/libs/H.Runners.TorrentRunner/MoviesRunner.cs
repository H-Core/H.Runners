using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Core.Settings;
using H.Runners.Utilities;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public class MoviesRunner : Runner
    {
        #region Properties

        private string Folder { get; set; } = string.Empty;
        private string Folder2 { get; set; } = string.Empty;
        private string Folder3 { get; set; } = string.Empty;
        private string MoviesExtensions { get; set; } = "avi;mkv;mp4";
        private int MaximumDistance { get; set; } = 2;
        private bool AutoTorrent { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public MoviesRunner()
        {
            AddSetting(nameof(Folder), o => Folder = o, NoEmpty, Folder, SettingType.Folder);
            AddSetting(nameof(Folder2), o => Folder2 = o, NoEmpty, Folder2, SettingType.Folder);
            AddSetting(nameof(Folder3), o => Folder3 = o, NoEmpty, Folder3, SettingType.Folder);
            AddSetting(nameof(MoviesExtensions), o => MoviesExtensions = o, NoEmpty, MoviesExtensions);
            AddSetting(nameof(MaximumDistance), o => MaximumDistance = o, Positive, MaximumDistance);
            AddSetting(nameof(AutoTorrent), o => AutoTorrent = o, Any, AutoTorrent);

            Add(AsyncAction.WithSingleArgument("find-movie", FindMovieCommandAsync, "name"));
        }

        #endregion

        #region Private methods

        private async Task FindMovieCommandAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(Folder) || 
                !Directory.Exists(Folder))
            {
                this.Say("Директория фильмов не найдена. Пожалуйста, укажите ее и попробуйте снова");

                this.ShowSettings();
                return;
            }

            this.Say($"Ищу фильм {name}");

            var files = GetFiles(Folder, Folder2, Folder3);
            if (!files.Any())
            {
                this.Say("Ничего не найдено");
                await CheckTorrentAsync(name, cancellationToken).ConfigureAwait(false);
                return;
            }

            var translitedGost = Transliterator.Convert(name, Transliterator.TranslateType.Gost);
            var translitedIso = Transliterator.Convert(name, Transliterator.TranslateType.Iso);

            var distances = new List<Tuple<int, string>>();
            foreach (var path in files)
            {
                distances.Add(GetDistance(path, name));
                distances.Add(GetDistance(path, translitedGost));
                distances.Add(GetDistance(path, translitedIso));
            }

            distances = distances.OrderBy(i => i.Item1).ToList();

            var minimumItem = distances.FirstOrDefault();
            var minimumDistance = minimumItem?.Item1 ?? int.MaxValue;
            if (minimumDistance > MaximumDistance)
            {
                this.Print($"Ближайшее совпадение: дистанция {minimumDistance} и строка {minimumItem?.Item2}");

                this.Say("Ничего подходящего не найдено");
                await CheckTorrentAsync(name, cancellationToken).ConfigureAwait(false);
                return;
            }

            var goodDistances = distances.Where(i => Math.Abs(i.Item1 - minimumDistance) <= 2).Distinct().ToList();
            if (goodDistances.Count == 1)
            {
                StartMovie(goodDistances[0].Item2);
                return;
            }

            foreach (var distance in goodDistances)
            {
                this.Print($"File {distance.Item2}. Distance: {distance.Item1}");
            }

            StartMovie(goodDistances[0].Item2);
        }

        private void StartMovie(string path)
        {
            this.Say("Нашла. Запускаю");
            Run($"explorer {path}");
        }

        private async Task CheckTorrentAsync(string text, CancellationToken cancellationToken = default)
        {
            if (!AutoTorrent && !await this.WaitAccept(
                "Скачать с торрента?",
                TimeSpan.FromSeconds(3), 
                cancellationToken, 
                "скачай", "скачать")
                .ConfigureAwait(false))
            {
                return;
            }

            await RunAsync(new Command("torrent", text), cancellationToken).ConfigureAwait(false);
        }

        private static Tuple<int, string> GetDistance(string path, string text)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (name == null)
            {
                return new Tuple<int, string>(int.MaxValue, path);
            }

            var distance = text.Length < name.Length
                ? TextUtilities.MinimalLevenshteinDistance(name, text)
                : TextUtilities.LevenshteinDistance(name, text);

            return new Tuple<int, string>(distance, path);
        }

        private bool IsGoodExtension(string path) => MoviesExtensions
            .Split(';')
            .Any(extension => string.Equals(Path.GetExtension(path), $".{extension}", StringComparison.OrdinalIgnoreCase));

        private List<string> GetFiles(params string[] folders) => folders
            .Where(folder => !string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
            .SelectMany(folder => Directory
                .EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(IsGoodExtension))
            .ToList();

        #endregion
    }
}
