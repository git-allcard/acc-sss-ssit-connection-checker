Public Class _frmSettings
    Dim db As New ConnectionString
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim temp As String = "DSN=" & txtDSN.Text & ";SERVER=" & txtSERVER.Text & ";DATABASE=" & TextBox5.Text & ";UID=" & txtUserName.Text & ";PWD=" & txtPassword.Text & ""
        'Dim temp As String = "SERVER=" & TextBox2.Text & ";DATABASE=" & TextBox5.Text & ";UID=" & TextBox3.Text & ";PWD=" & TextBox4.Text & ""
        ''Dim temp As String = "Data Source=" & TextBox2.Text & ";Initial Catalog=" & TextBox5.Text & ";User ID=" & TextBox3.Text & ";Password=" & TextBox4.Text & ""

        If db.webisconnected(temp) Then
            MsgBox("Parameters are correct" & vbNewLine & "You are now connected to server", MsgBoxStyle.Information)
            btnSave.Enabled = True
        Else
            MsgBox("Unable to connect!" & vbNewLine & "Please check if all the parameters are correct", MsgBoxStyle.Exclamation)
            btnSave.Enabled = False
        End If

    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            My.Settings.db_Server = txtSERVER.Text
            My.Settings.db_UName = txtUserName.Text
            My.Settings.db_Pass = txtPassword.Text
            My.Settings.DB_Name = TextBox5.Text
            My.Settings.db_DSN = txtDSN.Text
            My.Settings.firstRun = "1"
            My.Settings.Save()
            My.Settings.Reload()

            SSIT_Checker.config.DSN = txtDSN.Text
            SSIT_Checker.config.Server = txtSERVER.Text
            SSIT_Checker.config.Database = TextBox5.Text
            SSIT_Checker.config.User = txtUserName.Text
            SSIT_Checker.config.Password = txtPassword.Text
            System.IO.File.WriteAllText(SSIT_Checker.configFile, Newtonsoft.Json.JsonConvert.SerializeObject(SSIT_Checker.config))

            MsgBox("New paramenters has been saved.", MsgBoxStyle.Information)
            MsgBox("System automatically refresh your settings", MsgBoxStyle.Information)

            Application.Exit()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation)
        End Try
    End Sub
End Class