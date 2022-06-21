
Public Class localDAL

    Dim dal As New DAL.MsSql(Utilities.ConStr)

    Public Function IsConnected(ByVal con As String) As Boolean
        Dim bln = dal.IsConnectionOK(con)
        If bln Then
            Return True
        Else
            Log.SaveErrorLog(String.Concat("IsConnected(): ", dal.ErrorMessage))
            Return False
        End If
    End Function

    Public Function GetKioskListWithIPandNotVPN() As DataTable
        If dal.SelectQuery("SELECT KIOSK_NM as'Host Name', STATUS, BRANCH_IP as 'IP ADDRESS', BRANCH_CD as 'Branch Code', CLSTR, DIVSN FROM SSINFOTERMKIOSK where TAG = '1' and BRANCH_IP <> '' and isVPN = 'false'") Then
            Return dal.TableResult
        Else
            Log.SaveErrorLog(String.Concat("GetKioskListWithIPandNotVPN(): ", dal.ErrorMessage))
            Return Nothing
        End If
    End Function

    Public Function IsKioskExistInSSMonitoring(ByVal ip As String) As Short
        'select record with offline (status=1)
        If dal.ExecuteScalar(String.Format("SELECT COUNT(BRANCH_IP) FROM SSMONITORING WHERE STATUS = 'true' and BRANCH_IP = '{0}' and DATESTAMP between '{1} 00:00:00' and '{1} 23:59:59'", ip, Utilities.reportDate.ToShortDateString)) Then
            If CInt(dal.ObjectResult) > 0 Then
                Return 0
            Else
                Return 1
            End If
        Else
            Log.SaveErrorLog(String.Concat("IsKioskExistInSSMonitoring(): ", dal.ErrorMessage))
            Return -1
        End If
    End Function

    Public Function InsertSSMonitoring(ByVal ip As String, ByVal branchCode As String, ByVal cluster As String, ByVal division As String) As Boolean
        If dal.ExecuteQuery(String.Format("INSERT INTO SSMONITORING(BRANCH_IP,BRANCH_CD,CLSTR,DIVSN,OFFLINE_DT,DATESTAMP,STATUS) values ('{0}','{1}','{2}','{3}',GETDATE(),GETDATE(),'true')", ip, branchCode, cluster, division)) Then
            Return True
        Else
            Log.SaveErrorLog(String.Concat("InsertSSMonitoring(): ", dal.ErrorMessage))
            Return False
        End If
    End Function

    Public Function UpdateSSMonitoringOnlineDate(ByVal ip As String) As Boolean
        'If dal.ExecuteQuery(String.Format("UPDATE SSMONITORING SET ONLINE_DT = GETDATE(), STATUS = 'false' where BRANCH_IP = '{0}' and datestamp = (Select max(DATESTAMP) from SSMONITORING where BRANCH_IP = '{0}' and STATUS ='true')", ip)) Then
        If dal.ExecuteQuery(String.Format("UPDATE SSMONITORING SET ONLINE_DT = GETDATE(), STATUS = 'false' WHERE STATUS ='true' and BRANCH_IP = '{0}' and DATESTAMP between '{1} 00:00:00' and '{1} 23:59:59'", ip, Utilities.reportDate.ToShortDateString)) Then
            Return True
        Else
            Log.SaveErrorLog(String.Concat("UpdateSSMonitoringOnlineDate(): ", dal.ErrorMessage))
            Return False
        End If
    End Function

    Public Sub Dispose()
        dal.Dispose()
        dal = Nothing
    End Sub

End Class
