Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports ImGuiNET
Imports VBImGuiDx9.VBImGuiDx9.Core.ImGuiWindows

Namespace VBImGuiDx9.Sample.UI

    Public NotInheritable Class SettingsWindow

        Private ReadOnly _windowManager As ImGuiWindowManager

        Public Sub New(windowManager As ImGuiWindowManager)

            If windowManager Is Nothing Then
                Throw New ArgumentNullException(NameOf(windowManager))
            End If

            _windowManager = windowManager
        End Sub

        Public Sub Render()

            ImGui.Text("Settings")
            ImGui.Separator()
            ImGui.Text("Window Manager")
            ImGui.Spacing()

            RenderVisibilityOption("Main window", "main")
            RenderVisibilityOption("Debug window", "debug")

        End Sub

        Private Sub RenderVisibilityOption(
                            label As String,
                            windowId As String)

            Dim state As ImGuiWindowState =
        _windowManager.GetState(windowId)

            If state Is Nothing Then
                Return
            End If

            Dim visible As Boolean =
                            state.Visible

            If ImGui.Checkbox(
                            label,
                            visible) Then

                _windowManager.SetVisible(
                                windowId,
                                visible)

            End If

        End Sub

    End Class

End Namespace
