Imports System.ComponentModel
Imports System.Runtime.InteropServices

Public Class WinAPI
    ' Mouse events
    Public Const MOUSEEVENTF_MOVE As UInteger = &H1

    ' Virtual Keys
    Public Const VK_TAB As Integer = &H9
    Public Const VK_RETURN As Integer = &HD
    Public Const VK_SHIFT As Integer = &H10
    Public Const VK_CONTROL As Integer = &H11
    Public Const VK_MENU As Integer = &H12      ' Alt
    Public Const VK_PAUSE As Integer = &H13
    Public Const VK_CAPITAL As Integer = &H14   ' Caps Lock
    Public Const VK_ESCAPE As Integer = &H1B
    Public Const VK_SPACE As Integer = &H20

    Public Const VK_F1 As Integer = &H70
    Public Const VK_F2 As Integer = &H71
    Public Const VK_F3 As Integer = &H72
    Public Const VK_F4 As Integer = &H73
    Public Const VK_F5 As Integer = &H74
    Public Const VK_F6 As Integer = &H75
    Public Const VK_F7 As Integer = &H76
    Public Const VK_F8 As Integer = &H77
    Public Const VK_F9 As Integer = &H78
    Public Const VK_F10 As Integer = &H79
    Public Const VK_F11 As Integer = &H7A
    Public Const VK_F12 As Integer = &H7B

    ' Стрелки
    Public Const VK_LEFT As Integer = &H25
    Public Const VK_UP As Integer = &H26
    Public Const VK_RIGHT As Integer = &H27
    Public Const VK_DOWN As Integer = &H28

    ' Служебные
    Public Const VK_INSERT As Integer = &H2D
    Public Const VK_DELETE As Integer = &H2E
    Public Const VK_HOME As Integer = &H24
    Public Const VK_END As Integer = &H23
    Public Const VK_PRIOR As Integer = &H21     ' Page Up
    Public Const VK_NEXT As Integer = &H22      ' Page Down
    Public Const VK_BACK As Integer = &H8

    ' Мышь
    Public Const VK_LBUTTON As Integer = &H1
    Public Const VK_RBUTTON As Integer = &H2
    Public Const VK_MBUTTON As Integer = &H4
    Public Const VK_XBUTTON1 As Integer = &H5
    Public Const VK_XBUTTON2 As Integer = &H6

    ' Константы клавиш
    Public Const VK_SELECT As Integer = &H29
    Public Const VK_PRINT As Integer = &H2A
    Public Const VK_EXECUTE As Integer = &H2B
    Public Const VK_SNAPSHOT As Integer = &H2C
    Public Const VK_HELP As Integer = &H2F
    Public Const VK_0 As Integer = &H30
    Public Const VK_1 As Integer = &H31
    Public Const VK_2 As Integer = &H32
    Public Const VK_3 As Integer = &H33
    Public Const VK_4 As Integer = &H34
    Public Const VK_5 As Integer = &H35
    Public Const VK_6 As Integer = &H36
    Public Const VK_7 As Integer = &H37
    Public Const VK_8 As Integer = &H38
    Public Const VK_9 As Integer = &H39
    Public Const VK_A As Integer = &H41
    Public Const VK_B As Integer = &H42
    Public Const VK_C As Integer = &H43
    Public Const VK_D As Integer = &H44
    Public Const VK_E As Integer = &H45
    Public Const VK_F As Integer = &H46
    Public Const VK_G As Integer = &H47
    Public Const VK_H As Integer = &H48
    Public Const VK_I As Integer = &H49
    Public Const VK_J As Integer = &H4A
    Public Const VK_K As Integer = &H4B
    Public Const VK_L As Integer = &H4C
    Public Const VK_M As Integer = &H4D
    Public Const VK_N As Integer = &H4E
    Public Const VK_O As Integer = &H4F
    Public Const VK_P As Integer = &H50
    Public Const VK_Q As Integer = &H51
    Public Const VK_R As Integer = &H52
    Public Const VK_S As Integer = &H53
    Public Const VK_T As Integer = &H54
    Public Const VK_U As Integer = &H55
    Public Const VK_V As Integer = &H56
    Public Const VK_W As Integer = &H57
    Public Const VK_X As Integer = &H58
    Public Const VK_Y As Integer = &H59
    Public Const VK_Z As Integer = &H5A
    Public Const VK_NUMLOCK As Integer = &H90
    Public Const VK_SCROLL As Integer = &H91

    Public Const SW_HIDE As Integer = 0
    Public Const SW_SHOW As Integer = 5

    Public Shared _consoleHandle As IntPtr = IntPtr.Zero

    Public Shared Property OBSProtectionEnabled As Boolean = True

    ' Window styles
    Public Enum GWL As Integer
        GWL_EXSTYLE = -20
        GWL_STYLE = -16
    End Enum

    Public Enum WS_EX As Integer
        WS_EX_LAYERED = &H80000
        WS_EX_TRANSPARENT = &H20
        WS_EX_TOPMOST = &H8
        WS_EX_TOOLWINDOW = &H80
        WS_EX_APPWINDOW = &H40000
        WS_EX_NOACTIVATE = &H8000000
    End Enum

    Public Enum WDA As Integer
        WDA_NONE = 0
        WDA_EXCLUDEFROMCAPTURE = &H11
    End Enum


    ' === User32 ===
    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(ByVal vKey As Integer) As Short
    End Function

    ' Консольные функции для скрытия/показа
    <DllImport("kernel32.dll")>
    Public Shared Function GetConsoleWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function GetWindowLong(ByVal hWnd As IntPtr, ByVal nIndex As Integer) As Integer
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SetWindowLong(ByVal hWnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SetWindowDisplayAffinity(ByVal hWnd As IntPtr, ByVal dwAffinity As Integer) As Boolean
    End Function



End Class