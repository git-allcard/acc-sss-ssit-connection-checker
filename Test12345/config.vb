Public Class config

    Private newDSNValue As String = ""
    Public Property DSN() As String
        Get
            Return newDSNValue
        End Get
        Set(ByVal value As String)
            newDSNValue = value
        End Set
    End Property

    Private newServerValue As String = ""
    Public Property Server() As String
        Get
            Return newServerValue
        End Get
        Set(ByVal value As String)
            newServerValue = value
        End Set
    End Property

    Private newDatabaseValue As String = ""
    Public Property Database() As String
        Get
            Return newDatabaseValue
        End Get
        Set(ByVal value As String)
            newDatabaseValue = value
        End Set
    End Property

    Private newUserValue As String = ""
    Public Property User() As String
        Get
            Return newUserValue
        End Get
        Set(ByVal value As String)
            newUserValue = value
        End Set
    End Property

    Private newPasswordValue As String = ""
    Public Property Password() As String
        Get
            Return newPasswordValue
        End Get
        Set(ByVal value As String)
            newPasswordValue = value
        End Set
    End Property

End Class
