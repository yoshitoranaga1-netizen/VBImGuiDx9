Option Strict On
Option Explicit On
Option Infer On

Imports VBImGuiDx9.VBImGuiDx9.Contracts

Namespace VBImGuiDx9.Core

    ''' <summary>
    ''' Coordinates the lifetime of a rendering frame.
    ''' </summary>
    Public NotInheritable Class Renderer
        Implements IDisposable

#Region "Fields"

        Private ReadOnly _device As IGraphicsDevice
        Private ReadOnly _context As RenderContext
        Private ReadOnly _options As RendererOptions
        Private ReadOnly _statistics As FrameStatistics

        Private _frameActive As Boolean
        Private _disposed As Boolean

#End Region

#Region "Constructors"

        ''' <summary>
        ''' Initializes the renderer.
        ''' </summary>
        ''' <param name="device">Graphics device.</param>
        ''' <param name="options">Renderer options.</param>
        Public Sub New(
            device As IGraphicsDevice,
            Optional options As RendererOptions = Nothing)

            If device Is Nothing Then
                Throw New ArgumentNullException(NameOf(device))
            End If

            _device = device
            _options = If(options, New RendererOptions())
            _statistics = New FrameStatistics()
            _context = New RenderContext(
                _device.CreateGraphicsContext())

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the graphics device.
        ''' </summary>
        Public ReadOnly Property Device As IGraphicsDevice
            Get
                ThrowIfDisposed()
                Return _device
            End Get
        End Property

        ''' <summary>
        ''' Gets the renderer options.
        ''' </summary>
        Public ReadOnly Property Options As RendererOptions
            Get
                ThrowIfDisposed()
                Return _options
            End Get
        End Property

        ''' <summary>
        ''' Gets the current frame statistics.
        ''' </summary>
        Public ReadOnly Property Statistics As FrameStatistics
            Get
                ThrowIfDisposed()
                Return _statistics
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether a frame is currently active.
        ''' </summary>
        Public ReadOnly Property IsFrameActive As Boolean
            Get
                Return _frameActive
            End Get
        End Property

#End Region

#Region "Frame"

        ''' <summary>
        ''' Begins rendering a new frame.
        ''' </summary>
        Public Sub BeginFrame()

            ThrowIfDisposed()

            If _frameActive Then
                Throw New InvalidOperationException(
                    "A rendering frame is already active.")
            End If

            _statistics.Reset()
            _context.BeginFrame()

            _frameActive = True

        End Sub

        ''' <summary>
        ''' Ends the current rendering frame.
        ''' </summary>
        Public Sub EndFrame()

            ThrowIfDisposed()

            If Not _frameActive Then
                Throw New InvalidOperationException(
                    "No rendering frame is active.")
            End If

            _context.EndFrame()
            _frameActive = False

        End Sub

        ''' <summary>
        ''' Presents the completed frame.
        ''' </summary>
        Public Sub Present()

            ThrowIfDisposed()

            If _frameActive Then
                Throw New InvalidOperationException(
                    "The rendering frame must be ended before presenting.")
            End If

            _context.Present()

        End Sub

        ''' <summary>
        ''' Clears the current render target.
        ''' </summary>
        ''' <param name="color">ARGB32 clear color.</param>
        Public Sub Clear(color As UInteger)

            ThrowIfDisposed()

            If Not _frameActive Then
                Throw New InvalidOperationException(
                    "A rendering frame must be active before clearing.")
            End If

            _context.Clear(color)

        End Sub

#End Region

#Region "Private Methods"

        Private Sub ThrowIfDisposed()

            If _disposed Then
                Throw New ObjectDisposedException(NameOf(Renderer))
            End If

        End Sub

#End Region

#Region "Dispose"

        ''' <summary>
        ''' Releases renderer resources.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            If _frameActive Then
                Try
                    _context.EndFrame()
                Catch
                    ' The context is being disposed.
                End Try

                _frameActive = False
            End If

            _context.Dispose()
            _disposed = True

        End Sub

#End Region

    End Class

End Namespace