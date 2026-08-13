Architecture
1. Общая схема

VBImGuiDx9 разделён на несколько уровней:

┌───────────────────────────────────────────────┐
│                 Application                   │
│              пользовательский код             │
└──────────────────────┬────────────────────────┘
                       │
                       ▼
┌───────────────────────────────────────────────┐
│                   Sample                      │
│        примеры интерфейсов и диагностика      │
└──────────────────────┬────────────────────────┘
                       │
                       ▼
┌───────────────────────────────────────────────┐
│                    Core                       │
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
2. Core
ImGuiContextManager

ImGuiContextManager владеет жизненным циклом Dear ImGui context.

Основные обязанности:

создать ImGui context;
установить текущий context;
настроить IO;
установить начальный стиль;
построить первоначальный Font Atlas;
уничтожить context при Dispose().

Класс намеренно не содержит Direct3D9-логики. Это позволяет отделить жизненный цикл Dear ImGui от конкретного графического backend.

ImGuiFrameController

ImGuiFrameController отвечает за корректный lifecycle ImGui frame.

Цикл выглядит следующим образом:

BeginFrame()
     │
     ▼
ImGui.NewFrame()
     │
     ▼
пользовательский UI
     │
     ▼
EndFrame()
     │
     ▼
ImGui.Render()
     │
     ▼
ImDrawData

BeginFrame() запрещает начать второй активный frame, устанавливает текущий ImGui context и вызывает ImGui.NewFrame().

EndFrame() завершает frame через ImGui.Render(). После этого DrawData становится доступным для backend.

Таким образом, пользовательский UI не должен самостоятельно управлять переходом:

NewFrame → Render → GetDrawData
3. Window Management
ImGuiWindowManager

Window Manager отделяет состояние окна от его содержимого.

Каждое зарегистрированное окно имеет:

Id
Title
Visible
Position
Size
Collapsed
Flags

Менеджер позволяет:

зарегистрировать окно;
получить его состояние;
изменить visibility;
изменить collapsed state;
получить список зарегистрированных окон;
вызвать renderer каждого окна.

Во время RenderAll() менеджер:

пропускает невидимые окна;
устанавливает начальную позицию;
устанавливает начальный размер;
вызывает ImGui.Begin();
вызывает renderer окна;
сохраняет новую позицию;
сохраняет новый размер;
сохраняет collapsed state;
обрабатывает закрытие окна.

Это позволяет пользовательскому коду концентрироваться на содержимом:

Private Sub RenderSettings()

    ImGui.Text("Settings")

    ' Controls...

End Sub

а не на управлении жизненным циклом самого окна.

4. Contracts

Contracts определяет абстракции графического слоя.

Главная идея:

Core
  ↓
Contracts
  ↓
Backend

Core не должен знать, каким именно графическим API будет отрисован интерфейс.

Например:

IGraphicsDevice
IGraphicsContext
ITexture
IVertexBuffer
IIndexBuffer

Direct3D9 реализует эти контракты своими классами.

Например, Dx9GraphicsDevice.CreateGraphicsContext() возвращает Dx9GraphicsContext через контракт IGraphicsContext, а методы создания vertex/index buffers и texture реализуют соответствующие интерфейсы.

5. Direct3D9 Backend

Direct3D9 является конкретной реализацией графического слоя.

Dx9GraphicsDevice

Отвечает за:

создание Direct3D9;
создание устройства;
управление device state;
создание графического context;
создание GPU resources;
reset;
освобождение native resources.

Например:

Dx9GraphicsDevice
       │
       ├── Dx9GraphicsContext
       ├── Dx9VertexBuffer
       ├── Dx9IndexBuffer
       └── Dx9Texture

Native Direct3D9 objects не должны напрямую использоваться пользовательским UI-кодом.

6. Dx9GraphicsContext

Graphics Context предоставляет операции, необходимые для выполнения рендеринга.

В частности, он используется ImGui renderer для:

установки projection;
vertex buffer;
index buffer;
vertex format;
blend state;
depth/culling state;
scissor state;
texture;
indexed draw.

Это позволяет отделить непосредственное управление состоянием Direct3D9 от логики формирования ImGui draw data.

7. Dx9ImGuiRenderer

Это граница между Dear ImGui и Direct3D9.

На вход renderer получает:

ImDrawData

После чего:

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

Renderer отвечает за:

font texture;
dynamic vertex buffer;
dynamic index buffer;
изменение размера buffers;
копирование ImGui vertex data;
копирование index data;
projection;
clipping/scissor;
texture binding;
indexed triangle rendering.

Таким образом:

ImGui
  ↓
ImDrawData
  ↓
Dx9ImGuiRenderer
  ↓
Direct3D9
8. Font pipeline

Шрифты проходят отдельный pipeline:

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

FontService автоматически обнаруживает TTF-файлы из каталога приложения.

Названия файлов используются как имена шрифтов:

Inter.ttf
    ↓
Inter

Roboto.ttf
    ↓
Roboto

Segoe UI.ttf
    ↓
Segoe UI

Специального встроенного DemoFont библиотеки больше не предполагает.

9. Application lifecycle

Типичная последовательность запуска:

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

В текущем Sample сначала создаются ImGui context и frame controller, затем Window Manager, после чего создаются Direct3D9 device/context и ImGui renderer.

10. Runtime frame

Один кадр приложения выглядит концептуально так:

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
11. Device Lost / Reset

Direct3D9 имеет особенность: устройство может перейти в состояние Lost.

Backend предоставляет диагностику:

Operational
DeviceLost
DeviceNotReset
DriverInternalError
Unknown

и TryReset().

При reset должны корректно обрабатываться resources, находящиеся в D3DPOOL_DEFAULT. Реализация Dx9GraphicsDevice прямо учитывает эту модель Direct3D9.

12. Ответственность слоёв
Слой	Отвечает за	Не должен отвечать за
Application	бизнес-логика	native DX9
Sample	демонстрация API	библиотечную архитектуру
Core	ImGui lifecycle / окна	конкретный GPU API
Contracts	абстракции	реализацию DX9
Direct3D9 Backend	GPU rendering	пользовательскую UI-логику
FontService	TTF / font atlas	layout приложения
13. Главный принцип проекта

Архитектура строится вокруг разделения:

WHAT
│
├── пользовательский интерфейс
├── состояние окон
└── ImGui draw commands
        │
        ▼
HOW
│
├── Graphics contracts
├── Direct3D9 backend
└── GPU resources

Это позволяет в дальнейшем добавить другой backend, не переписывая пользовательские окна и Core.

Например, потенциально:

VBImGuiDx9
      │
      ├── Direct3D9
      │
      ├── Direct3D11    ← потенциально
      │
      └── OpenGL        ← потенциально

Но эти backend'ы пока не реализованы и не являются частью текущего API.
