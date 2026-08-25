# Repository Guidelines

## Project Structure & Module Organization

FolderLens is a Windows WPF application targeting .NET 8. Source files live at the repository root:

- `MainWindow.xaml` and `MainWindow.xaml.cs`: floating search palette, tray integration, hotkey, and interactions.
- `SettingsWindow.xaml(.cs)`: folder and startup configuration UI.
- `FolderIndexService.cs`: background folder indexing and lazy image previews.
- `IndexCacheStore.cs` and `SettingsStore.cs`: local persistence under `%APPDATA%\FolderLens`.
- `FolderLens.ico`: native application and tray icon resource.
- `Models.cs`, `App.xaml(.cs)`, and `FolderLens.csproj`: shared models, application startup, and project configuration.

`bin/`, `obj/`, `publish/`, and `.dotnet-cli/` are generated or local-only directories. There is currently no test directory.

## Build, Test, and Development Commands

```powershell
dotnet restore
dotnet build FolderLens.csproj --configuration Release
dotnet run
dotnet publish FolderLens.csproj --configuration Release --runtime win-x64 --self-contained false --property:PublishSingleFile=true --output publish
```

Use `dotnet run` for local development. The published executable requires the .NET 8 Windows Desktop runtime.

## Coding Style & Naming Conventions

Use four-space indentation, nullable reference types, file-scoped namespaces, and focused classes. Follow C# conventions: PascalCase for public types and methods, camelCase for locals, and `_camelCase` for private fields. Keep XAML event names and `x:Name` values descriptive and consistent. Avoid new third-party dependencies unless necessary; preserve lazy thumbnail loading and background indexing to keep resource usage low.

## Testing Guidelines

No automated test project or coverage requirement exists yet. Before submitting changes, build in Release and manually verify: `Alt + Space`, search filtering, single-click folder opening, hover previews, settings persistence, tray behavior, and startup with Windows.

## Commit & Pull Request Guidelines

No Git history is available in this checkout, so no existing convention can be inferred. Use concise imperative messages, preferably Conventional Commit style, such as `fix: open folders on single click`. PRs should describe behavior changes, include build/test results, and attach screenshots or a short recording for UI changes.

## Configuration & Security

User settings and the folder index are stored locally in `%APPDATA%\FolderLens`; never commit those files or real user paths. Keep indexing limited to configured roots and handle inaccessible folders without crashing.
