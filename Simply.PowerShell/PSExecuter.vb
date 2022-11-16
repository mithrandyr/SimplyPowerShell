Public Class PSExecuter
    Implements IDisposable

#Region "Public Events & Properties"
    Public Event DebugGenerated(currentDebug As PSDebugItem)
    Public Event ErrorGenerated(currentError As PSErrorItem)
    Public Event InformationGenerated(currentInformation As PSInformationItem)
    Public Event ProgressGenerated(currentProgress As PSProgressItem)
    Public Event OutputGenerated(currentOutput As PSOutputItem)
    Public Event VerboseGenerated(currentVerbose As PSVerboseItem)
    Public Event WarningGenerated(currentWarning As PSWarningItem)

    Public ReadOnly Property IsBusy As Boolean
        Get
            Return _isBusy
        End Get
    End Property

    Public ReadOnly Property IsStopped As Boolean
        Get
            Return _isStopped
        End Get
    End Property
#End Region

#Region "Private Variables & Properties"
    Private disposedValue As Boolean
    Private _isBusy As Boolean = False
    Private _isStopped As Boolean = False
    Private psCurrent As PowerShell
    Private psRunspace As Runspaces.Runspace
    Private ReadOnly Property SourceContext As Threading.SynchronizationContext
    Private ReadOnly Property SourceThreadId As Integer
#End Region

    Public Sub New(Optional listOfModules As List(Of String) = Nothing) 'Optional hostingApplication As Object = Nothing,
        'Track the thread and context of the source, used for raising events back on the original thread, making the events threadsafe.
        SourceContext = Threading.SynchronizationContext.Current
        SourceThreadId = Threading.Thread.CurrentThread.ManagedThreadId

        Dim PSInitialState = Runspaces.InitialSessionState.CreateDefault()
        PSInitialState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.RemoteSigned
        If listOfModules IsNot Nothing Then PSInitialState.ImportPSModule(listOfModules)

        psRunspace = Runspaces.RunspaceFactory.CreateRunspace(PSInitialState)
        psRunspace.Open()
    End Sub

    'Public Async Function InvokeAsync(script As String) As Task(Of List(Of String))
    '    ' REWRITE USING: https://dzone.com/articles/wrapping-beginend-asynchronous
    '    If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
    '    _isBusy = True
    '    Using ps = PowerShell.Create()
    '        psCurrent = ps
    '        ps.Runspace = psRunspace
    '        AddHandler ps.Streams.Progress.DataAdded, AddressOf ProgressAddedHandler
    '        ps.AddScript(script).AddCommand("out-string")
    '        Try
    '            Dim results = Await Task.Run(Function() ps.Invoke(Of String))
    '            RemoveHandler ps.Streams.Progress.DataAdded, AddressOf ProgressAddedHandler
    '            Return results.ToList
    '        Catch ex As Exception
    '            Return New List(Of String)({String.Concat("ERROR: ", ex.Message)})
    '        Finally
    '            psCurrent = Nothing
    '            _isBusy = False
    '        End Try
    '    End Using
    'End Function

    Public Async Function RunAsync(scriptBlock As String, Optional scriptParameters As Dictionary(Of String, Object) = Nothing) As Task(Of Boolean)
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")

        _isBusy = True
        Using ps = PowerShell.Create
            psCurrent = ps
            ps.Runspace = psRunspace
            ps.AddScript(scriptBlock)
            If scriptParameters IsNot Nothing Then ps.AddParameters(scriptParameters)

            Dim output As New PSDataCollection(Of PSObject)
            Try
                _isStopped = False
                HandlersAdd()
                AddHandler output.DataAdded, AddressOf OutputAddedHandler
                Return Await Task.Factory.FromAsync(ps.BeginInvoke(output, output), Function(iar)
                                                                                        If Not IsStopped Then
                                                                                            Try
                                                                                                ps.EndInvoke(iar)
                                                                                                Return True
                                                                                            Catch ex As RuntimeException
                                                                                                ps.Streams.Error.Add(New ErrorRecord(ex, Nothing, ErrorCategory.NotSpecified, Nothing))
                                                                                                Return False
                                                                                            End Try
                                                                                        Else
                                                                                            Return False
                                                                                        End If
                                                                                    End Function)
            Finally
                RemoveHandler output.DataAdded, AddressOf OutputAddedHandler
                HandlersRemove()
                output.Dispose()
                psCurrent = Nothing
                _isBusy = False
            End Try
        End Using

    End Function


    Public Sub Reset()
        psRunspace.ResetRunspaceState()
    End Sub

    Public Sub CancelAsync()
        'If cts.Token.CanBeCanceled Then cts.Cancel()
        If psCurrent IsNot Nothing Then
            _isStopped = True
            psCurrent.Stop()
        End If
    End Sub

