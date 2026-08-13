# Rendering

## Overview

VBImGuiDx9 uses Dear ImGui to build the user interface and Direct3D9
for its actual rendering.

Dear ImGui does not render interface elements directly through Direct3D9.
Instead, ImGui produces `ImDrawData` containing:

- vertex data;
- index data;
- draw commands;
- clipping rectangles;
- texture references.

The Direct3D9 backend converts this data into actual GPU draw calls.

Main pipeline:

```text
ImGui UI
    ↓
ImGui.NewFrame()
    ↓
Application UI
    ↓
ImGui.Render()
    ↓
ImDrawData
    ↓
Dx9ImGuiRenderer
    ↓
Vertex / Index Buffers
    ↓
Render State
    ↓
Scissor / Texture
    ↓
DrawIndexedTriangles()
    ↓
Direct3D9
    ↓
GPU
```

## 1. Frame lifecycle

Each frame consists of two main ImGui operations:

```text
BeginFrame
    ↓
ImGui.NewFrame()
    ↓
Render UI
    ↓
EndFrame
    ↓
ImGui.Render()
```

After `ImGui.Render()`, Dear ImGui produces `ImDrawData`.

`ImGuiFrameController` is responsible for managing this lifecycle.

User UI should be created between `BeginFrame()` and `EndFrame()`.

Example:

```vb
_frameController.BeginFrame()

_windowManager.RenderAll()

_frameController.EndFrame()
```

After the frame is completed:

```vb
Dim drawData As ImDrawDataPtr =
    _frameController.DrawData
```

this data is passed to the Direct3D9 renderer.

## 2. ImDrawData

`ImDrawData` contains the result of building the current interface.

Main parameters:

- `TotalVtxCount`
- `TotalIdxCount`
- `CmdListsCount`
- `DisplaySize`
- `FramebufferScale`

Each command list contains:

- Vertex Buffer
- Index Buffer
- Command Buffer

Simplified:

```text
ImDrawData
│
├── CmdList 0
│   ├── Vertices
│   ├── Indices
│   └── Commands
│
├── CmdList 1
│   ├── Vertices
│   ├── Indices
│   └── Commands
│
└── ...
```

## 3. Vertex format

The Direct3D9 renderer uses its own DX9 vertex format for ImGui.

One vertex contains:

```text
Position
    X
    Y
    Z
    RHW

Color

Texture coordinates
    U
    V
```

The current renderer uses a vertex size of:

```text
28 bytes
```

Structure:

```text
Offset  Size    Data
------  ------  ----------------
0       4       Position X
4       4       Position Y
8       4       Position Z
12      4       RHW
16      4       Color
20      4       UV X
24      4       UV Y
```

Direct3D9 uses:

```text
PositionRhw
Diffuse
Texture1
```

This allows ImGui vertex data to be used in the DX9 rendering pipeline after conversion to the corresponding buffer layout.

## 4. Vertex buffer

The renderer uses a dynamic Direct3D9 vertex buffer.

Initial capacity:

```text
5000 vertices
```

If the current ImGui frame contains more vertices, the buffer is automatically increased.

The strategy is:

```text
newCapacity =
    max(
        required + growth,
        currentCapacity * 2)
```

Additional growth:

```text
5000 vertices
```

This reduces the number of reallocations as interface complexity increases.

## 5. Index buffer

ImGui uses a 16-bit index buffer.

Initial capacity:

```text
10000 indices
```

Index size:

```text
2 bytes
```

If capacity is insufficient, the buffer is automatically increased.

The growth strategy is analogous to the vertex buffer.

## 6. Uploading vertex data

Before rendering the current frame, the renderer collects the vertices from all ImGui command lists and places them into a single Direct3D9 vertex buffer.

Conceptually:

```text
CmdList 0 vertices
        ↓
CmdList 1 vertices
        ↓
CmdList 2 vertices
        ↓
Single DX9 vertex buffer
```

The renderer also tracks the global vertex data offset.

## 7. Uploading index data

Indices are collected from all command lists in the same way:

```text
CmdList 0 indices
        ↓
CmdList 1 indices
        ↓
CmdList 2 indices
        ↓
Single DX9 index buffer
```

For the current frame, the index buffer is also uploaded in full.

## 8. Projection

