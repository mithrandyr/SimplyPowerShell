Imports System.Dynamic
Imports System.Runtime.CompilerServices

Module Extensions
    <Extension>
    Function AsExpandoObject(this As PSObject) As ExpandoObject
        Dim eo = CType(New ExpandoObject, IDictionary(Of String, Object))
        For Each prop In this.Properties
            eo.Add(prop.Name, prop.Value)
        Next
        Return eo
    End Function
End Module
