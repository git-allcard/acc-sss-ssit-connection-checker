
Imports System.IO
Public Class Log

    Public Shared ErrorLog As String = ""
    Public Shared SystemLog As String = ""
    Public Shared ConnectionStatusLog As String = ""

    Public Shared Sub SaveConnectionStatusLog(ByVal desc As String)
        Dim logFolder As String = String.Concat(Application.StartupPath, "\Log\", Now.ToString("yyyyMMdd"))
        If Not System.IO.Directory.Exists(logFolder) Then System.IO.Directory.CreateDirectory(logFolder)
        ConnectionStatusLog = Path.Combine(logFolder, "ConnectionStatus.txt")

        Dim SW As New IO.StreamWriter(ConnectionStatusLog, True)
        SW.WriteLine(String.Concat(Timestamp, " ", desc.Trim))
        SW.Close()
        SW.Dispose()
        SW = Nothing
    End Sub

    Public Shared Sub SaveSystemLog(ByVal desc As String)
        Dim logFolder As String = String.Concat(Application.StartupPath, "\Log\", Now.ToString("yyyyMMdd"))
        If Not System.IO.Directory.Exists(logFolder) Then System.IO.Directory.CreateDirectory(logFolder)
        SystemLog = Path.Combine(logFolder, "System.txt")

        Dim SW As New IO.StreamWriter(SystemLog, True)
        SW.WriteLine(String.Concat(Timestamp, " ", desc.Trim))
        SW.Close()
        SW.Dispose()
        SW = Nothing
    End Sub

    Public Shared Sub SaveErrorLog(ByVal desc As String)
        Dim logFolder As String = String.Concat(Application.StartupPath, "\Log\", Now.ToString("yyyyMMdd"))
        If Not System.IO.Directory.Exists(logFolder) Then System.IO.Directory.CreateDirectory(logFolder)
        ErrorLog = Path.Combine(logFolder, "Error.txt")

        Dim SW As New IO.StreamWriter(ErrorLog, True)
        SW.WriteLine(String.Concat(Timestamp, " ", desc.Trim))
        SW.Close()
        SW.Dispose()
        SW = Nothing
    End Sub

    Public Shared Function Timestamp() As String
        Return Now.ToString("MM/dd/yyyy hh:mm:ss tt")
    End Function



End Class
