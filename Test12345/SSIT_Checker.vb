Imports System.Threading
Imports System.Net.NetworkInformation

Public Class SSIT_Checker
    Dim trd As Thread

    Public config As New config
    Public configFile As String = Application.StartupPath & "\config"

    Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)
        MyBase.OnFormClosing(e)
        If Not e.Cancel AndAlso e.CloseReason = CloseReason.UserClosing Then
            Me.WindowState = FormWindowState.Minimized
        End If

    End Sub

    Private Sub LoadConfig()
        If System.IO.File.Exists(configFile) Then
            config = Newtonsoft.Json.JsonConvert.DeserializeObject(Of config)(System.IO.File.ReadAllText(configFile))
            My.Settings.db_DSN = config.DSN
            My.Settings.db_Server = config.Server
            My.Settings.DB_Name = config.Database
            My.Settings.db_UName = config.User
            My.Settings.db_Pass = config.Password
            My.Settings.Save()
            My.Settings.Reload()
        Else
            MessageBox.Show("Unable to find config file. Application will be closed.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Environment.Exit(0)
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadConfig()

        If My.Settings.firstRun = 0 Then
            _frmSettings.ShowDialog()
        Else
            'GC.Collect()
            Control.CheckForIllegalCrossThreadCalls = False
            trd = New Thread(AddressOf ThreadTask)
            trd.IsBackground = True
            trd.Start()
        End If

    End Sub
    Public Sub runTime()

        Dim ip As String = ""
        Dim db As New ConnectionString
        Dim lvPc As New ListView
        Dim getdate As String = Date.Today.ToShortDateString
        getdate = getdate.Replace("/", "-")
        Dim myPath As String = Application.StartupPath & "\LOGS"
        If (Not System.IO.Directory.Exists(myPath)) Then
            System.IO.Directory.CreateDirectory(myPath)
        End If
        Try
            GC.Collect()
            db.FillListView(db.ExecuteSQLQuery("SELECT KIOSK_NM as'Host Name',STATUS,BRANCH_IP as 'IP ADDRESS',BRANCH_CD as 'Branch Code',CLSTR,DIVSN FROM SSINFOTERMKIOSK where TAG = '1' and isVPN = 'false'"), lvPc)
            For Each lvi As ListViewItem In lvPc.Items
                lvi.UseItemStyleForSubItems = False
                ip = lvi.SubItems(2).Text
                If PING_SSS_IP(ip) Then
                    'db.ExecuteSQLQuery("Update SSINFOTERMKIOSK set STATUS = 'true' where BRANCH_IP='" & ip & "'")
                    If db.checkExistence("SELECT BRANCH_IP FROM SSMONITORING WHERE STATUS ='true' and BRANCH_IP = '" & ip & "' and cast(datestamp as date) = '" & Today & "'") Then
                        Dim timeStamp As String = db.putSingleValue("Select max(DATESTAMP) from SSMONITORING where BRANCH_IP = '" & ip & "'")
                        db.ExecuteSQLQuery("UPDATE SSMONITORING SET ONLINE_DT = '" & DateAndTime.Now & "',STATUS = 'false' where BRANCH_IP = '" & ip & "' and datestamp = '" & timeStamp & "'")
                    Else
                        ' db.ExecuteSQLQuery("UPDATE SSINFOTERMKIOSK SET LONLINE_DT = '" & DateAndTime.Now & "' where BRANCH_IP = '" & ip & "'")
                        ' db.ExecuteSQLQuery("UPDATE SSMONITORING SET STATUS = 'false' where BRANCH_IP = '" & ip & "'")
                    End If
                Else
                    'db.ExecuteSQLQuery("Update  SSINFOTERMKIOSK set STATUS = 'false' where BRANCH_IP='" & ip & "'")
                    'SELECT IP WITH STATUS = TRUE, IF FOUND, UPDATE ROW. IF NOT INSERT
                    If db.checkExistence("SELECT BRANCH_IP FROM SSMONITORING WHERE STATUS ='true' and BRANCH_IP = '" & ip & "' and cast(datestamp as date) = '" & Today & "'") Then
                        'db.ExecuteSQLQuery("UPDATE SSMONITORING SET STATUS = 'false', ONLINE_DT = '" & DateAndTime.Now & "' where BRANCH_IP = '" & ip & "'")
                        'db.ExecuteSQLQuery("UPDATE SSMONITORING SET STATUS = 'true' where BRANCH_IP = '" & ip & "'")
                    Else
                        db.ExecuteSQLQuery("INSERT INTO SSMONITORING(BRANCH_IP,BRANCH_CD,CLSTR,DIVSN,OFFLINE_DT,DATESTAMP,STATUS) values ('" & lvi.SubItems(2).Text & "','" & lvi.SubItems(3).Text & "','" & lvi.SubItems(4).Text & "','" & lvi.SubItems(5).Text & "','" & DateAndTime.Now & "','" & DateAndTime.Now & "','true')")
                        'db.ExecuteSQLQuery("UPDATE SSMONITORING SET STATUS = 'true' where BRANCH_IP = '" & ip & "'")
                    End If
                End If


                Dim SW As New IO.StreamWriter(myPath & "\" & "CONNECTION STATUS" & "," & getdate & ".txt", True)
                Dim Result As String = lvi.SubItems(1).Text
                Select Case Result
                    Case "True"
                        SW.WriteLine(ip & "," & "ONLINE" & "," & DateAndTime.Now)
                    Case "False"
                        SW.WriteLine(ip & "," & "OFFLINE" & "," & DateAndTime.Now)
                End Select

                SW.Close()
                SW.Dispose()
                SW = Nothing

                ip = ""
            Next

        Catch ex As Exception
            Dim errorLogs As String = ex.Message
            errorLogs = errorLogs.Trim
            Dim SW As New IO.StreamWriter(myPath & "\" & "ERROR LOGS" & getdate & ".txt", True)
            SW.WriteLine(ip & ": " & errorLogs & "," & DateAndTime.Now)
            SW.Close()
            SW.Dispose()
            SW = Nothing
        Finally
            db = Nothing
            lvPc = Nothing

        End Try
    End Sub

    Private Function PING_SSS_IP(ByVal IP As String) As Boolean
        Dim myping As Ping = New Ping
        Dim pingreply As PingReply
        'create the ping packet size in bytes               
        Dim buffer() As Byte = System.Text.Encoding.ASCII.GetBytes("aa")

        For x As Integer = 0 To 3
            pingreply = myping.Send(IP, 1000, buffer)
            If pingreply.Status = IPStatus.Success Then
                myping = Nothing
                pingreply = Nothing
                Return True
            End If
        Next
    End Function


    Private IsProcessing As Boolean = False

    Private Sub ThreadTask()
        Do While True
            If Not IsProcessing Then
                IsProcessing = True
                Try
                    runTime()
                    System.Threading.Thread.Sleep(1000)
                Catch ex As Exception
                    'MsgBox("Connection Time-out")
                End Try

                IsProcessing = False
            End If
        Loop
    End Sub
End Class

