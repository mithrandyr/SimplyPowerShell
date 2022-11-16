Public Class PSDebugItem
    Inherits PSStreamItem(Of VerboseRecord)
#Region "Properties"
    Public ReadOnly Property Message As String
        Get
            Return BaseObject.Message
        End Get
    End Property
#End Region

    Public Sub New(nDebugRecord As DebugRecord, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Verbose, nDebugRecord, nGenerated)
    End Sub
    Public Shared Function Create(nDebugRecord As DebugRecord) As PSDebugItem
        Return New PSDebugItem(nDebugRecord, DateTime.Now)
    End Function

End Class