ImGui uses screen coordinates.

The renderer creates the corresponding projection matrix based on:

```text
drawData.DisplaySize.X
drawData.DisplaySize.Y
```

Simplified:

```text
(0, 0)
   ┌───────────────────────────────┐
   │                               │
   │          ImGui UI             │
   │                               │
   │                               │
   └───────────────────────────────┘
                         (width,height)
```

The projection is passed to the Direct3D9 context before draw calls are executed.

## 9. Render state

Before rendering, the ImGui renderer sets the required Direct3D9 render states.

These include:

- vertex format;
- blend state;
- depth state;
- culling state;
- scissor state;
- texture state.

This is necessary because Direct3D9 state is global state belonging to the current device.

The renderer must put the device into a state suitable for ImGui rendering.

## 10. Alpha blending

ImGui uses transparency for interface elements.

Therefore, the renderer enables the corresponding blend state.

This allows correct rendering of:

- windows;
- panels;
- text;
- buttons;
- semi-transparent elements;
- overlay UI.

After ImGui rendering is complete, the application must restore the Direct3D9 state required by its own rendering pipeline if it uses the same device.

## 11. Scissor clipping

Each ImGui draw command contains a clipping rectangle.

The renderer converts it into a screen-space Direct3D9 scissor rectangle.

Pipeline:

```text
ImGui ClipRect
      ↓
FramebufferScale
      ↓
DX9 rectangle
      ↓
SetScissorRect
      ↓
DrawIndexed
```

This allows ImGui to restrict rendering to the appropriate window, child region, or other clipping container.

## 12. Texture binding

A draw command may contain `TextureId`.

For the standard Font Atlas, the renderer uses the Direct3D9 font texture.

Simplified:

```text
TextureId
    │
    ├── Font texture
    │
    └── Other texture
```

The font texture is created during renderer initialization.

When rendering a command:

```text
DrawCommand.TextureId
        ↓
Texture lookup
        ↓
Bind texture
        ↓
DrawIndexed
```

## 13. Draw commands

The renderer processes all command lists:

```text
For each CmdList
    For each DrawCommand
        validate command
        calculate clip rectangle
        bind texture
        calculate offsets
        draw triangles
```

For each command, the following values are determined:

- `ElemCount`
- `IdxOffset`
- `VtxOffset`
- `TextureId`
- `ClipRect`

`ElemCount` determines the number of indices.

Because one ImGui triangle consists of three indices:

```text
PrimitiveCount =
    ElemCount / 3
```

## 14. Vertex and index offsets

ImGui command lists may use local offsets.

The renderer supports two levels of offset:

```text
Global vertex offset
Global index offset
```

The resulting values are calculated as:

```text
BaseVertexIndex =
    GlobalVertexOffset +
    Command.VtxOffset
```

and:

```text
StartIndex =
    GlobalIndexOffset +
    Command.IdxOffset
```

This allows multiple ImGui command lists to be combined into one pair
of Direct3D9 buffers.

## 15. Empty frames

The renderer ignores a frame when the required draw data is absent.

For example:

```text
CmdListsCount <= 0
TotalVtxCount <= 0
TotalIdxCount <= 0
```

In that case, no GPU draw call is performed.

This is normal for a frame that contains no visible ImGui content.

## 16. Invalid display size

During resize, minimize/restore, or a Direct3D9 reset, the display area may temporarily have a zero size.

The renderer does not render when:

```text
DisplaySize.X <= 0
DisplaySize.Y <= 0
```

It also checks:

```text
FramebufferScale.X > 0
FramebufferScale.Y > 0
```

This prevents invalid dimensions from being passed into projection and scissor calculations.

## 17. Font texture

The Font Atlas is created by Dear ImGui.

After obtaining RGBA32 pixel data, the renderer:

- creates a Direct3D9 texture;
- locks the texture;
- copies the pixel rows;
- unlocks the texture;
- obtains the native texture pointer;
- assigns it as the ImGui `TexID`.

Pipeline:

```text
ImGui Font Atlas
      ↓
RGBA32 pixels
      ↓
IDirect3DTexture9
      ↓
ImGui TexID
      ↓
DrawCommand.TextureId
```

## 18. Resource lifetime

The renderer owns the following Direct3D9 resources:

