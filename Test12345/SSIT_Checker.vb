Imports System.Threading
Imports System.Net.NetworkInformation

Public Class SSIT_Checker
    Dim trd As Thread

    Public config As New config

    'Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)
    '    MyBase.OnFormClosing(e)
    '    If Not e.Cancel AndAlso e.CloseReason = CloseReason.UserClosing Then
    '        Me.WindowState = FormWindowState.Minimized
    '    End If

    'End Sub

    Private Sub Init()
        If System.IO.File.Exists(Utilities.configFile) Then
            config = Newtonsoft.Json.JsonConvert.DeserializeObject(Of config)(System.IO.File.ReadAllText(Utilities.configFile))
            My.Settings.db_DSN = config.DSN
            My.Settings.db_Server = config.Server
            My.Settings.DB_Name = config.Database
            My.Settings.db_UName = config.User
            My.Settings.db_Pass = config.Password
            My.Settings.Save()
            My.Settings.Reload()

            Dim logFolder As String = String.Concat(Application.StartupPath, "\Log")
            If Not System.IO.Directory.Exists(logFolder) Then System.IO.Directory.CreateDirectory(logFolder)
        Else
            Log.SaveErrorLog("Unable to find config file. Application will be closed.")
            MessageBox.Show("Unable to find config file. Application will be closed.", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Environment.Exit(0)
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Log.SaveSystemLog("Application started")
        Label3.Text = String.Concat("V", FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion)

        Init()

        If My.Settings.firstRun = 0 Then
            frmSettings.ShowDialog()
        Else
            Utilities.reportDate = Today
            'Utilities.reportDate = Convert.ToDateTime("2022-04-22")
            Utilities.ConStr = "SERVER=" & config.Server & ";DATABASE=" & config.Database & ";UID=" & config.User & ";PWD=" & config.Password & ""

            Control.CheckForIllegalCrossThreadCalls = False
            trd = New Thread(AddressOf ThreadTask)
            trd.IsBackground = True
            trd.Start()
        End If
    End Sub

    Private Sub SSIT_Checker_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Log.SaveSystemLog("Application closed")
    End Sub

    Private Sub LabelStatus(ByVal desc As String)
        lblStatus.BeginInvoke(Sub() Me.lblStatus.Text = desc)
        Application.DoEvents()
    End Sub

    Public Sub runTime()
        Dim lvPc As New ListView
        Dim localDAL As New localDAL
        Try
            'GC.Collect()
            Dim intCntr As Integer = 1

            LabelStatus("Extracting kiosks with IP...")

            Dim dtKiosk As DataTable = localDAL.GetKioskListWithIPandNotVPN
            If Not dtKiosk Is Nothing Then
                Utilities.FillListView(dtKiosk, lvPc)

                For Each lvi As ListViewItem In lvPc.Items
                    lvi.UseItemStyleForSubItems = False
                    Dim ip As String = lvi.SubItems(2).Text

                    LabelStatus(String.Format("Pinging {0}... {1} of {2}", ip, intCntr, dtKiosk.DefaultView.Count))

                    '0=existing, 1=not existing, -1=error
                    Dim intKioskExist = localDAL.IsKioskExistInSSMonitoring(ip)
                    Dim pingResult As String = ""

                    If PING_SSS_IP(ip) Then
                        If intKioskExist = 0 Then
                            localDAL.UpdateSSMonitoringOnlineDate(ip)
                            pingResult = "Success ping,Updated existing offline record"
                        Else
                            pingResult = "Success ping,No offline record"
                        End If

                    Else
                        If intKioskExist = 1 Then
                            localDAL.InsertSSMonitoring(ip, lvi.SubItems(3).Text, lvi.SubItems(4).Text, lvi.SubItems(5).Text)
                            pingResult = "Failed ping,Inserted new offline record"
                        Else
                            pingResult = "Failed ping,With existing offline record"
                        End If
                    End If

                    Dim Result As String = lvi.SubItems(1).Text
                    Select Case Result
                        Case "True"
                            Log.SaveConnectionStatusLog(ip & "," & "OFFLINE," & pingResult)
                        Case "False"
                            Log.SaveConnectionStatusLog(ip & "," & "ONLINE," & pingResult)
                    End Select

                    intCntr += 1
                Next
            Else
                Log.SaveErrorLog(String.Concat("runtime(): ", "dtKiosk is nothing"))
            End If
        Catch ex As Exception
            Log.SaveErrorLog(String.Concat("runtime(): ", ex.Message))
        Finally
            localDAL.Dispose()
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

        Return False
    End Function


    Private IsProcessing As Boolean = False

    Private Sub ThreadTask()
        'Do While True
        '    If Not IsProcessing Then
        '        IsProcessing = True
        '        Try
        '            runTime()
        '            System.Threading.Thread.Sleep(1000)
        '        Catch ex As Exception
        '            'MsgBox("Connection Time-out")
        '        End Try

        '        IsProcessing = False
        '    End If
        'Loop

        runTime()
        LabelStatus("Closing application...")
        Environment.Exit(0)
    End Sub

End Class

