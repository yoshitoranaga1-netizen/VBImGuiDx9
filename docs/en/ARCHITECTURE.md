# Architecture

## 1. General Overview

VBImGuiDx9 is divided into several layers:

```text
┌───────────────────────────────────────────────┐
│                 Application                   │
│                 user code                     │
└──────────────────────┬────────────────────────┘
                       │
                       ▼
┌───────────────────────────────────────────────┐
│                   Sample                      │
│        UI examples and diagnostics             │
└──────────────────────┬────────────────────────┘
                       │
                       ▼
┌───────────────────────────────────────────────┐
│                    Core                       │
│                                               │
│  ImGuiContextManager                          │
│  ImGuiFrameController                         │
│  ImGuiWindowManager                           │
│  ImGuiWindowState                             │
└──────────────────────┬────────────────────────┘
                       │
                       ▼
┌───────────────────────────────────────────────┐
│                 Contracts                     │
│                                               │
│  IGraphicsDevice                              │
│  IGraphicsContext                             │
│  IGraphicsResource                            │
│  ITexture / IVertexBuffer / IIndexBuffer      │
│  ...                                          │
└──────────────────────┬────────────────────────┘
                       │
                       ▼
┌───────────────────────────────────────────────┐
│             Direct3D9 Backend                 │
│                                               │
│  Dx9GraphicsDevice                            │
│  Dx9GraphicsContext                           │
│  Dx9ImGuiRenderer                             │
│  Dx9Texture                                   │
│  Dx9VertexBuffer                              │
│  Dx9IndexBuffer                               │
└──────────────────────┬────────────────────────┘
                       │
                       ▼
┌───────────────────────────────────────────────┐
│                Vortice.Direct3D9              │
└───────────────────────────────────────────────┘
```

## 2. Core

### ImGuiContextManager

`ImGuiContextManager` owns the Dear ImGui context lifecycle.

Main responsibilities:

- create the ImGui context;
- set the current context;
- configure IO;
- set the initial style;
- build the initial Font Atlas;
- destroy the context in `Dispose()`.

The class intentionally contains no Direct3D9-specific logic. This separates the Dear ImGui lifecycle from the specific graphics backend.

### ImGuiFrameController

`ImGuiFrameController` is responsible for the correct ImGui frame lifecycle.

The cycle is:

```text
BeginFrame()
     │
     ▼
ImGui.NewFrame()
     │
     ▼
user UI
     │
     ▼
EndFrame()
     │
     ▼
ImGui.Render()
     │
     ▼
ImDrawData
```

`BeginFrame()` prevents starting a second active frame, sets the current ImGui context, and calls `ImGui.NewFrame()`.

`EndFrame()` completes the frame through `ImGui.Render()`. After that, DrawData becomes available to the backend.

Therefore, user UI code should not manage the transition:

```text
NewFrame → Render → GetDrawData
```

## 3. Window Management

### ImGuiWindowManager

The Window Manager separates window state from window content.

Each registered window has:

- Id
- Title
- Visible
- Position
- Size
- Collapsed
- Flags

The manager allows you to:

- register a window;
- get its state;
- change visibility;
- change the collapsed state;
- get the list of registered windows;
- invoke the renderer for each window.

During `RenderAll()`, the manager:

- skips invisible windows;
- sets the initial position;
- sets the initial size;
- calls `ImGui.Begin()`;
- invokes the window renderer;
- saves the new position;
- saves the new size;
- saves the collapsed state;
- handles window closing.

This allows user code to focus on the content:

```vb
Private Sub RenderSettings()

    ImGui.Text("Settings")

    ' Controls...

End Sub
```

rather than managing the window lifecycle itself.

## 4. Contracts

`Contracts` defines the abstractions of the graphics layer.

The main idea is:

```text
Core
  ↓
Contracts
  ↓
Backend
```

Core should not know which specific graphics API is used to render the interface.

For example:

```text
IGraphicsDevice
IGraphicsContext
ITexture
IVertexBuffer
IIndexBuffer
```

Direct3D9 implements these contracts through its own classes.

For example, `Dx9GraphicsDevice.CreateGraphicsContext()` returns `Dx9GraphicsContext` through the `IGraphicsContext` contract, while the methods for creating vertex/index buffers and textures implement the corresponding interfaces.

## 5. Direct3D9 Backend

Direct3D9 is the concrete implementation of the graphics layer.

### Dx9GraphicsDevice

Responsible for:

- creating Direct3D9;
- creating the device;
- managing device state;
- creating the graphics context;
- creating GPU resources;
- reset;
- releasing native resources.

For example:

```text
Dx9GraphicsDevice
       │
       ├── Dx9GraphicsContext
       ├── Dx9VertexBuffer
       ├── Dx9IndexBuffer
       └── Dx9Texture
```

Native Direct3D9 objects should not be used directly by user UI code.

