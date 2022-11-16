Public Class PSWarningItem
    Inherits PSStreamItem(Of WarningRecord)
#Region "Properties"
    Public ReadOnly Property Message As String
        Get
            Return BaseObject.Message
        End Get
    End Property
#End Region

    Public Sub New(nWarningRecord As WarningRecord, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Warning, nWarningRecord, nGenerated)
    End Sub
    Public Shared Function Create(nWarningRecord As WarningRecord) As PSWarningItem
        Return New PSWarningItem(nWarningRecord, DateTime.Now)
    End Function

End Class
