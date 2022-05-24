Imports System.Management.Automation
Public Class PSStreamItem(Of T)
    Public ReadOnly Property Generated As DateTimeOffset
    Public ReadOnly Property StreamType As PSStreamType

    Public ReadOnly BaseObject As T

    Public Sub New(nStreamType As PSStreamType, nBaseObject As Object, Optional nGenerated As Nullable(Of DateTimeOffset) = Nothing)
        Me.StreamType = nStreamType
        Me.BaseObject = nBaseObject

        If nGenerated Is Nothing Then nGenerated = DateTimeOffset.Now
        Me.Generated = nGenerated
    End Sub

    Public Overrides Function ToString() As String
        If BaseObject IsNot Nothing Then Return BaseObject.ToString
    End Function
End Class

Public Enum PSStreamType
    Debug
    [Error]
    Information
    Progress
    Verbose
    Warning
    Output
End Enum
