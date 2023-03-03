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
    Public Sub CancelAsync()
        If psCurrent IsNot Nothing Then
            _isStopped = True
            psCurrent.Stop()
        End If
    End Sub

#Region "Async Functions that Execute Powershell"
    Public Async Function ExecuteAsync(Optional useDefaultOutputHandler As Boolean = True, Optional useDefaultHandlers As Boolean = True) As Task(Of Boolean)
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("There is nothing to execute!")
        RaiseEvent ExecutionStarted()
        _isBusy = True
        Dim ps = psCurrent
        ps.Runspace = psRunspace
        Dim output As New PSDataCollection(Of PSObject)
        Try
            _isStopped = False
            If useDefaultHandlers Then HandlersAdd()
            If useDefaultOutputHandler Then AddHandler output.DataAdded, AddressOf OutputAddedHandler
            Return Await Task.Run(Function()
                                      Try
                                          ps.Invoke(Nothing, output, Nothing)
                                          Return True
                                      Catch ex As RuntimeException
                                          ps.Streams.Error.Add(New ErrorRecord(ex, Nothing, ErrorCategory.NotSpecified, Nothing))
                                          Return False
                                      End Try
                                  End Function)
        Finally
            If useDefaultOutputHandler Then RemoveHandler output.DataAdded, AddressOf OutputAddedHandler
            If useDefaultHandlers Then HandlersRemove()
            output.Dispose()
            ps.Dispose()
            psCurrent = Nothing
            _isBusy = False
            RaiseEvent ExecutionFinished()
        End Try
    End Function
    Public Async Function RunAsync(scriptBlock As String, Optional scriptParameters As Dictionary(Of String, Object) = Nothing) As Task(Of Boolean)
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")

        NewPipeline(scriptBlock)
        If scriptParameters IsNot Nothing Then
            For Each kvp In scriptParameters
                AddParameter(kvp.Key, kvp.Value)
            Next
        End If
        Return Await ExecuteAsync()
        _isBusy = True
    End Function
    Public Async Function GetResults(Of T)(Optional useDefaultHandlers As Boolean = False) As Task(Of IEnumerable(Of T))
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("There is nothing to execute!")
        RaiseEvent ExecutionStarted()
        _isBusy = True
        Dim ps = psCurrent
        ps.Runspace = psRunspace
        Dim output As New PSDataCollection(Of PSObject)
        Try
            _isStopped = False
            If useDefaultHandlers Then HandlersAdd()
            Await Task.Run(Function()
                               Try
                                   ps.Invoke(Nothing, output, Nothing)
                                   Return True
                               Catch ex As RuntimeException
                                   ps.Streams.Error.Add(New ErrorRecord(ex, Nothing, ErrorCategory.NotSpecified, Nothing))
                                   Return False
                               End Try
                           End Function)
            Return output.Select(Function(x) DirectCast(x.BaseObject, T)).ToList
        Finally
            If useDefaultHandlers Then HandlersRemove()
            output.Dispose()
            ps.Dispose()
            psCurrent = Nothing
            _isBusy = False
            RaiseEvent ExecutionFinished()
        End Try
    End Function
    Public Async Function GetResults(Optional useDefaultHandlers As Boolean = False) As Task(Of IEnumerable(Of Object))
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then Throw New InvalidOperationException("There is nothing to execute!")
        RaiseEvent ExecutionStarted()
        _isBusy = True
        Dim ps = psCurrent
        ps.Runspace = psRunspace
        Dim output As New PSDataCollection(Of PSObject)
        Try
            _isStopped = False
            If useDefaultHandlers Then HandlersAdd()
            Await Task.Run(Function()
                               Try
                                   ps.Invoke(Nothing, output, Nothing)
                                   Return True
                               Catch ex As RuntimeException
                                   ps.Streams.Error.Add(New ErrorRecord(ex, Nothing, ErrorCategory.NotSpecified, Nothing))
                                   Return False
                               End Try
                           End Function)
            Return output.Select(Function(x) x.AsExpandoObject).ToList
        Finally
            If useDefaultHandlers Then HandlersRemove()
            output.Dispose()
            ps.Dispose()
            psCurrent = Nothing
            _isBusy = False
            RaiseEvent ExecutionFinished()
        End Try
    End Function
    Public Async Function GetResult(Of T)(Optional useDefaultHandlers As Boolean = False) As Task(Of T)
        Dim results = Await GetResults(Of T)(useDefaultHandlers)
        Return results.FirstOrDefault
    End Function
    Public Async Function GetResult(Optional useDefaultHandlers As Boolean = False) As Task(Of Object)
        Dim results = Await GetResults(useDefaultHandlers)
        Return results.FirstOrDefault
    End Function
    Public Async Function GetScriptVariable(Of T)(varName As String) As Task(Of IEnumerable(Of T))
        Return Await NewPipeline.AddCommand("Get-Variable").AddArgument(varName).AddParameter("ValueOnly").GetResults(Of T)()
    End Function
    Public Async Function GetScriptVariable(varName As String) As Task(Of IEnumerable(Of Object))
        Return Await NewPipeline.AddCommand("Get-Variable").AddArgument(varName).AddParameter("ValueOnly").GetResults()
    End Function
#End Region

#Region "Fluent Composition of Powershell Pipelines/Commands that will be Executed"
    Public Function NewPipeline(Optional withScript As String = Nothing) As PSExecuter
        If IsBusy Then Throw New InvalidOperationException("PowerShell is already executing!")
        If psCurrent Is Nothing Then psCurrent = PowerShell.Create
        If String.IsNullOrWhiteSpace(withScript) Then
            psCurrent.AddStatement()
        Else
            psCurrent.AddScript(withScript)
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
    Public Sub SetSessionVariable(varName As String, value As Object)
        psRunspace.SessionStateProxy.SetVariable(varName, value)
    End Sub

    Public Function SetScriptVariable(varName As String, value As Object) As PSExecuter
        NewPipeline.AddCommand("set-variable").AddArgument(varName).AddArgument(value)
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
