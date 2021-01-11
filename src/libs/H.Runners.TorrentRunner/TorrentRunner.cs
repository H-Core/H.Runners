using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using H.Core.Settings;
using H.Runners.Extensions;
using H.Runners.Utilities;
using HtmlAgilityPack;
using MonoTorrent;
using Process = System.Diagnostics.Process;

namespace H.Runners
{
    /// <summary>
    ///
    /// </summary>
    public class TorrentRunner : Runner
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string SaveTo { get; set; } = Path.Combine(Path.GetTempPath(), "H.Runners.TorrentRunner");
        
        /// <summary>
        /// 
        /// </summary>
        public string QBitTorrentPath { get; set; } = @"C:\Program Files\qBittorrent\qbittorrent.exe";

        /// <summary>
        /// 
        /// </summary>
        public string MpcPath { get; set; } = @"C:\Program Files (x86)\K-Lite Codec Pack\MPC-HC64\mpc-hc64.exe";

        /// <summary>
        /// 
        /// </summary>
        public int MaxDelaySeconds { get; set; } = 60;
        
        /// <summary>
        /// 
        /// </summary>
        public string SearchPattern { get; set; } = "download torrent *";
        
        /// <summary>
        /// 
        /// </summary>
        public double MinSizeGb { get; set; } = 1.0;
        
        /// <summary>
        /// 
        /// </summary>
        public double MaxSizeGb { get; set; } = 4.0;
        
        /// <summary>
        /// 
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        public double StartSizeMb { get; set; } = 20.0;
        
        /// <summary>
        /// 
        /// </summary>
        public int MaxSearchResults { get; set; } = 3;

        private string TorrentsFolder => Directory.CreateDirectory(Path.Combine(SaveTo, "Torrents")).FullName;
        private string DownloadsFolder => Directory.CreateDirectory(Path.Combine(SaveTo, "Downloads")).FullName;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public TorrentRunner()
        {
            AddSetting(nameof(SaveTo), o => SaveTo = o, NoEmpty, SaveTo, SettingType.Folder);
            AddSetting(nameof(QBitTorrentPath), o => QBitTorrentPath = o, FileExists, QBitTorrentPath, SettingType.Path);
            AddSetting(nameof(MpcPath), o => MpcPath = o, FileExists, MpcPath, SettingType.Path);
            AddSetting(nameof(MaxDelaySeconds), o => MaxDelaySeconds = o, Any, MaxDelaySeconds);
            AddSetting(nameof(SearchPattern), o => SearchPattern = o, NoEmpty, SearchPattern);
            AddSetting(nameof(MinSizeGb), o => MinSizeGb = o, Any, MinSizeGb);
            AddSetting(nameof(MaxSizeGb), o => MaxSizeGb = o, Any, MaxSizeGb);
            AddSetting(nameof(Extension), o => Extension = o, Any, Extension);
            AddSetting(nameof(StartSizeMb), o => StartSizeMb = o, Any, StartSizeMb);
            AddSetting(nameof(MaxSearchResults), o => MaxSearchResults = o, Any, MaxSearchResults);

            Add(AsyncAction.WithSingleArgument("torrent", TorrentAsync, "name"));

            Settings.PropertyChanged += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(SaveTo))
                {
                    return;
                }

