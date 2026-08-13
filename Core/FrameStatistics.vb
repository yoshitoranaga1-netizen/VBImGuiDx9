Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Core

    ''' <summary>
    ''' Contains rendering statistics collected for a frame.
    ''' </summary>
    Public NotInheritable Class FrameStatistics

#Region "Properties"

        ''' <summary>
        ''' Gets the number of draw calls.
        ''' </summary>
        Public Property DrawCalls As Integer

        ''' <summary>
        ''' Gets the number of vertices processed.
        ''' </summary>
        Public Property Vertices As Integer

        ''' <summary>
        ''' Gets the number of indices processed.
        ''' </summary>
        Public Property Indices As Integer

        ''' <summary>
        ''' Gets the number of render-state changes.
        ''' </summary>
        Public Property RenderStateChanges As Integer

        ''' <summary>
        ''' Gets the number of texture bindings.
        ''' </summary>
        Public Property TextureBindings As Integer

        ''' <summary>
        ''' Gets the measured frame time in milliseconds.
        ''' </summary>
        Public Property FrameTimeMilliseconds As Double

        ''' <summary>
        ''' Gets the measured frames per second.
        ''' </summary>
        Public ReadOnly Property FramesPerSecond As Double
            Get
                If FrameTimeMilliseconds <= 0.0R Then
                    Return 0.0R
                End If

                Return 1000.0R / FrameTimeMilliseconds
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Resets all frame counters and timing information.
        ''' </summary>
        Public Sub Reset()

            DrawCalls = 0
            Vertices = 0
            Indices = 0
            RenderStateChanges = 0
            TextureBindings = 0
            FrameTimeMilliseconds = 0.0R

        End Sub

#End Region

    End Class

End Namespace