'***************************************************************************/
'* DNNUtilities.vb
'*
'* COPYRIGHT (c) 2004 by DNNStuff
'* ALL RIGHTS RESERVED.
'*
'* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'* TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'* THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'* DEALINGS IN THE SOFTWARE.
'*************/
Option Strict On
Option Explicit On
Imports DotNetNuke
Imports System.Xml
Imports DotNetNuke.Common.Utilities
Imports System.Collections.Specialized
Imports System.Text.RegularExpressions
Imports DotNetNuke.Entities.Host

Namespace DNNStuff.Aggregator
    Public Class DNNUtilities

        Public Shared Function GetSetting(ByVal settings As Hashtable, ByVal key As String, Optional ByVal [default] As String = "") As String
            Dim ret As String

            If settings.ContainsKey(key) Then
                Try
                    ret = settings(key).ToString
                Catch ex As Exception
                    ret = [default]
                End Try
            Else
                ret = [default]
            End If
            Return ret
        End Function


        Public Shared Function GetEncryptionKey() As String
            Try
                Dim xmlConfig As System.Xml.XmlDocument = Config.Load
                Dim xmlMachineKey As XmlNode = xmlConfig.SelectSingleNode("configuration/system.web/machineKey")
                Return xmlMachineKey.Attributes("decryptionKey").InnerText
            Catch ex As Exception
                Return ""
            End Try
        End Function

#Region "QueryString"

        Public Shared Function GetParamsForNavigate(ByVal key As String, ByVal value As String) As String()
            Dim returnValue As String = ""
            Dim coll As NameValueCollection = HttpContext.Current.Request.QueryString
            Dim arrKeys(), arrValues() As String
            arrKeys = coll.AllKeys
            For i As Integer = 0 To arrKeys.GetUpperBound(0)
                If arrKeys(i) IsNot Nothing Then
                    Select Case arrKeys(i).ToLower
                        Case "tabid", "ctl", "language", key.ToLower
                            'skip parameter
                        Case Else
                            arrValues = coll.GetValues(i)
                            For j As Integer = 0 To arrValues.GetUpperBound(0)
                                If returnValue <> "" Then returnValue += "&"
                                returnValue += arrKeys(i) + "=" + arrValues(j)
                            Next
                    End Select
                End If
            Next
            If returnValue <> "" Then returnValue += "&"
            returnValue += key + "=" + value

            'return the new querystring as a string array
            Return returnValue.Split("&"c)
        End Function

#End Region

#Region " Skinning"
        Public Shared Sub InjectCSS(ByVal pg As System.Web.UI.Page, ByVal fileName As String)

            ' page style sheet reference
            Dim objCSS As Control = pg.FindControl("CSS")
            If objCSS Is Nothing Then
                ' DNN 4 doesn't have CSS control any more, look for Head
                objCSS = pg.FindControl("Head")
            End If

            ' container stylesheet
            If Not objCSS Is Nothing Then
                Dim CSSId As String = DotNetNuke.Common.Globals.CreateValidID(fileName)

                ' container package style sheet
                If objCSS.FindControl(CSSId) Is Nothing Then

                    Dim objLink As New HtmlGenericControl("link")
                    objLink.ID = CSSId
                    objLink.Attributes("rel") = "stylesheet"
                    objLink.Attributes("type") = "text/css"
                    objLink.Attributes("href") = fileName
                    objCSS.Controls.Add(objLink)
                End If
            End If

        End Sub
#End Region
#Region " Enums"
        Public Shared Sub EnumToListBox(ByVal EnumType As Type, ByVal TheListBox As ListControl)

            Dim Values As Array = System.[Enum].GetValues(EnumType)
            For Each Value As Integer In Values
                Dim Display As String = [Enum].GetName(EnumType, Value)
                Dim Item As New ListItem(Display, Value.ToString())
                TheListBox.Items.Add(Item)
            Next

        End Sub
#End Region

#Region "version checking"
        ''' <summary>
        ''' Returns a version-safe set of version numbers for DNN
        ''' </summary>
        ''' <param name="major">out int of the DNN Major version</param>
        ''' <param name="minor">out int of the DNN Minor version</param>
        ''' <param name="revision">out int of the DNN Revision</param>
        ''' <param name="build">out int of the DNN Build version</param>
        ''' <remarks>Dnn moved the version number during about the 4.9 version, which to me was a bit frustrating and caused the need for this reflection method call</remarks>
        ''' <returns>true if it worked.</returns>
        Public Shared Function SafeDNNVersion(major As Integer, minor As Integer, revision As Integer, build As Integer) As Boolean
            Dim ver As System.Version = System.Reflection.Assembly.GetAssembly(GetType(DotNetNuke.Common.Globals)).GetName().Version
            If ver IsNot Nothing Then
                major = ver.Major
                minor = ver.Minor
                build = ver.Build
                revision = ver.Revision
                Return True
            Else
                major = 0
                minor = 0
                build = 0
                revision = 0
                Return False
            End If
        End Function

        Public Shared Function SafeDNNVersion() As System.Version
            Dim ver As System.Version = System.Reflection.Assembly.GetAssembly(GetType(DotNetNuke.Common.Globals)).GetName().Version
            If ver IsNot Nothing Then
                Return ver
            End If
            Return New System.Version(0, 0, 0, 0)
        End Function
#End Region
    End Class
End Namespace