                Directory.CreateDirectory(TorrentsFolder);
                Directory.CreateDirectory(DownloadsFolder);
            };
        }

        private static bool FileExists(string key) => NoEmpty(key) && File.Exists(key);

        #endregion

        #region Private methods

        private static async Task<string[]> GetTorrentsFromUrlAsync(
            string url, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var web = new HtmlWeb();
                var document = await web.LoadFromWebAsync(url, cancellationToken).ConfigureAwait(false);

                var torrents = document.DocumentNode
                    .SelectNodes("//a[@href]")
                    .Select(i => i.Attributes["href"].Value)
                    .Where(i => i.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var uri = new Uri(url);
                var baseUrl = $"{uri.Scheme}://{uri.Host}";
                
                return torrents
                    .Select(i => i.Contains("http") ? i : $"{baseUrl}{i}")
                    .ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        private bool IsGoodFile(TorrentFile file)
        {
            var sizeInGigabytes = file.Length / 1_000_000_000.0;
            var extension = Path.GetExtension(file.Path);
            if (sizeInGigabytes > MinSizeGb &&
                sizeInGigabytes < MaxSizeGb &&
                (string.IsNullOrWhiteSpace(Extension) ||
                 string.Equals(extension, Extension, StringComparison.OrdinalIgnoreCase)))
            {
                //Print($"Size: {sizeGb:F2} Gb");
                //Print($"Path: {path}");
                //Print($"Extension: {extension}");

                return true;
            }

            return false;
        }

        private static async Task<string[]> DownloadFiles(IEnumerable<string> urls)
        {
            return await Task.WhenAll(urls.Select(async (url, i) =>
            {
                var temp = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "TorrentRunnerFiles")).FullName;
                var path = Path.Combine(temp, $"file_{i}");

                using var client = new WebClient();
                try
                {
                    await client.DownloadFileTaskAsync(url, path).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // ignored
                }

                return path;
            })).ConfigureAwait(false);
        }

        private static async Task<string[]> GetTorrents(IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            var values = await Task.WhenAll(
                urls.Select(url => GetTorrentsFromUrlAsync(url, cancellationToken)))
                .ConfigureAwait(false);

            return values
                .SelectMany(value => value)
                .ToArray();
        }

        private string? FindBestTorrent(IEnumerable<string> files)
        {
            var torrents = files
                .Select(path => Torrent.TryLoad(path, out var torrent) ? (path, torrent) : default)
                .Where(tuple => tuple != default)
                .ToDictionary(tuple => tuple.path, tuple => tuple.torrent);

            var goodTorrents = torrents
                .Where(torrent => torrent.Value.Files.Length == 1 && torrent.Value.Files.Any(IsGoodFile))
                .OrderByDescending(torrent => torrent.Value.AnnounceUrls.Count)
                .ToArray();

            if (!goodTorrents.Any())
            {
                goodTorrents = torrents
                    .Where(torrent => torrent.Value.Files?.Any(IsGoodFile) == true)
                    .OrderByDescending(torrent => torrent.Value.AnnounceUrls.Count)
                    .ToArray();
            }

            var bestTorrent = goodTorrents.FirstOrDefault();

            return bestTorrent.Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task TorrentAsync(string text, CancellationToken cancellationToken = default)
        {
            this.Say($"Ищу торрент {text}");

            var query = SearchPattern.Replace("*", text);
            var urls = await this.SearchAsync(query, cancellationToken).ConfigureAwait(false); //, MaxSearchResults
            if (!urls.Any())
            {
                await this.SayAsync("Поиск в гугле не дал результатов", cancellationToken).ConfigureAwait(false);
                return;
            }

            var torrents = await GetTorrents(urls, cancellationToken).ConfigureAwait(false);
            this.Print($"Torrents({torrents.Length})");

            var files = await DownloadFiles(torrents).ConfigureAwait(false);
            this.Print($"Files({torrents.Length})");

            var path = FindBestTorrent(files);
            if (path == null)
            {
                await this.SayAsync("Не найден подходящий торрент", cancellationToken).ConfigureAwait(false);
                return;
            }

            await QTorrentCommandAsync(path, cancellationToken).ConfigureAwait(false);
        }

        private async Task QTorrentCommandAsync(string torrentPath, CancellationToken cancellationToken = default)
        {
            var path = GetFilePath(torrentPath);
            //if (RunCommand(path))
            //{
            //    return;
            //}

            try
            {
                var temp = Path.GetTempFileName();
                File.Copy(torrentPath, temp, true);

                Process.Start(new ProcessStartInfo(
                    QBitTorrentPath,
                    $"--sequential --first-and-last --skip-dialog=true --save-path=\"{DownloadsFolder}\" {temp}")
                    {
                        UseShellExecute = true,
                    });

                this.Say(@"Загружаю. Запущу, когда загрузится базовая часть");
            }
            catch (Exception exception)
            {
                this.Say(@"Ошибка загрузки");
                OnExceptionOccurred(exception);
                return;
            }

            await WaitDownloadCommandAsync(path, StartSizeMb, cancellationToken).ConfigureAwait(false);
            
            await RunFileAsync(path, cancellationToken).ConfigureAwait(false);
        }

        private async Task WaitDownloadCommandAsync(
            string path, 
            double requiredSizeMb, 
            CancellationToken cancellationToken = default)
        {
            var seconds = 0;
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);

                var size = FileUtilities.GetFileSizeOnDisk(path);
                var requiredSize = requiredSizeMb * 1_000_000;
                var percents = 100.0 * size / requiredSize;

                // Every 5 seconds
                if (seconds % 5 == 0)
                {
                    this.Print($"Progress: {size}/{requiredSize}({percents:F2}%)");
                }

                if (size < uint.MaxValue - 1 &&
                    size > requiredSize)
                {
                    break;
                }

                ++seconds;
            }
        }

        private async Task RunFileAsync(string path, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(path))
            {
                throw new InvalidOperationException($"File is not exists: {path}");
            }

            var fileName = File.Exists(MpcPath) ? MpcPath : path;
            var arguments = File.Exists(MpcPath) ? $"/fullscreen \"{path}\"" : string.Empty;
            using var process = Process.Start(new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = true,
            });
            if (process == null)
            {
                throw new InvalidOperationException("Process is null");
            }

            this.Say(@"Запускаю");
            
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }

        private string GetFilePath(string torrentPath)
        {
            var torrent = Torrent.Load(torrentPath);
            var subPath = torrent.Files.FirstOrDefault(IsGoodFile)?.FullPath;
            var path = Path.Combine(DownloadsFolder, subPath ?? string.Empty);

            return path;
        }

        #endregion
    }
}
