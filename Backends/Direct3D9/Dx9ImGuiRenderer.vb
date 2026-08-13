Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Runtime.InteropServices

Imports ImGuiNET

Imports VBImGuiDx9.Native

Imports Dx9 = Vortice.Direct3D9

Namespace VBImGuiDx9.Backends.Direct3D9

    ''' <summary>
    ''' Dear ImGui renderer for the Direct3D9 backend.
    '''
    ''' Responsibilities:
    '''     - create the ImGui font texture;
    '''     - create dynamic vertex and index buffers;
    '''     - resize buffers when required;
    '''     - upload ImGui vertex/index data;
    '''     - expose prepared draw data to later rendering stages.
    '''
    ''' Frame lifetime itself is controlled by Core.ImGuiFrameController.
    ''' Direct3D9 frame lifetime is controlled by Dx9GraphicsContext.
    ''' </summary>
    Public NotInheritable Class Dx9ImGuiRenderer
        Implements IDisposable

#Region "Constants"

        ''' <summary>
        ''' Native ImGui vertex size:
        '''
        ''' Position = Vector2 = 8 bytes
        ''' UV       = Vector2 = 8 bytes
        ''' Color    = UInt32 = 4 bytes
        '''
        ''' Total = 28 bytes.
        ''' </summary>
        Private Const ImGuiVertexSizeInBytes As Integer = 28

        ''' <summary>
        ''' Initial vertex-buffer capacity.
        ''' </summary>
        Private Const InitialVertexCapacity As Integer = 5000

        ''' <summary>
        ''' Initial index-buffer capacity.
        ''' </summary>
        Private Const InitialIndexCapacity As Integer = 10000

        ''' <summary>
        ''' Additional vertex capacity when resizing.
        ''' </summary>
        Private Const VertexCapacityGrowth As Integer = 5000

        ''' <summary>
        ''' Additional index capacity when resizing.
        ''' </summary>
        Private Const IndexCapacityGrowth As Integer = 10000

#End Region

#Region "Fields"

        Private ReadOnly _graphicsDevice As Dx9GraphicsDevice
        Private ReadOnly _graphicsContext As Dx9GraphicsContext

        Private _fontTexture As Dx9.IDirect3DTexture9
        Private _fontTextureId As IntPtr

        Private _vertexBuffer As Dx9.IDirect3DVertexBuffer9
        Private _indexBuffer As Dx9.IDirect3DIndexBuffer9

        Private _vertexCapacity As Integer =
            InitialVertexCapacity

        Private _indexCapacity As Integer =
            InitialIndexCapacity

        Private _initialized As Boolean
        Private _disposed As Boolean

#End Region

#Region "Constructor"

        ''' <summary>
        ''' Creates the Direct3D9 ImGui renderer.
        ''' </summary>
        Public Sub New(
            graphicsDevice As Dx9GraphicsDevice,
            graphicsContext As Dx9GraphicsContext)

            If graphicsDevice Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(graphicsDevice))
            End If

            If graphicsContext Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(graphicsContext))
            End If

            _graphicsDevice = graphicsDevice
            _graphicsContext = graphicsContext

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets whether the renderer has been initialized.
        ''' </summary>
        Public ReadOnly Property IsInitialized As Boolean
            Get
                Return _initialized AndAlso Not _disposed
            End Get
        End Property

        ''' <summary>
        ''' Gets the Direct3D9 font texture.
        ''' </summary>
        Public ReadOnly Property FontTexture As Dx9.IDirect3DTexture9
            Get
                ThrowIfDisposed()
                Return _fontTexture
            End Get
        End Property

        ''' <summary>
        ''' Gets the native font texture identifier.
        ''' </summary>
        Public ReadOnly Property FontTextureId As IntPtr
            Get
                ThrowIfDisposed()
                Return _fontTextureId
            End Get
        End Property

        ''' <summary>
        ''' Gets the Direct3D9 vertex buffer.
        ''' </summary>
        Public ReadOnly Property VertexBuffer As Dx9.IDirect3DVertexBuffer9
            Get
                ThrowIfDisposed()
                Return _vertexBuffer
            End Get
        End Property

        ''' <summary>
        ''' Gets the Direct3D9 index buffer.
        ''' </summary>
        Public ReadOnly Property IndexBuffer As Dx9.IDirect3DIndexBuffer9
            Get
                ThrowIfDisposed()
                Return _indexBuffer
            End Get
        End Property

        ''' <summary>
        ''' Gets the current vertex-buffer capacity.
        ''' </summary>
        Public ReadOnly Property VertexCapacity As Integer
            Get
                Return _vertexCapacity
            End Get
        End Property

        ''' <summary>
        ''' Gets the current index-buffer capacity.
        ''' </summary>
        Public ReadOnly Property IndexCapacity As Integer
            Get
                Return _indexCapacity
            End Get
        End Property

