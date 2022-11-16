Public Class PSVerboseItem
    Inherits PSStreamItem
#Region "Properties"
    Private Shadows ReadOnly Property BaseObject As VerboseRecord
        Get
            Return DirectCast(MyBase.BaseObject, VerboseRecord)
        End Get
    End Property
    Public Overrides ReadOnly Property Message As String
        Get
            Return BaseObject.Message
        End Get
    End Property
#End Region

    Public Sub New(nVerboseRecord As VerboseRecord, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Verbose, nVerboseRecord, nGenerated)
    End Sub
    Public Shared Function Create(nVerboseRecord As VerboseRecord) As PSVerboseItem
        Return New PSVerboseItem(nVerboseRecord, DateTime.Now)
    End Function

End Class
