Option Strict On
Option Explicit On
Option Infer On

Imports ImGuiNET
Imports VBImGuiDx9.Sample.Diagnostics
Imports VBImGuiDx9.VBImGuiDx9.Core.ImGuiWindows

Namespace VBImGuiDx9.Sample.UI

    Public NotInheritable Class DebugWindow

        Private ReadOnly _profiler As FrameProfiler

        Private ReadOnly _windowManager As ImGuiWindowManager

        Public Sub New(
                        windowManager As ImGuiWindowManager,
                        profiler As FrameProfiler)

            If windowManager Is Nothing Then
                Throw New ArgumentNullException(NameOf(windowManager))
            End If

            If profiler Is Nothing Then
                Throw New ArgumentNullException(NameOf(profiler))
            End If

            _windowManager = windowManager
            _profiler = profiler

        End Sub


        Public Sub Render()

            ImGui.Text("Debug")
            ImGui.Separator()
            ImGui.Text("ImGui Window Manager")

            ImGui.Text(
                "Registered windows: " &
                _windowManager.Count.ToString())

            ImGui.Text("DX9 backend: OK")
            ImGui.Text("ImGui frame controller: OK")
            ImGui.Text("Window state: independent")

            ImGui.Separator()
            ImGui.Text("Frame profiler")

            ImGui.Text(
    $"Samples: {_profiler.SampleCountCurrent}")

            ImGui.Text(
    $"Average: {_profiler.AverageFrameMs:F3} ms")

            ImGui.Text(
    $"P50: {_profiler.P50FrameMs:F3} ms")

            ImGui.Text(
    $"P95: {_profiler.P95FrameMs:F3} ms")

            ImGui.Text(
    $"P99: {_profiler.P99FrameMs:F3} ms")

            ImGui.Text(
    $"Min: {_profiler.MinFrameMs:F3} ms")

            ImGui.Text(
    $"Max: {_profiler.MaxFrameMs:F3} ms")

            ImGui.Text(
    $"Average FPS: {_profiler.AverageFps:F1}")

            ImGui.Separator()

            ImGui.Text(
    $"Input: {_profiler.InputMs:F3} ms")

            ImGui.Text(
    $"Device: {_profiler.DeviceMs:F3} ms")

            ImGui.Text(
    $"ImGui Build: {_profiler.ImGuiBuildMs:F3} ms")

            ImGui.Text(
    $"ImGui Render: {_profiler.ImGuiRenderMs:F3} ms")

            ImGui.Text(
    $"DX9 Draw: {_profiler.Dx9DrawMs:F3} ms")

            ImGui.Text(
    $"Present: {_profiler.PresentMs:F3} ms")

            ImGui.Text(
    $"Other: {_profiler.OtherMs:F3} ms")

        End Sub

    End Class

End Namespace
