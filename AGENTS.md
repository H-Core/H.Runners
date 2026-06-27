# H.Runners Agent Notes

## Project Role

- This repo contains executable action modules for H-Core/HomeCenter.
- Runners can be triggered by events such as a global key combination, voice command, notifier, or service integration.
- For career/portfolio analysis, this repo is strong evidence of extensible command execution: process launch, keyboard automation, clipboard actions, screenshots, OCR, C# scripting, Telegram, Spotify, torrent workflows, sound playback, selection UI, and interop.

## Layout

- `H.Runners.sln` is the solution entry point.
- `src/libs/H.Runners.CommonRunners` contains common actions such as internet, process, sequence, alias, and user runners.
- Specialized runners live under `src/libs/H.Runners.*Runner`, including clipboard, keyboard, CSharp, OCR, Telegram, interop, screenshot, select, Spotify, sound, torrent, and DLink runners.
- Integration tests live under `src/tests/H.Runners.*.IntegrationTests`.
- `docs/nuget_icon.png` and package metadata support NuGet packaging.

## Build And Test

- Historical CI used Windows with .NET 5.
- From the repo root, the intended validation path is:

```bash
dotnet test H.Runners.sln --configuration Release --nologo
```

- Full validation is Windows-oriented because several projects use WPF, Windows Forms, screenshots, clipboard, global input simulation, sound, UI selection, and `net4.x` targets.
- Some integration tests may depend on network services, desktop session state, clipboard state, local files, Telegram credentials, Spotify auth, torrent/web responses, or audio/UI availability.

## Maintenance Notes

- `src/Directory.Build.props` enables `LangVersion=preview` and nullable reference types.
- Library package metadata and versions live under `src/libs/Directory.Build.props`.
- Many runners are separate NuGet packages with suffix versions. Keep package IDs and public API names stable unless doing a coordinated release.
- Avoid broad dependency upgrades or behavior rewrites without checking downstream consumers in `H.Services` and `HomeCenter`.
- Do not commit auth tokens, chat IDs, local media, private screenshots, HAR files with live credentials, or generated packages unless intentionally updating release artifacts.
