Imports System.Collections.ObjectModel
Imports System.Dynamic
Imports System.Threading
Imports CommunityToolkit.Mvvm.Input
Imports Simply.PSDotNet

Public Class MainWindowViewModel
    Inherits BaseViewModel

    Private _commandToExecute As String
    Property CommandToExecute As String
        Get
            Return _commandToExecute
        End Get
        Set(value As String)
            SetPropertyAndVerifyCanExecute(_commandToExecute, value)
        End Set
    End Property

    Private _IncludeData As Boolean
    Property IncludeData As Boolean
        Get
            Return _IncludeData
        End Get
        Set(value As Boolean)
            SetProperty(_IncludeData, value)
        End Set
    End Property

    ReadOnly Property CommandList As New ObservableCollection(Of String)
    Private _commandListSelected As String
    Public Property CommandListSelected As String
        Get
            Return _commandListSelected
        End Get
        Set(value As String)
            SetPropertyAndVerifyCanExecute(_commandListSelected, value)
        End Set
    End Property

    ReadOnly Property ReturnedData As New ObservableCollection(Of ReturnedData)

#Region "Streams"
    Private ReadOnly _streams As New List(Of PSStreamItem)
    Private _showInformation As Boolean = True
    Private _showWarning As Boolean = True
    Private _showVerbose As Boolean = False
    Private _showDebug As Boolean = False
    Private _showError As Boolean = True
    Private _showOutput As Boolean = False
    Private _showProgress As Boolean = False

    Property ShowInformation As Boolean
        Get
            Return _showInformation
        End Get
        Set(value As Boolean)
            If SetPropertyAndVerifyCanExecute(_showInformation, value) Then OnPropertyChanged(NameOf(VisibleStreams))
        End Set
    End Property
    Property ShowWarning As Boolean
        Get
            Return _showWarning
        End Get
        Set(value As Boolean)
            If SetPropertyAndVerifyCanExecute(_showWarning, value) Then OnPropertyChanged(NameOf(VisibleStreams))
        End Set
    End Property
    Property ShowVerbose As Boolean
        Get
            Return _showVerbose
        End Get
        Set(value As Boolean)
            If SetPropertyAndVerifyCanExecute(_showVerbose, value) Then OnPropertyChanged(NameOf(VisibleStreams))
        End Set
    End Property
    Property ShowDebug As Boolean
        Get
            Return _showDebug
        End Get
        Set(value As Boolean)
            If SetPropertyAndVerifyCanExecute(_showDebug, value) Then OnPropertyChanged(NameOf(VisibleStreams))
        End Set
    End Property
    Property ShowError As Boolean
        Get
            Return _showError
        End Get
        Set(value As Boolean)
            If SetPropertyAndVerifyCanExecute(_showError, value) Then OnPropertyChanged(NameOf(VisibleStreams))
        End Set
    End Property
    Property ShowOutput As Boolean
        Get
            Return _showOutput
        End Get
        Set(value As Boolean)
            If SetPropertyAndVerifyCanExecute(_showOutput, value) Then OnPropertyChanged(NameOf(VisibleStreams))
        End Set
    End Property
    Property ShowProgress As Boolean
        Get
            Return _showProgress
        End Get
        Set(value As Boolean)
            If SetPropertyAndVerifyCanExecute(_showProgress, value) Then OnPropertyChanged(NameOf(VisibleStreams))
        End Set
    End Property

    ReadOnly Property VisibleStreams As List(Of PSStreamItem)
        Get
            Dim query = _streams.AsQueryable
            If Not ShowInformation Then query = query.Where(Function(stream) stream.StreamType <> PSStreamType.Information)
            If Not ShowWarning Then query = query.Where(Function(stream) stream.StreamType <> PSStreamType.Warning)
            If Not ShowVerbose Then query = query.Where(Function(stream) stream.StreamType <> PSStreamType.Verbose)
            If Not ShowDebug Then query = query.Where(Function(stream) stream.StreamType <> PSStreamType.Debug)
            If Not ShowError Then query = query.Where(Function(stream) stream.StreamType <> PSStreamType.Error)
            If Not ShowOutput Then query = query.Where(Function(stream) stream.StreamType <> PSStreamType.Output)
            If Not ShowProgress Then query = query.Where(Function(stream) stream.StreamType <> PSStreamType.Progress)
            Return query.ToList
        End Get
    End Property
#End Region

    Public ReadOnly Property ClearStreamsCommand As IRelayCommand
    Public ReadOnly Property ExecuteCommand As IAsyncRelayCommand
    Public ReadOnly Property SelectHistoryCommand As IRelayCommand
    Public ReadOnly Property CancelExecutionCommand As IRelayCommand
    Private ExecutionCTS As CancellationTokenSource

    Public ReadOnly Property TestCommand As IAsyncRelayCommand

    Public Sub New()
        ExecuteCommand = New AsyncRelayCommand(
            Async Function()
                ReturnedData.Clear()
                Dim cmd = CommandToExecute
                CommandToExecute = String.Empty
                CommandList.Add(cmd)
                Using cts As New CancellationTokenSource
                    ExecutionCTS = cts
                    Return Await pse.NewPipeline(cmd).ExecuteAsync(ct:=cts.Token)
                End Using
            End Function, Function() Not String.IsNullOrWhiteSpace(CommandToExecute))
        RegisterCommand(ExecuteCommand)

        SelectHistoryCommand = New RelayCommand(Sub() CommandToExecute = CommandListSelected, Function() Not String.IsNullOrWhiteSpace(CommandListSelected))
        RegisterCommand(SelectHistoryCommand)

        ClearStreamsCommand = New RelayCommand(
            Sub()
                _streams.Clear()
                OnPropertyChanged(NameOf(VisibleStreams))
                VerifyCommandsCanExecute()
            End Sub, Function() _streams.Count > 0)

        RegisterCommand(ClearStreamsCommand)

        CancelExecutionCommand = New RelayCommand(Sub() ExecutionCTS.Cancel(), Function() ExecuteCommand.IsRunning)
        RegisterCommand(CancelExecutionCommand)

        'Test Command
        TestCommand = New AsyncRelayCommand(
            Async Function()
                Await pse.NewPipeline("write-information 'test function'").ExecuteAsync()
                Dim x As Object = New ExpandoObject
                x.Name = "Bob"
                x.Age = 39
                If x.NAME IsNot Nothing Then
                    Await pse.AddCommand("Write-Host").ExecuteAsync(x)
                End If
                Return True
            End Function, Function() Not ExecuteCommand.IsRunning)
        RegisterCommand(TestCommand)
    End Sub

#Region "Powershell Executer & Handlers"
    Private WithEvents pse As New PSExecuter

    Private Sub ProcessStream(streamData As PSStreamItem) Handles pse.InformationGenerated, pse.DebugGenerated, pse.ErrorGenerated, pse.OutputGenerated, pse.ProgressGenerated, pse.VerboseGenerated, pse.WarningGenerated
        _streams.Add(streamData)
        OnPropertyChanged(NameOf(VisibleStreams))
        VerifyCommandsCanExecute()
        If streamData.StreamType = PSStreamType.Output Then
            ReturnedData.Add(New ReturnedData(DirectCast(streamData, PSOutputItem).Value, False))
        End If
    End Sub
#End Region
End Class
