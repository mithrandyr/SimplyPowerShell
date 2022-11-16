Imports Simply.PSDotNet
Public Class Form1

    Private WithEvents PS As PSExecuter

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        PS = New PSExecuter()
    End Sub

    Private Async Sub btnExecute_Click(sender As Object, e As EventArgs) Handles btnExecute.Click
        IsWorking()
        Dim theCode As String = txtCode.Text
        txtCode.Clear()
        lstCommands.Items.Add(theCode)
        lstCommands.SelectedIndex = lstCommands.Items.Count - 1

        txtResults.AppendText(String.Format("==========[{0:hh:mm:ss tt}]========================================{1}", Now, Environment.NewLine))
        txtResults.AppendText(String.Format("[PowerShell] {0}{1}", theCode, Environment.NewLine))

        pbExecution.Style = ProgressBarStyle.Marquee
        pbExecution.Value = 0
        Dim sw As Stopwatch = Stopwatch.StartNew
        'Dim results = Await PS.InvokeAsync(theCode)
        Await PS.RunAsync(theCode)
        sw.Stop()

        txtResults.AppendText(String.Format("==========[Elapsed: {0}ms]========================================{1}", sw.Elapsed.TotalMilliseconds, Environment.NewLine))

        IsWorking(False)
    End Sub

    Private Sub btnClearResults_Click(sender As Object, e As EventArgs) Handles btnClearResults.Click
        txtResults.Clear()
    End Sub

    Private Sub btnClearSession_Click(sender As Object, e As EventArgs) Handles btnClearSession.Click
        PS.Reset()
    End Sub

    Private Sub IsWorking(Optional val As Boolean = True)
        btnExecute.Enabled = Not val
        btnClearSession.Enabled = Not val
        btnClearResults.Enabled = Not val
        btnCancel.Enabled = val
        If val Then
            pbExecution.Style = ProgressBarStyle.Marquee
        Else
            pbExecution.Style = ProgressBarStyle.Blocks
        End If
    End Sub

    Private Sub UpdateProgress(theRecord As PSProgressItem) Handles PS.ProgressGenerated
        If theRecord.PercentComplete = -1 Then
            pbExecution.Style = ProgressBarStyle.Marquee
        Else
            pbExecution.Style = ProgressBarStyle.Blocks
            pbExecution.Value = theRecord.PercentComplete
        End If
        'End If
    End Sub

    Private Sub UpdateOutput(theObject As Object) Handles PS.OutputGenerated
        txtResults.AppendText(theObject.ToString() + Environment.NewLine)
    End Sub

    Private Sub lstCommands_DoubleClick(sender As Object, e As EventArgs) Handles lstCommands.DoubleClick
        txtCode.Text = lstCommands.SelectedItem
        txtCode.Focus()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        PS.CancelAsync()
    End Sub

End Class
