Option Strict On
Option Explicit On
Option Infer On

Namespace VBImGuiDx9.Contracts

    ''' <summary>
    ''' Provides diagnostic logging services.
    ''' </summary>
    Public Interface ILogger

        ''' <summary>
        ''' Writes an informational message.
        ''' </summary>
        Sub Info(message As String)

        ''' <summary>
        ''' Writes a warning message.
        ''' </summary>
        Sub Warning(message As String)

        ''' <summary>
        ''' Writes an error message.
        ''' </summary>
        Sub [Error](message As String)

    End Interface

End Namespace