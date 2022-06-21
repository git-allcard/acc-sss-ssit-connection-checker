Public Class frmSettings

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtSERVER.Text = My.Settings.db_Server
        txtUserName.Text = My.Settings.db_UName
        txtPassword.Text = My.Settings.db_Pass
        txtDB.Text = My.Settings.DB_Name
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If txtSERVER.Text = "" Or txtDB.Text = "" Or txtUserName.Text = "" Or txtPassword.Text = "" Then
            MsgBox("Please re-check fields.", MsgBoxStyle.Exclamation)
            Return
        End If

        'Dim temp As String = "DSN=" & txtDSN.Text & ";SERVER=" & txtSERVER.Text & ";DATABASE=" & TextBox5.Text & ";UID=" & txtUserName.Text & ";PWD=" & txtPassword.Text & ""
        Dim temp As String = "SERVER=" & txtSERVER.Text & ";DATABASE=" & txtDB.Text & ";UID=" & txtUserName.Text & ";PWD=" & txtPassword.Text & ""

        Dim localDAL As New localDAL
        If localDAL.IsConnected(temp) Then
            MsgBox("Parameters are correct" & vbNewLine & "You are now connected to server.", MsgBoxStyle.Information)
            btnSave.Enabled = True
        Else
            MsgBox("Unable to connect!" & vbNewLine & "Please check if all the parameters are correct.", MsgBoxStyle.Exclamation)
            btnSave.Enabled = False
        End If
        localDAL.Dispose()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            If txtSERVER.Text = "" Or txtDB.Text = "" Or txtUserName.Text = "" Or txtPassword.Text = "" Then
                MsgBox("Please re-check fields.", MsgBoxStyle.Exclamation)
                Return
            End If

            My.Settings.db_Server = txtSERVER.Text
            My.Settings.db_UName = txtUserName.Text
            My.Settings.db_Pass = txtPassword.Text
            My.Settings.DB_Name = txtDB.Text
            My.Settings.db_DSN = ""
            My.Settings.firstRun = "1"
            My.Settings.Save()
            My.Settings.Reload()

            SSIT_Checker.config.DSN = ""
            SSIT_Checker.config.Server = txtSERVER.Text
            SSIT_Checker.config.Database = txtDB.Text
            SSIT_Checker.config.User = txtUserName.Text
            SSIT_Checker.config.Password = txtPassword.Text
            System.IO.File.WriteAllText(Utilities.configFile, Newtonsoft.Json.JsonConvert.SerializeObject(SSIT_Checker.config))

            MsgBox("New paramenters has been saved." & vbNewLine & "System automatically refresh your settings.", MsgBoxStyle.Information)

            Application.Exit()
        Catch ex As Exception
            Log.SaveErrorLog("btnSave_Click(): " & ex.Message)
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation)
        End Try
    End Sub

End Class