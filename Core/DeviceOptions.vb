Option Strict On
Option Explicit On
Option Infer On

Imports System

Namespace VBImGuiDx9.Core

    ''' <summary>
    ''' Specifies configuration options used when creating
    ''' a graphics device.
    ''' </summary>
    Public NotInheritable Class DeviceOptions

#Region "Window"

        ''' <summary>
        ''' Gets or sets the native window handle used by the graphics device.
        ''' </summary>
        Public Property WindowHandle As IntPtr

        ''' <summary>
        ''' Gets or sets the back-buffer width.
        ''' </summary>
        Public Property Width As Integer

        ''' <summary>
        ''' Gets or sets the back-buffer height.
        ''' </summary>
        Public Property Height As Integer

        ''' <summary>
        ''' Gets or sets whether the device operates in windowed mode.
        ''' </summary>
        Public Property Windowed As Boolean = True

#End Region

#Region "Presentation"

        ''' <summary>
        ''' Gets or sets whether vertical synchronization is enabled.
        ''' </summary>
        Public Property EnableVSync As Boolean = False

#End Region

#Region "Device"

        ''' <summary>
        ''' Gets or sets whether Direct3D9 multithreaded device
        ''' creation is enabled.
        ''' </summary>
        Public Property EnableMultithreading As Boolean = False

#End Region

    End Class

End Namespace