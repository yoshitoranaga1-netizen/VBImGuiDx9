Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Runtime.InteropServices
Imports VBImGuiDx9.VBImGuiDx9.Contracts
Imports Dx9 = Vortice.Direct3D9
Imports VorticeColor = Vortice.Mathematics.Color

Namespace VBImGuiDx9.Backends.Direct3D9

    ''' <summary>
    ''' Direct3D9 rendering context.
    '''
    ''' Responsible for:
    '''     - frame lifecycle;
    '''     - clearing the render target;
    '''     - configuring the basic ImGui render state.
    '''
    ''' Resource creation remains the responsibility of
    ''' Dx9GraphicsDevice.
    ''' </summary>
    Public NotInheritable Class Dx9GraphicsContext
        Implements IGraphicsContext

#Region "Fields"

        Private ReadOnly _graphicsDevice As Dx9GraphicsDevice
        Private ReadOnly _device As Dx9.IDirect3DDevice9

        Private _frameActive As Boolean
        Private _disposed As Boolean

#End Region

#Region "Constructor"

        ''' <summary>
        ''' Creates a Direct3D9 rendering context.
        ''' </summary>
        Public Sub New(
            graphicsDevice As Dx9GraphicsDevice)

            If graphicsDevice Is Nothing Then
                Throw New ArgumentNullException(
                    NameOf(graphicsDevice))
            End If

            If Not graphicsDevice.IsInitialized Then
                Throw New InvalidOperationException(
                    "The Direct3D9 graphics device is not initialized.")
            End If

            _graphicsDevice = graphicsDevice
            _device = graphicsDevice.NativeDeviceInternal

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets whether a frame is currently active.
        ''' </summary>
        Public ReadOnly Property IsFrameActive As Boolean

            Get
                Return _frameActive
            End Get

        End Property

#End Region

#Region "Frame Lifecycle"

        ''' <summary>
        ''' Begins a new Direct3D9 frame.
        ''' </summary>
        Public Sub BeginFrame() _
            Implements IGraphicsContext.BeginFrame

            ThrowIfDisposed()

            If _frameActive Then
                Throw New InvalidOperationException(
                    "A frame is already active.")
            End If

            _device.BeginScene()

            _frameActive = True

        End Sub

        ''' <summary>
        ''' Ends the current Direct3D9 frame.
        ''' </summary>
        Public Sub EndFrame() _
            Implements IGraphicsContext.EndFrame

            ThrowIfDisposed()

            If Not _frameActive Then
                Return
            End If

            _device.EndScene()

            _frameActive = False

        End Sub

        ''' <summary>
        ''' Presents the completed frame to the window.
        ''' </summary>
        Public Sub Present() _
    Implements IGraphicsContext.Present

            ThrowIfDisposed()

            If _frameActive Then
                Throw New InvalidOperationException(
            "Cannot present while a frame is active.")
            End If

            _device.Present()


        End Sub

#End Region

#Region "Clear"

        ''' <summary>
        ''' Clears the current render target.
        ''' </summary>
        ''' <param name="color">
        ''' Packed ARGB32 color.
        ''' </param>
        Public Sub Clear(
                    color As UInteger) _
    Implements IGraphicsContext.Clear

            ThrowIfDisposed()

            ' Clear должен очищать весь backbuffer.
            ' Scissor Test от предыдущего ImGui command
            ' не должен ограничивать Clear.
            _device.SetRenderState(
        Dx9.RenderState.ScissorTestEnable,
        False)

            Dim clearColor As VorticeColor =
        New VorticeColor(
            CByte((color >> 16) And &HFFUI),
            CByte((color >> 8) And &HFFUI),
            CByte(color And &HFFUI),
            CByte((color >> 24) And &HFFUI))

            _device.Clear(
                        Dx9.ClearFlags.Target,
                        clearColor,
                        1.0F,
                        0)

        End Sub

#End Region

