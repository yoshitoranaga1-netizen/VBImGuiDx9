Option Strict On
Option Explicit On
Option Infer On

Imports System.Numerics
Imports VBImGuiDx9.Sample.Diagnostics
Imports VBImGuiDx9.VBImGuiDx9.Core.ImGuiWindows

Namespace VBImGuiDx9.Sample.UI

    ''' <summary>
    ''' Composes and registers all ImGui windows belonging
    ''' to the Sample application.
    ''' </summary>
    Public NotInheritable Class SampleWindowSet
        Implements IDisposable

        Private ReadOnly _windowManager As ImGuiWindowManager
        Private ReadOnly _windowMenu As WindowMenu
        Private ReadOnly _mainWindow As MainWindow
        Private ReadOnly _settingsWindow As SettingsWindow
        Private ReadOnly _debugWindow As DebugWindow
        Private ReadOnly _imguiDemoWindow As ImGuiDemoWindow
        Private ReadOnly _profiler As FrameProfiler
        Private ReadOnly _fontService As FontService

        Private _initialized As Boolean
        Private _disposed As Boolean

        Public Sub New(
                    windowManager As ImGuiWindowManager,
                    profiler As FrameProfiler,
                    fontService As FontService)

            If windowManager Is Nothing Then
                Throw New ArgumentNullException(
            NameOf(windowManager))
            End If

            If profiler Is Nothing Then
                Throw New ArgumentNullException(
            NameOf(profiler))
            End If

            If fontService Is Nothing Then
                Throw New ArgumentNullException(
            NameOf(fontService))
            End If

            _windowManager = windowManager
            _profiler = profiler
            _fontService = fontService

            _mainWindow =
                    New MainWindow()

            _settingsWindow =
                    New SettingsWindow(
                        _windowManager)

            _debugWindow =
                    New DebugWindow(
                        _windowManager,
                        _profiler)

            _imguiDemoWindow =
                    New ImGuiDemoWindow(
                        _fontService)

            _windowMenu =
                    New WindowMenu(
                        _windowManager)

        End Sub

        Public Sub Initialize()

            If _disposed Then
                Throw New ObjectDisposedException(
                    NameOf(SampleWindowSet))
            End If

            If _initialized Then
                Return
            End If

            RegisterMainWindow()
            RegisterSettingsWindow()
            RegisterDebugWindow()
            RegisterImGuiDemoWindow()

            _initialized = True

        End Sub

        Public Sub RenderMenu()

            If _disposed Then
                Return
            End If

            If Not _initialized Then
                Return
            End If

            _windowMenu.Render()

        End Sub

        Private Sub RegisterImGuiDemoWindow()

            Dim state As New ImGuiWindowState(
        "imgui_demo",
        "ImGui Demo")

            state.Position =
        New Vector2(
            180.0F,
            120.0F)

            state.Size =
        New Vector2(
            900.0F,
            650.0F)

            _windowManager.Register(
        state,
        AddressOf _imguiDemoWindow.Render)

        End Sub

        Private Sub RegisterMainWindow()

            Dim state As New ImGuiWindowState(
                "main",
                "Main")

            state.Position =
                New Vector2(
                    40.0F,
                    40.0F)

            state.Size =
                New Vector2(
                    600.0F,
                    450.0F)

            _windowManager.Register(
                state,
                AddressOf _mainWindow.Render)

        End Sub

        Private Sub RegisterSettingsWindow()

            Dim state As New ImGuiWindowState(
                "settings",
                "Settings")

            state.Position =
                New Vector2(
                    660.0F,
                    40.0F)

            state.Size =
                New Vector2(
                    400.0F,
                    300.0F)

            _windowManager.Register(
                state,
                AddressOf _settingsWindow.Render)

        End Sub

        Private Sub RegisterDebugWindow()

            Dim state As New ImGuiWindowState(
                "debug",
                "Debug")

            state.Position =
                New Vector2(
                    40.0F,
                    530.0F)

            state.Size =
                New Vector2(
                    500.0F,
                    250.0F)

            _windowManager.Register(
                state,
                AddressOf _debugWindow.Render)

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            _disposed = True

        End Sub

    End Class

End Namespace