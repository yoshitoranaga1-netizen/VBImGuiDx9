# API

> Current target: .NET 9 / ImGui.NET 1.91.6.1 / Vortice.Direct3D9 3.8.3  
> This document describes the public API of the main `VBImGuiDx9` library.  
> `VBImGuiDx9.Sample` and its `FontService` are not part of this API.

---

## 1. Namespaces

Main public namespaces:

```text
VBImGuiDx9.Core
VBImGuiDx9.Core.ImGuiWindows
VBImGuiDx9.Contracts
VBImGuiDx9.Backends.Direct3D9
```

The main library also ships `VBImGuiDx9.Native` as an internal helper assembly included with the package.

---

# 2. Core

## 2.1 `ImGuiContextManager`

```vb
Public NotInheritable Class ImGuiContextManager
    Implements IDisposable
```

Owns the Dear ImGui context lifetime and contains no Direct3D9-specific logic.

### Properties

```vb
ReadOnly Property IsInitialized As Boolean
ReadOnly Property Context As IntPtr
ReadOnly Property IO As ImGuiIOPtr
```

- `IsInitialized` indicates whether the context is initialized.
- `Context` is the native pointer of the current ImGui context.
- `IO` is the ImGui IO interface.

### Methods

```vb
Sub Initialize()
Sub Dispose()
```

`Initialize()` creates the context, makes it current, configures IO/style, and builds the initial Font Atlas.

---

## 2.2 `ImGuiFrameController`

```vb
Public NotInheritable Class ImGuiFrameController
```

Controls the Dear ImGui frame lifecycle.

### Constructor

```vb
Sub New(contextManager As ImGuiContextManager)
```

`contextManager` must already be initialized.

### Properties

```vb
ReadOnly Property IsFrameActive As Boolean
ReadOnly Property IsRendered As Boolean
ReadOnly Property IO As ImGuiIOPtr
ReadOnly Property DrawData As ImDrawDataPtr
```

`DrawData` is available after `EndFrame()`.

### Methods

```vb
Sub BeginFrame()
Sub EndFrame()
Sub Dispose()
```

Lifecycle:

```text
BeginFrame()
    ↓
ImGui.NewFrame()
    ↓
UI
    ↓
EndFrame()
    ↓
ImGui.Render()
    ↓
DrawData
```

---

# 3. Window Management

## 3.1 `ImGuiWindowState`

Namespace:

```vb
VBImGuiDx9.Core.ImGuiWindows
```

```vb
Public NotInheritable Class ImGuiWindowState
```

Stores the state of one logical ImGui window.

### Constructor

```vb
Sub New(
    id As String,
    title As String,
    Optional flags As ImGuiWindowFlags = ImGuiWindowFlags.None)
```

### Properties

```vb
ReadOnly Property Id As String

Property Title As String
Property Visible As Boolean
Property Collapsed As Boolean
Property Flags As ImGuiWindowFlags
Property Position As Vector2
Property Size As Vector2
Property UseInitialPosition As Boolean
Property UseInitialSize As Boolean
```

`Id` является стабильным идентификатором окна.

Defaults:

```text
Visible = True
Collapsed = False
Position = (40, 40)
Size = (400, 300)
UseInitialPosition = True
UseInitialSize = True
```

---

## 3.2 `ImGuiWindowManager`

Namespace:

```vb
VBImGuiDx9.Core.ImGuiWindows
```

```vb
Public NotInheritable Class ImGuiWindowManager
    Implements IDisposable
```

Manages registered logical ImGui windows.

### Properties

```vb
ReadOnly Property Count As Integer
```

### Methods

```vb
Sub Register(
    state As ImGuiWindowState,
    render As Action)

Function Contains(
    id As String) As Boolean

Function GetState(
    id As String) As ImGuiWindowState

Function GetStates() As IReadOnlyList(Of ImGuiWindowState)

Function SetVisible(
    id As String,
    visible As Boolean) As Boolean

Function SetCollapsed(
    id As String,
    collapsed As Boolean) As Boolean

Sub RenderAll()

Sub Dispose()
```

`Register()` rejects duplicate window IDs.

`RenderAll()`:

1. skips invisible windows;
2. applies initial position/size;
3. calls `ImGui.Begin()`;
4. invokes the window callback;
5. stores position/size;
6. stores collapsed state;
7. handles window closing.

Пример:

```vb
Dim state As New ImGuiWindowState(
    "settings",
    "Settings")

windowManager.Register(
    state,
    AddressOf RenderSettings)
```

