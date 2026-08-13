Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports ImGuiNET
Imports VBImGuiDx9.VBImGuiDx9.Core.ImGuiWindows

Namespace VBImGuiDx9.Sample.UI

    ''' <summary>
    ''' Global application menu for controlling logical ImGui windows.
    ''' </summary>
    Public NotInheritable Class WindowMenu

        Private ReadOnly _windowManager As ImGuiWindowManager

        Public Sub New(
            windowManager As ImGuiWindowManager)

            If windowManager Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(windowManager))
            End If

            _windowManager = windowManager

        End Sub

        Public Sub Render()

            If Not ImGui.BeginMainMenuBar() Then
                Return
            End If

            If ImGui.BeginMenu("Windows") Then

                RenderWindowItem(
                    "ImGui Demo",
                    "imgui_demo")

                RenderWindowItem(
                    "Main",
                    "main")

                RenderWindowItem(
                    "Settings",
                    "settings")

                RenderWindowItem(
                    "Debug",
                    "debug")

                ImGui.EndMenu()

            End If

            ImGui.EndMainMenuBar()

        End Sub

        Private Sub RenderWindowItem(
    label As String,
    windowId As String)

            Dim state As ImGuiWindowState =
        _windowManager.GetState(windowId)

            If state Is Nothing Then
                Return
            End If

            Dim menuLabel As String

            If state.Visible Then
                menuLabel = "[x] " & label
            Else
                menuLabel = "[ ] " & label
            End If

            If ImGui.MenuItem(menuLabel) Then

                _windowManager.SetVisible(
            windowId,
            Not state.Visible)

            End If

        End Sub

    End Class

End Namespace