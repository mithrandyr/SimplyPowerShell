Public Class PSOutputItem
    Inherits PSStreamItem(Of PSObject)

    Public ReadOnly Property Value As Object
        Get
            Return MyBase.BaseObject.BaseObject
        End Get
    End Property

    Public Sub New(nObject As PSObject, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Output, nObject, nGenerated)
    End Sub
    Public Shared Function Create(nObject As PSObject) As PSOutputItem
        Return New PSOutputItem(nObject, DateTimeOffset.Now)
    End Function

End Class
