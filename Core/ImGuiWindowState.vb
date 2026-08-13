Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Numerics
Imports ImGuiNET

Namespace VBImGuiDx9.Core.ImGuiWindows

    ''' <summary>
    ''' Persistent state of one logical ImGui window.
    ''' </summary>
    Public NotInheritable Class ImGuiWindowState

        Public Sub New(
            id As String,
            title As String,
            Optional flags As ImGuiWindowFlags = ImGuiWindowFlags.None)

            If String.IsNullOrWhiteSpace(id) Then
                Throw New ArgumentException(
                    "Window id cannot be empty.",
                    NameOf(id))
            End If

            If String.IsNullOrWhiteSpace(title) Then
                Throw New ArgumentException(
                    "Window title cannot be empty.",
                    NameOf(title))
            End If

            Me.Id = id
            Me.Title = title
            Me.Flags = flags

            Me.Visible = True
            Me.Collapsed = False

            Me.Position =
                New Vector2(
                    40.0F,
                    40.0F)

            Me.Size =
                New Vector2(
                    400.0F,
                    300.0F)

            Me.UseInitialPosition = True
            Me.UseInitialSize = True

        End Sub

        ''' <summary>
        ''' Gets the stable logical identifier of the window.
        ''' </summary>
        Public ReadOnly Property Id As String

        ''' <summary>
        ''' Gets or sets the visible title of the window.
        ''' </summary>
        Public Property Title As String

        ''' <summary>
        ''' Gets or sets whether the window is visible.
        ''' </summary>
        Public Property Visible As Boolean

        ''' <summary>
        ''' Gets or sets whether the window is collapsed.
        ''' </summary>
        Public Property Collapsed As Boolean

        ''' <summary>
        ''' Gets or sets the ImGui window flags.
        ''' </summary>
        Public Property Flags As ImGuiWindowFlags

        ''' <summary>
        ''' Gets or sets the last known window position.
        ''' </summary>
        Public Property Position As Vector2

        ''' <summary>
        ''' Gets or sets the last known window size.
        ''' </summary>
        Public Property Size As Vector2

        ''' <summary>
        ''' Indicates that the initial position should be supplied to ImGui.
        ''' </summary>
        Public Property UseInitialPosition As Boolean

        ''' <summary>
        ''' Indicates that the initial size should be supplied to ImGui.
        ''' </summary>
        Public Property UseInitialSize As Boolean

    End Class

End Namespace