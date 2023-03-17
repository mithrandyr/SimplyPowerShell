'Pulled from https://github.com/mithrandyr/SimplyPowerShell/tree/initial/Simply.PowerShell
Imports System.Threading

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
    Public Event ExecutionStarted()
    Public Event ExecutionFinished()

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
    Public ReadOnly Property IsErrored As Boolean
        Get
            Return _isErrored
        End Get
    End Property
    Public ReadOnly Property LastErrorMessage As String
        Get
            Return _lastErrorMessage
        End Get
    End Property
    Public Property UseDefaultHandlers As Boolean = True
#End Region

#Region "Private Variables & Properties"
    Private disposedValue As Boolean
    Private _isBusy As Boolean = False
    Private _isStopped As Boolean = False
    Private _isErrored As Boolean = False
    Private _lastErrorMessage As String
    Private psCurrent As PowerShell
    Private psRunspace As Runspaces.Runspace
    Private ReadOnly Property SourceContext As Threading.SynchronizationContext
    Private ReadOnly Property SourceThreadId As Integer
#End Region

    Public Sub New(Optional srcCtx As SynchronizationContext = Nothing, Optional srcThreadId As Integer = 0)
        If srcCtx Is Nothing Then
            srcCtx = SynchronizationContext.Current
            srcThreadId = Thread.CurrentThread.ManagedThreadId
        End If
        Initialize(srcCtx, srcThreadId)
    End Sub

    Private Sub Initialize(srcCtx As SynchronizationContext, srcThreadId As Integer)
        'Track the thread and context of the source, used for raising events back on the original thread, making the events threadsafe.
        _SourceContext = srcCtx
        _SourceThreadId = srcThreadId

        Dim PSInitialState = Runspaces.InitialSessionState.CreateDefault()
        PSInitialState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.RemoteSigned
        psRunspace = Runspaces.RunspaceFactory.CreateRunspace(PSInitialState)
        psRunspace.Open()
    End Sub
    Public Sub Reset()
        If IsBusy Then Throw New InvalidOperationException("Cannot Reset while executing powershell.")
        psRunspace.ResetRunspaceState()
    End Sub
    Public Sub SetSessionVariable(varName As String, value As Object)
        psRunspace.SessionStateProxy.SetVariable(varName, value)
    End Sub
    Public Sub CancelAsync()
        If psCurrent IsNot Nothing Then
            _isStopped = True
            psCurrent.Stop()
        End If
    End Sub