---

# 4. Rendering Core

## 4.1 `RenderContext`

```vb
Public NotInheritable Class RenderContext
    Implements IDisposable
```

High-level wrapper around `IGraphicsContext`.

### Constructor

```vb
Sub New(context As IGraphicsContext)
```

### Property

```vb
ReadOnly Property Context As IGraphicsContext
```

### Methods

```vb
Sub BeginFrame()
Sub EndFrame()
Sub Present()
Sub Clear(color As UInteger)
Sub Dispose()
```

`color` is supplied as ARGB32.

---

## 4.2 `Renderer`

```vb
Public NotInheritable Class Renderer
    Implements IDisposable
```

Coordinates the graphics frame lifecycle.

### Constructor

```vb
Sub New(
    device As IGraphicsDevice,
    Optional options As RendererOptions = Nothing)
```

### Properties

```vb
ReadOnly Property Device As IGraphicsDevice
ReadOnly Property Options As RendererOptions
ReadOnly Property Statistics As FrameStatistics
ReadOnly Property IsFrameActive As Boolean
```

### Methods

```vb
Sub BeginFrame()
Sub EndFrame()
Sub Present()
Sub Clear(color As UInteger)
Sub Dispose()
```

Rules:

- a second active frame is rejected;
- `Clear()` requires an active frame;
- `Present()` requires the frame to be ended.

---

## 4.3 `RendererOptions`

```vb
Public NotInheritable Class RendererOptions
```

### Properties

```vb
Property EnableDebugLogging As Boolean
Property ValidateGraphicsState As Boolean
Property CollectStatistics As Boolean
```

`CollectStatistics` defaults to:

```text
True
```

---

## 4.4 `FrameStatistics`

```vb
Public NotInheritable Class FrameStatistics
```

### Properties

```vb
Property DrawCalls As Integer
Property Vertices As Integer
Property Indices As Integer
Property RenderStateChanges As Integer
Property TextureBindings As Integer
Property FrameTimeMilliseconds As Double

ReadOnly Property FramesPerSecond As Double
```

### Methods

```vb
Sub Reset()
```

`FramesPerSecond` is calculated as:

```text
1000 / FrameTimeMilliseconds
```

если `FrameTimeMilliseconds > 0`.

---

## 4.5 `DeviceOptions`

```vb
Public NotInheritable Class DeviceOptions
```

### Properties

```vb
Property WindowHandle As IntPtr
Property Width As Integer
Property Height As Integer
Property Windowed As Boolean
Property EnableVSync As Boolean
Property EnableMultithreading As Boolean
```

Значения по умолчанию:

```text
Windowed = True
EnableVSync = False
EnableMultithreading = False
```

---

## 4.6 `VersionInfo`

```vb
Public NotInheritable Class VersionInfo
```

### Constants

```vb
Const Name As String = "VBImGuiDx9"
Const Major As Integer = 0
Const Minor As Integer = 1
Const Patch As Integer = 0
```

### Properties

```vb
Shared ReadOnly Property Version As Version
Shared ReadOnly Property FullVersion As String
```

---

# 5. Graphics Contracts

## 5.1 `IGraphicsDevice`

```vb
Public Interface IGraphicsDevice
    Inherits IDisposable
```

### Properties

```vb
ReadOnly Property Width As Integer
ReadOnly Property Height As Integer
ReadOnly Property IsInitialized As Boolean
```

### Methods

```vb
Function CreateGraphicsContext() As IGraphicsContext

Function CreateVertexBuffer(
    sizeInBytes As Integer,
    dynamic As Boolean) As IVertexBuffer

Function CreateIndexBuffer(
    sizeInBytes As Integer,
    dynamic As Boolean) As IIndexBuffer

Function CreateTexture2D(
    width As Integer,
    height As Integer) As ITexture
```

---

## 5.2 `IGraphicsContext`

```vb
Public Interface IGraphicsContext
    Inherits IDisposable
```

### Methods

```vb
Sub BeginFrame()
Sub EndFrame()
Sub Present()
Sub Clear(color As UInteger)
```

`Clear()` использует ARGB32 color.

---

## 5.3 `IGraphicsResource`

```vb
Public Interface IGraphicsResource
    Inherits IDisposable
```

### Property

```vb
ReadOnly Property Device As IGraphicsDevice
```

---

## 5.4 `IBuffer`

`IBuffer` is the base contract for GPU buffers.

Используемые членами backend реализации:

