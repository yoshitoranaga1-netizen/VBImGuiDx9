Option Strict On
Option Explicit On
Option Infer On

Imports VBImGuiDx9.VBImGuiDx9.Contracts
Imports VBImGuiDx9.VBImGuiDx9.Core
Imports Dx9 = Vortice.Direct3D9

Namespace VBImGuiDx9.Backends.Direct3D9

    ''' <summary>
    ''' Represents the cooperative-level state of a Direct3D9 device.
    ''' </summary>
    Public Enum Dx9DeviceStatus
        ''' <summary>Device is ready for rendering.</summary>
        Operational

        ''' <summary>Device is lost and cannot currently be reset.</summary>
        DeviceLost

        ''' <summary>Device is lost but can be reset.</summary>
        DeviceNotReset

        ''' <summary>Direct3D reported a driver internal error.</summary>
        DriverInternalError

        ''' <summary>An unrecognized cooperative-level status was returned.</summary>
        Unknown
    End Enum

    ''' <summary>
    ''' Direct3D9 implementation of the graphics device.
    ''' </summary>
    Public NotInheritable Class Dx9GraphicsDevice
        Implements IGraphicsDevice

#Region "Fields"

        Private ReadOnly _direct3D As Dx9.IDirect3D9
        Private ReadOnly _device As Dx9.IDirect3DDevice9


        Private _disposed As Boolean

        Private Const DeviceLostHResult As Integer = -2005530520
        Private Const DeviceNotResetHResult As Integer = -2005530519
        Private Const DriverInternalErrorHResult As Integer = -2005530585

        Private _width As Integer
        Private _height As Integer

        Private ReadOnly _windowHandle As IntPtr
        Private ReadOnly _windowed As Boolean
        Private ReadOnly _enableVSync As Boolean

#End Region

#Region "Constructor"

        ''' <summary>
        ''' Creates a Direct3D9 device.
        ''' </summary>
        Public Sub New(options As DeviceOptions)

            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If

            If options.WindowHandle = IntPtr.Zero Then
                Throw New ArgumentException(
                    "WindowHandle must not be zero.",
                    NameOf(options))
            End If

            If options.Width <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(options.Width))
            End If

            If options.Height <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(options.Height))
            End If

            _width = options.Width
            _height = options.Height
            _windowHandle = options.WindowHandle
            _windowed = options.Windowed
            _enableVSync = options.EnableVSync


            ' Native Direct3D9 entry point:
            ' Direct3DCreate9(D3D_SDK_VERSION)
            _direct3D = Dx9.D3D9.Direct3DCreate9()

            If _direct3D Is Nothing Then
                Throw New InvalidOperationException(
                    "Direct3DCreate9 returned Nothing.")
            End If

            Dim pp As New Dx9.PresentParameters()

            pp.Windowed = options.Windowed
            pp.SwapEffect = Dx9.SwapEffect.Discard
            pp.DeviceWindowHandle = options.WindowHandle

            pp.BackBufferWidth = CUInt(options.Width)
            pp.BackBufferHeight = CUInt(options.Height)
            pp.BackBufferCount = 1UI
            pp.BackBufferFormat = Dx9.Format.A8R8G8B8

            If options.EnableVSync Then
                pp.PresentationInterval =
                    Dx9.PresentInterval.Default
            Else
                pp.PresentationInterval =
                    Dx9.PresentInterval.Immediate
            End If

            pp.MultiSampleType =
                Dx9.MultisampleType.None

            Dim flags As Dx9.CreateFlags =
                Dx9.CreateFlags.HardwareVertexProcessing

            If options.EnableMultithreading Then
                flags = flags Or Dx9.CreateFlags.Multithreaded
            End If

            _device = _direct3D.CreateDevice(
                0UI,
                Dx9.DeviceType.Hardware,
                options.WindowHandle,
                flags,
                pp)

        End Sub

#End Region

