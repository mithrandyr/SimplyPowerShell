Public Class PSOutputItem
    Inherits PSStreamItem(Of PSObject)

    Public Sub New(nObject As PSObject, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Progress, nObject, nGenerated)
    End Sub
    Public Shared Function Create(nObject As PSObject) As PSOutputItem
        Return New PSOutputItem(nObject, DateTimeOffset.Now)
    End Function

End Class
