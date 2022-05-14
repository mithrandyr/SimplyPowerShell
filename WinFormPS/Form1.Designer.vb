<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.txtCode = New System.Windows.Forms.TextBox()
        Me.btnExecute = New System.Windows.Forms.Button()
        Me.btnClearSession = New System.Windows.Forms.Button()
        Me.btnClearResults = New System.Windows.Forms.Button()
        Me.txtResults = New System.Windows.Forms.TextBox()
        Me.pbExecution = New System.Windows.Forms.ProgressBar()
        Me.lstCommands = New System.Windows.Forms.ListBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtCode
        '
        Me.txtCode.Location = New System.Drawing.Point(10, 10)
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Size = New System.Drawing.Size(565, 20)
        Me.txtCode.TabIndex = 0
        '
        'btnExecute
        '
        Me.btnExecute.Location = New System.Drawing.Point(10, 118)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(129, 20)
        Me.btnExecute.TabIndex = 1
        Me.btnExecute.Text = "Execute"
        Me.btnExecute.UseVisualStyleBackColor = True
        '
        'btnClearSession
        '
        Me.btnClearSession.Location = New System.Drawing.Point(144, 118)
        Me.btnClearSession.Name = "btnClearSession"
        Me.btnClearSession.Size = New System.Drawing.Size(129, 20)
        Me.btnClearSession.TabIndex = 2
        Me.btnClearSession.Text = "Clear Session"
        Me.btnClearSession.UseVisualStyleBackColor = True
        '
        'btnClearResults
        '
        Me.btnClearResults.Location = New System.Drawing.Point(278, 118)
        Me.btnClearResults.Name = "btnClearResults"
        Me.btnClearResults.Size = New System.Drawing.Size(129, 20)
        Me.btnClearResults.TabIndex = 3
        Me.btnClearResults.Text = "Clear Results"
        Me.btnClearResults.UseVisualStyleBackColor = True
        '
        'txtResults
        '
        Me.txtResults.Location = New System.Drawing.Point(10, 143)
        Me.txtResults.Multiline = True
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtResults.Size = New System.Drawing.Size(565, 237)
        Me.txtResults.TabIndex = 4
        '
        'pbExecution
        '
        Me.pbExecution.Location = New System.Drawing.Point(12, 36)
        Me.pbExecution.Name = "pbExecution"
        Me.pbExecution.Size = New System.Drawing.Size(563, 23)
        Me.pbExecution.TabIndex = 6
        '
        'lstCommands
        '
        Me.lstCommands.FormattingEnabled = True
        Me.lstCommands.Location = New System.Drawing.Point(581, 10)
        Me.lstCommands.Name = "lstCommands"
        Me.lstCommands.Size = New System.Drawing.Size(257, 368)
        Me.lstCommands.TabIndex = 7
        '
        'btnCancel
        '
        Me.btnCancel.Enabled = False
        Me.btnCancel.Location = New System.Drawing.Point(413, 118)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(129, 20)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AcceptButton = Me.btnExecute
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(850, 388)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lstCommands)
        Me.Controls.Add(Me.pbExecution)
        Me.Controls.Add(Me.txtResults)
        Me.Controls.Add(Me.btnClearResults)
        Me.Controls.Add(Me.btnClearSession)
        Me.Controls.Add(Me.btnExecute)
        Me.Controls.Add(Me.txtCode)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtCode As TextBox
    Friend WithEvents btnExecute As Button
    Friend WithEvents btnClearSession As Button
    Friend WithEvents btnClearResults As Button
    Friend WithEvents txtResults As TextBox
    Friend WithEvents pbExecution As ProgressBar
    Friend WithEvents lstCommands As ListBox
    Friend WithEvents btnCancel As Button
End Class