#End Region

#Region "Initialization"

        ''' <summary>
        ''' Initializes all Direct3D9 resources required by ImGui.
        ''' </summary>
        Public Sub Initialize()

            ThrowIfDisposed()

            If _initialized Then
                Return
            End If

            If Not _graphicsDevice.IsInitialized Then
                Throw New InvalidOperationException(
                    "The Direct3D9 graphics device is not initialized.")
            End If

            CreateDeviceObjects()

            _initialized = True

        End Sub

#End Region

#Region "Device Objects"

        ''' <summary>
        ''' Creates all Direct3D9 resources required by ImGui.
        ''' </summary>
        Public Sub CreateDeviceObjects()

            ThrowIfDisposed()

            If Not _graphicsDevice.IsInitialized Then
                Throw New InvalidOperationException(
                    "The Direct3D9 graphics device is not initialized.")
            End If

            CreateFontTexture()
            CreateVertexBuffer()
            CreateIndexBuffer()

        End Sub

        ''' <summary>
        ''' Creates the Dear ImGui font texture.
        ''' </summary>
        Private Sub CreateFontTexture()

            Dim io As ImGuiIOPtr =
                ImGui.GetIO()

            Dim pixels As IntPtr = IntPtr.Zero
            Dim width As Integer = 0
            Dim height As Integer = 0
            Dim bytesPerPixel As Integer = 0

            io.Fonts.GetTexDataAsRGBA32(
                pixels,
                width,
                height,
                bytesPerPixel)

            If pixels = IntPtr.Zero Then
                Throw New InvalidOperationException(
                    "ImGui font atlas returned a null pixel pointer.")
            End If

            If width <= 0 OrElse height <= 0 Then
                Throw New InvalidOperationException(
                    "ImGui font atlas returned an invalid size.")
            End If

            If bytesPerPixel <> 4 Then
                Throw New InvalidOperationException(
                    "Expected an RGBA32 ImGui font atlas.")
            End If

            ReleaseFontTexture()

            _fontTexture =
                _graphicsDevice.NativeDeviceInternal.CreateTexture(
                    CUInt(width),
                    CUInt(height),
                    1UI,
                    Dx9.Usage.None,
                    Dx9.Format.A8R8G8B8,
                    Dx9.Pool.Managed)

            If _fontTexture Is Nothing Then
                Throw New InvalidOperationException(
                    "Direct3D9 failed to create the ImGui font texture.")
            End If

            Dim lockedRect =
                _fontTexture.LockRect(
                    0UI,
                    Dx9.LockFlags.None)

            Try

                Dim sourceRowBytes As Integer =
                    width * bytesPerPixel

                Dim destinationPitch As Integer =
                    lockedRect.Pitch

                Dim rowBuffer As Byte() =
                    New Byte(sourceRowBytes - 1) {}

                For y As Integer = 0 To height - 1

                    Dim sourceAddress As IntPtr =
                        IntPtr.Add(
                            pixels,
                            y * sourceRowBytes)

                    Marshal.Copy(
                        sourceAddress,
                        rowBuffer,
                        0,
                        sourceRowBytes)

                    Dim destinationAddress As IntPtr =
                        IntPtr.Add(
                            lockedRect.DataPointer,
                            y * destinationPitch)

                    Marshal.Copy(
                        rowBuffer,
                        0,
                        destinationAddress,
                        sourceRowBytes)

                Next

            Finally

                _fontTexture.UnlockRect(
                    0UI)

            End Try

            _fontTextureId =
                _fontTexture.NativePointer

            If _fontTextureId = IntPtr.Zero Then

                ReleaseFontTexture()

                Throw New InvalidOperationException(
                    "Direct3D9 returned a null font texture pointer.")

            End If

            io.Fonts.SetTexID(
                _fontTextureId)

            io.Fonts.ClearTexData()

        End Sub

        ''' <summary>
        ''' Recreates the Direct3D9 font texture from the current
        ''' Dear ImGui font atlas.
        ''' </summary>
        Public Sub RebuildFontTexture()

            ThrowIfDisposed()

            If Not _initialized Then
                Throw New InvalidOperationException(
            "Dx9ImGuiRenderer has not been initialized.")
            End If

            CreateFontTexture()

        End Sub

        ''' <summary>
        ''' Creates the dynamic vertex buffer used by ImGui.
        ''' </summary>
        Private Sub CreateVertexBuffer()

            ReleaseVertexBuffer()

            Dim sizeInBytes As Integer =
                            _vertexCapacity *
                            ImGuiVertexSizeInBytes

            _vertexBuffer =
                        _graphicsDevice.NativeDeviceInternal.CreateVertexBuffer(
                            CUInt(sizeInBytes),
                            Dx9.Usage.Dynamic Or Dx9.Usage.WriteOnly,
                            Dx9.VertexFormat.PositionRhw Or
                            Dx9.VertexFormat.Diffuse Or
                            Dx9.VertexFormat.Texture1,
                            Dx9.Pool.Default)

            If _vertexBuffer Is Nothing Then

                Throw New InvalidOperationException(
            "Direct3D9 failed to create the ImGui vertex buffer.")

            End If

        End Sub

        ''' <summary>
        ''' Creates the dynamic 16-bit index buffer used by ImGui.
        ''' </summary>
        Private Sub CreateIndexBuffer()

            ReleaseIndexBuffer()

            Dim sizeInBytes As Integer =
                _indexCapacity * 2

            ' Vortice.Direct3D9 3.8.3:
            '
            ' CreateIndexBuffer(
            '     sizeInBytes,
            '     usage,
            '     sixteenBit,
            '     pool)
            '
            ' ImGui uses 16-bit indices here.

            _indexBuffer =
                _graphicsDevice.NativeDeviceInternal.CreateIndexBuffer(
                    CUInt(sizeInBytes),
                    Dx9.Usage.Dynamic Or Dx9.Usage.WriteOnly,
                    True,
                    Dx9.Pool.Default)

            If _indexBuffer Is Nothing Then

                Throw New InvalidOperationException(
                    "Direct3D9 failed to create the ImGui index buffer.")

            End If

        End Sub

#End Region

#Region "Buffer Capacity"

        ''' <summary>
        ''' Ensures that the vertex and index buffers can contain
        ''' the complete current ImGui frame.
        ''' </summary>
        Private Sub EnsureBufferCapacity(
            vertexCount As Integer,
            indexCount As Integer)

            If vertexCount < 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(vertexCount))
            End If

            If indexCount < 0 Then
                Throw New ArgumentOutOfRangeException(
                    NameOf(indexCount))
            End If

            If vertexCount > _vertexCapacity Then

                _vertexCapacity =
                    Math.Max(
                        vertexCount + VertexCapacityGrowth,
                        _vertexCapacity * 2)

                CreateVertexBuffer()

            End If

            If indexCount > _indexCapacity Then

                _indexCapacity =
                    Math.Max(
                        indexCount + IndexCapacityGrowth,
                        _indexCapacity * 2)

                CreateIndexBuffer()

            End If

        End Sub

#End Region

#Region "Buffer Upload"

        ''' <summary>
        ''' Uploads all ImGui vertices into the Direct3D9 vertex buffer.
        ''' </summary>
        Private Sub UploadVertexData(
    drawData As ImDrawDataPtr)

            Dim totalVertexBytesLong As Long =
        CLng(drawData.TotalVtxCount) *
        CLng(ImGuiVertexSizeInBytes)

            If totalVertexBytesLong <= 0 Then
                Return
            End If

            If totalVertexBytesLong > Integer.MaxValue Then

                Throw New OverflowException(
            "ImGui vertex buffer size is too large.")

            End If

            Dim totalVertexBytes As Integer =
        CInt(totalVertexBytesLong)

            Dim source As IntPtr =
        Marshal.AllocHGlobal(totalVertexBytes)

            Try

                Dim offset As Integer = 0

                For commandListIndex As Integer =
            0 To drawData.CmdListsCount - 1

                    Dim commandList =
                drawData.CmdLists(commandListIndex)

                    For vertexIndex As Integer =
                0 To commandList.VtxBuffer.Size - 1

                        Dim vertex =
                    commandList.VtxBuffer(vertexIndex)

                        ' X
                        Marshal.WriteInt32(
                    IntPtr.Add(source, offset),
                    BitConverter.ToInt32(
                        BitConverter.GetBytes(vertex.pos.X),
                        0))

                        ' Y
                        Marshal.WriteInt32(
                    IntPtr.Add(source, offset + 4),
                    BitConverter.ToInt32(
                        BitConverter.GetBytes(vertex.pos.Y),
                        0))

                        ' Z
                        Marshal.WriteInt32(
                    IntPtr.Add(source, offset + 8),
                    BitConverter.ToInt32(
                        BitConverter.GetBytes(0.0F),
                        0))

                        ' RHW
                        Marshal.WriteInt32(
                    IntPtr.Add(source, offset + 12),
                    BitConverter.ToInt32(
                        BitConverter.GetBytes(1.0F),
                        0))

                        ' COLOR
                        Dim colorBits As Integer =
                    BitConverter.ToInt32(
                        BitConverter.GetBytes(vertex.col),
                        0)

                        Marshal.WriteInt32(
                    IntPtr.Add(source, offset + 16),
                    colorBits)

                        ' UV.X
                        Marshal.WriteInt32(
                    IntPtr.Add(source, offset + 20),
                    BitConverter.ToInt32(
                        BitConverter.GetBytes(vertex.uv.X),
                        0))

                        ' UV.Y
                        Marshal.WriteInt32(
                    IntPtr.Add(source, offset + 24),
                    BitConverter.ToInt32(
                        BitConverter.GetBytes(vertex.uv.Y),
                        0))

                        offset += ImGuiVertexSizeInBytes

                    Next

                Next

                Dx9BufferNative.SetVertexBufferData(
            _vertexBuffer,
            source,
            totalVertexBytes,
            Dx9.LockFlags.Discard)

            Finally

                Marshal.FreeHGlobal(source)

            End Try

        End Sub


        ''' <summary>
        ''' Uploads all ImGui indices into the Direct3D9 index buffer.
        ''' </summary>
        Private Sub UploadIndexData(
            drawData As ImDrawDataPtr)

            Dim totalIndexBytes As Integer =
                drawData.TotalIdxCount * 2

            If totalIndexBytes <= 0 Then
                Return
            End If

            Dim source As IntPtr =
                Marshal.AllocHGlobal(
                    totalIndexBytes)

            Try

                Dim offset As Integer = 0

                For commandListIndex As Integer =
                    0 To drawData.CmdListsCount - 1

                    Dim commandList =
                        drawData.CmdLists(
                            commandListIndex)

                    For indexIndex As Integer =
                        0 To commandList.IdxBuffer.Size - 1

                        Dim indexValue As UShort =
                            CUShort(
                                commandList.IdxBuffer(
                                    indexIndex))


                        Marshal.WriteInt16(
                            IntPtr.Add(
                                source,
                                offset),
                            CShort(indexValue))

                        offset += 2

                    Next

                Next

                Dx9BufferNative.SetIndexBufferData(
                    _indexBuffer,
                    source,
                    totalIndexBytes,
                    Dx9.LockFlags.Discard)

            Finally

                Marshal.FreeHGlobal(
                    source)

            End Try

        End Sub

#End Region

#Region "Rendering"

        ''' <summary>
        ''' Uploads the current ImGui frame into the Direct3D9 buffers.
        '''
        ''' Actual draw calls are intentionally deferred until the
        ''' Render State and Draw Command stages.
        ''' </summary>
        Public Sub RenderDrawData(
                    drawData As ImDrawDataPtr)


            ThrowIfDisposed()

            If Not _initialized Then
                Throw New InvalidOperationException(
            "Dx9ImGuiRenderer has not been initialized.")
            End If

            If drawData.CmdListsCount <= 0 Then
                Return
            End If

            If drawData.TotalVtxCount <= 0 Then
                Return
            End If

            If drawData.TotalIdxCount <= 0 Then
                Return
            End If

            ' ----------------------------------------------------
            ' Invalid/temporary display size
            '
            ' WinForms can briefly expose a zero-sized client area
            ' during minimize/restore and DX9 reset transitions.
            ' ImGui may still return draw data for that frame.
            '
            ' Do not pass a zero-sized projection to DX9.
            ' The next valid frame will render normally.
            ' ----------------------------------------------------
            If drawData.DisplaySize.X <= 0.0F OrElse
               drawData.DisplaySize.Y <= 0.0F Then

                Return
            End If

            If drawData.FramebufferScale.X <= 0.0F OrElse
               drawData.FramebufferScale.Y <= 0.0F Then

                Return
            End If

            EnsureBufferCapacity(
                        drawData.TotalVtxCount,
                        drawData.TotalIdxCount)

            UploadVertexData(
                         drawData)

            UploadIndexData(
                        drawData)

            _graphicsContext.SetImGuiProjection(
                            drawData.DisplaySize.X,
                            drawData.DisplaySize.Y)

            _graphicsContext.BindImGuiVertexBuffer(
                                _vertexBuffer)

            _graphicsContext.BindImGuiIndexBuffer(
                                _indexBuffer)

            _graphicsContext.SetImGuiVertexFormat()

            _graphicsContext.SetImGuiBlendState()

            _graphicsContext.SetImGuiDepthAndCullingState()

            _graphicsContext.SetImGuiScissorState()


            ' ----------------------------------------------------
            ' Draw command lists
            ' ----------------------------------------------------

            Dim globalVertexOffset As Integer = 0

            Dim globalIndexOffset As UInteger = 0UI

            For commandListIndex As Integer =
        0 To drawData.CmdListsCount - 1

                Dim commandList =
            drawData.CmdLists(
                commandListIndex)

                For commandIndex As Integer =
                            0 To commandList.CmdBuffer.Size - 1

                    Dim drawCommand =
                                commandList.CmdBuffer(commandIndex)

                    If drawCommand.ElemCount <= 0UI Then
                        Continue For
                    End If

                    ' ------------------------------------------------
                    ' Texture
                    ' ------------------------------------------------

                    If drawCommand.TextureId = _fontTextureId Then

                        _graphicsContext.BindImGuiTexture(
            _fontTexture)

                    Else

                        _graphicsContext.BindImGuiTexture(
            Nothing)

                    End If


                    ' ------------------------------------------------
                    ' Clip rectangle
                    ' ------------------------------------------------

                    Dim clipRect As System.Numerics.Vector4 =
                drawCommand.ClipRect

                    Dim clipLeft As Integer =
                CInt(Math.Floor(
                    clipRect.X *
                    drawData.FramebufferScale.X))

                    Dim clipTop As Integer =
                CInt(Math.Floor(
                    clipRect.Y *
                    drawData.FramebufferScale.Y))

                    Dim clipRight As Integer =
                CInt(Math.Ceiling(
                    clipRect.Z *
                    drawData.FramebufferScale.X))

                    Dim clipBottom As Integer =
                CInt(Math.Ceiling(
                    clipRect.W *
                    drawData.FramebufferScale.Y))

                    If clipRight <= clipLeft OrElse
               clipBottom <= clipTop Then

                        Continue For

                    End If


                    _graphicsContext.SetImGuiScissorRect(
                                    clipLeft,
                                    clipTop,
                                    clipRight,
                                    clipBottom)

                    ' ------------------------------------------------
                    ' Primitive count
                    ' ------------------------------------------------

                    Dim primitiveCount As UInteger =
                drawCommand.ElemCount \ 3UI

                    If primitiveCount = 0UI Then
                        Continue For
                    End If

                    ' ------------------------------------------------
                    ' Global vertex/index offsets
                    ' ------------------------------------------------

                    Dim baseVertexIndex As Integer =
                globalVertexOffset +
                CInt(drawCommand.VtxOffset)

                    Dim startIndex As UInteger =
                globalIndexOffset +
                drawCommand.IdxOffset

                    ' ------------------------------------------------
                    ' Draw
                    ' ------------------------------------------------

                    _graphicsContext.DrawIndexedTriangles(
                baseVertexIndex,
                0,
                CUInt(commandList.VtxBuffer.Size),
                startIndex,
                primitiveCount)

                Next

                globalVertexOffset +=
            commandList.VtxBuffer.Size

                globalIndexOffset +=
            CUInt(commandList.IdxBuffer.Size)

            Next

        End Sub

#End Region

#Region "Resource Release"

        ''' <summary>
        ''' Releases the Direct3D9 font texture.
        ''' </summary>
        Private Sub ReleaseFontTexture()

            If _fontTexture IsNot Nothing Then

                _fontTexture.Dispose()
                _fontTexture = Nothing

            End If

            _fontTextureId =
                IntPtr.Zero

        End Sub

        ''' <summary>
        ''' Releases the Direct3D9 vertex buffer.
        ''' </summary>
        Private Sub ReleaseVertexBuffer()

            If _vertexBuffer IsNot Nothing Then

                _vertexBuffer.Dispose()
                _vertexBuffer = Nothing

            End If

        End Sub

        ''' <summary>
        ''' Releases the Direct3D9 index buffer.
        ''' </summary>
        Private Sub ReleaseIndexBuffer()

            If _indexBuffer IsNot Nothing Then

                _indexBuffer.Dispose()
                _indexBuffer = Nothing

            End If

        End Sub

        ''' <summary>
        ''' Releases all Direct3D9 resources used by ImGui.
        ''' </summary>
        Public Sub InvalidateDeviceObjects()

            ThrowIfDisposed()

            ReleaseFontTexture()
            ReleaseVertexBuffer()
            ReleaseIndexBuffer()

            _initialized = False

        End Sub

#End Region

#Region "Private"

        ''' <summary>
        ''' Recreates all ImGui Direct3D9 resources after a device reset.
        ''' </summary>
        Public Sub RestoreDeviceObjects()

            ThrowIfDisposed()

            If Not _graphicsDevice.IsInitialized Then
                Throw New InvalidOperationException(
                    "The Direct3D9 graphics device is not initialized.")
            End If

            If _initialized Then
                Return
            End If

            CreateDeviceObjects()

            _initialized = True

        End Sub

        ''' <summary>
        ''' Throws when the renderer has already been disposed.
        ''' </summary>
        Private Sub ThrowIfDisposed()

            If _disposed Then

                Throw New ObjectDisposedException(
                    NameOf(Dx9ImGuiRenderer))

            End If

        End Sub

#End Region

#Region "Dispose"

        ''' <summary>
        ''' Releases the Direct3D9 ImGui renderer.
        ''' </summary>
        Public Sub Dispose() _
            Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            ReleaseFontTexture()
            ReleaseVertexBuffer()
            ReleaseIndexBuffer()

            _initialized = False
            _disposed = True

        End Sub

#End Region

    End Class

End Namespace