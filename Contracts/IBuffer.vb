Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Contracts

    ''' <summary>
    ''' Represents a generic GPU buffer.
    ''' </summary>
    Public Interface IBuffer
        Inherits IGraphicsResource

#Region "Properties"

        ''' <summary>
        ''' Gets the size of the buffer in bytes.
        ''' </summary>
        ReadOnly Property SizeInBytes As Integer

        ''' <summary>
        ''' Gets a value indicating whether the buffer is dynamic.
        ''' </summary>
        ReadOnly Property IsDynamic As Boolean

#End Region

#Region "Methods"

        ''' <summary>
        ''' Uploads data to the buffer.
        ''' </summary>
        ''' <param name="source">Pointer to the source memory.</param>
        ''' <param name="sizeInBytes">Number of bytes to copy.</param>
        Sub SetData(
            source As IntPtr,
            sizeInBytes As Integer)

#End Region

    End Interface

End Namespace