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
        Me.pbExecution = New System.Windows.Forms.ProgressBar()
        Me.lstCommands = New System.Windows.Forms.ListBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tcResults = New System.Windows.Forms.TabControl()
        Me.tpResults = New System.Windows.Forms.TabPage()
        Me.txtResults = New System.Windows.Forms.TextBox()
        Me.tpStreams = New System.Windows.Forms.TabPage()
        Me.lvStreams = New System.Windows.Forms.ListView()
        Me.colGenerated = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colMessage = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.tcResults.SuspendLayout()
        Me.tpResults.SuspendLayout()
        Me.tpStreams.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtCode
        '
        Me.txtCode.AcceptsReturn = True
        Me.txtCode.AcceptsTab = True
        Me.txtCode.Location = New System.Drawing.Point(15, 15)
        Me.txtCode.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtCode.Multiline = True
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Size = New System.Drawing.Size(846, 110)
        Me.txtCode.TabIndex = 0
        '
        'btnExecute
        '
        Me.btnExecute.Location = New System.Drawing.Point(15, 182)
        Me.btnExecute.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(194, 31)
        Me.btnExecute.TabIndex = 1
        Me.btnExecute.Text = "Execute"
        Me.btnExecute.UseVisualStyleBackColor = True
        '
        'btnClearSession
        '
        Me.btnClearSession.Location = New System.Drawing.Point(216, 182)
        Me.btnClearSession.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnClearSession.Name = "btnClearSession"
        Me.btnClearSession.Size = New System.Drawing.Size(194, 31)
        Me.btnClearSession.TabIndex = 2
        Me.btnClearSession.Text = "Clear Session"
        Me.btnClearSession.UseVisualStyleBackColor = True
        '
        'btnClearResults
        '
        Me.btnClearResults.Location = New System.Drawing.Point(417, 182)
        Me.btnClearResults.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnClearResults.Name = "btnClearResults"
        Me.btnClearResults.Size = New System.Drawing.Size(194, 31)
        Me.btnClearResults.TabIndex = 3
        Me.btnClearResults.Text = "Clear Results"
        Me.btnClearResults.UseVisualStyleBackColor = True
        '
        'pbExecution
        '
        Me.pbExecution.Location = New System.Drawing.Point(15, 137)
        Me.pbExecution.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pbExecution.Name = "pbExecution"
        Me.pbExecution.Size = New System.Drawing.Size(844, 35)
        Me.pbExecution.TabIndex = 6
        '
        'lstCommands
        '
        Me.lstCommands.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstCommands.FormattingEnabled = True
        Me.lstCommands.ItemHeight = 20
        Me.lstCommands.Location = New System.Drawing.Point(891, 15)
        Me.lstCommands.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.lstCommands.Name = "lstCommands"
        Me.lstCommands.Size = New System.Drawing.Size(384, 524)
        Me.lstCommands.TabIndex = 7
        '
        'btnCancel
        '
        Me.btnCancel.Enabled = False
        Me.btnCancel.Location = New System.Drawing.Point(620, 182)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(194, 31)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'tcResults
        '
        Me.tcResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tcResults.Controls.Add(Me.tpResults)
        Me.tcResults.Controls.Add(Me.tpStreams)
        Me.tcResults.Location = New System.Drawing.Point(18, 222)
        Me.tcResults.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tcResults.Name = "tcResults"
        Me.tcResults.SelectedIndex = 0
        Me.tcResults.Size = New System.Drawing.Size(861, 318)
        Me.tcResults.TabIndex = 9
        '
        'tpResults
        '
        Me.tpResults.Controls.Add(Me.txtResults)
        Me.tpResults.Location = New System.Drawing.Point(4, 29)
        Me.tpResults.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpResults.Name = "tpResults"
        Me.tpResults.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpResults.Size = New System.Drawing.Size(853, 285)
        Me.tpResults.TabIndex = 0
        Me.tpResults.Text = "Results"
        Me.tpResults.UseVisualStyleBackColor = True
        '
        'txtResults
        '
        Me.txtResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtResults.Location = New System.Drawing.Point(9, 5)
        Me.txtResults.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtResults.Multiline = True
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtResults.Size = New System.Drawing.Size(829, 262)
        Me.txtResults.TabIndex = 5
        '
        'tpStreams
        '
        Me.tpStreams.Controls.Add(Me.lvStreams)
        Me.tpStreams.Location = New System.Drawing.Point(4, 29)
        Me.tpStreams.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpStreams.Name = "tpStreams"
        Me.tpStreams.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpStreams.Size = New System.Drawing.Size(853, 285)
        Me.tpStreams.TabIndex = 1
        Me.tpStreams.Text = "Streams"
        Me.tpStreams.UseVisualStyleBackColor = True
        '
        'lvStreams
        '
        Me.lvStreams.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colGenerated, Me.colType, Me.colMessage})
        Me.lvStreams.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvStreams.FullRowSelect = True
        Me.lvStreams.HideSelection = False
        Me.lvStreams.Location = New System.Drawing.Point(4, 5)
        Me.lvStreams.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.lvStreams.Name = "lvStreams"
        Me.lvStreams.Size = New System.Drawing.Size(845, 275)
        Me.lvStreams.TabIndex = 0
        Me.lvStreams.UseCompatibleStateImageBehavior = False
        Me.lvStreams.View = System.Windows.Forms.View.Details
        '
        'colGenerated
        '
        Me.colGenerated.Text = "Generated"
        Me.colGenerated.Width = 150
        '
        'colType
        '
        Me.colType.Text = "Type"
        Me.colType.Width = 150
        '
        'colMessage
        '
        Me.colMessage.Text = "Message"
        Me.colMessage.Width = 500
        '
        'Form1
        '
        Me.AcceptButton = Me.btnExecute
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1288, 555)
        Me.Controls.Add(Me.tcResults)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lstCommands)
        Me.Controls.Add(Me.pbExecution)
        Me.Controls.Add(Me.btnClearResults)
        Me.Controls.Add(Me.btnClearSession)
        Me.Controls.Add(Me.btnExecute)
        Me.Controls.Add(Me.txtCode)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(1302, 585)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.tcResults.ResumeLayout(False)
        Me.tpResults.ResumeLayout(False)
        Me.tpResults.PerformLayout()
        Me.tpStreams.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtCode As TextBox
    Friend WithEvents btnExecute As Button
    Friend WithEvents btnClearSession As Button
    Friend WithEvents btnClearResults As Button
    Friend WithEvents pbExecution As ProgressBar
    Friend WithEvents lstCommands As ListBox
    Friend WithEvents btnCancel As Button
    Friend WithEvents tcResults As TabControl
    Friend WithEvents tpResults As TabPage
    Friend WithEvents txtResults As TextBox
    Friend WithEvents tpStreams As TabPage
    Friend WithEvents colGenerated As ColumnHeader
    Friend WithEvents colType As ColumnHeader
    Friend WithEvents colMessage As ColumnHeader
    Friend WithEvents lvStreams As ListView
End Class
