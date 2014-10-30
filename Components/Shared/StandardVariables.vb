Namespace DNNStuff.Common
    Public Class HashTableHelpers
        Public Shared Function Merge(ByVal h1 As Hashtable, ByVal h2 As Hashtable) As Hashtable

            For Each key As String In h2.Keys
                If h1.ContainsKey(key) Then
                    h1(key) = h2(key)
                Else
                    h1.Add(key, h2(key))
                End If
            Next

            Return h1
        End Function
    End Class

    Public Class StandardVariables
        Public Shared Function QueryString(ByVal req As HttpRequest) As Hashtable
            Return BuildVars(req, "QS", "QUERYSTRING", req.QueryString)
        End Function

        Public Shared Function ServerVars(ByVal req As HttpRequest, Optional ByVal makeSQLReady As Boolean = False) As Hashtable
            Return BuildVars(req, "SV", "SERVERVAR", req.ServerVariables)
        End Function

        Public Shared Function FormVars(ByVal req As HttpRequest, Optional ByVal makeSQLReady As Boolean = False) As Hashtable
            Return BuildVars(req, "FV", "FORMVAR", req.Form)
        End Function

        Public Shared Function BuildVars(ByVal req As HttpRequest, ByVal shortPrefix As String, ByVal longPrefix As String, ByVal namevals As Specialized.NameValueCollection, Optional ByVal makeSQLReady As Boolean = False) As Hashtable
            Dim hs As Hashtable = New Hashtable()

            ' add server variables
            Dim vals As New Specialized.NameValueCollection(namevals) ' create a copy
            Dim keyval As Object
            Dim debugOutput As New System.Text.StringBuilder
            For Each key As String In vals.Keys
                keyval = vals(key)
                If key IsNot Nothing And keyval IsNot Nothing Then
                    Dim value As String = keyval.ToString
                    If makeSQLReady Then value = value.Replace("'", "''")

                    hs.Add(String.Format("{0}:{1}", shortPrefix, key.ToUpper()), value)
                    hs.Add(String.Format("{0}:{1}", longPrefix, key.ToUpper()), value)

                    ' add to all
                    debugOutput.AppendFormat("<strong>{2}:{0}</strong>={1}<br />", key, value, shortPrefix)
                End If
            Next
            hs.Add("DEBUG:" & longPrefix, debugOutput.ToString)
            Return hs
        End Function

    End Class
End Namespace