Imports System.IO

Public Class Logger
    Private Shared logPath As String = "log.txt"

    Public Shared Sub Log(msg As String)
        Try
            Dim line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}"
            File.AppendAllText(logPath, line & Environment.NewLine)
        Catch
        End Try
    End Sub
End Class
