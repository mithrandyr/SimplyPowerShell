Public Class PSInformationItem
    Inherits PSStreamItem

#Region "Properties"
    Private Shadows ReadOnly Property BaseObject As InformationRecord
        Get
            Return DirectCast(MyBase.BaseObject, InformationRecord)
        End Get
    End Property
    Public Overrides ReadOnly Property Message As String
        Get
            Return MessageData.ToString
        End Get
    End Property

    Public ReadOnly Property Computer As String
        Get
            Return BaseObject.Computer
        End Get
    End Property
    Public ReadOnly Property ManagedThreadId As UInteger
        Get
            Return BaseObject.ManagedThreadId
        End Get
    End Property
    Public ReadOnly Property MessageData As Object
        Get
            Return BaseObject.MessageData
        End Get
    End Property
    Public ReadOnly Property NativeThreadId As UInteger
        Get
            Return BaseObject.NativeThreadId
        End Get
    End Property
    Public ReadOnly Property ProcessId As UInteger
        Get
            Return BaseObject.ProcessId
        End Get
    End Property
    Public ReadOnly Property Source As String
        Get
            Return BaseObject.Source
        End Get
    End Property
    Public ReadOnly Property Tags As List(Of String)
        Get
            Return BaseObject.Tags
        End Get
    End Property
    Public ReadOnly Property User As String
        Get
            Return BaseObject.User
        End Get
    End Property
#End Region

    Public Sub New(nInformationRecord As InformationRecord)
        MyBase.New(PSStreamType.Information, nInformationRecord, nInformationRecord.TimeGenerated)
    End Sub
    Public Shared Function Create(nInformationRecord As InformationRecord) As PSInformationItem
        Return New PSInformationItem(nInformationRecord)
    End Function

End Class
