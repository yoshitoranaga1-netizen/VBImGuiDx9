# Changelog

All notable changes to VBImGuiDx9 are documented in this file.

The project follows Semantic Versioning where applicable.

---

# [Unreleased]

Preparation of the project for public release.

## Changed

### Project Structure

- Updated the solution structure.
- Added the separate `VBImGuiDx9.Native` helper project.
- Moved `VBImGuiDx9.Sample` into a separate project.
- Refined the project documentation structure.
- Documentation is being maintained in both Russian and English.
- Removed `BLUEPRINT.md` as a separate project document.

### Packaging

- Configured NuGet package generation for the library.
- Configured `.snupkg` symbols package generation.
- Included `VBImGuiDx9.Native.dll` in the main package.
- Included `README.md` in the package.
- Configured MIT license metadata.

### Validation

- Verified the solution builds in `Release`.
- Verified the NuGet package structure.
- Verified the presence of:
  - `VBImGuiDx9.dll`;
  - `VBImGuiDx9.Native.dll`;
  - XML documentation;
  - `README.md`.

---

# [0.1.0] - 2026-08-08

## Added

### Project Foundation

- Created the VBImGuiDx9 solution.
- Added the main `VBImGuiDx9` library project.
- Added the `VBImGuiDx9.Sample` project.
- Established the initial project architecture.
- Established project development conventions.
- Added shared build configuration.
- Added `.editorconfig`.
- Enabled XML documentation generation.
- Enabled strict Visual Basic compiler options.
- Enabled .NET analyzers.

### Documentation

Added the initial project documentation:

- `README.md`
- `PROJECT_TREE.md`
- `ROADMAP.md`
- `CODING_STANDARD.md`
- `CONTRIBUTING.md`
- `API.md`
- `CHANGELOG.md`

### Contracts

Established the public graphics contracts.

Added:

- `IGraphicsDevice`
- `IGraphicsContext`
- `IGraphicsResource`
- `IBuffer`
- `IVertexBuffer`
- `IIndexBuffer`
- `ITexture`
- `IRenderTarget`
- `IBlendState`
- `IRasterizerState`
- `IDepthStencilState`
- `ISamplerState`
- `ILogger`

The responsibilities of the graphics device and graphics context are separated.

`IGraphicsDevice` is responsible for:

- device information;
- graphics context creation;
- vertex buffer creation;
- index buffer creation;
- texture creation.

`IGraphicsContext` is responsible for:

- beginning a frame;
- ending a frame;
- presenting a frame;
- clearing the current render target.

### Core

Added the initial Core layer.

Added:

- `VersionInfo`
- `DeviceOptions`
- `RendererOptions`
- `FrameStatistics`
- `RenderContext`
- `Renderer`

### Renderer

Implemented the initial renderer lifecycle:

```text
BeginFrame
    ↓
Clear
    ↓
rendering
    ↓
EndFrame
    ↓
Present
```

### Direct3D9 Backend

Added the Direct3D9 backend implementation.

Included:

- `Dx9GraphicsDevice`
- `Dx9GraphicsContext`
- `Dx9ImGuiRenderer`
- `Dx9VertexBuffer`
- `Dx9IndexBuffer`
- `Dx9Texture`

### Native Helper

Added the `VBImGuiDx9.Native` project for native/helper functionality used by the Direct3D9 backend.

### Sample

Added the `VBImGuiDx9.Sample` demonstration application.

The Sample demonstrates:

- ImGui controls;
- windows;
- settings;
- diagnostics;
- fonts;
- Direct3D9 rendering.

### Fonts

Added TTF font handling to the Sample.

Font discovery from `Assets/Fonts` is supported.

### Build

- Library target framework: `.NET 9`.
- Sample target framework: `net9.0-windows`.
- XML documentation generation is enabled.
- Native helper project build is configured.

---

# Versioning

Project versions follow Semantic Versioning where applicable:

```text
MAJOR.MINOR.PATCH
```

Changes currently under development are documented under `[Unreleased]`.
