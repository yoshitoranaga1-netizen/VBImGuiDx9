Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Numerics
Imports ImGuiNET


Namespace VBImGuiDx9.Sample.UI

    Public NotInheritable Class MainWindow

        Private _testCheckbox As Boolean
        Private _testValue As Single = 50.0F

        Public Sub Render()

            ImGui.Text("VBImGuiDx9")
            ImGui.Separator()

            ImGui.Text("Dear ImGui context is working.")
            ImGui.Text("ImGui.NewFrame() is working.")
            ImGui.Text("ImGui.Render() is working.")

            ImGui.Separator()
            ImGui.Text("DX9 backend rendering is working.")
            ImGui.Separator()

            If ImGui.Button(
                "Test Button",
                New Vector2(160.0F, 40.0F)) Then

                Debug.WriteLine("MAIN BUTTON CLICK")
            End If

            ImGui.Checkbox(
                "Test checkbox",
                _testCheckbox)

            ImGui.SliderFloat(
                "Test slider",
                _testValue,
                0.0F,
                100.0F)

            ImGui.Text(
                "Checkbox: " &
                _testCheckbox.ToString())

            ImGui.Text(
                "Value: " &
                _testValue.ToString("F1"))

        End Sub

    End Class

End Namespace
