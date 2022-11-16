Public Class PSErrorItem
    Inherits PSStreamItem

#Region "Public Properties"
    Private Shadows ReadOnly Property BaseObject As ErrorRecord
        Get
            Return DirectCast(MyBase.BaseObject, ErrorRecord)
        End Get
    End Property
    Public Overrides ReadOnly Property Message As String
        Get
            Return BaseObject.Exception.Message
        End Get
    End Property

#End Region

    Public Sub New(nErrorRecord As ErrorRecord, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Error, nErrorRecord, nGenerated)
    End Sub
    Public Shared Function Create(nErrorRecord As ErrorRecord) As PSErrorItem
        Return New PSErrorItem(nErrorRecord, DateTime.Now)
    End Function
End Class
