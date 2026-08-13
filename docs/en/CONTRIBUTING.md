# Contributing

Thank you for your interest in VBImGuiDx9.

VBImGuiDx9 is developed as a VB.NET library for building interfaces with Dear ImGui, ImGui.NET, and Direct3D9.

Before making changes, review:

- `README.md`
- `docs/ARCHITECTURE.md`
- `docs/GETTING_STARTED.md`
- `docs/FONTS.md`
- `docs/RENDERING.md`

## Project Structure

```text
VBImGuiDx9/
├── Backends/
│   └── Direct3D9/
├── Contracts/
├── Core/
├── Diagnostics/
├── docs/
├── Sample/
├── VBImGuiDx9.Native/
├── VBImGuiDx9.Sample/
├── Directory.Build.props
├── LICENSE
├── README.md
├── VBImGuiDx9.slnx
└── VBImGuiDx9.vbproj
```

### Core

Contains code related to the ImGui lifecycle and UI management.

Core changes should not depend on the concrete Direct3D9 implementation unless that dependency is part of the existing API.

### Contracts

Contains graphics abstractions and should remain as independent of a concrete backend as possible.

### Backends

Contains concrete graphics API implementations.

Current backend:

```text
Direct3D9
```

### Native

`VBImGuiDx9.Native` содержит helper functionality, необходимую Direct3D9 backend. В NuGet он поставляется как часть основной библиотеки.

### Sample

The Sample is a demonstration application used for API demonstration, change verification, visual testing, and usage examples.

Sample code must not become a mandatory dependency of the main library.

## Development Environment

The project uses:

- Windows;
- .NET 9;
- Visual Studio;
- VB.NET;
- ImGui.NET;
- Vortice.Direct3D9.

New code must preserve:

```text
Option Strict On
Option Explicit On
Option Infer On
```

## Style

Use the existing project style. Предпочтительно:

```vb
Dim value As Integer =
    CalculateValue()
```

Public classes and methods should have clear names and XML documentation.

## Error Handling

State errors must be reported explicitly. If a required resource is missing and the operation cannot proceed, do not silently continue.

For expected transient states, such as a temporarily invalid Direct3D9 display size, it is acceptable to safely skip the current frame.

## Resource Ownership

Each component must have clear ownership of the resources it manages.

Например:

```text
ImGuiContextManager
    └── ImGui context

Dx9ImGuiRenderer
    ├── Font texture
    ├── Vertex buffer
    └── Index buffer

Dx9GraphicsDevice
    └── Direct3D9 device
```

A component releases only resources it owns.

## IDisposable

Классы, владеющие unmanaged/native resources, должны корректно реализовывать `IDisposable`.

Lifecycle:

```text
Create
  ↓
Initialize
  ↓
Use
  ↓
Dispose
```

After `Dispose()`, the object must not be used.

## ImGui Context and Frame Lifecycle

UI must be built only inside an active ImGui frame:

```text
BeginFrame
    ↓
Build UI
    ↓
EndFrame
    ↓
Render DrawData
```

## Direct3D9

When changing the backend, consider:

- device lost;
- device reset;
- resource lifetime;
- dynamic buffers;
- render state;
- scissor state;
- texture binding.

Rendering pipeline changes must be visually verified through the Sample.

## Fonts

When changing FontService, verify:

- загрузку нескольких TTF;
- размеры 13/16/22 px;
- Latin;
- Cyrillic;
- Font Atlas;
- DX9 font texture.

Before adding a TTF to the repository, verify its license and redistribution rights.

## UI changes

Minimum verification:

```text
Application starts
    ↓
ImGui initializes
    ↓
Windows appear
    ↓
Controls respond
    ↓
Fonts render correctly
    ↓
Resize works
    ↓
Minimize / restore works
    ↓
Application closes cleanly
```

## Build verification

Run a full build before submitting changes.

Minimum requirement:

```text
0 errors
```

Warnings are evaluated separately.

A change is not considered complete merely because the modified file compiles by itself.

## Regression testing

After changing Core or Backend, verify at least:

- запуск Sample;
- основные окна;
- buttons;
- checkbox;
- sliders;
- combo;
- tabs;
- fonts;
- Cyrillic text;
- resize;
- minimize/restore;
- закрытие приложения.

Pay particular attention to Direct3D9 reset after renderer changes.

## Pull Requests

A Pull Request should contain:

- a short description;
- the reason for the change;
- validation information;
- known limitations, if any.

## Commit messages

Примеры:

```text
Add font service
Fix DX9 font texture upload
Improve window manager
Add rendering documentation
Prepare NuGet packaging
```

Не рекомендуется:

```text
fix
changes
update
test
stuff
```

## Breaking changes

A public API change is considered a potential breaking change.

Before removing or renaming a public type, check the Sample and the other projects.

Breaking changes must be explicitly noted in the release notes.

## Dependencies

Before adding a dependency, evaluate:

- необходимость;
- размер;
- лицензию;
- совместимость с .NET 9;
- влияние на NuGet package;
- наличие уже существующей функциональности.

## Documentation

Public API changes must be accompanied by documentation updates.

Documentation must describe implemented behavior, not plans.

## Release Process

```text
Build
  ↓
Sample
  ↓
Documentation
  ↓
Package
  ↓
NuGet validation
  ↓
GitHub release
```

The NuGet package must contain only required library assemblies, dependencies, and metadata. The Sample remains in the repository but must not become part of the runtime package.

## License

Before release, consider the project license and the licenses of:

- ImGui;
- ImGui.NET;
- Vortice;
- других dependencies;
- распространяемых TTF.

## Final checklist

Before merge:

- [ ] Build succeeds
- [ ] 0 errors
- [ ] No accidental debug files
- [ ] Sample starts
- [ ] Main UI works
- [ ] Fonts work
- [ ] Cyrillic works where supported by TTF
- [ ] Resize works
- [ ] Device reset path checked
- [ ] Resources disposed correctly
- [ ] Documentation updated
- [ ] Public API reviewed
- [ ] Dependencies reviewed

Before release:

- [ ] Version updated
- [ ] NuGet metadata checked
- [ ] README checked
- [ ] LICENSE checked
- [ ] Dependency licenses checked
- [ ] Package contents checked
- [ ] Sample excluded from runtime package
- [ ] NuGet package tested
- [ ] GitHub release prepared