#Region "Private EventHandlers"
    Private Sub HandlersAdd()
        With psCurrent.Streams
            AddHandler .Debug.DataAdded, AddressOf DebugAddedHandler
            AddHandler .Error.DataAdded, AddressOf ErrorAddedHandler
            AddHandler .Information.DataAdded, AddressOf InformationAddedHandler
            AddHandler .Progress.DataAdded, AddressOf ProgressAddedHandler
            AddHandler .Verbose.DataAdded, AddressOf VerboseAddedHandler
            AddHandler .Warning.DataAdded, AddressOf WarningAddedHandler
        End With

    End Sub
    Private Sub HandlersRemove()
        With psCurrent.Streams
            RemoveHandler .Debug.DataAdded, AddressOf DebugAddedHandler
            RemoveHandler .Error.DataAdded, AddressOf ErrorAddedHandler
            RemoveHandler .Information.DataAdded, AddressOf InformationAddedHandler
            RemoveHandler .Progress.DataAdded, AddressOf ProgressAddedHandler
            RemoveHandler .Verbose.DataAdded, AddressOf VerboseAddedHandler
            RemoveHandler .Warning.DataAdded, AddressOf WarningAddedHandler
        End With
    End Sub
    Private Sub DebugAddedHandler(sender As Object, e As DataAddedEventArgs)
        Dim item = PSDebugItem.Create(DirectCast(sender, PSDataCollection(Of DebugRecord))(e.Index))

        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent DebugGenerated(item)
        Else
            SourceContext.Post(Sub() RaiseEvent DebugGenerated(item), Nothing)
        End If
    End Sub
    Private Sub ErrorAddedHandler(sender As Object, e As DataAddedEventArgs)
        Dim item = PSErrorItem.Create(DirectCast(sender, PSDataCollection(Of ErrorRecord))(e.Index))
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent ErrorGenerated(item)
        Else
            SourceContext.Post(Sub() RaiseEvent ErrorGenerated(item), Nothing)
        End If
    End Sub
    Private Sub InformationAddedHandler(sender As Object, e As DataAddedEventArgs)
        Dim item = PSInformationItem.Create(DirectCast(sender, PSDataCollection(Of InformationRecord))(e.Index))
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent InformationGenerated(item)
        Else
            SourceContext.Post(Sub() RaiseEvent InformationGenerated(item), Nothing)
        End If

    End Sub
    Private Sub OutputAddedHandler(sender As Object, e As DataAddedEventArgs)
        Dim item = PSOutputItem.Create(DirectCast(sender, PSDataCollection(Of PSObject))(e.Index))
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent OutputGenerated(item)
        Else
            SourceContext.Post(Sub() RaiseEvent OutputGenerated(item), Nothing)
        End If
    End Sub
    Private Sub ProgressAddedHandler(sender As Object, e As DataAddedEventArgs)
        Dim item = PSProgressItem.Create(DirectCast(sender, PSDataCollection(Of ProgressRecord))(e.Index))
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent ProgressGenerated(item)
        Else
            SourceContext.Post(Sub() RaiseEvent ProgressGenerated(item), Nothing)
        End If
    End Sub
    Private Sub VerboseAddedHandler(sender As Object, e As DataAddedEventArgs)
        Dim item = PSVerboseItem.Create(DirectCast(sender, PSDataCollection(Of VerboseRecord))(e.Index))
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent VerboseGenerated(item)
        Else
            SourceContext.Post(Sub() RaiseEvent VerboseGenerated(item), Nothing)
        End If
    End Sub
    Private Sub WarningAddedHandler(sender As Object, e As DataAddedEventArgs)
        Dim item = PSWarningItem.Create(DirectCast(sender, PSDataCollection(Of WarningRecord))(e.Index))
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent WarningGenerated(item)
        Else
            SourceContext.Post(Sub() RaiseEvent WarningGenerated(item), Nothing)
        End If
    End Sub
#End Region
#Region "Disposable"
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                If psCurrent IsNot Nothing Then
                    psCurrent.Stop()
                    psCurrent.Dispose()
                End If
                PSRunspace.Dispose()
                ' TODO: dispose managed state (managed objects)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
