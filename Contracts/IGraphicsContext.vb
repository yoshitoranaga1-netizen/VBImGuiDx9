Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Contracts

    ''' <summary>
    ''' Represents a graphics command context used to execute rendering operations.
    ''' </summary>
    Public Interface IGraphicsContext
        Inherits IDisposable

#Region "Frame"

        ''' <summary>
        ''' Begins a new rendering frame.
        ''' </summary>
        Sub BeginFrame()

        ''' <summary>
        ''' Ends the current rendering frame.
        ''' </summary>
        Sub EndFrame()

        ''' <summary>
        ''' Presents the rendered frame to the display.
        ''' </summary>
        Sub Present()

#End Region

#Region "Render Target"

        ''' <summary>
        ''' Clears the current render target.
        ''' </summary>
        ''' <param name="color">
        ''' Color encoded as ARGB32.
        ''' </param>
        Sub Clear(color As UInteger)

#End Region

    End Interface

End Namespace