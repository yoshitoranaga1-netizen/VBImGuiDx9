# Быстрый старт

## Overview

VBImGuiDx9 позволяет создавать интерфейсы на VB.NET с использованием Dear ImGui,
ImGui.NET и Direct3D9.

Приложение состоит из нескольких основных частей:

- ImGui context;
- frame controller;
- window manager;
- Direct3D9 graphics device;
- Direct3D9 graphics context;
- ImGui Direct3D9 renderer.

Минимальная схема запуска:

Application
    ↓
ImGuiContextManager
    ↓
ImGuiFrameController
    ↓
Dx9GraphicsDevice
    ↓
Dx9GraphicsContext
    ↓
Dx9ImGuiRenderer
    ↓
Frame loop

Requirements

Для работы проекта необходимы:

Windows;
.NET 9;
VB.NET;
ImGui.NET;
Vortice.Direct3D9;
Direct3D9-compatible graphics device.

Проект использует Option Strict On, Option Explicit On и Option Infer On.

1. Create the ImGui context

Первым создаётся ImGuiContextManager.

Dim imguiContext As New ImGuiContextManager()

imguiContext.Initialize()

После Initialize() Dear ImGui context создан и готов к использованию.

Проверить состояние можно через:

If imguiContext.IsInitialized Then

    ' ImGui is ready.

End If
2. Create the Direct3D9 device

После создания ImGui context создаётся Direct3D9 device.

Dim deviceOptions As New DeviceOptions()

deviceOptions.WindowHandle =
    Handle

deviceOptions.Width =
    ClientSize.Width

deviceOptions.Height =
    ClientSize.Height

deviceOptions.Windowed =
    True

deviceOptions.EnableVSync =
    False

deviceOptions.EnableMultithreading =
    False

Dim graphicsDevice As New Dx9GraphicsDevice(
    deviceOptions)

Dx9GraphicsDevice отвечает за создание и жизненный цикл Direct3D9 device.

3. Create the graphics context

Graphics context создаётся из graphics device:

Dim graphicsContext As Dx9GraphicsContext =
    DirectCast(
        graphicsDevice.CreateGraphicsContext(),
        Dx9GraphicsContext)

Dx9GraphicsContext предоставляет операции, необходимые для настройки Direct3D9 render state и выполнения draw commands.

4. Create the ImGui renderer

После создания graphics device и graphics context создаётся ImGui renderer:

Dim imguiRenderer As New Dx9ImGuiRenderer(
    graphicsDevice,
    graphicsContext)

imguiRenderer.Initialize()

Renderer создаёт необходимые Direct3D9 resources для Dear ImGui, включая:

font texture;
vertex buffer;
index buffer.
5. Create the frame controller

После инициализации ImGui context приложение использует ImGuiFrameController
для управления жизненным циклом ImGui frame.

Основной цикл:

BeginFrame
    ↓
ImGui.NewFrame
    ↓
Render UI
    ↓
EndFrame
    ↓
ImGui.Render
    ↓
ImDrawData
    ↓
Dx9ImGuiRenderer

Пример:

_frameController.BeginFrame()

' Render application UI here.

_frameController.EndFrame()
6. Create a window

Для управления окнами используется ImGuiWindowManager.

Создаём состояние окна:

Dim state As New ImGuiWindowState(
    "main",
    "Main")

Задаём начальную позицию:

state.Position =
    New Vector2(
        40.0F,
        40.0F)

Задаём начальный размер:

state.Size =
    New Vector2(
        600.0F,
        450.0F)

Регистрируем renderer окна:

windowManager.Register(
    state,
    AddressOf RenderMainWindow)
7. Render the window

Содержимое окна представляет собой обычный VB.NET метод:

Private Sub RenderMainWindow()

    ImGui.Text(
        "Hello from VBImGuiDx9")

    If ImGui.Button(
        "Click me") Then

        ' Button clicked.

    End If

End Sub

ImGuiWindowManager самостоятельно управляет:

ImGui.Begin();
ImGui.End();
visibility;
position;
size;
collapsed state.

Поэтому renderer пользовательского окна не должен вручную управлять его жизненным циклом.

8. Multiple windows

Можно зарегистрировать любое количество независимых окон.

Например:

Dim mainState As New ImGuiWindowState(
    "main",
    "Main")

mainState.Position =
    New Vector2(
        40.0F,
        40.0F)

mainState.Size =
    New Vector2(
        600.0F,
        450.0F)

windowManager.Register(
    mainState,
    AddressOf RenderMainWindow)

И второе:

Dim settingsState As New ImGuiWindowState(
    "settings",
    "Settings")

settingsState.Position =
    New Vector2(
        660.0F,
        40.0F)

settingsState.Size =
    New Vector2(
        400.0F,
        300.0F)

windowManager.Register(
    settingsState,
    AddressOf RenderSettingsWindow)

Каждое окно имеет собственное состояние.

9. Rendering controls

Пользовательский интерфейс создаётся непосредственно через ImGui.NET.

Например:

ImGui.Text("Settings")

Dim enabled As Boolean = True

ImGui.Checkbox(
    "Enable feature",
    enabled)

Dim value As Single = 50.0F

ImGui.SliderFloat(
    "Value",
    value,
    0.0F,
    100.0F)

If ImGui.Button(
    "Apply") Then

    ' Apply settings.

End If

Можно комбинировать элементы в одном окне:

Settings
──────────────────────────────

[✓] Enable feature

Value
[────────●────────] 50

[ Apply ]
10. Tabs and grouped interfaces

Для больших интерфейсов рекомендуется группировать функциональность.

Например:

