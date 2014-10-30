'***************************************************************************/
'* ManageSkin.ascx.vb
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
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Exceptions
Imports Dotnetnuke.Services.Localization
Imports System.Collections.Generic
Imports System.IO

Namespace DNNStuff.Aggregator

    Partial Class ManageSkin
        Inherits Entities.Modules.PortalModuleBase

        ' other
        Private Const ALLTEMPLATES As String = "_All"
        Private Const NONE As String = "_None"

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            MyBase.HelpURL = "http://www.dnnstuff.com/"
        End Sub

#End Region

#Region " Page Level"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If DNNUtilities.SafeDNNVersion().Major = 5 Then
                    DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit_5.css"))
                Else
                    DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit.css"))
                End If
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType, "yeti", ResolveUrl("resources/support/yetii-min.js"))

                If Page.IsPostBack = False Then
                    LoadSettings()
                End If

            Catch ex As Exception 'Module failed to load
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                ReturnToPage()
            Catch ex As Exception 'Module failed to load
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Private Sub cmdCopySkin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCopySkin.Click
            Try
                If Page.IsValid Then
                    CopySkin()
                End If
            Catch ex As Exception 'Module failed to load
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub
        Private Sub cmdSaveFile_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSaveFile.Click
            Try
                If Page.IsValid Then
                    SaveTextToFile(txtSkinText.Text, cboEditTabFile.SelectedItem.Value)
                    phEditSkinResults.Controls.Add(New LiteralControl(String.Format("<span class=""normal"">{0} saved</span>", cboEditTabFile.SelectedItem.Value)))
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub
        Private Sub ReturnToPage()
            Entities.Modules.ModuleController.SynchronizeModule(ModuleId)

            ' clear the tab cache
            DataCache.ClearModuleCache(TabId)

            ' Redirect back to the portal home page
            Response.Redirect(NavigateURL(), True)

        End Sub
