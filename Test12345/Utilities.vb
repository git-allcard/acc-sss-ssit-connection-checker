
Public Class Utilities

    Public Shared ConStr As String = ""
    Public Shared configFile As String = String.Concat(Application.StartupPath, "\config")
    Public Shared reportDate As Date

    Public Shared Sub FillListView(ByVal sqlData As DataTable, ByVal lvList As ListView)
        Try
            lvList.Items.Clear()
            lvList.Columns.Clear()
            Dim i As Integer
            Dim j As Integer
            For i = 0 To sqlData.Columns.Count - 1
                lvList.Columns.Add(sqlData.Columns(i).ColumnName)
            Next i
            For i = 0 To sqlData.Rows.Count - 1
                lvList.Items.Add(sqlData.Rows(i).Item(0))
                For j = 1 To sqlData.Columns.Count - 1
                    If Not IsDBNull(sqlData.Rows(i).Item(j)) Then
                        lvList.Items(i).SubItems.Add(sqlData.Rows(i).Item(j))
                    Else
                        lvList.Items(i).SubItems.Add("")
                    End If

                Next j
            Next i
            For i = 0 To sqlData.Columns.Count - 1
                lvList.Columns(i).AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize)
            Next i
        Catch ex As Exception
            Log.SaveErrorLog(String.Concat("FillListView(): ", ex.Message))
        End Try

    End Sub

End Class
