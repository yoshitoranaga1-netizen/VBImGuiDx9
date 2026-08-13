# Rendering

## Overview

VBImGuiDx9 использует Dear ImGui для формирования интерфейса и Direct3D9
для его фактической отрисовки.

Dear ImGui не рисует элементы интерфейса непосредственно через Direct3D9.
Вместо этого ImGui формирует `ImDrawData`, содержащий:

- vertex data;
- index data;
- draw commands;
- clipping rectangles;
- texture references.

Direct3D9 backend преобразует эти данные в реальные GPU draw calls.

Основной pipeline:

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
1. Frame lifecycle

Каждый кадр состоит из двух основных ImGui операций:

BeginFrame
    ↓
ImGui.NewFrame()
    ↓
Render UI
    ↓
EndFrame
    ↓
ImGui.Render()

После ImGui.Render() Dear ImGui формирует ImDrawData.

ImGuiFrameController отвечает за управление этим lifecycle.

Пользовательский UI должен создаваться между BeginFrame() и EndFrame().

Пример:

_frameController.BeginFrame()

_windowManager.RenderAll()

_frameController.EndFrame()

После завершения frame:

Dim drawData As ImDrawDataPtr =
    _frameController.DrawData

эти данные передаются Direct3D9 renderer.

2. ImDrawData

ImDrawData содержит результат построения текущего интерфейса.

Основные параметры:

TotalVtxCount
TotalIdxCount
CmdListsCount
DisplaySize
FramebufferScale

Каждый command list содержит:

Vertex Buffer
Index Buffer
Command Buffer

Упрощённо:

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
3. Vertex format

Direct3D9 renderer использует собственный DX9 vertex format для ImGui.

Один vertex содержит:

Position
    X
    Y
    Z
    RHW

Color

Texture coordinates
    U
    V

В текущем renderer размер vertex:

28 bytes

Структура:

Offset  Size    Data
------  ------  ----------------
0       4       Position X
4       4       Position Y
8       4       Position Z
12      4       RHW
16      4       Color
20      4       UV X
24      4       UV Y

Для Direct3D9 используется:

PositionRhw
Diffuse
Texture1

Это позволяет использовать ImGui vertex data непосредственно в DX9
rendering pipeline после преобразования в соответствующий buffer layout.

4. Vertex buffer

Renderer использует динамический Direct3D9 vertex buffer.

Начальная ёмкость:

5000 vertices

Если текущий ImGui frame содержит больше vertices, buffer автоматически
увеличивается.

Используется стратегия:

newCapacity =
    max(
        required + growth,
        currentCapacity * 2)

Дополнительный рост:

5000 vertices

Это уменьшает количество reallocations при увеличении сложности интерфейса.

5. Index buffer

Для ImGui используется 16-bit index buffer.

Начальная ёмкость:

10000 indices

Размер одного index:

2 bytes

При недостаточной ёмкости buffer автоматически увеличивается.

Стратегия роста аналогична vertex buffer.

6. Uploading vertex data

Перед rendering текущего frame renderer собирает vertices всех ImGui command
lists и помещает их в единый Direct3D9 vertex buffer.

Концептуально:

CmdList 0 vertices
        ↓
CmdList 1 vertices
        ↓
CmdList 2 vertices
        ↓
Single DX9 vertex buffer

При этом renderer отслеживает глобальное смещение vertex data.

7. Uploading index data

Индексы аналогично собираются из всех command lists:

CmdList 0 indices
        ↓
CmdList 1 indices
        ↓
CmdList 2 indices
        ↓
Single DX9 index buffer

Для текущего frame index buffer также загружается целиком.

8. Projection

ImGui использует экранные координаты.

Renderer создаёт соответствующую projection matrix на основании:

drawData.DisplaySize.X
drawData.DisplaySize.Y

Упрощённо:

(0, 0)
   ┌───────────────────────────────┐
   │                               │
   │          ImGui UI             │
   │                               │
   │                               │
   └───────────────────────────────┘
                         (width,height)

Projection передаётся Direct3D9 context перед выполнением draw calls.

9. Render state

Перед отрисовкой ImGui renderer устанавливает необходимые Direct3D9
render states.

К ним относятся:

vertex format;
blend state;
depth state;
culling state;
scissor state;
texture state.

Это необходимо потому, что состояние Direct3D9 является глобальным состоянием
текущего device.

Renderer должен привести device в состояние, подходящее для ImGui rendering.

10. Alpha blending

ImGui использует прозрачность элементов интерфейса.

Поэтому renderer включает соответствующий blend state.

Это позволяет корректно отображать:

- окна;
- панели;
- текст;
- кнопки;
- полупрозрачные элементы;
- overlay UI.

После завершения ImGui rendering приложение должно восстановить состояние
Direct3D9, необходимое для собственного rendering pipeline, если оно использует
тот же device.

11. Scissor clipping

Каждый ImGui draw command содержит clipping rectangle.

Renderer преобразует его в экранный Direct3D9 scissor rectangle.

Pipeline:

ImGui ClipRect
      ↓
FramebufferScale
      ↓
DX9 rectangle
      ↓
SetScissorRect
      ↓
DrawIndexed

Это позволяет ImGui ограничивать rendering областью соответствующего окна,
child region или другого clipping container.

12. Texture binding

Draw command может содержать TextureId.

Для стандартного Font Atlas renderer использует Direct3D9 font texture.

Упрощённо:

TextureId
    │
    ├── Font texture
    │
    └── Other texture

