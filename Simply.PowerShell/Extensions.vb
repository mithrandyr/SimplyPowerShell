Imports System.Dynamic
Imports System.Runtime.CompilerServices

Module Extensions
    <Extension>
    Function AsExpandoObject(this As PSObject) As ExpandoObject
        Dim eo = CType(New ExpandoObject, IDictionary(Of String, Object))
        For Each prop In this.Properties
            If prop.TypeNameOfValue.EndsWith("System.Management.Automation.PSCustomObject", StringComparison.OrdinalIgnoreCase) Or
                prop.TypeNameOfValue.EndsWith("System.Management.Automation.PSObject", StringComparison.OrdinalIgnoreCase) Then
                eo.Add(prop.Name, DirectCast(prop.Value, PSObject).AsExpandoObject)
            ElseIf TypeOf prop.Value Is ICollection Then
                eo.Add(prop.Name, DirectCast(prop.Value, ICollection).AsExpandoObject)
            Else
                eo.Add(prop.Name, prop.Value)
            End If
        Next
        Return eo
    End Function
    <Extension>
    Function AsExpandoObject(this As ICollection) As ICollection
        Dim result As New ArrayList
        For Each item In this
            If TypeOf item Is ICollection Then
                result.Add(DirectCast(item, ICollection).AsExpandoObject)
            ElseIf TypeOf item Is PSCustomObject Or TypeOf item Is PSObject Then
                result.Add(DirectCast(item, PSObject).AsExpandoObject)
            Else
                result.Add(item)
            End If
        Next
        Return result
    End Function
End Module
