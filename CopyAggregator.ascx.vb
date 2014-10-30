'***************************************************************************/
'* CopyAggregator.ascx.vb
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

Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports System.Xml

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Permissions

Namespace DNNStuff.Aggregator

    Partial Class CopyAggregator
        Inherits Entities.Modules.PortalModuleBase

        ' other

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
        Private Sub ReturnToPage()

            ' Redirect back to the portal home page
            Response.Redirect(NavigateURL(), True)

        End Sub
#End Region

#Region " Process"
        Private Function CopyAggregator(ByVal AggregatorModuleId As Integer, ByVal CopyTabId As Integer, ByVal CopyOptions As ArrayList, ByVal AggregatorTitle As String, ByVal CopyLevel As Integer) As ModuleInfo
            CopyLevel += 1

            Dim objTabs As New TabController
            Dim objTab As TabInfo

            LogResults(String.Format("Copying Aggregator - {0}", AggregatorModuleId), CopyLevel)

            ' grab tab
            objTab = objTabs.GetTab(CopyTabId, PortalId, True)

            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo

            Dim objModuleMapping As New Dictionary(Of Integer, Integer)

            For Each copyOption As ModuleCopyOptions In CopyOptions
                LogResults(String.Format("Copying Module - {0} Copy={1}, Ref={2}", copyOption.Title, copyOption.Copy, copyOption.Reference), CopyLevel)
                If copyOption.Active Then
                    objModule = objModules.GetModule(copyOption.ModuleId, copyOption.TabId, False)
                    If Not objModule Is Nothing Then
                        If copyOption.Copy Then
                            Dim newModule As ModuleInfo
                            If Me.ModuleConfiguration.ModuleDefID = objModule.ModuleDefID Then
                                ' another Aggregator - do a special copy
                                newModule = CopyAggregator(copyOption.ModuleId, CopyTabId, GetCopyOptions(copyOption.ModuleId), copyOption.Title, CopyLevel)

                                ' replace old tabmoduleid with new one
                                objModuleMapping.Add(copyOption.TabModuleId, newModule.TabModuleID)
                            Else
                                'Clone module as it exists in the cache and changes we make will update the cached object
                                newModule = objModule.Clone()

                                newModule.ModuleID = Null.NullInteger
                                newModule.TabID = objTab.TabID
                                newModule.ModuleTitle = copyOption.Title
                                newModule.ModuleID = objModules.AddModule(newModule)

                                Dim businessController As String = Compatibility.ModuleBusinessControllerClass(newModule)
                                If businessController <> "" Then
                                    Dim objObject As Object = Framework.Reflection.CreateObject(businessController, businessController)
                                    If TypeOf objObject Is IPortable Then
                                        Try
                                            Dim Content As String = CType(CType(objObject, IPortable).ExportModule(copyOption.ModuleId), String)
                                            If Content <> "" Then
                                                CType(objObject, IPortable).ImportModule(newModule.ModuleID, Content, Compatibility.ModuleVersion(newModule), UserInfo.UserID)
                                            End If
                                        Catch exc As Exception
                                            ' the export/import operation failed
                                            ProcessModuleLoadException(Me, exc)
                                        End Try
                                    End If
                                End If
                                ' get new module so we can get it's tabmoduleid
                                newModule = objModules.GetModule(newModule.ModuleID, CopyTabId, False)

                                ' replace old tabmoduleid with new one
                                objModuleMapping.Add(copyOption.TabModuleId, newModule.TabModuleID)
                            End If
                        Else
                            ' reference of original module
                            objModuleMapping.Add(copyOption.TabModuleId, copyOption.TabModuleId)
                        End If
                    Else
                        LogResults(String.Format("Module Not Found - {0} Copy={1}, Ref={2}", copyOption.Title, copyOption.Copy, copyOption.Reference), CopyLevel)
                    End If
                End If
            Next
            ' get aggregator export xml
            Dim AggContent As String = ExportAggregator(AggregatorModuleId, objModuleMapping)

            Dim objAggregator As ModuleInfo = ImportAggregator(AggregatorModuleId, CopyTabId, AggContent, AggregatorTitle)

            CopyLevel -= 1
            Return objAggregator
        End Function

        Private Function ImportAggregator(ByVal AggregatorModuleId As Integer, ByVal newTabId As Integer, ByVal Content As String, ByVal AggregatorTitle As String) As ModuleInfo
            Dim objCtrl As New AggregatorController
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo

            objModule = objModules.GetModule(AggregatorModuleId, TabId, False)
            If Not objModule Is Nothing Then
                'Clone module as it exists in the cache and changes we make will update the cached object
                Dim newModule As ModuleInfo = objModule.Clone()

                newModule.ModuleID = Null.NullInteger
                newModule.TabID = newTabId
                If AggregatorTitle.Length > 0 Then newModule.ModuleTitle = AggregatorTitle
                newModule.ModuleID = objModules.AddModule(newModule)

                objCtrl.ImportModule(newModule.ModuleID, Content, Compatibility.ModuleVersion(newModule), UserInfo.UserID)

                ' get new module so we can get it's tabmoduleid
                newModule = objModules.GetModule(newModule.ModuleID, newTabId, False)
                Return newModule
            End If
            Return objModule

        End Function

        Private Function ExportAggregator(ByVal AggregatorModuleId As Integer, ByVal ModuleMapping As System.Collections.Generic.Dictionary(Of Integer, Integer)) As String
            Dim objCtrl As New AggregatorController

            Dim Content As String = objCtrl.ExportModule(AggregatorModuleId, ModuleMapping)
            Return Content
        End Function

        Private Sub LogResults(ByVal s As String, ByVal level As Integer)
            'phResults.Controls.Add(New LiteralControl(String.Format("<li>{0}{1}</li>", Space(level), s)))
        End Sub
