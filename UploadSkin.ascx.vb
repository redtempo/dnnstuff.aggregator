Imports System
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml
Imports System.Text.RegularExpressions

Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization
Imports DotNetNuke
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Host

Namespace DNNStuff.Aggregator

    Partial Class UploadSkin
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase


#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit.css"))
            Page.ClientScript.RegisterClientScriptInclude(Me.GetType, "yeti", ResolveUrl("resources/support/yetii-min.js"))

        End Sub
#End Region

#Region " Navigation"
        Private Sub cmdReturn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReturn.Click, cmdReturn2.Click
            Response.Redirect(NavigateBack())
        End Sub

        Private Function NavigateBack() As String
            Return DotNetNuke.Common.NavigateURL(TabId)
        End Function
#End Region

#Region " Upload"
        Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
            Try
                Dim strFileName As String
                Dim strExtension As String = ""
                Dim strMessage As String = ""

                Dim postedFile As HttpPostedFile = cmdBrowse.PostedFile

                'Get localized Strings
                Dim strInvalid As String = DotNetNuke.Services.Localization.Localization.GetString("InvalidExt", Me.LocalResourceFile)

                strFileName = System.IO.Path.GetFileName(postedFile.FileName)
                strExtension = Path.GetExtension(strFileName)

                If postedFile.FileName <> "" Then
                    If strExtension.ToLower = ".zip" Then
                        Dim objLbl As New Label
                        objLbl.CssClass = "Normal"
                        objLbl.Text = UploadSkin(MapPath("Skins"), postedFile.InputStream)
                        phPaLogs.Controls.Add(objLbl)
                    Else
                        strMessage += strInvalid & " skin file " & strFileName
                    End If
                Else
                    strMessage = DotNetNuke.Services.Localization.Localization.GetString("NoFile", Me.LocalResourceFile)
                End If

                If phPaLogs.Controls.Count > 0 Then
                    tblLogs.Visible = True
                ElseIf strMessage = "" Then
                    Response.Redirect(NavigateBack())
                Else
                    lblMessage.Text = strMessage
                End If

            Catch exc As Exception    'Module failed to load
                DotNetNuke.Services.Exceptions.ProcessModuleLoadException(Me, exc)
            End Try
        End Sub



        Public Shared Function UploadSkin(ByVal RootPath As String, ByVal objInputStream As Stream) As String

            Dim objZipInputStream As New ZipInputStream(objInputStream)

            Dim objZipEntry As ZipEntry
            Dim strExtension As String
            Dim strFileName As String
            Dim objFileStream As FileStream
            Dim intSize As Integer = 2048
            Dim arrData(2048) As Byte
            Dim strMessage As String = ""
            Dim arrSkinFiles As New ArrayList

            'Localized Strings
            Dim ResourcePortalSettings As PortalSettings = DotNetNuke.Common.Globals.GetPortalSettings()
            Dim BEGIN_MESSAGE As String = Localization.GetString("BeginZip", ResourcePortalSettings)
            Dim CREATE_DIR As String = Localization.GetString("CreateDir", ResourcePortalSettings)
            Dim WRITE_FILE As String = Localization.GetString("WriteFile", ResourcePortalSettings)
            Dim FILE_ERROR As String = Localization.GetString("FileError", ResourcePortalSettings)
            Dim END_MESSAGE As String = Localization.GetString("EndZip", ResourcePortalSettings)
            Dim FILE_RESTICTED As String = Localization.GetString("FileRestricted", ResourcePortalSettings)

            strMessage += FormatMessage(BEGIN_MESSAGE, "", -1, False)

            objZipEntry = objZipInputStream.GetNextEntry
            While Not objZipEntry Is Nothing
                If Not objZipEntry.IsDirectory Then
                    ' validate file extension
                    strExtension = objZipEntry.Name.Substring(objZipEntry.Name.LastIndexOf(".") + 1)
                    If Compatibility.IsAllowedExtension(strExtension) Then
                        ' process embedded zip files
                        Select Case objZipEntry.Name.ToLower
                            Case Else
                                strFileName = RootPath & "\" & objZipEntry.Name

                                ' create the directory if it does not exist
                                If Not Directory.Exists(Path.GetDirectoryName(strFileName)) Then
                                    strMessage += FormatMessage(CREATE_DIR, Path.GetDirectoryName(strFileName), 2, False)
                                    Directory.CreateDirectory(Path.GetDirectoryName(strFileName))
                                End If

                                ' remove the old file
                                If File.Exists(strFileName) Then
                                    File.SetAttributes(strFileName, FileAttributes.Normal)
                                    File.Delete(strFileName)
                                End If
                                ' create the new file
                                objFileStream = File.Create(strFileName)

                                ' unzip the file
                                strMessage += FormatMessage(WRITE_FILE, Path.GetFileName(strFileName), 2, False)
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                While intSize > 0
                                    objFileStream.Write(arrData, 0, intSize)
                                    intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                End While
                                objFileStream.Close()

                                ' save the skin file
                                Select Case Path.GetExtension(strFileName)
                                    Case ".htm", ".html", ".ascx", ".css", ".txt"
                                        If strFileName.ToLower.IndexOf(DotNetNuke.Common.glbAboutPage.ToLower) < 0 Then
                                            arrSkinFiles.Add(strFileName)
                                        End If
                                End Select
                        End Select
                    Else
                        strMessage += FormatMessage(FILE_ERROR, String.Format(FILE_RESTICTED, objZipEntry.Name, Compatibility.AllowedExtensions), 2, True)
                    End If
                End If
                objZipEntry = objZipInputStream.GetNextEntry
            End While
            strMessage += FormatMessage(END_MESSAGE, "", 1, False)
            objZipInputStream.Close()

            Return strMessage

        End Function

        Public Shared Function FormatMessage(ByVal Title As String, ByVal Body As String, ByVal Level As Integer, ByVal IsError As Boolean) As String
            Dim Message As String = Title

            If IsError Then
                Message = "<font class=""NormalRed"">" & Title & "</font>"
            End If

            Select Case Level
                Case -1
                    Message = "<hr><br><b>" & Message & "</b>"
                Case 0
                    Message = "<br><br><b>" & Message & "</b>"
                Case 1
                    Message = "<br><b>" & Message & "</b>"
                Case Else
                    Message = "<br><li>" & Message
            End Select

            Return Message & ": " & Body & vbCrLf

        End Function
#End Region


    End Class

End Namespace
