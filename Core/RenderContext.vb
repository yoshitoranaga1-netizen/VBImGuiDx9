Option Strict On
Option Explicit On
Option Infer On

Imports VBImGuiDx9.VBImGuiDx9.Contracts

Namespace VBImGuiDx9.Core

    ''' <summary>
    ''' Provides the high-level rendering context used by the renderer.
    ''' </summary>
    Public NotInheritable Class RenderContext
        Implements IDisposable

#Region "Fields"

        Private ReadOnly _context As IGraphicsContext
        Private _disposed As Boolean

#End Region

#Region "Constructors"

        ''' <summary>
        ''' Initializes a new rendering context.
        ''' </summary>
        ''' <param name="context">Low-level graphics context.</param>
        Public Sub New(context As IGraphicsContext)

            If context Is Nothing Then
                Throw New ArgumentNullException(NameOf(context))
            End If

            _context = context

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the underlying graphics context.
        ''' </summary>
        Public ReadOnly Property Context As IGraphicsContext
            Get
                ThrowIfDisposed()
                Return _context
            End Get
        End Property

#End Region

#Region "Frame"

        ''' <summary>
        ''' Begins a new frame.
        ''' </summary>
        Public Sub BeginFrame()

            ThrowIfDisposed()
            _context.BeginFrame()

        End Sub

        ''' <summary>
        ''' Ends the current frame.
        ''' </summary>
        Public Sub EndFrame()

            ThrowIfDisposed()
            _context.EndFrame()

        End Sub

        ''' <summary>
        ''' Presents the current frame.
        ''' </summary>
        Public Sub Present()

            ThrowIfDisposed()
            _context.Present()

        End Sub

        ''' <summary>
        ''' Clears the current render target.
        ''' </summary>
        ''' <param name="color">ARGB32 clear color.</param>
        Public Sub Clear(color As UInteger)

            ThrowIfDisposed()
            _context.Clear(color)

        End Sub

#End Region

#Region "Private Methods"

        Private Sub ThrowIfDisposed()

            If _disposed Then
                Throw New ObjectDisposedException(NameOf(RenderContext))
            End If

        End Sub

#End Region

#Region "Dispose"

        ''' <summary>
        ''' Releases the underlying graphics context.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            _context.Dispose()
            _disposed = True

        End Sub

#End Region

    End Class

End Namespace