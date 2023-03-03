Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports System.Runtime.CompilerServices
Public Class BaseViewModel
    Inherits ObservableObject

    Private ReadOnly commandList As New List(Of IRelayCommand)

    Private Protected Sub RegisterCommand(cmd As IRelayCommand)
        If commandList.IndexOf(cmd) = -1 Then commandList.Add(cmd)
    End Sub

    Private Protected Sub VerifyCommandsCanExecute()
        commandList.ForEach(Sub(cmd) cmd.NotifyCanExecuteChanged())
    End Sub

    Private Protected Function SetPropertyAndVerifyCanExecute(Of T)(ByRef field As T, newValue As T, <CallerMemberName> Optional propertyName As String = "") As Boolean
        If SetProperty(field, newValue, propertyName) Then
            VerifyCommandsCanExecute()
            Return True
        Else
            Return False
        End If
    End Function
End Class