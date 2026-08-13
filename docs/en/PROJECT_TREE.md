# VBImGuiDx9 Project Tree

> Version: 2.0  
> Status: Current repository structure

## Solution

```text
VBImGuiDx9/
├── Backends/
│   └── Direct3D9/
│       ├── Dx9GraphicsContext.vb
│       ├── Dx9GraphicsDevice.vb
│       ├── Dx9ImGuiRenderer.vb
│       ├── Dx9IndexBuffer.vb
│       ├── Dx9Texture.vb
│       └── Dx9VertexBuffer.vb
│
├── Contracts/
│   ├── IBlendState.vb
│   ├── IBuffer.vb
│   ├── IDepthStencilState.vb
│   ├── IGraphicsContext.vb
│   ├── IGraphicsDevice.vb
│   ├── IGraphicsResource.vb
│   ├── IIndexBuffer.vb
│   ├── ILogger.vb
│   ├── IRasterizerState.vb
│   ├── IRenderTarget.vb
│   ├── ISamplerState.vb
│   ├── ITexture.vb
│   └── IVertexBuffer.vb
│
├── Core/
│   ├── DeviceOptions.vb
│   ├── FrameStatistics.vb
│   ├── ImGuiContextManager.vb
│   ├── ImGuiFrameController.vb
│   ├── ImGuiWindowManager.vb
│   ├── ImGuiWindowState.vb
│   ├── RenderContext.vb
│   ├── Renderer.vb
│   ├── RendererOptions.vb
│   └── VersionInfo.vb
│
├── Diagnostics/
│   └── FrameProfiler.vb
│
├── docs/
│   ├── en/
│   └── ru/
│
├── Sample/
│   └── Assets/
│       └── Fonts/
│           ├── Inter.ttf
│           ├── Roboto.ttf
│           └── Segoe UI.ttf
│
├── VBImGuiDx9.Native/
│   └── VBImGuiDx9.Native.csproj
│
├── VBImGuiDx9.Sample/
│   ├── UI/
│   ├── MainForm.vb
│   ├── Program.vb
│   └── WinAPI.vb
│
├── Directory.Build.props
├── LICENSE
├── Logger.vb
├── README.md
├── VBImGuiDx9.slnx
└── VBImGuiDx9.vbproj
```

## Library

`VBImGuiDx9` is the main library.

It contains Core, Contracts, Diagnostics, and the Direct3D9 backend.

## Contracts

Contracts contains graphics abstractions and must not depend on the concrete Direct3D9 implementation.

## Core

Core contains high-level logic:

- ImGui context;
- frame lifecycle;
- window management;
- renderer orchestration;
- options;
- statistics.

## Backends

The backend contains the concrete graphics API implementation.

Текущий backend:

```text
Direct3D9
```

## Native

`VBImGuiDx9.Native` is a separate C# helper project. Its assembly is included in the main NuGet package.

## Sample

`VBImGuiDx9.Sample` is a separate WinForms demonstration application.

The Sample is not part of the main runtime library.

## Documentation

Documentation is maintained in two language versions:

```text
docs/
├── en/
└── ru/
```

## Dependency direction

```text
Application / Sample
        ↓
      Core
        ↓
    Contracts
        ↓
Direct3D9 Backend
        ↓
Vortice.Direct3D9
```

Concrete backend dependencies must not leak into Core without an architectural reason.

## Repository hygiene

The following directories and files must not be committed to Git:

```text
.vs/
bin/
obj/
*.user
*.suo
*.slnLaunch.user
```

Build artifacts, IDE state, and temporary files are excluded through `.gitignore`.
