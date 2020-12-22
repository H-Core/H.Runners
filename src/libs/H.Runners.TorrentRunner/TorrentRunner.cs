﻿using System;
using System.Collections.Generic;
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
using MonoTorrent.Common;
using Process = System.Diagnostics.Process;

namespace H.Runners
{
    /// <summary>
    ///
    /// </summary>
    public class TorrentRunner : Runner
    {
        #region Properties

        private string SaveTo { get; set; } = Path.Combine(Path.GetTempPath(), "H.Runners.TorrentRunner");
        private string QBitTorrentPath { get; set; } = @"C:\Program Files\qBittorrent\qbittorrent.exe";
        private string MpcPath { get; set; } = @"C:\Program Files (x86)\K-Lite Codec Pack\MPC-HC64\mpc-hc64_nvo.exe";
        private int MaxDelaySeconds { get; set; } = 60;
        private string SearchPattern { get; set; } = "download torrent *";
        private double MinSizeGb { get; set; } = 1.0;
        private double MaxSizeGb { get; set; } = 4.0;
        private string Extension { get; set; } = string.Empty;
        private double StartSizeMb { get; set; } = 20.0;
        private int MaxSearchResults { get; set; } = 3;

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
                var document = await web.LoadFromWebAsync(url, cancellationToken);

                var torrents = document.DocumentNode
                    .SelectNodes("//a[@href]")
                    .Select(i => i.Attributes["href"].Value)
                    .Where(i => i.EndsWith(".torrent"))
                    .ToList();

                var uri = new Uri(url);
                var baseUrl = $"{uri.Scheme}://{uri.Host}";
                return torrents
                    .Select(i => i.Contains("http") ? i : $"{baseUrl}{i}")
                    .ToArray();
            }
            catch (Exception)
            {
                //Log(exception.ToString());
                return new string[0];
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
                    await client.DownloadFileTaskAsync(url, path);
                }
                catch (Exception)
                {
                    // ignored
                }