## 6. Dx9GraphicsContext

The Graphics Context provides the operations required to perform rendering.

In particular, it is used by the ImGui renderer for:

- setting the projection;
- vertex buffer;
- index buffer;
- vertex format;
- blend state;
- depth/culling state;
- scissor state;
- texture;
- indexed draw.

This separates direct Direct3D9 state management from the logic that builds ImGui draw data.

## 7. Dx9ImGuiRenderer

This is the boundary between Dear ImGui and Direct3D9.

The renderer receives:

```text
ImDrawData
```

and then:

```text
ImDrawData
    │
    ├── Vertex data
    ├── Index data
    └── Draw commands
            │
            ▼
       DX9 buffers
            │
            ▼
        DrawIndexed
```

The renderer is responsible for:

- font texture;
- dynamic vertex buffer;
- dynamic index buffer;
- resizing buffers;
- copying ImGui vertex data;
- copying index data;
- projection;
- clipping/scissor;
- texture binding;
- indexed triangle rendering.

Therefore:

```text
ImGui
  ↓
ImDrawData
  ↓
Dx9ImGuiRenderer
  ↓
Direct3D9
```

## 8. Font Pipeline

Fonts use a separate pipeline:

```text
Assets/Fonts/*.ttf
        │
        ▼
    FontService
        │
        ├── 13 px
        ├── 16 px
        └── 22 px
        │
        ▼
   ImGui Font Atlas
        │
        ▼
   DX9 Font Texture
        │
        ▼
       GPU
```

`FontService` automatically discovers TTF files from the application directory.

File names are used as font names:

```text
Inter.ttf
    ↓
Inter

Roboto.ttf
    ↓
Roboto

Segoe UI.ttf
    ↓
Segoe UI
```

The library no longer assumes a special built-in `DemoFont`.

## 9. Application Lifecycle

Typical startup sequence:

```text
Application starts
        │
        ▼
Create ImGuiContextManager
        │
        ▼
Initialize ImGui
        │
        ▼
Create ImGuiFrameController
        │
        ▼
Create WindowManager
        │
        ▼
Create Direct3D9 Device
        │
        ▼
Create Direct3D9 Context
        │
        ▼
Create Dx9ImGuiRenderer
        │
        ▼
Initialize renderer
        │
        ▼
Start frame loop
```

In the current Sample, the ImGui context and frame controller are created first, followed by the Window Manager. The Direct3D9 device/context and ImGui renderer are then created.

## 10. Runtime Frame

A single application frame conceptually looks like this:

```text
┌──────────────────────────────┐
│ BeginFrame                   │
│                              │
│ ImGui.NewFrame()             │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│ Application UI               │
│                              │
│ WindowManager.RenderAll()    │
│                              │
│ ┌──────────────────────────┐ │
│ │ Main Window              │ │
│ ├──────────────────────────┤ │
│ │ Settings                 │ │
│ ├──────────────────────────┤ │
│ │ Debug                    │ │
│ └──────────────────────────┘ │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│ EndFrame                     │
│                              │
│ ImGui.Render()               │
└──────────────┬───────────────┘
               │
               ▼
        ImDrawData
               │
               ▼
┌──────────────────────────────┐
│ Dx9ImGuiRenderer             │
│                              │
│ Upload vertex/index data     │
│ Set render state             │
│ Set scissor                  │
│ Bind texture                 │
│ DrawIndexedTriangles         │
└──────────────┬───────────────┘
               │
               ▼
             GPU
```

## 11. Device Lost / Reset

Direct3D9 has a specific behavior where the device can enter the Lost state.

The backend provides the following diagnostics:

```text
Operational
DeviceLost
DeviceNotReset
DriverInternalError
Unknown
```

and `TryReset()`.

During a reset, resources located in `D3DPOOL_DEFAULT` must be handled correctly. The `Dx9GraphicsDevice` implementation explicitly takes this Direct3D9 model into account.

## 12. Layer Responsibilities

| Layer | Responsible for | Must not be responsible for |
|---|---|---|
| Application | business logic | native DX9 |
| Sample | API demonstration | library architecture |
| Core | ImGui lifecycle / windows | specific GPU API |
| Contracts | abstractions | DX9 implementation |
| Direct3D9 Backend | GPU rendering | user UI logic |
| FontService | TTF / font atlas | application layout |

## 13. Main Project Principle

The architecture is built around separation of:

```text
WHAT
│
├── user interface
├── window state
└── ImGui draw commands
        │
        ▼
HOW
│
├── Graphics contracts
├── Direct3D9 backend
└── GPU resources
```

This makes it possible to add another backend in the future without rewriting user windows and Core.

For example, potentially:

```text
VBImGuiDx9
      │
      ├── Direct3D9
      │
      ├── Direct3D11    ← potential
      │
      └── OpenGL        ← potential
```

However, these backends are not implemented yet and are not part of the current API.
