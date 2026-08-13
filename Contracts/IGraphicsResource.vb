Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Contracts

    ''' <summary>
    ''' Represents a resource owned by a graphics device.
    ''' </summary>
    Public Interface IGraphicsResource
        Inherits IDisposable

        ''' <summary>
        ''' Gets the graphics device that owns the resource.
        ''' </summary>
        ReadOnly Property Device As IGraphicsDevice

    End Interface

End Namespace