```vb
ReadOnly Property SizeInBytes As Integer
ReadOnly Property IsDynamic As Boolean

Sub SetData(
    source As IntPtr,
    sizeInBytes As Integer)
```

---

## 5.5 `IVertexBuffer`

```vb
Public Interface IVertexBuffer
    Inherits IBuffer
```

Specialized graphics contract for vertex buffers.

---

## 5.6 `IIndexBuffer`

```vb
Public Interface IIndexBuffer
    Inherits IBuffer
```

Specialized graphics contract for index buffers.

The current Direct3D9 implementation uses 16-bit indices.

---

## 5.7 `ITexture`

```vb
Public Interface ITexture
    Inherits IGraphicsResource
```

### Properties

```vb
ReadOnly Property Width As Integer
ReadOnly Property Height As Integer
```

---

## 5.8 `IRenderTarget`

```vb
Public Interface IRenderTarget
    Inherits ITexture
```

Specialized texture contract for render-target usage.

---

## 5.9 State Contracts

The following interfaces are graphics-resource contracts:

```vb
IBlendState
IDepthStencilState
IRasterizerState
ISamplerState
```

Каждый наследует:

```vb
IGraphicsResource
```

In the current Direct3D9 backend implementation they expose no additional public members beyond the base contract.

---

## 5.10 `ILogger`

```vb
Public Interface ILogger
```

### Methods

```vb
Sub Info(message As String)
Sub Warning(message As String)
Sub [Error](message As String)
```

---

# 6. Direct3D9 Backend

Namespace:

```vb
VBImGuiDx9.Backends.Direct3D9
```

## 6.1 `Dx9DeviceStatus`

```vb
Public Enum Dx9DeviceStatus
```

Values:

```text
Operational
DeviceLost
DeviceNotReset
DriverInternalError
Unknown
```

---

## 6.2 `Dx9GraphicsDevice`

```vb
Public NotInheritable Class Dx9GraphicsDevice
    Implements IGraphicsDevice
```

### Constructor

```vb
Sub New(options As DeviceOptions)
```

Creates a hardware Direct3D9 device.

### Properties

```vb
ReadOnly Property Width As Integer
ReadOnly Property Height As Integer
ReadOnly Property IsInitialized As Boolean

ReadOnly Property NativeDevice As IDirect3DDevice9
ReadOnly Property NativeDirect3D As IDirect3D9
```

`NativeDevice` and `NativeDirect3D` expose low-level Vortice APIs and should be used with care.

### Methods

```vb
Function GetDeviceStatus() As Dx9DeviceStatus

Function TryReset(
    width As Integer,
    height As Integer) As Boolean

Function CreateGraphicsContext() As IGraphicsContext

Function CreateVertexBuffer(
    sizeInBytes As Integer,
    dynamic As Boolean) As IVertexBuffer

Function CreateIndexBuffer(
    sizeInBytes As Integer,
    dynamic As Boolean) As IIndexBuffer

Function CreateTexture2D(
    width As Integer,
    height As Integer) As ITexture

Sub Dispose()
```

`TryReset()` returns `False` when the device is still in a state that prevents reset.

---

## 6.3 `Dx9GraphicsContext`

```vb
Public NotInheritable Class Dx9GraphicsContext
    Implements IGraphicsContext
```

### Constructor

```vb
Sub New(
    graphicsDevice As Dx9GraphicsDevice)
```

### Property

```vb
ReadOnly Property IsFrameActive As Boolean
```

### Methods

```vb
Sub BeginFrame()
Sub EndFrame()
Sub Present()
Sub Clear(color As UInteger)
Sub Dispose()
```

Additional backend rendering operations used by `Dx9ImGuiRenderer` include:

- projection;
- vertex/index buffers;
- texture binding;
- scissor;
- indexed drawing;
- ImGui render state.

---

## 6.4 `Dx9ImGuiRenderer`

```vb
Public NotInheritable Class Dx9ImGuiRenderer
    Implements IDisposable
```

Boundary between Dear ImGui `ImDrawData` and Direct3D9.

### Constructor

```vb
Sub New(
    graphicsDevice As Dx9GraphicsDevice,
    graphicsContext As Dx9GraphicsContext)
```

### Properties

```vb
ReadOnly Property IsInitialized As Boolean
ReadOnly Property FontTexture As IDirect3DTexture9
ReadOnly Property FontTextureId As IntPtr
ReadOnly Property VertexBuffer As IDirect3DVertexBuffer9
ReadOnly Property IndexBuffer As IDirect3DIndexBuffer9
ReadOnly Property VertexCapacity As Integer
ReadOnly Property IndexCapacity As Integer
```