                return path;
            }));
        }

        private static async Task<string[]> GetTorrents(IEnumerable<string> urls, CancellationToken cancellationToken = default)
        {
            var values = await Task.WhenAll(
                urls.Select(url => GetTorrentsFromUrlAsync(url, cancellationToken)));

            return values
                .SelectMany(value => value)
                .ToArray();
        }

        private string? FindBestTorrent(ICollection<string> files)
        {
            var torrents = files
                .Select(i => Torrent.TryLoad(i, out var torrent) ? torrent : null)
                .Where(i => i != null)
                .ToArray();

            var goodTorrents = torrents
                .Where(torrent => torrent?.Files.Length == 1 && torrent.Files.Any(IsGoodFile))
                .OrderByDescending(torrent => torrent?.AnnounceUrls.Count)
                .ToArray();

            if (!goodTorrents.Any())
            {
                goodTorrents = torrents
                    .Where(torrent => torrent?.Files?.Any(IsGoodFile) == true)
                    .OrderByDescending(torrent => torrent?.AnnounceUrls.Count)
                    .ToArray();
            }

            var bestTorrent = goodTorrents.FirstOrDefault();

            return bestTorrent?.TorrentPath;
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
            var urls = await this.SearchAsync(query, cancellationToken); //, MaxSearchResults
            if (!urls.Any())
            {
                await this.SayAsync("Поиск в гугле не дал результатов", cancellationToken);
                return;
            }

            var torrents = await GetTorrents(urls, cancellationToken);
            this.Print($"Torrents({torrents.Length})");

            var files = await DownloadFiles(torrents);
            this.Print($"Files({torrents.Length})");

            var path = FindBestTorrent(files);
            if (path == null)
            {
                await this.SayAsync("Не найден подходящий торрент", cancellationToken);
                return;
            }

            this.Say("Нашла!");
            await QTorrentCommand(path);
        }

        private async Task QTorrentCommand(string torrentPath)
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

                Process.Start(
                    QBitTorrentPath, 
                    $"--sequential --first-and-last --skip-dialog=true --save-path=\"{DownloadsFolder}\" {temp}");
                this.Say(@"Загружаю. Запущу, когда загрузится базовая часть");
            }
            catch (Exception exception)
            {
                this.Say(@"Ошибка загрузки");
                OnExceptionOccurred(exception);
                return;
            }

            await WaitDownloadCommandAsync(path, StartSizeMb, MaxDelaySeconds);
            
            await RunFileAsync(path);
        }

        private async Task WaitDownloadCommandAsync(
            string path, 
            double requiredSizeMb, 
            int maxDelaySeconds,
            CancellationToken cancellationToken = default)
        {
            var seconds = 0;
            while (seconds < maxDelaySeconds)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

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

            using var process = File.Exists(MpcPath)
                ? Process.Start(MpcPath, $"/fullscreen \"{path}\"")
                : Process.Start(path);
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

        /*

        private ClientEngine CreateEngine()
        {
            // Torrents will be downloaded here by default when they are registered with the engine
            // Tell the engine to listen at port 6969 for incoming connections
            // If both encrypted and unencrypted connections are supported, an encrypted connection will be attempted
            // first if this is true. Otherwise an unencrypted connection will be attempted first.
            return new ClientEngine(new EngineSettings(TorrentsFolder, 6969)
            {
                AllowedEncryption = EncryptionTypes.All,
                PreferEncryption = true
            });
        }

        private void TorrentCommand(string text)
        {
            var torrent = Torrent.Load(text);
            Print($"Files in torrent: {text}");
            foreach (var file in torrent.Files)
            {
                Print($"Path: {file.Path}");
                Print($"FullPath: {file.FullPath}");
            }

            Print($"Created by: {torrent.Files}");
            Print($"Creation date: {torrent.CreationDate}");
            Print($"Comment: {torrent.Comment}");
            Print($"Publish URL: {torrent.PublisherUrl}");
            Print($"Size: {torrent.Size}");

            /*
            Engine?.Dispose();
            Engine = CreateEngine();

            var torrent = Torrent.Load(text);
            Print($"Created by: {torrent.CreatedBy}");
            Print($"Creation date: {torrent.CreationDate}");
            Print($"Comment: {torrent.Comment}");
            Print($"Publish URL: {torrent.PublisherUrl}");
            Print($"Size: {torrent.Size}");

            var manager = new TorrentManager(torrent, DownloadsFolder, new TorrentSettings(10, 10), SaveTo);
            Engine.Register(manager);

            manager.TorrentStateChanged += (sender, args) => Print($"New state: {args.NewState:G}");

            //manager.Start();

            Managers.Add(manager);

            Engine.StartAll();

            /* Generate the paths to the folder we will save .torrent files to and where we download files to 
            main.basePath = SaveTo;						// This is the directory we are currently in
            main.torrentsPath = Path.Combine(main.basePath, "Torrents");				// This is the directory we will save .torrents to
            main.downloadsPath = Path.Combine(main.basePath, "Downloads");			// This is the directory we will save downloads to
            main.fastResumeFile = Path.Combine(main.torrentsPath, "fastresume.data");
            main.dhtNodeFile = Path.Combine(main.basePath, "DhtNodes");
            main.torrents = new List<TorrentManager>();							// This is where we will store the torrentmanagers
            main.listener = new Top10Listener(10);

            // We need to cleanup correctly when the user closes the window by using ctrl-c
            // or an unhandled exception happens
            //Console.CancelKeyPress += delegate { shutdown(); };
            //AppDomain.CurrentDomain.ProcessExit += delegate { shutdown(); };
            //AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { Print(e.ExceptionObject.ToString()); shutdown(); };
            //Thread.GetDomain().UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { Print(e.ExceptionObject.ToString()); shutdown(); };

            main.PrintAction = Log;
            main.StartEngine(31337);
            //main.Main(SaveTo, 6969, Log);
        }
        */
        #endregion
    }
}
