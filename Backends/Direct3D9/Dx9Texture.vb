Option Strict On
Option Explicit On
Option Infer On

Imports VBImGuiDx9.VBImGuiDx9.Contracts
Imports Dx9 = Vortice.Direct3D9

Namespace VBImGuiDx9.Backends.Direct3D9

    ''' <summary>
    ''' Direct3D9 implementation of a 2D texture.
    ''' </summary>
    Public NotInheritable Class Dx9Texture
        Implements ITexture

#Region "Fields"

        Private ReadOnly _device As Dx9GraphicsDevice
        Private ReadOnly _texture As Dx9.IDirect3DTexture9

        Private ReadOnly _width As Integer
        Private ReadOnly _height As Integer

        Private _disposed As Boolean

#End Region

#Region "Constructors"

        ''' <summary>
        ''' Creates a managed Direct3D9 texture.
        ''' </summary>
        Public Sub New(
            device As Dx9GraphicsDevice,
            width As Integer,
            height As Integer)

            If device Is Nothing Then
                Throw New ArgumentNullException(NameOf(device))
            End If

            If width <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(width))
            End If

            If height <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(height))
            End If

            _device = device
            _width = width
            _height = height

            _texture =
                device.NativeDeviceInternal.CreateTexture(
                    CUInt(width),
                    CUInt(height),
                    1UI,
                    Dx9.Usage.None,
                    Dx9.Format.A8R8G8B8,
                    Dx9.Pool.Managed)

        End Sub

#End Region

#Region "Properties"

        ''' <inheritdoc />
        Public ReadOnly Property Device As IGraphicsDevice _
            Implements IGraphicsResource.Device

            Get
                Return _device
            End Get

        End Property

        ''' <inheritdoc />
        Public ReadOnly Property Width As Integer _
            Implements ITexture.Width

            Get
                Return _width
            End Get

        End Property

        ''' <inheritdoc />
        Public ReadOnly Property Height As Integer _
            Implements ITexture.Height

            Get
                Return _height
            End Get

        End Property

        ''' <summary>
        ''' Gets the native Direct3D9 texture.
        ''' </summary>
        Public ReadOnly Property NativeTexture As Dx9.IDirect3DTexture9
            Get
                ThrowIfDisposed()
                Return _texture
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub ThrowIfDisposed()

            If _disposed Then
                Throw New ObjectDisposedException(
                    NameOf(Dx9Texture))
            End If

        End Sub

#End Region

#Region "Dispose"

        ''' <inheritdoc />
        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            _texture.Dispose()
            _disposed = True

        End Sub

#End Region

    End Class

End Namespace