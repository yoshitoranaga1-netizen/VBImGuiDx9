Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Core

    ''' <summary>
    ''' Provides version information for VBImGuiDx9.
    ''' </summary>
    Public NotInheritable Class VersionInfo

        Private Sub New()
        End Sub

#Region "Constants"

        ''' <summary>
        ''' Gets the library name.
        ''' </summary>
        Public Const Name As String = "VBImGuiDx9"

        ''' <summary>
        ''' Gets the major version number.
        ''' </summary>
        Public Const Major As Integer = 0

        ''' <summary>
        ''' Gets the minor version number.
        ''' </summary>
        Public Const Minor As Integer = 1

        ''' <summary>
        ''' Gets the patch version number.
        ''' </summary>
        Public Const Patch As Integer = 0

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current library version.
        ''' </summary>
        Public Shared ReadOnly Property Version As Version
            Get
                Return New Version(Major, Minor, Patch)
            End Get
        End Property

        ''' <summary>
        ''' Gets the complete library version string.
        ''' </summary>
        Public Shared ReadOnly Property FullVersion As String
            Get
                Return $"{Name} {Version}"
            End Get
        End Property

#End Region

    End Class

End Namespace