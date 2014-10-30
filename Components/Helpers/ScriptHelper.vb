''' <summary>
''' Utility class for extracting embedded scripts.
''' </summary>
''' <remarks>
''' Uses a naming convention. All scripts should be placed 
''' in the Resources\Scripts folder. The scriptName is just 
''' the filename of the script.
''' </remarks>
Public Class ScriptHelper
    ''' <summary>
    ''' Returns the contents of the embedded script as
    ''' a stringwrapped with the start / end script tags.
    ''' </summary>
    ''' <param name="scriptName">FileName of the script.</param>
    ''' <returns>Contents of the script.</returns>
    Public Shared Function UnpackScript(ByVal scriptName As String) As String
        Dim scriptType As String = "text/javascript"
        Dim extension As String = IO.Path.GetExtension(scriptName)

        If 0 = String.Compare(extension, ".vbs", True, System.Globalization.CultureInfo.InvariantCulture) Then
            scriptType = "text/vbscript"
        End If

        Return UnpackScript(scriptName, scriptType)
    End Function

    Public Shared Function UnpackScript(ByVal scriptName As String, ByVal scriptType As String) As String
        Return "<script type=""" & scriptType & """>" + Environment.NewLine + UnpackEmbeddedResourceToString(scriptName) + Environment.NewLine + "</script>"
    End Function
    Public Shared Function PrepareScript(ByVal script As String, ByVal scriptType As String) As String
        Return "<script type=""" & scriptType & """>" + Environment.NewLine + script + Environment.NewLine + "</script>"
    End Function
    ' Unpacks the embedded resource to string.
    Shared Function UnpackEmbeddedResourceToString(ByVal resourceName As String) As String
        Dim executingAssembly As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
        Dim resourceStream As IO.Stream = executingAssembly.GetManifestResourceStream(GetType(ScriptHelper), resourceName)
        Using reader As New IO.StreamReader(resourceStream, System.Text.Encoding.ASCII)
            Return reader.ReadToEnd()
        End Using
    End Function
End Class