#Region "Async Functions that Execute Powershell"
    Private Sub PreExecution()
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("There is nothing to execute!")

        RaiseEvent ExecutionStarted()

        _isErrored = False
        _lastErrorMessage = String.Empty
        _isBusy = True
        _isStopped = False
        psCurrent.Runspace = psRunspace
        If UseDefaultHandlers Then HandlersAdd()
    End Sub
    Private Sub PostExecution()
        If UseDefaultHandlers Then HandlersRemove()
        psCurrent = Nothing
        _isBusy = False
        RaiseEvent ExecutionFinished()
    End Sub
    Public Async Function ExecuteAsync(Optional input As Object = Nothing, Optional ct As CancellationToken = Nothing) As Task(Of Boolean)
        PreExecution()
        If ct.CanBeCanceled Then ct.Register(AddressOf CancelAsync)
        If input IsNot Nothing AndAlso TypeOf input IsNot ICollection Then input = New Object() {input}
        Using ps = psCurrent
            Using output As New PSDataCollection(Of PSObject)
                Try
                    If UseDefaultHandlers Then AddHandler output.DataAdded, AddressOf OutputAddedHandler
                    Return Await Task.Run(Function()
                                              Try
                                                  ps.Invoke(input, output, Nothing)
                                                  Return True
                                              Catch ex As RuntimeException
                                                  ps.Streams.Error.Add(New ErrorRecord(ex, Nothing, ErrorCategory.NotSpecified, Nothing))
                                                  _isErrored = True
                                                  _lastErrorMessage = ex.Message
                                                  Return False
                                              End Try
                                          End Function)
                Finally
                    If UseDefaultHandlers Then RemoveHandler output.DataAdded, AddressOf OutputAddedHandler
                    PostExecution()
                End Try
            End Using
        End Using
    End Function
    Public Async Function GetResults(Of T)(Optional input As IEnumerable = Nothing, Optional ct As CancellationToken = Nothing) As Task(Of IEnumerable(Of T))
        PreExecution()
        If ct.CanBeCanceled Then ct.Register(AddressOf CancelAsync)
        Using ps = psCurrent
            Using output As New PSDataCollection(Of PSObject)
                Try
                    Await Task.Run(Function()
                                       Try
                                           ps.Invoke(input, output, Nothing)
                                           Return True
                                       Catch ex As RuntimeException
                                           ps.Streams.Error.Add(New ErrorRecord(ex, Nothing, ErrorCategory.NotSpecified, Nothing))
                                           _isErrored = True
                                           _lastErrorMessage = ex.Message
                                           Return False
                                       End Try
                                   End Function)
                    Return output.Select(Function(x) DirectCast(x.BaseObject, T)).ToList
                Finally
                    PostExecution()
                End Try
            End Using
        End Using
    End Function
    Public Async Function GetResults(Optional input As IEnumerable = Nothing, Optional ct As CancellationToken = Nothing) As Task(Of IEnumerable(Of Object))
        PreExecution()
        If ct.CanBeCanceled Then ct.Register(AddressOf CancelAsync)
        Using ps = psCurrent
            Using output As New PSDataCollection(Of PSObject)
                Try
                    Await Task.Run(Function()
                                       Try
                                           ps.Invoke(input, output, Nothing)
                                           Return True
                                       Catch ex As RuntimeException
                                           ps.Streams.Error.Add(New ErrorRecord(ex, Nothing, ErrorCategory.NotSpecified, Nothing))
                                           _isErrored = True
                                           _lastErrorMessage = ex.Message
                                           Return False
                                       End Try
                                   End Function)
                    Return output.Select(Function(x) x.AsExpandoObject).ToList
                Finally
                    PostExecution()
                End Try
            End Using
        End Using
    End Function
    Public Async Function GetResult(Of T)(Optional input As IEnumerable = Nothing, Optional ct As CancellationToken = Nothing) As Task(Of T)
        Dim results = Await GetResults(Of T)(input, ct:=ct)
        Return results.FirstOrDefault
    End Function
    Public Async Function GetResult(Optional input As IEnumerable = Nothing, Optional ct As CancellationToken = Nothing) As Task(Of Object)
        Dim results = Await GetResults(input, ct:=ct)
        Return results.FirstOrDefault
    End Function
    Public Async Function RunAsync(scriptBlock As String, Optional scriptParameters As Dictionary(Of String, Object) = Nothing, Optional ct As CancellationToken = Nothing) As Task(Of Boolean)
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")

        NewPipeline(scriptBlock)
        If scriptParameters IsNot Nothing Then
            For Each kvp In scriptParameters
                AddParameter(kvp.Key, kvp.Value)
            Next
        End If
        Return Await ExecuteAsync(ct:=ct)
        _isBusy = True
    End Function
    Public Async Function GetScriptVariable(Of T)(varName As String, Optional ct As CancellationToken = Nothing) As Task(Of IEnumerable(Of T))
        Return Await NewPipeline.AddCommand("Get-Variable").AddArgument(varName).AddParameter("ValueOnly").GetResults(Of T)(ct:=ct)
    End Function
    Public Async Function GetScriptVariable(varName As String, Optional ct As CancellationToken = Nothing) As Task(Of IEnumerable(Of Object))
        Return Await NewPipeline.AddCommand("Get-Variable").AddArgument(varName).AddParameter("ValueOnly").GetResults(ct:=ct)
    End Function
#End Region

#Region "Fluent Composition of Powershell Pipelines/Commands that will be Executed"
    Public Function NewPipeline(Optional withScript As String = Nothing) As PSExecuter
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then
            psCurrent = PowerShell.Create()
            If Not String.IsNullOrWhiteSpace(withScript) Then
                psCurrent.AddScript(withScript)
            End If
        Else
            psCurrent.AddStatement()
            If Not String.IsNullOrWhiteSpace(withScript) Then psCurrent.AddScript(withScript)
        End If
        Return Me
    End Function

    Public Function AddCommand(cmd As String) As PSExecuter
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then psCurrent = PowerShell.Create
        psCurrent.AddCommand(cmd)
        Return Me
    End Function

    Public Function AddParameter(switchParamName As String) As PSExecuter
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("Cannot add parameters to nothing!")
        psCurrent.AddParameter(switchParamName)
        Return Me
    End Function
    Public Function AddParameter(paramName As String, paramValue As Object, Optional makeScriptBlock As Boolean = False) As PSExecuter
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("Cannot add parameters to nothing!")
        If makeScriptBlock Then paramValue = ScriptBlock.Create(paramValue)
        psCurrent.AddParameter(paramName, paramValue)
        Return Me
    End Function

    Public Function AddParameters(scriptParameters As Dictionary(Of String, Object)) As PSExecuter
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("Cannot add parameters to nothing!")
        If scriptParameters IsNot Nothing Then
            For Each kvp In scriptParameters
                AddParameter(kvp.Key, kvp.Value)
            Next
        End If
        Return Me
    End Function

    Public Function AddArgument(argValue As Object, Optional makeScriptBlock As Boolean = False) As PSExecuter
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("Cannot add parameters to nothing!")
        If makeScriptBlock Then argValue = ScriptBlock.Create(argValue)
        psCurrent.AddArgument(argValue)
        Return Me
    End Function
    Public Function SetScriptVariable(varName As String, value As Object) As PSExecuter
        NewPipeline.AddCommand("Set-Variable").AddArgument(varName).AddArgument(value)
        Return Me
    End Function
#End Region

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
                psRunspace.Dispose()
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
