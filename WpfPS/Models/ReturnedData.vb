Public Structure ReturnedData
    Public ReadOnly Property Data As Object
    Public Property IsSelected As Boolean

    Public Sub New(d As Object, sel As Boolean)
        Data = d
        IsSelected = sel
    End Sub
End Structure
