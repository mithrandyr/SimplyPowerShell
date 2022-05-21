Public Class PSProgressItem
    Inherits PSStreamItem(Of ProgressRecord)

    Public ReadOnly Property Activity As String
        Get
            Return BaseObject.Activity
        End Get
    End Property
    Public ReadOnly Property ActivityId As Integer
        Get
            Return BaseObject.ActivityId
        End Get
    End Property
    Public ReadOnly Property CurrentOperation As String
        Get
            Return BaseObject.CurrentOperation
        End Get
    End Property
    Public ReadOnly Property ParentActivityId As Integer
        Get
            Return BaseObject.ParentActivityId
        End Get
    End Property
    Public ReadOnly Property PercentComplete As Integer
        Get
            Return BaseObject.PercentComplete
        End Get
    End Property
    Public ReadOnly Property SecondsRemaining As Integer
        Get
            Return BaseObject.SecondsRemaining
        End Get
    End Property
    Public ReadOnly Property StatusDescription As String
        Get
            Return BaseObject.StatusDescription
        End Get
    End Property
    Public ReadOnly Property IsComplete As Boolean
        Get
            Return BaseObject.RecordType = ProgressRecordType.Completed
        End Get
    End Property

    Public Sub New(nProgressRecord As ProgressRecord, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        MyBase.New(PSStreamType.Progress, nProgressRecord, nGenerated)
    End Sub

    Public Shared Function Create(nProgressRecord As ProgressRecord) As PSProgressItem
        Return New PSProgressItem(nProgressRecord, DateTimeOffset.Now)
    End Function

End Class