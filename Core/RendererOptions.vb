Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Core

    ''' <summary>
    ''' Specifies renderer configuration options.
    ''' </summary>
    Public NotInheritable Class RendererOptions

#Region "Properties"

        ''' <summary>
        ''' Gets or sets whether diagnostic logging is enabled.
        ''' </summary>
        Public Property EnableDebugLogging As Boolean

        ''' <summary>
        ''' Gets or sets whether graphics-state validation is enabled.
        ''' </summary>
        Public Property ValidateGraphicsState As Boolean

        ''' <summary>
        ''' Gets or sets whether frame statistics are collected.
        ''' </summary>
        Public Property CollectStatistics As Boolean = True

#End Region

    End Class

End Namespace