Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Numerics
Imports ImGuiNET

Namespace VBImGuiDx9.Core

    ''' <summary>
    ''' Owns the lifetime and basic configuration of the Dear ImGui context.
    '''
    ''' This class is independent from Direct3D9.
    ''' It is responsible for creation, initial IO configuration,
    ''' style configuration and initial font-atlas construction.
    ''' </summary>
    Public NotInheritable Class ImGuiContextManager
        Implements IDisposable

#Region "Fields"

        ''' <summary>
        ''' Native pointer to the Dear ImGui context.
        ''' </summary>
        Private _context As IntPtr

        Private _initialized As Boolean
        Private _disposed As Boolean

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets whether the ImGui context is initialized.
        ''' </summary>
        Public ReadOnly Property IsInitialized As Boolean

            Get
                Return _initialized AndAlso Not _disposed
            End Get

        End Property

        ''' <summary>
        ''' Gets the native ImGui context pointer.
        ''' </summary>
        Public ReadOnly Property Context As IntPtr

            Get

                ThrowIfDisposed()

                If Not _initialized Then
                    Throw New InvalidOperationException(
                        "ImGui context has not been initialized.")
                End If

                Return _context

            End Get

        End Property

        ''' <summary>
        ''' Gets the ImGui IO interface.
        ''' </summary>
        Public ReadOnly Property IO As ImGuiIOPtr

            Get

                ThrowIfDisposed()

                If Not _initialized Then
                    Throw New InvalidOperationException(
                        "ImGui context has not been initialized.")
                End If

                Return ImGui.GetIO()

            End Get

        End Property

#End Region

#Region "Initialization"

        ''' <summary>
        ''' Creates and initializes the Dear ImGui context.
        ''' </summary>
        Public Sub Initialize()

            ThrowIfDisposed()

            If _initialized Then
                Return
            End If

            _context =
                ImGui.CreateContext()
            Dim io As ImGuiIOPtr = ImGui.GetIO()

            If _context = IntPtr.Zero Then

                Throw New InvalidOperationException(
                    "ImGui.CreateContext returned a null context.")

            End If

            ImGui.SetCurrentContext(
                _context)

            ConfigureIO()

            ConfigureStyle()

            BuildFontAtlas()

            _initialized = True

        End Sub

#End Region

#Region "IO"

        ''' <summary>
        ''' Configures the initial ImGui IO state.
        '''
        ''' Actual display size, timing and input events
        ''' will be supplied by the host/frame layer later.
        ''' </summary>
        Private Sub ConfigureIO()

            Dim io As ImGuiIOPtr =
                ImGui.GetIO()

            io.DisplaySize =
                New Vector2(
                    1280.0F,
                    720.0F)

        End Sub

#End Region

#Region "Style"

        ''' <summary>
        ''' Configures the default ImGui style.
        ''' </summary>
        Private Sub ConfigureStyle()

            ImGui.StyleColorsDark()

        End Sub

#End Region

#Region "Font Atlas"

        ''' <summary>
        ''' Builds the default Dear ImGui font atlas.
        '''
        ''' The pixel data is intentionally kept alive at this stage.
        ''' The Direct3D9 ImGui renderer will later upload this data
        ''' into a GPU texture and assign the resulting texture ID
        ''' to ImGui.
        ''' </summary>
        Private Sub BuildFontAtlas()

            Dim io As ImGuiIOPtr =
                ImGui.GetIO()

            Dim pixels As IntPtr
            Dim width As Integer
            Dim height As Integer
            Dim bytesPerPixel As Integer

            io.Fonts.GetTexDataAsRGBA32(
                pixels,
                width,
                height,
                bytesPerPixel)

            If pixels = IntPtr.Zero Then

                Throw New InvalidOperationException(
                    "Dear ImGui font atlas returned a null pixel pointer.")

            End If

            If width <= 0 OrElse
               height <= 0 Then

                Throw New InvalidOperationException(
                    "Dear ImGui font atlas has an invalid size.")

            End If

            If bytesPerPixel <= 0 Then

                Throw New InvalidOperationException(
                    "Dear ImGui font atlas returned an invalid pixel format.")

            End If

        End Sub

        ''' <summary>
        ''' Rebuilds the Dear ImGui font atlas.
        '''
        ''' Existing font configuration is preserved.
        ''' The Direct3D9 renderer must recreate its font texture
        ''' after this method completes.
        ''' </summary>
        Public Sub RebuildFontAtlas()

            ThrowIfDisposed()

            If Not _initialized Then
                Throw New InvalidOperationException(
            "ImGui context has not been initialized.")
            End If

            Dim io As ImGuiIOPtr =
        ImGui.GetIO()

            Dim pixels As IntPtr
            Dim width As Integer
            Dim height As Integer
            Dim bytesPerPixel As Integer

            io.Fonts.GetTexDataAsRGBA32(
        pixels,
        width,
        height,
        bytesPerPixel)

            If pixels = IntPtr.Zero Then

                Throw New InvalidOperationException(
            "Dear ImGui font atlas returned a null pixel pointer.")

            End If

            If width <= 0 OrElse
       height <= 0 Then

                Throw New InvalidOperationException(
            "Dear ImGui font atlas has an invalid size.")

            End If

            If bytesPerPixel <> 4 Then

                Throw New InvalidOperationException(
            "Dear ImGui font atlas is not RGBA32.")

            End If

        End Sub

#End Region

#Region "Private"

        ''' <summary>
        ''' Throws when the context manager has already been disposed.
        ''' </summary>
        Private Sub ThrowIfDisposed()

            If _disposed Then

                Throw New ObjectDisposedException(
                    NameOf(ImGuiContextManager))

            End If

        End Sub

#End Region

#Region "Dispose"

        ''' <summary>
        ''' Destroys the Dear ImGui context.
        ''' </summary>
        Public Sub Dispose() _
            Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            If _initialized AndAlso
               _context <> IntPtr.Zero Then

                ImGui.SetCurrentContext(
                    _context)

                ImGui.DestroyContext(
                    _context)

            End If

            _context = IntPtr.Zero

            _initialized = False
            _disposed = True

        End Sub

#End Region

    End Class

End Namespace