#End Region

#Region " Settings"

        Private Sub LoadSettings()

            ' pages
            BindPages()

        End Sub

        Private Sub BindPages()
            cboCopyPage.DataSource = TabController.GetPortalTabs(PortalId, -1, False, True)
            cboCopyPage.DataBind()
        End Sub

#End Region

#Region " Data"
        Private Function LoadTabModules() As ArrayList

            Dim ctrl As New AggregatorController
            Dim arrModules As New ArrayList

            Dim _aggregator As AggregatorInfo = New AggregatorInfo
            _aggregator = ctrl.GetAggregatorObjectGraph(ModuleId, "", "")

            Dim tabNumber As Integer = 1
            For Each ati As AggregatorTabInfo In _aggregator.Tabs
                For Each ami As AggregatorModuleInfo In ati.Modules
                    ami.TagData = tabNumber.ToString
                    If ModulePermissionController.CanAdminModule(ami.ModuleInfo) AndAlso Not ami.ModuleInfo.AllTabs Then
                        arrModules.Add(ami)
                    End If
                Next
                tabNumber += 1
            Next

            Return arrModules

        End Function

        Private Sub DisplayTabModules()
            grdModules.DataSource = LoadTabModules()
            grdModules.DataBind()
            rowModules.Visible = True
        End Sub

        Private Function GetCopyOptions() As ArrayList
            ' return list of options from page input
            Dim options As New ArrayList

            Dim chkModule As CheckBox
            Dim txtTabModuleId As HtmlInputHidden
            Dim txtTabId As HtmlInputHidden
            Dim optCopy As RadioButton
            Dim optReference As RadioButton
            Dim txtCopyTitle As TextBox

            For Each objDataGridItem As DataGridItem In grdModules.Items
                Dim intModuleID As Integer = CType(grdModules.DataKeys(objDataGridItem.ItemIndex), Integer)
                chkModule = CType(objDataGridItem.FindControl("chkModule"), CheckBox)
                txtTabModuleId = CType(objDataGridItem.FindControl("txtTabModuleId"), HtmlInputHidden)
                txtTabId = CType(objDataGridItem.FindControl("txtTabId"), HtmlInputHidden)
                optCopy = CType(objDataGridItem.FindControl("optCopy"), RadioButton)
                optReference = CType(objDataGridItem.FindControl("optReference"), RadioButton)
                txtCopyTitle = CType(objDataGridItem.FindControl("txtCopyTitle"), TextBox)

                Dim copyoption As New ModuleCopyOptions
                With copyoption
                    .Active = chkModule.Checked
                    .ModuleId = intModuleID
                    .TabModuleId = Int32.Parse(txtTabModuleId.Value)
                    .TabId = Int32.Parse(txtTabId.Value)
                    .Copy = optCopy.Checked
                    .Reference = optReference.Checked
                    .Title = txtCopyTitle.Text
                End With
                options.Add(copyoption)
            Next
            Return options
        End Function

        Private Function GetCopyOptions(ByVal AggregatorModuleId As Integer) As ArrayList
            ' fill list of copy options with intent to copy all modules
            Dim ctrl As New AggregatorController
            Dim options As New ArrayList

            Dim _aggregator As AggregatorInfo = New AggregatorInfo
            _aggregator = ctrl.GetAggregatorObjectGraph(AggregatorModuleId, "", "")

            Dim tabNumber As Integer = 1
            For Each ati As AggregatorTabInfo In _aggregator.Tabs
                For Each ami As AggregatorModuleInfo In ati.Modules
                    If ModulePermissionController.CanAdminModule(ami.ModuleInfo) AndAlso Not ami.ModuleInfo.IsDeleted AndAlso Not ami.ModuleInfo.AllTabs Then
                        Dim copyoption As New ModuleCopyOptions
                        With copyoption
                            .Active = True
                            .ModuleId = ami.ModuleId
                            .TabModuleId = ami.TabModuleId
                            .TabId = ami.TabId
                            .Copy = ami.IsPortable
                            .Reference = Not .Copy
                            .Title = ami.ModuleTitle
                        End With
                        options.Add(copyoption)
                    End If
                Next
                tabNumber += 1
            Next

            Return options
        End Function
        Private Class ModuleCopyOptions
            Public Active As Boolean
            Public ModuleId As Integer
            Public TabModuleId As Integer
            Public TabId As Integer
            Public Copy As Boolean
            Public Reference As Boolean
            Public Title As String
        End Class
#End Region

#Region " Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If DNNUtilities.SafeDNNVersion().Major = 5 Then
                    DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit_5.css"))
                Else
                    DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit.css"))
                End If

                If Page.IsPostBack = False Then
                    LoadSettings()
                    DisplayTabModules()
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                ReturnToPage()
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCopyAggregator_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCopyAggregator.Click
            Try
                If Page.IsValid Then
                    Dim copyTabId As Integer = Int32.Parse(cboCopyPage.SelectedItem.Value)
                    If copyTabId <> -1 Then
                        CopyAggregator(ModuleId, copyTabId, GetCopyOptions(), "", 0)
                        DataCache.ClearModuleCache(copyTabId)
                    End If
                    'cmdCopyAggregator.Visible = False
                    ReturnToPage()
                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


#End Region
    End Class


End Namespace