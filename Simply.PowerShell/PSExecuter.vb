Public Class PSExecuter
    Implements IDisposable

    Public Event ProgressGenerated(currentProgress As PSProgressItem)
    Public Event OutputGenerated(currentOutput As PSOutputItem)

    Private disposedValue As Boolean
    Private _isBusy As Boolean = False
    Private _isStopped As Boolean = False
    Private Property PSRunspace As Runspaces.Runspace
    Private ReadOnly Property SourceContext As Threading.SynchronizationContext
    Private ReadOnly Property SourceThreadId As Integer

    Public Property IsBusy As Boolean
        Get
            Return _isBusy
        End Get
        Private Set(value As Boolean)
            _isBusy = value
        End Set
    End Property

    Public Property IsStopped As Boolean
        Get
            Return _isStopped
        End Get
        Private Set(value As Boolean)
            _isStopped = value
        End Set
    End Property

    Private currentPS As PowerShell

    Public Sub New(Optional listOfModules As List(Of String) = Nothing) 'Optional hostingApplication As Object = Nothing,
        'Track the thread and context of the source, used for raising events back on the original thread, making the events threadsafe.
        SourceContext = Threading.SynchronizationContext.Current
        SourceThreadId = Threading.Thread.CurrentThread.ManagedThreadId

        Dim PSInitialState = Runspaces.InitialSessionState.CreateDefault()
        PSInitialState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.RemoteSigned
        If listOfModules IsNot Nothing Then PSInitialState.ImportPSModule(listOfModules)

        PSRunspace = Runspaces.RunspaceFactory.CreateRunspace(PSInitialState)
        PSRunspace.Open()
    End Sub

    Public Async Function InvokeAsync(script As String) As Task(Of List(Of String))
        ' REWRITE USING: https://dzone.com/articles/wrapping-beginend-asynchronous
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        IsBusy = True
        Using ps = PowerShell.Create()
            currentPS = ps
            ps.Runspace = PSRunspace
            AddHandler ps.Streams.Progress.DataAdded, AddressOf ProgressAddedHandler
            ps.AddScript(script).AddCommand("out-string")
            Try
                Dim results = Await Task.Run(Function() ps.Invoke(Of String))
                RemoveHandler ps.Streams.Progress.DataAdded, AddressOf ProgressAddedHandler
                Return results.ToList
            Catch ex As Exception
                Return New List(Of String)({String.Concat("ERROR: ", ex.Message)})
            Finally
                currentPS = Nothing
                IsBusy = False
            End Try
        End Using
    End Function

    Public Async Function RunAsync(Of T)(scriptBlock As String, Optional input As Object = Nothing) As Task(Of Boolean)
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        IsBusy = True
        Using ps = PowerShell.Create
            currentPS = ps
            ps.Runspace = PSRunspace
            ps.AddScript(scriptBlock)

            AddHandler ps.Streams.Progress.DataAdded, AddressOf ProgressAddedHandler

            Dim output As New PSDataCollection(Of PSObject)
            Try
                IsStopped = False
                AddHandler output.DataAdded, AddressOf OutputAddedHandler
                Return Await Task.Factory.FromAsync(ps.BeginInvoke(output, output), Function(iar)
                                                                                        If Not IsStopped Then
                                                                                            ps.EndInvoke(iar)
                                                                                            Return True
                                                                                        Else
                                                                                            Return False
                                                                                        End If
                                                                                    End Function)
            Finally
                RemoveHandler ps.Streams.Progress.DataAdded, AddressOf ProgressAddedHandler
                RemoveHandler output.DataAdded, AddressOf OutputAddedHandler
                output.Dispose()
                currentPS = Nothing
                IsBusy = False
            End Try
        End Using

    End Function


    Public Sub Reset()
        PSRunspace.ResetRunspaceState()
    End Sub

    Public Sub CancelAsync()
        'If cts.Token.CanBeCanceled Then cts.Cancel()
        If currentPS IsNot Nothing Then
            IsStopped = True
            currentPS.Stop()
        End If
    End Sub

    Private Sub ProgressAddedHandler(sender As PSDataCollection(Of ProgressRecord), e As DataAddedEventArgs)
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent ProgressGenerated(PSProgressItem.Create(sender(e.Index)))
        Else
            SourceContext.Post(Sub() RaiseEvent ProgressGenerated(PSProgressItem.Create(sender(e.Index))), Nothing)
        End If
    End Sub

    Private Sub OutputAddedHandler(sender As PSDataCollection(Of PSObject), e As DataAddedEventArgs)
        If Threading.Thread.CurrentThread.ManagedThreadId = SourceThreadId Then
            RaiseEvent OutputGenerated(PSOutputItem.Create(sender(e.Index)))
        Else
            SourceContext.Post(Sub() RaiseEvent OutputGenerated(PSOutputItem.Create(sender(e.Index))), Nothing)
        End If
    End Sub

#Region "Disposable"
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                If currentPS IsNot Nothing Then
                    currentPS.Stop()
                    currentPS.Dispose()
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