Font texture создаётся во время initialization renderer.

При rendering command:

DrawCommand.TextureId
        ↓
Texture lookup
        ↓
Bind texture
        ↓
DrawIndexed
13. Draw commands

Renderer проходит все command lists:

For each CmdList
    For each DrawCommand
        validate command
        calculate clip rectangle
        bind texture
        calculate offsets
        draw triangles

Для каждого command определяется:

ElemCount
IdxOffset
VtxOffset
TextureId
ClipRect

ElemCount определяет количество indices.

Поскольку один ImGui triangle состоит из трёх indices:

PrimitiveCount =
    ElemCount / 3
14. Vertex and index offsets

ImGui command lists могут использовать локальные offsets.

Renderer поддерживает два уровня смещения:

Global vertex offset
Global index offset

Итоговые значения вычисляются как:

BaseVertexIndex =
    GlobalVertexOffset +
    Command.VtxOffset

и:

StartIndex =
    GlobalIndexOffset +
    Command.IdxOffset

Это позволяет объединять несколько ImGui command lists в один pair
of Direct3D9 buffers.

15. Empty frames

Renderer игнорирует frame, если отсутствуют необходимые draw data.

Например:

CmdListsCount <= 0
TotalVtxCount <= 0
TotalIdxCount <= 0

В таком случае GPU draw call не выполняется.

Это нормально для кадра, в котором нет визуального ImGui content.

16. Invalid display size

Во время resize, minimize/restore или Direct3D9 reset возможен временный
нулевой размер display area.

Renderer не выполняет rendering, если:

DisplaySize.X <= 0
DisplaySize.Y <= 0

Также проверяется:

FramebufferScale.X > 0
FramebufferScale.Y > 0

Это предотвращает передачу некорректных размеров в projection и scissor
calculation.

17. Font texture

Font Atlas создаётся Dear ImGui.

После получения RGBA32 pixel data renderer:

создаёт Direct3D9 texture;
блокирует texture;
копирует pixel rows;
разблокирует texture;
получает native texture pointer;
устанавливает его как ImGui TexID.

Pipeline:

ImGui Font Atlas
      ↓
RGBA32 pixels
      ↓
IDirect3DTexture9
      ↓
ImGui TexID
      ↓
DrawCommand.TextureId
18. Resource lifetime

Renderer владеет следующими Direct3D9 resources:

FontTexture
VertexBuffer
IndexBuffer

Они создаются во время:

Initialize()

и освобождаются во время:

Dispose()

Также renderer предоставляет:

InvalidateDeviceObjects()
RestoreDeviceObjects()

для восстановления ресурсов после изменения состояния Direct3D9 device.

19. Device reset

Direct3D9 имеет специальную модель device loss.

Устройство может перейти:

Operational
      ↓
DeviceLost
      ↓
DeviceNotReset
      ↓
Operational

При необходимости resources, принадлежащие D3DPOOL_DEFAULT, должны быть
освобождены перед reset и созданы заново после восстановления device.

Для ImGui renderer это означает:

InvalidateDeviceObjects()
        ↓
DX9 device reset
        ↓
RestoreDeviceObjects()

Font texture, vertex buffer и index buffer должны быть восстановлены.

20. Separation of responsibilities

Rendering pipeline намеренно разделён.

ImGuiFrameController

Отвечает за:

NewFrame
Render
DrawData
ImGuiWindowManager

Отвечает за:

Windows
Window state
Window rendering callbacks
Dx9ImGuiRenderer

Отвечает за:

ImDrawData
GPU buffers
Font texture
Draw commands
Dx9GraphicsContext

Отвечает за:

DX9 render state
Projection
Buffers
Texture binding
Draw calls
Dx9GraphicsDevice

Отвечает за:

DX9 device
Device state
Reset
Resource creation
21. Rendering flow

Полный pipeline:

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
22. Performance considerations

Текущий renderer использует динамические vertex/index buffers и увеличивает
их ёмкость при необходимости.

Это позволяет избежать постоянного создания GPU buffers при каждом frame.

Однако пользовательский интерфейс всё равно должен избегать ненужного
создания большого количества draw commands.

Рекомендуется:

переиспользовать UI state;
не пересоздавать ресурсы каждый frame;
не пересобирать Font Atlas каждый frame;
не создавать Direct3D9 textures без необходимости;
использовать clipping containers для больших областей;
не выполнять тяжёлые операции внутри rendering callback без необходимости.
23. Threading

Direct3D9 device и ImGui context требуют аккуратного обращения из потоков.

Текущая Sample-конфигурация использует:

deviceOptions.EnableMultithreading = False

UI rendering должен выполняться последовательно в рамках основного rendering
pipeline.

Параллельное выполнение CPU-задач возможно для независимых операций, однако
доступ к ImGui context и Direct3D9 device нельзя считать автоматически
thread-safe.

24. Rendering responsibilities for application developers

Пользовательский код должен отвечать за:

Application state
        ↓
UI construction
        ↓
Window registration
        ↓
Frame loop integration

Backend отвечает за:

ImDrawData
        ↓
GPU rendering

Не рекомендуется напрямую обращаться к внутренним Direct3D9 resources
renderer из пользовательских окон.

25. Summary

Текущий Direct3D9 rendering pipeline:

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

Основная цель архитектуры — изолировать пользовательский UI от низкоуровневой
Direct3D9 реализации.

Это позволяет изменять UI без необходимости разбираться с vertex buffers,
index buffers, clipping, projection и Direct3D9 resource lifetime.