Initial capacity of the current renderer:

```text
VertexCapacity = 5000 vertices
IndexCapacity  = 10000 indices
```

### Initialization

```vb
Sub Initialize()
Sub CreateDeviceObjects()
Sub RebuildFontTexture()
```

### Rendering

Renderer принимает `ImDrawData` через публичный метод `RenderDrawData(...)` и преобразует:

```text
ImDrawData
    ↓
Vertex / Index buffers
    ↓
Clip rectangles
    ↓
Texture binding
    ↓
DrawIndexedTriangles
```

The current renderer uses:

```text
28 bytes / ImGui vertex
16-bit indices
```

### Device lifecycle

```vb
Sub InvalidateDeviceObjects()
Sub RestoreDeviceObjects()
```

Used during Direct3D9 device reset.

### Dispose

```vb
Sub Dispose()
```

---

## 6.5 `Dx9VertexBuffer`

```vb
Public NotInheritable Class Dx9VertexBuffer
    Implements IVertexBuffer
```

### Constructor

```vb
Sub New(
    device As Dx9GraphicsDevice,
    sizeInBytes As Integer,
    dynamic As Boolean)
```

### Properties

```vb
ReadOnly Property Device As IGraphicsDevice
ReadOnly Property SizeInBytes As Integer
ReadOnly Property IsDynamic As Boolean
ReadOnly Property NativeBuffer As IDirect3DVertexBuffer9
```

### Methods

```vb
Sub SetData(
    source As IntPtr,
    sizeInBytes As Integer)

Sub Dispose()
```

---

## 6.6 `Dx9IndexBuffer`

```vb
Public NotInheritable Class Dx9IndexBuffer
    Implements IIndexBuffer
```

### Constructor

```vb
Sub New(
    device As Dx9GraphicsDevice,
    sizeInBytes As Integer,
    dynamic As Boolean)
```

### Properties

```vb
ReadOnly Property Device As IGraphicsDevice
ReadOnly Property SizeInBytes As Integer
ReadOnly Property IsDynamic As Boolean
ReadOnly Property NativeBuffer As IDirect3DIndexBuffer9
```

### Methods

```vb
Sub SetData(
    source As IntPtr,
    sizeInBytes As Integer)

Sub Dispose()
```

---

## 6.7 `Dx9Texture`

```vb
Public NotInheritable Class Dx9Texture
    Implements ITexture
```

### Constructor

```vb
Sub New(
    device As Dx9GraphicsDevice,
    width As Integer,
    height As Integer)
```

### Properties

```vb
ReadOnly Property Device As IGraphicsDevice
ReadOnly Property Width As Integer
ReadOnly Property Height As Integer
ReadOnly Property NativeTexture As IDirect3DTexture9
```

### Methods

```vb
Sub Dispose()
```

---

# 7. API boundaries

## Included in the library API

```text
Core
Contracts
Direct3D9 Backend
```

## Not part of the library API

```text
VBImGuiDx9.Sample
FontService
Sample UI
Sample diagnostics
Sample WinForms code
Sample TTF assets
```

Sample resources are demonstration resources and are not required by the main runtime library.

---

# 8. Device reset contract

Typical reset sequence:

```text
Dx9ImGuiRenderer.InvalidateDeviceObjects()
        ↓
Dx9GraphicsDevice.TryReset(width, height)
        ↓
Dx9ImGuiRenderer.RestoreDeviceObjects()
```

The renderer must not attempt to render with invalid display dimensions. During resize/minimize/restore it ignores draw data when `DisplaySize` is non-positive or `FramebufferScale` is invalid.

---

# 9. Recommended lifecycle

```text
ImGuiContextManager.Initialize()
        ↓
Dx9GraphicsDevice
        ↓
Dx9GraphicsContext
        ↓
Dx9ImGuiRenderer.Initialize()
        ↓
ImGuiFrameController.BeginFrame()
        ↓
ImGuiWindowManager.RenderAll()
        ↓
ImGuiFrameController.EndFrame()
        ↓
Dx9ImGuiRenderer.RenderDrawData()
        ↓
Dx9GraphicsContext.EndFrame()
        ↓
Dx9GraphicsContext.Present()
```

Dispose resources in reverse ownership order.

---

# 10. API stability

This document describes the public API of the current project version.

Internal implementation details, private methods, `Friend` members, native helper internals, and Sample-only types are intentionally excluded unless exposed as public API.