#End Region
#Region " Process"
        Private Sub CopySkin()
            Dim templateFolder As String = ""
            If cboTabTemplate.SelectedValue = ALLTEMPLATES Then
                CopySkin(cboTabSkin.SelectedValue, txtNewSkinName.Text)
            Else
                CopyTemplate(cboTabSkin.SelectedValue, cboTabTemplate.SelectedValue, txtNewSkinName.Text)
            End If
        End Sub

        Private Sub CopySkin(ByVal FromSkin As String, ByVal ToSkin As String)
            ' create to skin folder if it doesn't exist
            EnsureToFolder(ToSkin)

            ' loop through from folder and copy each template
            Dim fromSkinFolder As String = IO.Path.Combine(MapPath("Skins"), FromSkin)
            Dim fromSkinDirectory As DirectoryInfo = New DirectoryInfo(fromSkinFolder)
            For Each fromTemplateDirectory As DirectoryInfo In fromSkinDirectory.GetDirectories
                CopyTemplate(FromSkin, fromTemplateDirectory.Name, ToSkin)
            Next

        End Sub

        Private Sub EnsureToFolder(ByVal ToSkin As String)
            ' create to skin folder if it doesn't exist
            Dim toSkinFolder As String = IO.Path.Combine(MapPath("Skins"), ToSkin)
            If Not IO.Directory.Exists(toSkinFolder) Then IO.Directory.CreateDirectory(toSkinFolder)
        End Sub

        Private Sub CopyTemplate(ByVal FromSkin As String, ByVal FromTemplate As String, ByVal ToSkin As String)
            ' create to skin folder if it doesn't exist
            EnsureToFolder(ToSkin)

            ' copy files
            Dim toTemplateFolder As String = IO.Path.Combine(MapPath("Skins"), ToSkin & IO.Path.DirectorySeparatorChar & FromTemplate)
            Dim fromTemplateFolder As String = IO.Path.Combine(MapPath("Skins"), FromSkin & IO.Path.DirectorySeparatorChar & FromTemplate)
            Try
                CopyDirectory(fromTemplateFolder, toTemplateFolder, chkOverwrite.Checked)
                ' report to user
                phCopySkinResults.Controls.Add(New LiteralControl(String.Format("<li>Copied {0}/{1} to {2}/{1}</li>", FromSkin, FromTemplate, ToSkin)))

                ' fix styles css file
                Dim stylesFile As IO.FileInfo = New IO.FileInfo(IO.Path.Combine(toTemplateFolder, "styles.css"))
                If stylesFile.Exists Then
                    ' replace selectors
                    Dim contents As String = GetFileContents(stylesFile.FullName)
                    contents = contents.Replace(FromSkin & "_" & FromTemplate, ToSkin & "_" & FromTemplate)
                    SaveTextToFile(contents, stylesFile.FullName)

                    ' report to user
                    phCopySkinResults.Controls.Add(New LiteralControl(String.Format("<li>Fixed styles.css in {2}/{1}</li>", "", FromTemplate, ToSkin)))
                End If
            Catch ex As Exception
                phCopySkinResults.Controls.Add(New LiteralControl(String.Format("<li>Error copying {0}/{1} to {2}/{1}</li>", FromSkin, FromTemplate, ToSkin)))
                phCopySkinResults.Controls.Add(New LiteralControl(String.Format("<li>Error was: {0}</li>", Err.Description)))
            End Try



        End Sub

        Sub CopyDirectory(ByVal SourcePath As String, ByVal DestPath As String, Optional ByVal Overwrite As Boolean = False)
            Dim SourceDir As DirectoryInfo = New DirectoryInfo(SourcePath)
            Dim DestDir As DirectoryInfo = New DirectoryInfo(DestPath)

            ' the source directory must exist, otherwise throw an exception
            If SourceDir.Exists Then
                ' if destination SubDir's parent SubDir does not exist throw an exception
                If Not DestDir.Parent.Exists Then
                    Throw New DirectoryNotFoundException _
                        ("Destination directory does not exist: " + DestDir.Parent.FullName)
                End If

                If Not DestDir.Exists Then
                    DestDir.Create()
                End If

                ' copy all the files of the current directory
                Dim ChildFile As FileInfo
                For Each ChildFile In SourceDir.GetFiles()
                    If Overwrite Then
                        ChildFile.CopyTo(Path.Combine(DestDir.FullName, ChildFile.Name), True)
                    Else
                        ' if Overwrite = false, copy the file only if it does not exist
                        ' this is done to avoid an IOException if a file already exists
                        ' this way the other files can be copied anyway...
                        If Not File.Exists(Path.Combine(DestDir.FullName, ChildFile.Name)) Then
                            ChildFile.CopyTo(Path.Combine(DestDir.FullName, ChildFile.Name), False)
                        End If
                    End If
                Next

                ' copy all the sub-directories by recursively calling this same routine
                Dim SubDir As DirectoryInfo
                For Each SubDir In SourceDir.GetDirectories()
                    CopyDirectory(SubDir.FullName, Path.Combine(DestDir.FullName, _
                        SubDir.Name), Overwrite)
                Next
            Else
                Throw New DirectoryNotFoundException("Source directory does not exist: " + SourceDir.FullName)
            End If
        End Sub

        Public Function GetFileContents(ByVal FullPath As String, _
       Optional ByRef ErrInfo As String = "") As String

            Dim strContents As String
            Dim objReader As IO.StreamReader
            Try

                objReader = New IO.StreamReader(FullPath)
                strContents = objReader.ReadToEnd()
                objReader.Close()
                Return strContents
            Catch Ex As Exception
                ErrInfo = Ex.Message
            End Try
            Return ""
        End Function

        Public Function SaveTextToFile(ByVal strData As String, _
         ByVal FullPath As String, _
           Optional ByVal ErrInfo As String = "") As Boolean

            Dim bAns As Boolean = False
            Dim objReader As IO.StreamWriter
            Try
                objReader = New IO.StreamWriter(FullPath)
                objReader.Write(strData)
                objReader.Close()
                bAns = True
            Catch Ex As Exception
                ErrInfo = Ex.Message

            End Try
            Return bAns
        End Function

#End Region

