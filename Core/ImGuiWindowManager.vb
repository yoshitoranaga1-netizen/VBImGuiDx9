Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Collections.Generic
Imports System.Numerics
Imports ImGuiNET
Imports DearImGui = ImGuiNET.ImGui

Namespace VBImGuiDx9.Core.ImGuiWindows

    ''' <summary>
    ''' Owns the lifecycle of logical ImGui windows inside one ImGui context.
    ''' Does not own the ImGui frame lifecycle and does not touch Direct3D9.
    ''' </summary>
    Public NotInheritable Class ImGuiWindowManager
        Implements IDisposable

        Private NotInheritable Class WindowEntry

            Public ReadOnly State As ImGuiWindowState
            Public ReadOnly Render As Action

            Public Sub New(
                state As ImGuiWindowState,
                render As Action)

                Me.State = state
                Me.Render = render

            End Sub

        End Class

        Private ReadOnly _windows As New List(Of WindowEntry)()
        Private ReadOnly _ids As New HashSet(Of String)(
            StringComparer.Ordinal)

        Private _disposed As Boolean

        Public ReadOnly Property Count As Integer
            Get

                ThrowIfDisposed()

                Return _windows.Count

            End Get
        End Property

        Public Sub Register(
            state As ImGuiWindowState,
            render As Action)

            ThrowIfDisposed()

            If state Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(state))
            End If

            If render Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(render))
            End If

            If Not _ids.Add(state.Id) Then
                Throw New InvalidOperationException(
                    "An ImGui window with id '" &
                    state.Id &
                    "' is already registered.")
            End If

            _windows.Add(
                New WindowEntry(
                    state,
                    render))

        End Sub

        Public Function Contains(
            id As String) As Boolean

            ThrowIfDisposed()

            If String.IsNullOrWhiteSpace(id) Then
                Return False
            End If

            Return _ids.Contains(id)

        End Function

        Public Function GetState(
            id As String) As ImGuiWindowState

            ThrowIfDisposed()

            For Each entry As WindowEntry In _windows

                If String.Equals(
                    entry.State.Id,
                    id,
                    StringComparison.Ordinal) Then

                    Return entry.State

                End If

            Next

            Return Nothing

        End Function

        Public Function GetStates() As IReadOnlyList(Of ImGuiWindowState)

            ThrowIfDisposed()

            Dim states As New List(Of ImGuiWindowState)(
        _windows.Count)

            For Each entry As WindowEntry In _windows

                states.Add(
            entry.State)

            Next

            Return states

        End Function

        Public Function SetVisible(
            id As String,
            visible As Boolean) As Boolean

            Dim state As ImGuiWindowState =
                GetState(id)

            If state Is Nothing Then
                Return False
            End If

            state.Visible = visible

            Return True

        End Function

        Public Function SetCollapsed(
            id As String,
            collapsed As Boolean) As Boolean

            Dim state As ImGuiWindowState =
                GetState(id)

            If state Is Nothing Then
                Return False
            End If

            state.Collapsed = collapsed

            Return True

        End Function

        Public Sub RenderAll()

            ThrowIfDisposed()

            For Each entry As WindowEntry In _windows

                Dim state As ImGuiWindowState =
                    entry.State

                If Not state.Visible Then
                    Continue For
                End If

                If state.UseInitialPosition Then

                    DearImGui.SetNextWindowPos(
                        state.Position,
                        ImGuiCond.FirstUseEver)

                End If

                If state.UseInitialSize Then

                    DearImGui.SetNextWindowSize(
                        state.Size,
                        ImGuiCond.FirstUseEver)

                End If

                ' ImGui.Begin writes the close state
                ' into this local variable.
                Dim visible As Boolean = True

                Dim opened As Boolean =
                    DearImGui.Begin(
                        state.Title,
                        visible,
                        state.Flags)

                If opened Then

                    entry.Render.Invoke()

                End If

                ' Capture the latest geometry.
                state.Position =
                    DearImGui.GetWindowPos()

                state.Size =
                    DearImGui.GetWindowSize()

                ' Capture collapsed state.
                state.Collapsed =
                    DearImGui.IsWindowCollapsed()

                DearImGui.End()

                ' User clicked the X.
                If Not visible Then

                    state.Visible = False

                End If

                state.UseInitialPosition = False
                state.UseInitialSize = False

            Next

        End Sub

        Private Sub ThrowIfDisposed()

            If _disposed Then

                Throw New ObjectDisposedException(
                    NameOf(ImGuiWindowManager))

            End If

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            _windows.Clear()
            _ids.Clear()

            _disposed = True

        End Sub

    End Class

End Namespace