```text
FontTexture
VertexBuffer
IndexBuffer
```

They are created during:

```text
Initialize()
```

and released during:

```text
Dispose()
```

The renderer also provides:

```text
InvalidateDeviceObjects()
RestoreDeviceObjects()
```

for restoring resources after a change in Direct3D9 device state.

## 19. Device reset

Direct3D9 has a special device-loss model.

The device may transition through:

```text
Operational
      ↓
DeviceLost
      ↓
DeviceNotReset
      ↓
Operational
```

When required, resources owned by `D3DPOOL_DEFAULT` must be released before reset and recreated after the device is restored.

For the ImGui renderer this means:

```text
InvalidateDeviceObjects()
        ↓
DX9 device reset
        ↓
RestoreDeviceObjects()
```

The font texture, vertex buffer, and index buffer must be restored.

## 20. Separation of responsibilities

The rendering pipeline is intentionally separated.

### ImGuiFrameController

Responsible for:

```text
NewFrame
Render
DrawData
```

### ImGuiWindowManager

Responsible for:

```text
Windows
Window state
Window rendering callbacks
```

### Dx9ImGuiRenderer

Responsible for:

```text
ImDrawData
GPU buffers
Font texture
Draw commands
```

### Dx9GraphicsContext

Responsible for:

```text
DX9 render state
Projection
Buffers
Texture binding
Draw calls
```

### Dx9GraphicsDevice

Responsible for:

```text
DX9 device
Device state
Reset
Resource creation
```

## 21. Rendering flow

Complete pipeline:

```text
                     APPLICATION
                         │
                         ▼
                  BeginFrame()
                         │
                         ▼
                   ImGui.NewFrame()
                         │
                         ▼
                   User Interface
                         │
                         ▼
                 WindowManager
                         │
                         ▼
                    ImGui.Render()
                         │
                         ▼
                     ImDrawData
                         │
                         ▼
                 Dx9ImGuiRenderer
                         │
               ┌──────────┴──────────┐
               ▼                     ▼
         Vertex Buffer          Index Buffer
               │                     │
               └──────────┬──────────┘
                         ▼
                    Draw Commands
                         │
              ┌───────────┼───────────┐
              ▼           ▼           ▼
           Texture     Scissor     Projection
              │           │           │
              └───────────┼───────────┘
                         ▼
                    DrawIndexed
                         │
                         ▼
                     Direct3D9
                         │
                         ▼
                         GPU
```

## 22. Performance considerations

The current renderer uses dynamic vertex/index buffers and increases their
capacity when necessary.

This avoids continuously creating GPU buffers on every frame.

However, user interfaces should still avoid unnecessarily creating a large
number of draw commands.

Recommended practices:

- reuse UI state;
- do not recreate resources every frame;
- do not rebuild the Font Atlas every frame;
- do not create Direct3D9 textures without necessity;
- use clipping containers for large areas;
- avoid performing expensive operations inside rendering callbacks unless necessary.

## 23. Threading

The Direct3D9 device and ImGui context require careful handling across threads.

The current Sample configuration uses:

```text
deviceOptions.EnableMultithreading = False
```

UI rendering should execute sequentially within the main rendering pipeline.

Parallel CPU operations are possible for independent work, but access to the ImGui context and Direct3D9 device must not be considered automatically thread-safe.

## 24. Rendering responsibilities for application developers

User code should be responsible for:

```text
Application state
        ↓
UI construction
        ↓
Window registration
        ↓
Frame loop integration
```

The backend is responsible for:

```text
ImDrawData
        ↓
GPU rendering
```

Direct access to the renderer's internal Direct3D9 resources from user windows is not recommended.

## 25. Summary

The current Direct3D9 rendering pipeline is:

```text
VB.NET UI
    ↓
ImGui.NET
    ↓
ImDrawData
    ↓
Dx9ImGuiRenderer
    ↓
DX9 vertex/index buffers
    ↓
DX9 render state
    ↓
DX9 texture
    ↓
DX9 indexed draw
    ↓
GPU
```

The main architectural goal is to isolate user UI from the low-level
Direct3D9 implementation.

This allows UI to be changed without requiring knowledge of vertex buffers,
index buffers, clipping, projection, and Direct3D9 resource lifetime.