#Region " Settings"
        Private Sub UpdateSettings()

        End Sub

        Private Sub LoadSettings()
            ' settings
            Dim ms As ModuleSettings = New ModuleSettings(ModuleId)

            ' theme/skin
            BindSkinFolder(cboTabSkin)
            BindSkinFolder(cboEditTabSkin, IncludeNone:=True, Selected:=ms.TabSkin)

            ' template
            BindTemplateFolder(cboTabTemplate, cboTabSkin.SelectedItem.Value)
            BindTemplateFolder(cboEditTabTemplate, cboEditTabSkin.SelectedItem.Value, False, IncludeNone:=True, Selected:=ms.TabTemplate)

            ' file
            BindTemplateFile(cboEditTabFile, cboEditTabSkin.SelectedItem.Value, cboEditTabTemplate.SelectedItem.Value, IncludeNone:=True)

        End Sub

        Private Sub BindSkinFolder(ByVal o As ListControl, Optional ByVal IncludeNone As Boolean = False, Optional ByVal Selected As String = "")
            Dim skinFolder As New IO.DirectoryInfo(Server.MapPath(ResolveUrl("Skins")))
            o.Items.Clear()
            For Each folder As IO.DirectoryInfo In skinFolder.GetDirectories()
                If folder.GetDirectories.Length > 0 Then
                    o.Items.Add(folder.Name)
                End If
            Next
            If IncludeNone Then
                o.Items.Insert(0, New ListItem("<None>", NONE))
            End If
            If Selected <> "" Then
                Dim si As ListItem = o.Items.FindByValue(Selected)
                If si IsNot Nothing Then si.Selected = True
            End If
        End Sub

        Private Sub BindTemplateFolder(ByVal o As ListControl, ByVal skinName As String, Optional ByVal IncludeAll As Boolean = True, Optional ByVal IncludeNone As Boolean = False, Optional ByVal Selected As String = "")
            If Not skinName = NONE Then
                Dim skinFolder As New IO.DirectoryInfo(IO.Path.Combine(Server.MapPath(ResolveUrl("Skins")), skinName))
                o.Items.Clear()
                For Each folder As IO.DirectoryInfo In skinFolder.GetDirectories()
                    If Not folder.Name.StartsWith("_") Then o.Items.Add(folder.Name)
                Next
                If IncludeAll Then
                    o.Items.Insert(0, New ListItem("All Templates", ALLTEMPLATES))
                End If
            End If
            If IncludeNone Then
                o.Items.Insert(0, New ListItem("<None>", NONE))
            End If
            If Selected <> "" Then
                Dim si As ListItem = o.Items.FindByValue(Selected)
                If si IsNot Nothing Then si.Selected = True
            End If
        End Sub

        Private Sub BindTemplateFile(ByVal o As ListControl, ByVal skinName As String, ByVal templateName As String, Optional ByRef IncludeNone As Boolean = False)
            If Not skinName = NONE Then
                Dim templateFolder As New IO.DirectoryInfo(IO.Path.Combine(IO.Path.Combine(Server.MapPath(ResolveUrl("Skins")), skinName), templateName))
                o.Items.Clear()
                For Each file As IO.FileInfo In templateFolder.GetFiles()
                    If Not ".gif.jpg.jpeg.png".Contains(file.Extension.ToLower) Then
                        o.Items.Add(New ListItem(file.Name, file.FullName))
                    End If
                Next
            End If
            If IncludeNone Then
                o.Items.Insert(0, New ListItem("<None>", NONE))
            End If
        End Sub

        Private Sub BindFile(ByVal txt As TextBox, ByVal fileName As String)
            txt.Text = GetFileContents(fileName)

        End Sub

        Private Sub cboTabSkin_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboTabSkin.SelectedIndexChanged
            BindTemplateFolder(cboTabTemplate, cboTabSkin.SelectedItem.Value, IncludeAll:=True, IncludeNone:=False)
        End Sub

        Private Sub cboEditTabSkin_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboEditTabSkin.SelectedIndexChanged
            If cboEditTabSkin.SelectedItem.Value <> NONE Then
                BindTemplateFolder(cboEditTabTemplate, cboEditTabSkin.SelectedItem.Value, IncludeAll:=False, IncludeNone:=True)
                cboEditTabFile.SelectedValue = NONE
            Else
                cboEditTabTemplate.Items.Clear()
            End If
            cboEditTabFile.Items.Clear()
            txtSkinText.Text = ""
        End Sub

        Private Sub cboEditTabTemplate_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboEditTabTemplate.SelectedIndexChanged
            If cboEditTabTemplate.SelectedItem.Value <> NONE Then
                BindTemplateFile(cboEditTabFile, cboEditTabSkin.SelectedItem.Value, cboEditTabTemplate.SelectedItem.Value, IncludeNone:=True)
            Else
                cboEditTabFile.Items.Clear()
            End If
            txtSkinText.Text = ""
        End Sub

        Private Sub cboEditTabFile_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboEditTabFile.SelectedIndexChanged
            If cboEditTabFile.SelectedItem.Value <> NONE Then
                BindFile(txtSkinText, cboEditTabFile.SelectedItem.Value)
            End If
        End Sub


#End Region

#Region " Validation"
        Private Sub vldNewSkinName_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles vldNewSkinName.ServerValidate
            args.IsValid = Not (args.Value.Length = 0 Or args.Value.Contains(" "))
        End Sub
#End Region

    End Class


End Namespace