#Region "Properties"

        ''' <inheritdoc />
        Public ReadOnly Property Width As Integer _
            Implements IGraphicsDevice.Width

            Get
                Return _width
            End Get

        End Property

        ''' <inheritdoc />
        Public ReadOnly Property Height As Integer _
            Implements IGraphicsDevice.Height

            Get
                Return _height
            End Get

        End Property

        ''' <inheritdoc />
        Public ReadOnly Property IsInitialized As Boolean _
            Implements IGraphicsDevice.IsInitialized

            Get
                Return Not _disposed AndAlso
                       _device IsNot Nothing
            End Get

        End Property

        ''' <summary>
        ''' Gets the native Direct3D9 device.
        ''' </summary>
        Public ReadOnly Property NativeDevice As Dx9.IDirect3DDevice9
            Get
                ThrowIfDisposed()
                Return _device
            End Get
        End Property

        ''' <summary>
        ''' Gets the native Direct3D9 interface.
        ''' </summary>
        Public ReadOnly Property NativeDirect3D As Dx9.IDirect3D9
            Get
                ThrowIfDisposed()
                Return _direct3D
            End Get
        End Property

#End Region

#Region "Device State"

        ''' <summary>
        ''' Gets the current Direct3D9 cooperative-level state.
        ''' </summary>
        Public Function GetDeviceStatus() As Dx9DeviceStatus

            ThrowIfDisposed()

            Try
                ' В нашей версии Vortice этот метод не возвращает Result.
                ' HRESULT ошибки приходит через исключение.
                _device.TestCooperativeLevel()

                Return Dx9DeviceStatus.Operational

            Catch ex As Exception

                Dim code As Integer = ex.HResult

                Select Case code

                    Case DeviceLostHResult
                        Return Dx9DeviceStatus.DeviceLost

                    Case DeviceNotResetHResult
                        Return Dx9DeviceStatus.DeviceNotReset

                    Case DriverInternalErrorHResult
                        Return Dx9DeviceStatus.DriverInternalError

                    Case Else
                        Throw New InvalidOperationException(
                    $"Direct3D9 TestCooperativeLevel failed. HRESULT=0x{code:X8}.",
                    ex)

                End Select

            End Try

        End Function

        ''' <summary>
        ''' Resets the Direct3D9 device with the requested back-buffer size.
        ''' D3DPOOL_DEFAULT resources must already have been released.
        ''' </summary>
        ''' <param name="width">New back-buffer width.</param>
        ''' <param name="height">New back-buffer height.</param>
        ''' <returns><c>True</c> when the reset succeeds; otherwise <c>False</c> when the device is still lost.</returns>
        Public Function TryReset(
    width As Integer,
    height As Integer) As Boolean

            ThrowIfDisposed()

            If width <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(width))
            End If

            If height <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(height))
            End If

            Dim status As Dx9DeviceStatus =
        GetDeviceStatus()

            Select Case status

                Case Dx9DeviceStatus.DeviceLost
                    Return False

                Case Dx9DeviceStatus.DriverInternalError
                    Throw New InvalidOperationException(
                "Direct3D9 reported a driver internal error.")

                Case Dx9DeviceStatus.Unknown
                    Throw New InvalidOperationException(
                "Direct3D9 returned an unknown cooperative-level status.")

            End Select

            Dim presentParameters As Dx9.PresentParameters =
        CreatePresentParameters(
            width,
            height)

            Try

                ' В используемой версии Vortice Reset является Sub.
                ' При ошибке он выбрасывает исключение.
                _device.Reset(
            presentParameters)

                _width = width
                _height = height

                Return True

            Catch ex As Exception

                Dim code As Integer = ex.HResult

                Select Case code

                    Case DeviceLostHResult,
                 DeviceNotResetHResult

                        Return False

                    Case Else

                        Throw New InvalidOperationException(
                    $"Direct3D9 device reset failed. HRESULT=0x{code:X8}.",
                    ex)

                End Select

            End Try

        End Function

        ''' <summary>
        ''' Creates the presentation parameters used by the Direct3D9 device.
        ''' </summary>
        Private Function CreatePresentParameters(
            width As Integer,
            height As Integer) As Dx9.PresentParameters

            Dim pp As New Dx9.PresentParameters()

            pp.Windowed = _windowed
            pp.SwapEffect = Dx9.SwapEffect.Discard
            pp.DeviceWindowHandle = _windowHandle
            pp.BackBufferWidth = CUInt(width)
            pp.BackBufferHeight = CUInt(height)
            pp.BackBufferCount = 1UI
            pp.BackBufferFormat = Dx9.Format.A8R8G8B8
            pp.PresentationInterval = If(
                _enableVSync,
                Dx9.PresentInterval.Default,
                Dx9.PresentInterval.Immediate)
            pp.MultiSampleType = Dx9.MultisampleType.None

            Return pp
        End Function

#End Region

#Region "Context"

        ''' <inheritdoc />
        Public Function CreateGraphicsContext() As IGraphicsContext _
            Implements IGraphicsDevice.CreateGraphicsContext

            ThrowIfDisposed()

            Return New Dx9GraphicsContext(Me)

        End Function

#End Region

#Region "Resources"

        ''' <inheritdoc />
        Public Function CreateVertexBuffer(
            sizeInBytes As Integer,
            dynamic As Boolean) As IVertexBuffer _
            Implements IGraphicsDevice.CreateVertexBuffer

            ThrowIfDisposed()

            If sizeInBytes <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(sizeInBytes))
            End If

            Return New Dx9VertexBuffer(
                Me,
                sizeInBytes,
                dynamic)

        End Function

        ''' <inheritdoc />
        Public Function CreateIndexBuffer(
            sizeInBytes As Integer,
            dynamic As Boolean) As IIndexBuffer _
            Implements IGraphicsDevice.CreateIndexBuffer

            ThrowIfDisposed()

            If sizeInBytes <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(sizeInBytes))
            End If

            Return New Dx9IndexBuffer(
                Me,
                sizeInBytes,
                dynamic)

        End Function

        ''' <inheritdoc />
        Public Function CreateTexture2D(
            width As Integer,
            height As Integer) As ITexture _
            Implements IGraphicsDevice.CreateTexture2D

            ThrowIfDisposed()

            If width <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(width))
            End If

            If height <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(height))
            End If

            Return New Dx9Texture(
                Me,
                width,
                height)

        End Function

#End Region

#Region "Internal"

        Friend ReadOnly Property NativeDeviceInternal As Dx9.IDirect3DDevice9
            Get
                ThrowIfDisposed()
                Return _device
            End Get
        End Property

#End Region

#Region "Private"

        Private Sub ThrowIfDisposed()

            If _disposed Then
                Throw New ObjectDisposedException(
                    NameOf(Dx9GraphicsDevice))
            End If

        End Sub

#End Region

#Region "Dispose"

        ''' <inheritdoc />
        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            If _device IsNot Nothing Then
                _device.Dispose()
            End If

            If _direct3D IsNot Nothing Then
                _direct3D.Dispose()
            End If

            _disposed = True

        End Sub

#End Region

    End Class

End Namespace