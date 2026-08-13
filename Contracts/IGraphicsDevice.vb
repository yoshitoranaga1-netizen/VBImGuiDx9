Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Contracts

    ''' <summary>
    ''' Represents the graphics device responsible for creating graphics resources
    ''' and graphics contexts.
    ''' </summary>
    Public Interface IGraphicsDevice
        Inherits IDisposable

#Region "Properties"

        ''' <summary>
        ''' Gets the width of the current render target.
        ''' </summary>
        ReadOnly Property Width As Integer

        ''' <summary>
        ''' Gets the height of the current render target.
        ''' </summary>
        ReadOnly Property Height As Integer

        ''' <summary>
        ''' Gets a value indicating whether the graphics device is initialized.
        ''' </summary>
        ReadOnly Property IsInitialized As Boolean

#End Region

#Region "Context"

        ''' <summary>
        ''' Creates a graphics context associated with this device.
        ''' </summary>
        Function CreateGraphicsContext() As IGraphicsContext

#End Region

#Region "Resources"

        ''' <summary>
        ''' Creates a vertex buffer.
        ''' </summary>
        ''' <param name="sizeInBytes">Size of the buffer in bytes.</param>
        ''' <param name="dynamic">Indicates whether the buffer is dynamic.</param>
        Function CreateVertexBuffer(
            sizeInBytes As Integer,
            dynamic As Boolean) As IVertexBuffer

        ''' <summary>
        ''' Creates an index buffer.
        ''' </summary>
        ''' <param name="sizeInBytes">Size of the buffer in bytes.</param>
        ''' <param name="dynamic">Indicates whether the buffer is dynamic.</param>
        Function CreateIndexBuffer(
            sizeInBytes As Integer,
            dynamic As Boolean) As IIndexBuffer

        ''' <summary>
        ''' Creates a 2D texture.
        ''' </summary>
        ''' <param name="width">Texture width.</param>
        ''' <param name="height">Texture height.</param>
        Function CreateTexture2D(
            width As Integer,
            height As Integer) As ITexture

#End Region

    End Interface

End Namespace