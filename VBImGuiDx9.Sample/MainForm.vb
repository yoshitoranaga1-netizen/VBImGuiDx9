Option Strict On
Option Explicit On
Option Infer On

Imports System.Drawing
Imports System.Windows.Forms
Imports ImGuiNET
Imports VBImGuiDx9.Sample.Diagnostics
Imports VBImGuiDx9.Sample.UI
Imports VBImGuiDx9.VBImGuiDx9.Backends.Direct3D9
Imports VBImGuiDx9.VBImGuiDx9.Core
Imports VBImGuiDx9.VBImGuiDx9.Core.ImGuiWindows

Namespace VBImGuiDx9.Sample

    ''' <summary>
    ''' Minimal host used to validate the integration between
    ''' WinForms, Direct3D9 and Dear ImGui.
    '''
    ''' At this stage ImGui generates ImDrawData, while the
    ''' Direct3D9 ImGui renderer is intentionally not implemented.
    ''' </summary>
    Public NotInheritable Class MainForm
        Inherits Form

#Region "Fields"

        Private _graphicsDevice As Dx9GraphicsDevice
        Private _graphicsContext As Dx9GraphicsContext
        Private _imguiContext As ImGuiContextManager

        Private _imguiFrame As ImGuiFrameController
        Private _imguiRenderer As Dx9ImGuiRenderer
        Private _imguiWindowManager As ImGuiWindowManager
        Private _sampleWindows As SampleWindowSet
        Private _frameProfiler As FrameProfiler
        Private ReadOnly _frameTimer As Timer
        Private _insertWasDown As Boolean

        Private _fontService As FontService

        Private _initialized As Boolean
        Private _closing As Boolean
        Private _resizePending As Boolean

        Private COLOR_RED As UInteger = &HFFFF0000UI
        Private COLOR_GREEN As UInteger = &HFF00FF00UI
        Private COLOR_BLUE As UInteger = &HFF0000FFUI
        Private COLOR_YELLOW_GREEN As UInteger = &HFFADFF2FUI
        Private COLOR_WHITE As UInteger = &HFFFFFFFFUI
        Private COLOR_BLACK As UInteger = &HFF000000UI
        Private COLOR_GRAY As UInteger = &HFF808080UI
        Private COLOR_DARKGRAY As UInteger = &HFFA9A9A9UI
        Private COLOR_LIGHTGRAY As UInteger = &HFFD3D3D3UI
        Private COLOR_YELLOW As UInteger = &HFFFFFF00UI
        Private COLOR_ORANGE As UInteger = &HFFFFA500UI
        Private COLOR_GOLD As UInteger = &HFFFFD700UI
        Private COLOR_CYAN As UInteger = &HFF00FFFFUI
        Private COLOR_MAGENTA As UInteger = &HFF00FFUI
        Private COLOR_PURPLE As UInteger = &HFF800080UI
        Private COLOR_LIME As UInteger = &HFF32CD32UI
        Private COLOR_AQUA As UInteger = &HFF00CED1UI
        Private COLOR_PINK As UInteger = &HFFFF69B4UI
        Private COLOR_BROWN As UInteger = &HFFA52A2AUI
        Private COLOR_MAROON As UInteger = &HFF800000UI
        Private COLOR_OLIVE As UInteger = &HFF808000UI
        Private COLOR_NAVY As UInteger = &HFF000080UI

#End Region

#Region "Constructor"

        Public Sub New()

            Text =
                "VBImGuiDx9 - ImGui Integration Test"

            'ClientSize =
            '    New Size(
            '        1280,
            '        720)

            Me.Width = 1280
            Me.Height = 720
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.ShowInTaskbar = True
            Me.BackColor = Color.Gray
            Me.TopMost = True
            'Me.TransparencyKey = Color.Magenta
            'Me.WindowState = FormWindowState.Maximized
            'Me.StartPosition = FormStartPosition.Manual
            'Me.Bounds = Screen.PrimaryScreen.Bounds

            ' ДОПОЛНИТЕЛЬНЫЕ СТИЛИ ДЛЯ ПРОЗРАЧНОСТИ
            'Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.Opaque, True)
            Me.DoubleBuffered = True

            'Dim exStyle As Integer = WinAPI.GetWindowLong(Me.Handle, WinAPI.GWL.GWL_EXSTYLE)
            'exStyle = exStyle Or WinAPI.WS_EX.WS_EX_LAYERED
            'exStyle = exStyle Or WinAPI.WS_EX.WS_EX_TRANSPARENT
            'exStyle = exStyle Or WinAPI.WS_EX.WS_EX_TOPMOST
            'exStyle = exStyle Or WinAPI.WS_EX.WS_EX_TOOLWINDOW
            'WinAPI.SetWindowLong(Me.Handle, WinAPI.GWL.GWL_EXSTYLE, exStyle)

            'If WinAPI.OBSProtectionEnabled Then
            'WinAPI.SetWindowDisplayAffinity(Me.Handle, WinAPI.WDA.WDA_EXCLUDEFROMCAPTURE)
            'End If

            _frameTimer =
                New Timer With {
                    .Interval = 16
                }

            AddHandler Resize,
                AddressOf OnFormResize

            AddHandler _frameTimer.Tick,
                AddressOf OnFrame

        End Sub

#End Region


#Region "Initialization"

        Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
            ' Ничего не рисуем.
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            ' Ничего не рисуем.
        End Sub

        Protected Overrides Sub OnShown(
                                       e As EventArgs)

            MyBase.OnShown(e)

            Try

                ' ----------------------------------------------------
                ' Dear ImGui
                ' ----------------------------------------------------

                _imguiContext =
                    New ImGuiContextManager()

                _imguiContext.Initialize()

                _imguiFrame =
                    New ImGuiFrameController(
                        _imguiContext)

                ' ----------------------------------------------------
                ' ImGui Window Manager
                ' ----------------------------------------------------
                _frameProfiler =
                        New FrameProfiler()

                _imguiWindowManager =
                         New ImGuiWindowManager()

                ' ----------------------------------------------------
                ' Direct3D9
                ' ----------------------------------------------------

                Dim deviceOptions As New DeviceOptions()

                deviceOptions.WindowHandle =
                                            Handle

                deviceOptions.Width =
                                    ClientSize.Width

                deviceOptions.Height =
                                ClientSize.Height

                deviceOptions.Windowed =
                                        True

                deviceOptions.EnableVSync =
                                        False

                deviceOptions.EnableMultithreading =
                                                    False

                _graphicsDevice =
                    New Dx9GraphicsDevice(
                        deviceOptions)

                _graphicsContext =
                    DirectCast(
                        _graphicsDevice.CreateGraphicsContext(),
                        Dx9GraphicsContext)

                _imguiRenderer =
                    New Dx9ImGuiRenderer(
                        _graphicsDevice,
                        _graphicsContext)

                _imguiRenderer.Initialize()


                _fontService =
                        New FontService(
                            _imguiContext,
                            _imguiRenderer)

                _fontService.Initialize()

                _sampleWindows =
                    New SampleWindowSet(
                        _imguiWindowManager,
                        _frameProfiler,
                        _fontService)

                _sampleWindows.Initialize()


                _initialized = True
                _resizePending = False

                _frameTimer.Start()

            Catch ex As Exception

                _frameTimer.Stop()

                MessageBox.Show(
                    Me,
                    ex.ToString(),
                    "Initialization failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)

                Close()

            End Try

        End Sub


#End Region

#Region "Frame"

        Private Sub OnFrame(
                            sender As Object,
                            e As EventArgs)

            If Not _initialized OrElse _closing Then
                Return
            End If

            Dim frameWatch As Stopwatch =
        Stopwatch.StartNew()

            If Not PrepareDeviceForFrame() Then
                Return
            End If

            Dim imguiFrameStarted As Boolean = False
            Dim dxFrameStarted As Boolean = False

            Try

                ' ----------------------------------------------------
                ' Input
                ' ----------------------------------------------------

                _frameProfiler.BeginInput()

                UpdateImGuiInput()

                _frameProfiler.EndInput()

                UpdateSettingsHotkey()

                ' ----------------------------------------------------
                ' Device / BeginFrame
                ' ----------------------------------------------------

                _frameProfiler.BeginDevice()

                _graphicsContext.BeginFrame()
                dxFrameStarted = True

                ' _graphicsContext.Clear(&HFFFF00FFUI)COLOR_GRAY
                _graphicsContext.Clear(COLOR_GRAY)

                _frameProfiler.EndDevice()


                ' ----------------------------------------------------
                ' ImGui build
                ' ----------------------------------------------------

                _frameProfiler.BeginImGuiBuild()

                _imguiFrame.BeginFrame()
                imguiFrameStarted = True

                _sampleWindows.RenderMenu()

                _imguiWindowManager.RenderAll()

                _frameProfiler.EndImGuiBuild()


                ' ----------------------------------------------------
                ' ImGui render
                ' ----------------------------------------------------

                _frameProfiler.BeginImGuiRender()

                _imguiFrame.EndFrame()
                imguiFrameStarted = False

                _frameProfiler.EndImGuiRender()


                ' ----------------------------------------------------
                ' DX9 draw
                ' ----------------------------------------------------

                _frameProfiler.BeginDx9Draw()

                Dim drawData As ImDrawDataPtr =
            _imguiFrame.DrawData

                _imguiRenderer.RenderDrawData(
            drawData)

                _frameProfiler.EndDx9Draw()


                ' ----------------------------------------------------
                ' DX9 EndFrame
                ' ----------------------------------------------------

                _graphicsContext.EndFrame()
                dxFrameStarted = False


                ' ----------------------------------------------------
                ' Present
                ' ----------------------------------------------------

                _frameProfiler.BeginPresent()

                _graphicsContext.Present()

                _frameProfiler.EndPresent()


                ' ----------------------------------------------------
                ' Complete frame measurement
                ' ----------------------------------------------------

                _frameProfiler.RecordFrame(
            frameWatch.Elapsed.TotalMilliseconds)


            Catch ex As Exception

                _frameTimer.Stop()
                _closing = True

                MessageBox.Show(
            Me,
            ex.ToString(),
            "Frame rendering failed",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

                Close()


            Finally

                ' ----------------------------------------------------
                ' Safety cleanup: ImGui frame
                ' ----------------------------------------------------

                If imguiFrameStarted Then

                    Try

                        _imguiFrame.EndFrame()

                    Catch ex As Exception

                        Debug.WriteLine(ex)

                    End Try

                End If


                ' ----------------------------------------------------
                ' Safety cleanup: DX9 frame
                ' ----------------------------------------------------

                If dxFrameStarted Then

                    Try

                        _graphicsContext.EndFrame()

                    Catch ex As Exception

                        Debug.WriteLine(ex)

                    End Try

                End If

            End Try

        End Sub


        Private Function PrepareDeviceForFrame() As Boolean
            If _graphicsDevice Is Nothing OrElse
               _imguiRenderer Is Nothing Then
                Return False
            End If

            If ClientSize.Width <= 0 OrElse
               ClientSize.Height <= 0 Then
                Return False
            End If

            Dim status As Dx9DeviceStatus =
                _graphicsDevice.GetDeviceStatus()

            If status = Dx9DeviceStatus.DeviceLost Then
                Return False
            End If

            If status = Dx9DeviceStatus.DriverInternalError Then
                Throw New InvalidOperationException(
                    "Direct3D9 reported a driver internal error.")
            End If

            If status = Dx9DeviceStatus.Unknown Then
                Throw New InvalidOperationException(
                    "Direct3D9 returned an unknown device status.")
            End If

            Dim requiresReset As Boolean =
                _resizePending OrElse
                status = Dx9DeviceStatus.DeviceNotReset

            If Not requiresReset Then
                Return True
            End If

            _imguiRenderer.InvalidateDeviceObjects()

            If Not _graphicsDevice.TryReset(
                ClientSize.Width,
                ClientSize.Height) Then
                Return False
            End If

            _imguiRenderer.Initialize()
            _resizePending = False

            Return True
        End Function

        Private Sub OnFormResize(
            sender As Object,
            e As EventArgs)

            If _closing Then
                Return
            End If

            _resizePending = True
        End Sub

#End Region

#Region "Input"

        Private Sub UpdateImGuiInput()

            Try

                ImGui.SetCurrentContext(
            _imguiContext.Context)

                Dim io As ImGuiIOPtr =
            ImGui.GetIO()

                ' ------------------------------------------------------------
                ' Display
                ' ------------------------------------------------------------

                io.DisplaySize =
            New System.Numerics.Vector2(
                CSng(ClientSize.Width),
                CSng(ClientSize.Height))

                ' ------------------------------------------------------------
                ' Mouse
                ' ------------------------------------------------------------

                Dim mousePosition As Point =
            PointToClient(Cursor.Position)

                io.AddMousePosEvent(
            CSng(mousePosition.X),
            CSng(mousePosition.Y))

                io.AddMouseButtonEvent(
            0,
            (WinAPI.GetAsyncKeyState(
                WinAPI.VK_LBUTTON) And &H8000) <> 0)

                io.AddMouseButtonEvent(
            1,
            (WinAPI.GetAsyncKeyState(
                WinAPI.VK_RBUTTON) And &H8000) <> 0)

                io.AddMouseButtonEvent(
            2,
            (WinAPI.GetAsyncKeyState(
                WinAPI.VK_MBUTTON) And &H8000) <> 0)

                ' ------------------------------------------------------------
                ' Keyboard
                ' ------------------------------------------------------------

                For vk As Integer = 0 To 255

                    Dim imguiKey As ImGuiKey =
                ConvertVKToImGuiKey(vk)

                    If imguiKey <> ImGuiKey.None Then

                        Dim pressed As Boolean =
                    (WinAPI.GetAsyncKeyState(vk) And &H8000) <> 0

                        io.AddKeyEvent(
                    imguiKey,
                    pressed)

                    End If

                Next

                ' ------------------------------------------------------------
                ' Modifiers
                ' ------------------------------------------------------------

                io.AddKeyEvent(
            ImGuiKey.ModShift,
            (WinAPI.GetAsyncKeyState(
                Keys.ShiftKey) And &H8000) <> 0)

                io.AddKeyEvent(
            ImGuiKey.ModCtrl,
            (WinAPI.GetAsyncKeyState(
                Keys.ControlKey) And &H8000) <> 0)

                io.AddKeyEvent(
            ImGuiKey.ModAlt,
            (WinAPI.GetAsyncKeyState(
                Keys.Menu) And &H8000) <> 0)

            Catch ex As Exception

            End Try

        End Sub

        Private Function ConvertVKToImGuiKey(
                    vk As Integer) As ImGuiKey

            Select Case vk

        ' ------------------------------------------------------------
        ' Control
        ' ------------------------------------------------------------

                Case &H9
                    Return ImGuiKey.Tab

                Case &HD
                    Return ImGuiKey.Enter

                Case &H1B
                    Return ImGuiKey.Escape

                Case &H20
                    Return ImGuiKey.Space

                Case &H8
                    Return ImGuiKey.Backspace

        ' ------------------------------------------------------------
        ' Arrows
        ' ------------------------------------------------------------

                Case &H25
                    Return ImGuiKey.LeftArrow

                Case &H26
                    Return ImGuiKey.UpArrow

                Case &H27
                    Return ImGuiKey.RightArrow

                Case &H28
                    Return ImGuiKey.DownArrow

        ' ------------------------------------------------------------
        ' Navigation
        ' ------------------------------------------------------------

                Case &H2D
                    Return ImGuiKey.Insert

                Case &H2E
                    Return ImGuiKey.Delete

                Case &H24
                    Return ImGuiKey.Home

                Case &H23
                    Return ImGuiKey.End

                Case &H21
                    Return ImGuiKey.PageUp

                Case &H22
                    Return ImGuiKey.PageDown

        ' ------------------------------------------------------------
        ' Letters A-Z
        ' ------------------------------------------------------------

                Case &H41
                    Return ImGuiKey.A

                Case &H42
                    Return ImGuiKey.B

                Case &H43
                    Return ImGuiKey.C

                Case &H44
                    Return ImGuiKey.D

                Case &H45
                    Return ImGuiKey.E

                Case &H46
                    Return ImGuiKey.F

                Case &H47
                    Return ImGuiKey.G

                Case &H48
                    Return ImGuiKey.H

                Case &H49
                    Return ImGuiKey.I

                Case &H4A
                    Return ImGuiKey.J

                Case &H4B
                    Return ImGuiKey.K

                Case &H4C
                    Return ImGuiKey.L

                Case &H4D
                    Return ImGuiKey.M

                Case &H4E
                    Return ImGuiKey.N

                Case &H4F
                    Return ImGuiKey.O

                Case &H50
                    Return ImGuiKey.P

                Case &H51
                    Return ImGuiKey.Q

                Case &H52
                    Return ImGuiKey.R

                Case &H53
                    Return ImGuiKey.S

                Case &H54
                    Return ImGuiKey.T

                Case &H55
                    Return ImGuiKey.U

                Case &H56
                    Return ImGuiKey.V

                Case &H57
                    Return ImGuiKey.W

                Case &H58
                    Return ImGuiKey.X

                Case &H59
                    Return ImGuiKey.Y

                Case &H5A
                    Return ImGuiKey.Z

        ' ------------------------------------------------------------
        ' Number row
        ' ------------------------------------------------------------

                Case &H30
                    Return ImGuiKey._0

                Case &H31
                    Return ImGuiKey._1

                Case &H32
                    Return ImGuiKey._2

                Case &H33
                    Return ImGuiKey._3

                Case &H34
                    Return ImGuiKey._4

                Case &H35
                    Return ImGuiKey._5

                Case &H36
                    Return ImGuiKey._6

                Case &H37
                    Return ImGuiKey._7

                Case &H38
                    Return ImGuiKey._8

                Case &H39
                    Return ImGuiKey._9

        ' ------------------------------------------------------------
        ' Function keys
        ' ------------------------------------------------------------

                Case &H70
                    Return ImGuiKey.F1

                Case &H71
                    Return ImGuiKey.F2

                Case &H72
                    Return ImGuiKey.F3

                Case &H73
                    Return ImGuiKey.F4

                Case &H74
                    Return ImGuiKey.F5

                Case &H75
                    Return ImGuiKey.F6

                Case &H76
                    Return ImGuiKey.F7

                Case &H77
                    Return ImGuiKey.F8

                Case &H78
                    Return ImGuiKey.F9

                Case &H79
                    Return ImGuiKey.F10

                Case &H7A
                    Return ImGuiKey.F11

                Case &H7B
                    Return ImGuiKey.F12

        ' ------------------------------------------------------------
        ' Numpad
        ' ------------------------------------------------------------

                Case &H60
                    Return ImGuiKey.Keypad0

                Case &H61
                    Return ImGuiKey.Keypad1

                Case &H62
                    Return ImGuiKey.Keypad2

                Case &H63
                    Return ImGuiKey.Keypad3

                Case &H64
                    Return ImGuiKey.Keypad4

                Case &H65
                    Return ImGuiKey.Keypad5

                Case &H66
                    Return ImGuiKey.Keypad6

                Case &H67
                    Return ImGuiKey.Keypad7

                Case &H68
                    Return ImGuiKey.Keypad8

                Case &H69
                    Return ImGuiKey.Keypad9

                Case Else
                    Return ImGuiKey.None

            End Select

        End Function

#End Region

#Region "Shutdown"

        Protected Overrides Sub OnFormClosed(
                                            e As FormClosedEventArgs)

            MyBase.OnFormClosed(e)

            _frameTimer.Stop()

            _closing = True
            _initialized = False

            If _sampleWindows IsNot Nothing Then
                _sampleWindows.Dispose()
                _sampleWindows = Nothing
            End If

            If _imguiWindowManager IsNot Nothing Then
                _imguiWindowManager.Dispose()
                _imguiWindowManager = Nothing
            End If


            If _imguiFrame IsNot Nothing Then
                _imguiFrame.Dispose()
                _imguiFrame = Nothing
            End If

            If _imguiRenderer IsNot Nothing Then
                _imguiRenderer.Dispose()
                _imguiRenderer = Nothing
            End If

            If _imguiContext IsNot Nothing Then
                _imguiContext.Dispose()
                _imguiContext = Nothing
            End If

            If _graphicsContext IsNot Nothing Then
                _graphicsContext.Dispose()
                _graphicsContext = Nothing
            End If

            If _graphicsDevice IsNot Nothing Then
                _graphicsDevice.Dispose()
                _graphicsDevice = Nothing
            End If

        End Sub

        Private Sub InitializeComponent()
            SuspendLayout()
            ' 
            ' MainForm
            ' 
            ClientSize = New Size(958, 595)
            Name = "MainForm"
            ResumeLayout(False)

        End Sub

#End Region

#Region "HotKeys"
        Private Sub UpdateSettingsHotkey()

            Dim insertDown As Boolean =
                (WinAPI.GetAsyncKeyState(
                    WinAPI.VK_INSERT) And &H8000) <> 0

            If insertDown AndAlso Not _insertWasDown Then

                Dim state As ImGuiWindowState =
                    _imguiWindowManager.GetState(
                        "settings")

                If state IsNot Nothing Then

                    _imguiWindowManager.SetVisible(
                        "settings",
                        Not state.Visible)

                End If

            End If

            _insertWasDown = insertDown

        End Sub
#End Region

    End Class

End Namespace