#Region "Render State"

        ''' <summary>
        ''' Configures the Direct3D9 fixed-function state required
        ''' for Dear ImGui rendering.
        '''
        ''' This method intentionally contains only state setup.
        ''' Vertex/index buffers, textures, scissors and draw calls
        ''' are handled by later renderer stages.
        ''' </summary>
        Public Sub ResetState()

            ThrowIfDisposed()

            ' --------------------------------------------------------
            ' Depth / lighting
            ' --------------------------------------------------------


            _device.SetRenderState(
                Dx9.RenderState.ZEnable,
                False)

            _device.SetRenderState(
                Dx9.RenderState.ZWriteEnable,
                False)

            _device.SetRenderState(
                Dx9.RenderState.Lighting,
                False)

            ' --------------------------------------------------------
            ' Face culling
            ' --------------------------------------------------------

            _device.SetRenderState(
                Dx9.RenderState.CullMode,
                Dx9.Cull.None)

            ' --------------------------------------------------------
            ' Alpha blending
            ' --------------------------------------------------------

            _device.SetRenderState(
                Dx9.RenderState.AlphaBlendEnable,
                True)

            _device.SetRenderState(
                Dx9.RenderState.SourceBlend,
                Dx9.Blend.SourceAlpha)

            _device.SetRenderState(
                Dx9.RenderState.DestinationBlend,
                Dx9.Blend.InverseSourceAlpha)

            _device.SetRenderState(
                    Dx9.RenderState.FogEnable,
                    False)

            _device.SetRenderState(
                    Dx9.RenderState.NormalizeNormals,
                    False)

            ' --------------------------------------------------------
            ' Alpha test
            ' --------------------------------------------------------

            _device.SetRenderState(
                Dx9.RenderState.AlphaTestEnable,
                False)

            ' --------------------------------------------------------
            ' Scissor testing
            '
            ' ImGui uses a scissor rectangle for every draw command.
            ' The actual rectangle is configured by the draw-command
            ' stage.
            ' --------------------------------------------------------

            _device.SetRenderState(
                Dx9.RenderState.ScissorTestEnable,
                True)

            ' --------------------------------------------------------
            ' Fixed-function texture pipeline
            ' --------------------------------------------------------

            _device.SetTextureStageState(
                0UI,
                Dx9.TextureStage.ColorOperation,
                Dx9.TextureOperation.Modulate)

            _device.SetTextureStageState(
                0UI,
                Dx9.TextureStage.ColorArg1,
                Dx9.TextureArgument.Texture)

            _device.SetTextureStageState(
                0UI,
                Dx9.TextureStage.ColorArg2,
                Dx9.TextureArgument.Diffuse)

            _device.SetTextureStageState(
                0UI,
                Dx9.TextureStage.AlphaOperation,
                Dx9.TextureOperation.Modulate)

            _device.SetTextureStageState(
                0UI,
                Dx9.TextureStage.AlphaArg1,
                Dx9.TextureArgument.Texture)

            _device.SetTextureStageState(
                0UI,
                Dx9.TextureStage.AlphaArg2,
                Dx9.TextureArgument.Diffuse)

            ' --------------------------------------------------------
            ' Sampler state
            '
            ' Linear filtering is appropriate for the ImGui font
            ' atlas and regular UI textures.
            ' --------------------------------------------------------

            _device.SetSamplerState(
                0UI,
                Dx9.SamplerState.MinFilter,
                Dx9.TextureFilter.Linear)

            _device.SetSamplerState(
                0UI,
                Dx9.SamplerState.MagFilter,
                Dx9.TextureFilter.Linear)

            _device.SetSamplerState(
                0UI,
                Dx9.SamplerState.MipFilter,
                Dx9.TextureFilter.None)

            _device.SetSamplerState(
                0UI,
                Dx9.SamplerState.AddressU,
                Dx9.TextureAddress.Clamp)

            _device.SetSamplerState(
                0UI,
                Dx9.SamplerState.AddressV,
                Dx9.TextureAddress.Clamp)

        End Sub

        ''' <summary>
        ''' Sets the orthographic projection used by Dear ImGui.
        ''' </summary>

        Public Sub SetImGuiProjection(
                        width As Single,
                        height As Single)

            ThrowIfDisposed()

            If width <= 0.0F Then
                Throw New ArgumentOutOfRangeException(
            NameOf(width))
            End If

            If height <= 0.0F Then
                Throw New ArgumentOutOfRangeException(
            NameOf(height))
            End If

            ' View = Identity
            _device.SetTransform(
        CInt(Dx9.TransformState.View),
        System.Numerics.Matrix4x4.Identity)

            ' Orthographic projection for ImGui:
            ' (0,0) = top-left
            ' (width,height) = bottom-right
            Dim projection As System.Numerics.Matrix4x4 =
                            System.Numerics.Matrix4x4.CreateOrthographicOffCenter(
            0.0F,
            width,
            height,
            0.0F,
            -1.0F,
            1.0F)

            _device.SetTransform(
        CInt(Dx9.TransformState.Projection),
        projection)

        End Sub


        ''' <summary>
        ''' Binds the ImGui vertex buffer to Direct3D9 stream 0.
        ''' </summary>
        Public Sub BindImGuiVertexBuffer(
    vertexBuffer As Dx9.IDirect3DVertexBuffer9)

            ThrowIfDisposed()

            If vertexBuffer Is Nothing Then
                Throw New ArgumentNullException(
            NameOf(vertexBuffer))
            End If

            _device.SetStreamSource(
                            0UI,
                            vertexBuffer,
                            0UI,
                            28UI)

        End Sub

        ''' <summary>
        ''' Sets the Direct3D9 vertex format used by ImGui.
        ''' </summary>
        Public Sub SetImGuiVertexFormat()

            ThrowIfDisposed()

            _device.VertexFormat =
                    Dx9.VertexFormat.PositionRhw Or
                    Dx9.VertexFormat.Diffuse Or
                    Dx9.VertexFormat.Texture1

        End Sub

        ''' <summary>
        ''' Binds an index buffer to the Direct3D9 device.
        ''' </summary>
        Public Sub BindImGuiIndexBuffer(
                indexBuffer As Dx9.IDirect3DIndexBuffer9)

            ThrowIfDisposed()

            If indexBuffer Is Nothing Then
                Throw New ArgumentNullException(
            NameOf(indexBuffer))
            End If

            _device.Indices = indexBuffer

        End Sub

        ''' <summary>
        ''' Draws ImGui indexed triangles.
        ''' </summary>
        Public Sub DrawImGuiIndexedPrimitive(
                        baseVertexIndex As Integer,
                        minVertexIndex As Integer,
                        vertexCount As Integer,
                        startIndex As Integer,
                        primitiveCount As Integer)

            ThrowIfDisposed()

            If vertexCount <= 0 OrElse
       primitiveCount <= 0 Then

                Return

            End If
            
            _device.DrawIndexedPrimitive(
                    Dx9.PrimitiveType.TriangleList,
                    baseVertexIndex,
                    CUInt(minVertexIndex),
                    CUInt(vertexCount),
                    CUInt(startIndex),
                    CUInt(primitiveCount))

        End Sub

        ''' <summary>
        ''' Binds an ImGui texture to texture stage 0.
        ''' </summary>
        Public Sub BindImGuiTexture(
    texture As Dx9.IDirect3DTexture9)

            ThrowIfDisposed()

            If texture Is Nothing Then
                _device.SetTexture(
            0UI,
            Nothing)
                Return
            End If

            _device.SetTexture(
        0UI,
        texture)

        End Sub


        ''' <summary>
        ''' Sets the Direct3D9 scissor rectangle used by ImGui.
        ''' </summary>
        Public Sub SetImGuiScissorRect(
                                        left As Integer,
                                        top As Integer,
                                        right As Integer,
                                        bottom As Integer)

            ThrowIfDisposed()

            _device.ScissorRect =
                        New Dx9.Rect(
                            left,
                            top,
                            right,
                            bottom)

        End Sub

        ''' <summary>
        ''' Configures Direct3D9 alpha blending for Dear ImGui.
        ''' </summary>
        Public Sub SetImGuiBlendState()

            ThrowIfDisposed()

            _device.SetRenderState(
        Dx9.RenderState.AlphaBlendEnable,
        True)

            _device.SetRenderState(
        Dx9.RenderState.SourceBlend,
        Dx9.Blend.SourceAlpha)

            _device.SetRenderState(
        Dx9.RenderState.DestinationBlend,
        Dx9.Blend.InverseSourceAlpha)

            _device.SetRenderState(
        Dx9.RenderState.SeparateAlphaBlendEnable,
        False)

            _device.SetRenderState(
        Dx9.RenderState.BlendOperation,
        Dx9.BlendOperation.Add)

        End Sub


        ''' <summary>
        ''' Configures depth, stencil and culling state for Dear ImGui.
        ''' </summary>
        Public Sub SetImGuiDepthAndCullingState()

            ThrowIfDisposed()

            _device.SetRenderState(
        Dx9.RenderState.ZEnable,
        False)

            _device.SetRenderState(
        Dx9.RenderState.ZWriteEnable,
        False)

            _device.SetRenderState(
        Dx9.RenderState.StencilEnable,
        False)

            _device.SetRenderState(
        Dx9.RenderState.CullMode,
        Dx9.Cull.None)

        End Sub


        ''' <summary>
        ''' Enables scissor testing for Dear ImGui.
        ''' </summary>
        Public Sub SetImGuiScissorState()

            ThrowIfDisposed()

            _device.SetRenderState(
        Dx9.RenderState.ScissorTestEnable,
        True)

        End Sub


        ''' <summary>
        ''' Issues a Direct3D9 indexed triangle draw call.
        ''' </summary>
        Public Sub DrawIndexedTriangles(
                        baseVertexIndex As Integer,
                        minVertexIndex As Integer,
                        vertexCount As UInteger,
                        startIndex As UInteger,
                        primitiveCount As UInteger)

            ThrowIfDisposed()

            If vertexCount = 0UI OrElse
       primitiveCount = 0UI Then

                Return

            End If

            _device.DrawIndexedPrimitive(
        Dx9.PrimitiveType.TriangleList,
        baseVertexIndex,
        CUInt(minVertexIndex),
        vertexCount,
        startIndex,
        primitiveCount)

        End Sub


        ''' <summary>
        ''' Draws a simple Direct3D9 test rectangle using XYZRHW + Diffuse.
        ''' This bypasses ImGui completely.
        ''' </summary>
        Public Sub DrawDx9TestRectangle(
    left As Single,
    top As Single,
    right As Single,
    bottom As Single,
    color As UInteger)

            ThrowIfDisposed()

            If Not _frameActive Then
                Throw New InvalidOperationException(
            "DrawDx9TestRectangle requires an active frame.")
            End If

            ' ------------------------------------------------------------
            ' Vertex layout:
            '
            ' X      0
            ' Y      4
            ' Z      8
            ' RHW   12
            ' COLOR 16
            '
            ' Total = 20 bytes
            ' ------------------------------------------------------------

            Const VertexSize As Integer = 20

            ' Triangle list = 6 vertices
            Dim buffer As IntPtr =
        Runtime.InteropServices.Marshal.AllocHGlobal(
            VertexSize * 6)

            Try

                Dim offset As Integer = 0

                ' --------------------------------------------------------
                ' Vertex 0
                ' --------------------------------------------------------
                WriteDx9TestVertex(
            buffer,
            offset,
            left,
            top,
            color)

                offset += VertexSize

                ' --------------------------------------------------------
                ' Vertex 1
                ' --------------------------------------------------------
                WriteDx9TestVertex(
            buffer,
            offset,
            right,
            top,
            color)

                offset += VertexSize

                ' --------------------------------------------------------
                ' Vertex 2
                ' --------------------------------------------------------
                WriteDx9TestVertex(
            buffer,
            offset,
            right,
            bottom,
            color)

                offset += VertexSize

                ' --------------------------------------------------------
                ' Vertex 3
                ' --------------------------------------------------------
                WriteDx9TestVertex(
            buffer,
            offset,
            left,
            top,
            color)

                offset += VertexSize

                ' --------------------------------------------------------
                ' Vertex 4
                ' --------------------------------------------------------
                WriteDx9TestVertex(
            buffer,
            offset,
            right,
            bottom,
            color)

                offset += VertexSize

                ' --------------------------------------------------------
                ' Vertex 5
                ' --------------------------------------------------------
                WriteDx9TestVertex(
            buffer,
            offset,
            left,
            bottom,
            color)

                ' --------------------------------------------------------
                ' Fixed function state
                ' --------------------------------------------------------

                _device.SetRenderState(
            Dx9.RenderState.ZEnable,
            False)

                _device.SetRenderState(
            Dx9.RenderState.ZWriteEnable,
            False)

                _device.SetRenderState(
            Dx9.RenderState.Lighting,
            False)

                _device.SetRenderState(
            Dx9.RenderState.CullMode,
            Dx9.Cull.None)

                _device.SetRenderState(
            Dx9.RenderState.AlphaBlendEnable,
            False)

                ' No texture
                _device.SetTexture(
            0UI,
            Nothing)

                ' --------------------------------------------------------
                ' IMPORTANT:
                ' XYZRHW + DIFFUSE
                ' --------------------------------------------------------

                _device.VertexFormat =
            Dx9.VertexFormat.PositionRhw Or
            Dx9.VertexFormat.Diffuse

                ' --------------------------------------------------------
                ' Draw
                ' --------------------------------------------------------

                _device.DrawPrimitiveUP(
            Dx9.PrimitiveType.TriangleList,
            2UI,
            buffer,
            VertexSize)

            Finally

                Runtime.InteropServices.Marshal.FreeHGlobal(buffer)

            End Try

        End Sub

        Private Shared Sub WriteDx9TestVertex(
    destination As IntPtr,
    offset As Integer,
    x As Single,
    y As Single,
    color As UInteger)

            Marshal.WriteInt32(
        IntPtr.Add(destination, offset + 0),
        BitConverter.ToInt32(
            BitConverter.GetBytes(x),
            0))

            Marshal.WriteInt32(
        IntPtr.Add(destination, offset + 4),
        BitConverter.ToInt32(
            BitConverter.GetBytes(y),
            0))

            Marshal.WriteInt32(
        IntPtr.Add(destination, offset + 8),
        BitConverter.ToInt32(
            BitConverter.GetBytes(0.0F),
            0))

            Marshal.WriteInt32(
        IntPtr.Add(destination, offset + 12),
        BitConverter.ToInt32(
            BitConverter.GetBytes(1.0F),
            0))

            Marshal.WriteInt32(
        IntPtr.Add(destination, offset + 16),
        BitConverter.ToInt32(
            BitConverter.GetBytes(color),
            0))

        End Sub

        Public Sub SetImGuiTextureDisabled()

            ThrowIfDisposed()

            _device.SetTexture(
        0UI,
        Nothing)

            _device.SetTextureStageState(
        0UI,
        Dx9.TextureStage.ColorOperation,
        Dx9.TextureOperation.SelectArg2)

            _device.SetTextureStageState(
        0UI,
        Dx9.TextureStage.ColorArg2,
        Dx9.TextureArgument.Diffuse)

            _device.SetTextureStageState(
        0UI,
        Dx9.TextureStage.AlphaOperation,
        Dx9.TextureOperation.SelectArg2)

            _device.SetTextureStageState(
        0UI,
        Dx9.TextureStage.AlphaArg2,
        Dx9.TextureArgument.Diffuse)

        End Sub

#End Region

#Region "Private"

        ''' <summary>
        ''' Validates the lifetime of this rendering context.
        ''' </summary>
        Private Sub ThrowIfDisposed()

            If _disposed Then
                Throw New ObjectDisposedException(
                    NameOf(Dx9GraphicsContext))
            End If

        End Sub

#End Region

#Region "Dispose"

        ''' <summary>
        ''' Releases the rendering context.
        ''' </summary>
        Public Sub Dispose() _
            Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If

            If _frameActive Then

                Try
                    _device.EndScene()
                Catch
                    ' Ignore cleanup failure during disposal.
                End Try

                _frameActive = False

            End If

            _disposed = True

        End Sub

#End Region

    End Class

End Namespace