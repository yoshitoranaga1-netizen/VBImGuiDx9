# Дерево проекта VBImGuiDx9

> Версия: 2.0  
> Статус: Актуальная структура repository

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

`VBImGuiDx9` — основная библиотека.

Она содержит Core, Contracts, Diagnostics и Direct3D9 backend.

## Contracts

Contracts содержит только graphics abstractions и не должен зависеть от конкретной реализации Direct3D9.

## Core

Core содержит высокоуровневую логику:

- ImGui context;
- frame lifecycle;
- window management;
- renderer orchestration;
- options;
- statistics.

## Backends

Backend содержит конкретную реализацию graphics API.

Текущий backend:

```text
Direct3D9
```

## Native

`VBImGuiDx9.Native` — отдельный C# helper project. Его assembly включается в основной NuGet package.

## Sample

`VBImGuiDx9.Sample` — отдельное WinForms demonstration application.

Sample не является частью основной runtime-библиотеки.

## Documentation

Документация хранится в двух языковых версиях:

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

Конкретные backend dependencies не должны протекать в Core без архитектурной необходимости.

## Repository hygiene

Следующие каталоги и файлы не должны попадать в Git:

```text
.vs/
bin/
obj/
*.user
*.suo
*.slnLaunch.user
```

Build artifacts, IDE state и временные файлы исключаются через `.gitignore`.