If ImGui.BeginTabBar(
    "SettingsTabs") Then

    If ImGui.BeginTabItem(
        "General") Then

        RenderGeneralSettings()

        ImGui.EndTabItem()

    End If

    If ImGui.BeginTabItem(
        "Graphics") Then

        RenderGraphicsSettings()

        ImGui.EndTabItem()

    End If

    If ImGui.BeginTabItem(
        "Debug") Then

        RenderDebugSettings()

        ImGui.EndTabItem()

    End If

    ImGui.EndTabBar()

End If

Это позволяет строить сложные интерфейсы без создания огромного вертикального списка элементов.

11. Fonts

TTF-файлы располагаются в:

Assets
└── Fonts
    ├── Inter.ttf
    ├── Roboto.ttf
    └── Segoe UI.ttf

FontService автоматически обнаруживает TTF-файлы.

Для каждого найденного шрифта создаются варианты:

13 px
16 px
22 px

Пример получения шрифта:

Dim font As ImFontPtr =
    fontService.GetFont(
        "Inter",
        16.0F)

Использование:

ImGui.PushFont(font)

ImGui.Text(
    "Custom font text")

ImGui.PopFont()
12. Cyrillic text

При загрузке font variants используется диапазон кириллических glyphs.

Однако сам TTF должен содержать соответствующие glyphs.

Например, если файл содержит кириллицу:

Roboto.ttf

русский текст будет отображаться:

Съешь ещё этих мягких французских булок, да выпей чаю.

Если конкретный TTF не содержит кириллицу, выбор GetGlyphRangesCyrillic() не добавляет отсутствующие glyphs в сам файл шрифта.

13. Complete application structure

Типичное приложение может выглядеть следующим образом:

Public Class MainForm

    Private _imguiContext As ImGuiContextManager
    Private _frameController As ImGuiFrameController

    Private _graphicsDevice As Dx9GraphicsDevice
    Private _graphicsContext As Dx9GraphicsContext
    Private _imguiRenderer As Dx9ImGuiRenderer

    Private _windowManager As ImGuiWindowManager

    Private Sub InitializeApplication()

        _imguiContext =
            New ImGuiContextManager()

        _imguiContext.Initialize()

        _frameController =
            New ImGuiFrameController(
                _imguiContext)

        Dim options As New DeviceOptions()

        options.WindowHandle =
            Handle

        options.Width =
            ClientSize.Width

        options.Height =
            ClientSize.Height

        options.Windowed =
            True

        options.EnableVSync =
            False

        options.EnableMultithreading =
            False

        _graphicsDevice =
            New Dx9GraphicsDevice(
                options)

        _graphicsContext =
            DirectCast(
                _graphicsDevice.CreateGraphicsContext(),
                Dx9GraphicsContext)

        _imguiRenderer =
            New Dx9ImGuiRenderer(
                _graphicsDevice,
                _graphicsContext)

        _imguiRenderer.Initialize()

        _windowManager =
            New ImGuiWindowManager()

    End Sub

End Class

The exact initialization order used by the Sample should be treated as the reference implementation for the current project.

14. Frame rendering

A frame consists conceptually of three stages:

1. Begin frame

2. Build UI

3. Render frame

Example:

_frameController.BeginFrame()

_windowManager.RenderAll()

_frameController.EndFrame()

Dim drawData As ImDrawDataPtr =
    _frameController.DrawData

_imguiRenderer.RenderDrawData(
    drawData)

The application is responsible for integrating this sequence into its WinForms/DX9 render loop.

15. Shutdown

Resources should be released in the reverse order of their creation.

Conceptually:

Dx9ImGuiRenderer
        ↓
Dx9GraphicsContext
        ↓
Dx9GraphicsDevice
        ↓
ImGuiFrameController
        ↓
ImGuiContextManager

Each disposable component should be disposed only once.

16. Device reset

Direct3D9 resources may have to be recreated after a device reset.

The application should distinguish between:

DeviceLost
DeviceNotReset
Operational

and perform the appropriate reset sequence.

ImGui-specific Direct3D9 resources must also be invalidated and recreated when required.

17. Sample project

The repository contains a Sample application.

The Sample is intended to demonstrate:

basic controls;
selection controls;
sliders;
tabs;
layouts;
fonts;
window management;
diagnostics;
Direct3D9 rendering.

The Sample is also used as a practical verification environment for the library.

18. Recommended application architecture

For a larger application, avoid placing all UI code into one renderer.

Prefer:

UI
├── MainWindow.vb
├── SettingsWindow.vb
├── DebugWindow.vb
├── ToolsWindow.vb
└── Components
    ├── SettingsPanel.vb
    ├── StatisticsPanel.vb
    └── ...

Each window should expose a small rendering method:

Public Sub Render()

    ' Window-specific UI.

End Sub

The ImGuiWindowManager can then own the lifetime and visibility of those windows.

19. First application checklist

Before running an application, verify:

ImGui context initialized;
Direct3D9 device initialized;
Direct3D9 context created;
ImGui renderer initialized;
font atlas created;
windows registered;
frame controller initialized;
frame loop running.

The expected sequence is:

✓ ImGui Context
✓ Graphics Device
✓ Graphics Context
✓ ImGui Renderer
✓ Font Atlas
✓ Window Manager
✓ Frame Loop
20. Next steps

After completing the basic setup, see:

ARCHITECTURE.md — project architecture;
FONTS.md — font management;
RENDERING.md — Direct3D9 rendering;
CONTRIBUTING.md — contributing to the project.


> Примечание: `VBImGuiDx9.Sample` является reference implementation для текущего порядка инициализации. Конкретные вызовы должны соответствовать актуальному API библиотеки.
