Option Strict On
Option Explicit On
Option Infer On

Imports VBImGuiDx9.Native
Imports VBImGuiDx9.VBImGuiDx9.Contracts
Imports Dx9 = Vortice.Direct3D9

Namespace VBImGuiDx9.Backends.Direct3D9

    ''' <summary>
    ''' Direct3D9 implementation of a vertex buffer.
    ''' </summary>
    Public NotInheritable Class Dx9VertexBuffer
        Implements IVertexBuffer

        Private ReadOnly _device As Dx9GraphicsDevice
        Private ReadOnly _buffer As Dx9.IDirect3DVertexBuffer9
        Private ReadOnly _sizeInBytes As Integer
        Private ReadOnly _isDynamic As Boolean

        Private _disposed As Boolean

        Public Sub New(
            device As Dx9GraphicsDevice,
            sizeInBytes As Integer,
            dynamic As Boolean)

            If device Is Nothing Then
                Throw New ArgumentNullException(NameOf(device))
            End If

            If sizeInBytes <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(sizeInBytes))
            End If

            _device = device
            _sizeInBytes = sizeInBytes
            _isDynamic = dynamic

            Dim usage As Dx9.Usage = Dx9.Usage.WriteOnly

            If dynamic Then
                usage = usage Or Dx9.Usage.Dynamic
            End If

            _buffer =
                device.NativeDeviceInternal.CreateVertexBuffer(
                    CUInt(sizeInBytes),
                    usage,
                    Dx9.VertexFormat.None,
                    Dx9.Pool.Default)

        End Sub

        Public ReadOnly Property Device As IGraphicsDevice _
            Implements IGraphicsResource.Device

            Get
                Return _device
            End Get

        End Property

        Public ReadOnly Property SizeInBytes As Integer _
            Implements IBuffer.SizeInBytes

            Get
                Return _sizeInBytes
            End Get

        End Property

        Public ReadOnly Property IsDynamic As Boolean _
            Implements IBuffer.IsDynamic

            Get
                Return _isDynamic
            End Get

        End Property

        Public ReadOnly Property NativeBuffer As Dx9.IDirect3DVertexBuffer9
            Get
                ThrowIfDisposed()
                Return _buffer
            End Get
        End Property

        Public Sub SetData(
            source As IntPtr,
            sizeInBytes As Integer) _
            Implements IBuffer.SetData

            ThrowIfDisposed()

            If source = IntPtr.Zero Then
                Throw New ArgumentException(
                    "Source pointer must not be zero.",
                    NameOf(source))
            End If

            If sizeInBytes < 0 OrElse
               sizeInBytes > _sizeInBytes Then

                Throw New ArgumentOutOfRangeException(
                    NameOf(sizeInBytes))
            End If

            If sizeInBytes = 0 Then
                Return
            End If

            Dim flags As Dx9.LockFlags = Dx9.LockFlags.None

            If _isDynamic Then
                flags = Dx9.LockFlags.Discard
            End If

            Dx9BufferNative.SetVertexBufferData(
                _buffer,
                source,
                sizeInBytes,
                flags)

        End Sub

        Private Sub ThrowIfDisposed()

            If _disposed Then
                Throw New ObjectDisposedException(
                    NameOf(Dx9VertexBuffer))
            End If

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            _buffer.Dispose()
            _disposed = True

        End Sub

    End Class

End Namespace