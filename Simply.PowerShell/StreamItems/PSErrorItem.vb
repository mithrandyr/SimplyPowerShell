Public Class PSErrorItem
    Inherits PSStreamItem(Of ErrorRecord)

#Region "Public Properties"

#End Region

    Public Sub New(nErrorRecord As ErrorRecord, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Error, nErrorRecord, nGenerated)
    End Sub
    Public Shared Function Create(nErrorRecord As ErrorRecord) As PSErrorItem
        Return New PSErrorItem(nErrorRecord, DateTime.Now)
    End Function
End Class
