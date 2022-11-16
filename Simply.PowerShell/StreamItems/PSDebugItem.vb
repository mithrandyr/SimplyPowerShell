Public Class PSDebugItem
    Inherits PSStreamItem
#Region "Properties"
    Private Shadows ReadOnly Property BaseObject As DebugRecord
        Get
            Return DirectCast(MyBase.BaseObject, DebugRecord)
        End Get
    End Property

    Public Overrides ReadOnly Property Message As String
        Get
            Return BaseObject.Message
        End Get
    End Property
#End Region

    Public Sub New(nDebugRecord As DebugRecord, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Debug, nDebugRecord, nGenerated)
    End Sub
    Public Shared Function Create(nDebugRecord As DebugRecord) As PSDebugItem
        Return New PSDebugItem(nDebugRecord, DateTime.Now)
    End Function
End Class
