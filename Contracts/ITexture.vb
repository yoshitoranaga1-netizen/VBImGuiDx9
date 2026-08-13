Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Contracts

    ''' <summary>
    ''' Represents a GPU texture.
    ''' </summary>
    Public Interface ITexture
        Inherits IGraphicsResource

#Region "Properties"

        ''' <summary>
        ''' Gets the texture width.
        ''' </summary>
        ReadOnly Property Width As Integer

        ''' <summary>
        ''' Gets the texture height.
        ''' </summary>
        ReadOnly Property Height As Integer

#End Region

    End Interface

End Namespace