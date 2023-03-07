Imports System.Collections.ObjectModel
Imports Simply.PSDotNet

Public Class MainWindowViewModel
    Inherits BaseViewModel

    Private _commandToExecute
    Property CommandToExecute As String
        Get
            Return _commandToExecute
        End Get
        Set(value As String)
            SetPropertyAndVerifyCanExecute(_commandToExecute, value)
        End Set
    End Property

    ReadOnly Property CommandList As New ObservableCollection(Of String)

    Private _streams As New List(Of PSStreamItem)
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
            SetPropertyAndVerifyCanExecute(_showInformation, value)
        End Set
    End Property
    Property ShowWarning As Boolean
        Get
            Return _showWarning
        End Get
        Set(value As Boolean)
            SetPropertyAndVerifyCanExecute(_showWarning, value)
        End Set
    End Property
    Property ShowVerbose As Boolean
        Get
            Return _showVerbose
        End Get
        Set(value As Boolean)
            SetPropertyAndVerifyCanExecute(_showVerbose, value)
        End Set
    End Property
    Property ShowDebug As Boolean
        Get
            Return _showDebug
        End Get
        Set(value As Boolean)
            SetPropertyAndVerifyCanExecute(_showDebug, value)
        End Set
    End Property
    Property ShowError As Boolean
        Get
            Return _showError
        End Get
        Set(value As Boolean)
            SetPropertyAndVerifyCanExecute(_showError, value)
        End Set
    End Property
    Property ShowOutput As Boolean
        Get
            Return _showOutput
        End Get
        Set(value As Boolean)
            SetPropertyAndVerifyCanExecute(_showOutput, value)
        End Set
    End Property
    Property ShowProgress As Boolean
        Get
            Return _showProgress
        End Get
        Set(value As Boolean)
            SetPropertyAndVerifyCanExecute(_showProgress, value)
        End Set
    End Property

    ReadOnly Property VisibleStreams As List(Of PSStreamItem)
        Get
            Dim query = _streams.AsQueryable
            If Not ShowInformation Then query.Where(Function(stream) stream.StreamType <> PSStreamType.Information)
            If Not ShowWarning Then query.Where(Function(stream) stream.StreamType <> PSStreamType.Warning)
            If Not ShowVerbose Then query.Where(Function(stream) stream.StreamType <> PSStreamType.Verbose)
            If Not ShowDebug Then query.Where(Function(stream) stream.StreamType <> PSStreamType.Debug)
            If Not ShowError Then query.Where(Function(stream) stream.StreamType <> PSStreamType.Error)
            If Not ShowOutput Then query.Where(Function(stream) stream.StreamType <> PSStreamType.Output)
            If Not ShowProgress Then query.Where(Function(stream) stream.StreamType <> PSStreamType.Progress)
            Return query.ToList
        End Get
    End Property

    Private WithEvents pse As New PSExecuter

End Class
