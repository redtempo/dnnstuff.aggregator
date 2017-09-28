Option Strict On
Option Explicit On

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic
Imports DotNetNuke.Security.Permissions

Namespace DNNStuff.Aggregator

    Partial Class ManageAggregator
        Inherits Entities.Modules.PortalModuleBase

        ' other
        Private _numberOfTabs As Integer = 0
        Private _numberOfModules As Integer = 0
        Private _numberOfTargets As Integer = 0
        Private _aggregatorId As Integer = -1


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
                DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit.css"))
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType, "yeti", ResolveUrl("resources/support/yetii-min.js"))

                cmdAddAllModules.Attributes.Add("onclick", _
                    String.Format("return confirm('{0}');", Localization.GetString("ConfirmAddAllModules", LocalResourceFile)))

                ' Obtain PortalSettings from Current Context
                Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
                If Page.IsPostBack = False Then
                    LoadSettings()
                    BindTabGrid()
                    BindTargetGrid(False)
                End If

                ' Load custom settings (if any)
                LoadAggregatorCustomProperties(cpvMain)

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try

                If Page.IsValid Then
                    UpdateSettings()
                    ReturnToPage()
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

        Private Sub ReturnToPage()
            Entities.Modules.ModuleController.SynchronizeModule(ModuleId)

            ' clear the tab cache
            DataCache.ClearModuleCache(TabId)

            ' Redirect back to the portal home page
            Response.Redirect(NavigateURL(), True)

        End Sub
#End Region

#Region " Targets"
        Private Sub BindTargetGrid(ByVal InsertRow As Boolean)

            Localization.LocalizeDataGrid(grdTargets, Me.LocalResourceFile)

            Dim ctrl As New AggregatorController
            Dim dr As IDataReader = ctrl.GetTargets(ModuleId)
            Dim ds As DataSet

            ds = ConvertDataReaderToDataSet(dr)
            dr.Close()

            ' save number of modules
            _numberOfTargets = ds.Tables(0).Rows.Count

            ' inserting a new field
            If InsertRow Then
                Dim row As DataRow
                row = ds.Tables(0).NewRow()
                row("AggregatorTargetId") = "-1"
                row("TargetModuleId") = "-1"
                ds.Tables(0).Rows.InsertAt(row, 0)
                grdTargets.EditItemIndex = 0
            End If

            grdTargets.DataSource = ds
            grdTargets.DataBind()

            ' hide if nothing and not inserting
            grdTargets.Visible = Not (_numberOfTargets = 0 And Not InsertRow)

        End Sub

        Public Sub grdTargets_CancelEdit(ByVal sender As Object, ByVal e As DataGridCommandEventArgs)
            Try
                grdTargets.EditItemIndex = -1
                BindTargetGrid(False)
            Catch exc As Exception 'Target failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Sub grdTargets_Edit(ByVal sender As Object, ByVal e As DataGridCommandEventArgs)
            Try
                grdTargets.EditItemIndex = e.Item.ItemIndex
                grdTargets.SelectedIndex = -1
                BindTargetGrid(False)
            Catch exc As Exception 'Target failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


        Sub grdTargets_Item_Bound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
            Try
                If (e.Item.ItemType = ListItemType.EditItem) Then
                    Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)

                    Dim cboTarget As WebControls.DropDownList
                    cboTarget = CType(e.Item.FindControl("cboTarget"), WebControls.DropDownList)
                    BindAvailableTargetList(cboTarget, False, Convert.ToInt32(drv("AggregatorTargetId")))

                    Dim item As ListItem = cboTarget.Items.FindByValue(drv("TargetModuleId").ToString)
                    If Not item Is Nothing Then item.Selected = True

                End If
            Catch exc As Exception 'Target failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub BindAvailableTargetList(ByVal o As DropDownList, ByVal showAllTargets As Boolean, ByVal AggregatorTargetId As Integer)
            Dim dr As IDataReader

            ' targets
            dr = DataProvider.Instance().GetAvailableTargets(TabId, ModuleId, AggregatorTargetId)
            With o
                .DataSource = dr
                .DataValueField = "ModuleId"
                .DataTextField = "FullModuleTitle"
                .DataBind()
            End With

        End Sub

        Public Sub grdTargets_Update(ByVal sender As Object, ByVal e As DataGridCommandEventArgs)

            Try

                Dim cboTarget As DropDownList = CType(e.Item.FindControl("cboTarget"), WebControls.DropDownList)

                If cboTarget.SelectedValue <> "" Then
                    Dim ctrl As New AggregatorController
                    Dim ati As New AggregatorTargetInfo

                    If Integer.Parse(Convert.ToString(grdTargets.DataKeys(e.Item.ItemIndex))) = -1 Then
                        ' insert target
                        With ati
                            .ModuleId = ModuleId
                            .AggregatorTargetId = -1
                            .TargetModuleId = Convert.ToInt32(cboTarget.SelectedItem.Value)
                        End With
                    Else
                        ' update module
                        With ati
                            .ModuleId = ModuleId
                            .AggregatorTargetId = Convert.ToInt32(grdTargets.DataKeys(e.Item.ItemIndex).ToString)
                            .TargetModuleId = Convert.ToInt32(cboTarget.SelectedItem.Value)
                        End With
                    End If
                    ctrl.UpdateTarget(ati)

                    grdTargets.EditItemIndex = -1
                    BindTargetGrid(False)
                Else
                    grdTargets.EditItemIndex = -1
                    BindTargetGrid(False)
                End If
            Catch exc As Exception 'Target failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Sub grdTargets_Delete(ByVal sender As Object, ByVal e As DataGridCommandEventArgs)
            Try
                Dim ctrl As New AggregatorController

                ctrl.DeleteTarget(Integer.Parse(Convert.ToString(grdTargets.DataKeys(e.Item.ItemIndex))))

                grdTargets.EditItemIndex = -1
                BindTargetGrid(False)
            Catch exc As Exception 'Target failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdAddTarget_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddTarget.Click
            Try
                BindTargetGrid(True)
            Catch exc As Exception 'Target failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub grdTargets_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdTargets.ItemCreated
            Try
                If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
                    Dim cmdDeleteTarget As Control = e.Item.FindControl("cmdDeleteTarget")

                    If Not cmdDeleteTarget Is Nothing Then
                        CType(cmdDeleteTarget, ImageButton).Attributes.Add("onClick", "javascript: return confirm('Are You Sure You Wish To Delete This Target ?')")
                    End If
                End If
            Catch exc As Exception 'Target failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

#Region " Style Settings"
        Private Sub UpdateSettings()
            Dim ms As New ModuleSettings(ModuleId)
            With ms
                .HideTitles = chkHideTitles.Checked
                .TabSkin = cboTabSkin.SelectedItem.Value
                .TabTemplate = cboTabTemplate.SelectedItem.Value
                .ShowPrevNext = chkShowPrevNext.Checked
                .HideSingleTab = chkHideSingleTab.Checked
                .ActiveHover = chkActiveHover.Checked
                Int32.TryParse(txtActiveHoverDelay.Text, .ActiveHoverDelay)
                .HideTabs = chkHideTabs.Checked
                Int32.TryParse(txtDefaultTab.Text, .DefaultTab)
                .RememberLastOpenTab = chkRememberLastOpenTab.Checked
                .Width = txtWidth.Text
                .Height = txtHeight.Text

                ' rss
                .RSSUrl = txtRSSUrl.Text
                Int32.TryParse(txtRSSMaxItems.Text, .RSSMaxItems)
                Int32.TryParse(txtRSSCacheTime.Text, .RSSCacheTime)
                .RSSTabCaption = txtRSSTabCaption.Text
                .RSSUsername = txtRSSUsername.Text
                .RSSPassword = txtRSSPassword.Text
            End With
            ms.UpdateSettings()

            Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
            objModules.UpdateTabModuleOrder(TabId)

            ' update properties
            cpvMain.SetProperties()
            For Each prop As CustomProperty In cpvMain.Properties.Values
                DataProvider.Instance().UpdateProperties(ModuleId, -1, cboTabSkin.SelectedValue, cboTabTemplate.SelectedValue, prop.Name, prop.Value)
            Next

            ' clear rss cache
            DataCache.RemoveCache("RSS_" & ModuleId.ToString)
        End Sub

        Private Sub LoadSettings()
            Dim ms As New ModuleSettings(ModuleId)
            Dim item As ListItem

            ' hide titles
            chkHideTitles.Checked = ms.HideTitles

            ' theme/skin
            BindSkinFolder(cboTabSkin)
            item = cboTabSkin.Items.FindByValue(ms.TabSkin)
            If Not item Is Nothing Then item.Selected = True

            ' tab style
            BindTemplateFolder(cboTabTemplate, cboTabSkin.SelectedItem.Text)
            item = cboTabTemplate.Items.FindByValue(ms.TabTemplate)
            If Not item Is Nothing Then item.Selected = True

            ' prev next
            chkShowPrevNext.Checked = ms.ShowPrevNext

            ' hide single tab
            chkHideSingleTab.Checked = ms.HideSingleTab

            ' active hover
            chkActiveHover.Checked = ms.ActiveHover
            txtActiveHoverDelay.Text = ms.ActiveHoverDelay.ToString

            ' hide tabs
            chkHideTabs.Checked = ms.HideTabs

            ' default tab
            txtDefaultTab.Text = ms.DefaultTab.ToString
            chkRememberLastOpenTab.Checked = ms.RememberLastOpenTab

            ' height/width
            txtHeight.Text = ms.Height
            txtWidth.Text = ms.Width

            ' rss
            txtRSSUrl.Text = ms.RSSUrl
            txtRSSMaxItems.Text = ms.RSSMaxItems.ToString
            txtRSSCacheTime.Text = ms.RSSCacheTime.ToString
            txtRSSTabCaption.Text = ms.RSSTabCaption
            txtRSSUsername.Text = ms.RSSUsername
            txtRSSPassword.Text = ms.RSSPassword

        End Sub

        Private Sub BindSkinFolder(ByVal o As ListControl)
            Dim skinFolder As New IO.DirectoryInfo(Server.MapPath(ResolveUrl("Skins")))
            o.Items.Clear()
            For Each folder As IO.DirectoryInfo In skinFolder.GetDirectories()
                If folder.GetDirectories.Length > 0 Then
                    o.Items.Add(folder.Name)
                End If
            Next
        End Sub

        Private Sub BindTemplateFolder(ByVal o As ListControl, ByVal skinName As String)
            Dim skinFolder As New IO.DirectoryInfo(IO.Path.Combine(Server.MapPath(ResolveUrl("Skins")), skinName))
            o.Items.Clear()
            For Each folder As IO.DirectoryInfo In skinFolder.GetDirectories()
                If Not folder.Name.StartsWith("_") Then o.Items.Add(folder.Name)
            Next
        End Sub

        Private Sub cboTabSkin_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboTabSkin.SelectedIndexChanged
            BindTemplateFolder(cboTabTemplate, cboTabSkin.SelectedItem.Text)
        End Sub

#End Region

#Region " Tabs"
        Private Sub BindTabGrid()

            Dim ctrl As New AggregatorController
            Dim drTabs As IDataReader = ctrl.ListAggregator(ModuleId)

            Try
                dlTabs.DataKeyField = "AggregatorTabId"
                dlTabs.DataSource = drTabs
                dlTabs.DataBind()

            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try

            ' hide unnecessary move buttons
            Dim ib As ImageButton
            For Each r As DataListItem In dlTabs.Items
                If r.ItemType = ListItemType.Item Or r.ItemType = ListItemType.AlternatingItem Then
                    If r.ItemIndex = 0 Then
                        ib = CType(r.FindControl("cmdMoveTabUp"), ImageButton)
                        If Not ib Is Nothing Then
                            ib.Visible = False
                        End If
                    End If
                    If r.ItemIndex = dlTabs.Items.Count - 1 Then
                        ib = CType(r.FindControl("cmdMoveTabDown"), ImageButton)
                        If Not ib Is Nothing Then
                            ib.Visible = False
                        End If
                    End If

                    ' hide move/up down buttons for modules
                    Dim moduleList As DataList
                    moduleList = CType(r.FindControl("dlTabModules"), DataList)
                    If moduleList IsNot Nothing Then
                        For Each m As DataListItem In moduleList.Items
                            If m.ItemType = ListItemType.Item Or m.ItemType = ListItemType.AlternatingItem Then
                                If m.ItemIndex = 0 Then
                                    ib = CType(m.FindControl("cmdMoveModuleUp"), ImageButton)
                                    If Not ib Is Nothing Then
                                        ib.Visible = False
                                    End If
                                End If
                                If m.ItemIndex = moduleList.Items.Count - 1 Then
                                    ib = CType(m.FindControl("cmdMoveModuleDown"), ImageButton)
                                    If Not ib Is Nothing Then
                                        ib.Visible = False
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            Next

        End Sub

        Private Sub cmdAddTab_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddTab.Click
            ' add a new tab
            Response.Redirect(NavigateEditTab(-1, 0))
        End Sub

        Private Sub cmdAddModule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddModule.Click
            ' add a new module
            Response.Redirect(EditUrl("EditModule"))
        End Sub

        Private Sub cmdQuickTabsAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdQuickTabsAdd.Click
            ' loop through lines of text and create a new empty tab for each one
            Dim objAggregatorController As AggregatorController = New AggregatorController
            Dim tabs As String() = System.Text.RegularExpressions.Regex.Split(txtQuickTabs.Text, "\r\n")
            For Each s As String In tabs
                Dim objAggregator As New AggregatorTabInfo
                With objAggregator
                    .ModuleId = ModuleId
                    .AggregatorTabId = -1
                    .Caption = s
                    .Locale = "All"
                    .HtmlText = s & " content"
                End With
                objAggregator.AggregatorTabId = objAggregatorController.UpdateAggregatorTab(objAggregator)
            Next
            txtQuickTabs.Text = ""

            ' refresh tabs
            LoadSettings()
            BindTabGrid()

        End Sub

        Private Sub cmdAddAllModules_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddAllModules.Click
            ' grab all remaining candidate modules and add them to new tabs

            Dim dr As IDataReader
            ' modules
            dr = DataProvider.Instance().GetTabModules(TabId, ModuleId, False, ModuleId, PortalId)
            While dr.Read
                Dim objAggregatorController As AggregatorController = New AggregatorController

                Dim objAggregatorModule As AggregatorModuleInfo = New AggregatorModuleInfo
                With objAggregatorModule
                    .AggregatorTabId = -1
                    .AggregatorModuleId = -1
                    .TabModuleId = Convert.ToInt32(dr("TabModuleId"))
                    .Locale = "All"
                    .InsertBreak = False
                End With

                Dim tabCaption As String = System.Text.RegularExpressions.Regex.Replace(dr("FullModuleTitle").ToString, ".*>", "")

                objAggregatorModule.AggregatorModuleId = objAggregatorController.UpdateAggregatorModule(objAggregatorModule, ModuleId, tabCaption)

            End While

            ' refresh tabs
            LoadSettings()
            BindTabGrid()

        End Sub

        Public Function LinkPath(ByVal ItemIndex As Integer) As String
            ' returns the url to select the tab
            Return NavigateURL(TabId, "", "Agg" & ModuleId & "_SelectTab=" & ItemIndex + 1)
        End Function

        Public Function NavigateEditTab(ByVal TabId As Integer, ByVal tabNumber As Integer) As String
            Return EditUrl("AggregatorTabId", TabId.ToString, "EditTab", "TabNumber=" & tabNumber)
        End Function

        Private Sub dlTabs_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlTabs.ItemCommand
            Select Case e.CommandName.ToLower
                Case "edit"
                    Response.Redirect(NavigateEditTab(Convert.ToInt32(dlTabs.DataKeys(e.Item.ItemIndex)), e.Item.ItemIndex + 1))
                Case "delete"
                    Dim ctrl As New AggregatorController
                    ctrl.DeleteAggregatorTab(Integer.Parse(Convert.ToString(dlTabs.DataKeys(e.Item.ItemIndex))))
                    BindTabGrid()
                Case "moveup"
                    Dim ctrl As New AggregatorController
                    ctrl.UpdateTabOrder(Integer.Parse(Convert.ToString(dlTabs.DataKeys(e.Item.ItemIndex))), -1)
                    BindTabGrid()
                Case "movedown"
                    Dim ctrl As New AggregatorController
                    ctrl.UpdateTabOrder(Integer.Parse(Convert.ToString(dlTabs.DataKeys(e.Item.ItemIndex))), 1)
                    BindTabGrid()
            End Select
        End Sub

        Private Sub dlTabs_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlTabs.ItemCreated
            Try
                Dim cmdDeleteTab As Control = e.Item.FindControl("cmdDeleteTab")

                If Not cmdDeleteTab Is Nothing Then
                    CType(cmdDeleteTab, ImageButton).Attributes.Add("onClick", "javascript: return confirm('Are You Sure You Wish To Delete This Tab ?')")
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub dlTabs_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlTabs.ItemDataBound

            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim dl As DataList = DirectCast(e.Item.FindControl("dlTabModules"), DataList)

                Dim id As Integer = Convert.ToInt32(DirectCast(e.Item.DataItem, IDataRecord)("AggregatorTabId"))

                Dim ctrl As New AggregatorController
                Dim dr As IDataReader = ctrl.ListAggregatorModule(Convert.ToInt32(id))

                dl.DataKeyField = "AggregatorModuleId"
                dl.DataSource = dr
                dl.DataBind()

                ' collapse row if no items
                If dl.Items.Count = 0 Then
                    Dim tr As HtmlTableRow = DirectCast(e.Item.FindControl("trTabModules"), HtmlTableRow)
                    tr.Visible = False
                End If
            End If
        End Sub

#End Region

#Region " Modules in Tabs"
        Public Sub dlTabModules_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs)
            Try
                If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                    ' add delete confirmation
                    Dim cmdDeleteTabModule As Control = e.Item.FindControl("cmdDeleteTabModule")
                    If Not cmdDeleteTabModule Is Nothing Then
                        CType(cmdDeleteTabModule, ImageButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure you wish to remove this module from the tab ?')")
                    End If

                    ' check module edit rights and enable editing if necessary
                    Dim ctrl As New AggregatorController
                    Dim ami As AggregatorModuleInfo = ctrl.GetAggregatorModule(Convert.ToInt32(Convert.ToString(DirectCast(sender, DataList).DataKeys(e.Item.ItemIndex))))
                    If ModulePermissionController.CanEditModuleContent(ami.ModuleInfo) Then
                        Dim cmdEditTabModuleSettings As Control = e.Item.FindControl("cmdEditTabModuleSettings")
                        If Not cmdEditTabModuleSettings Is Nothing Then
                            CType(cmdEditTabModuleSettings, ImageButton).Visible = True
                        End If
                    End If
                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Public Sub dlTabModules_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs)
            Dim moduleList As DataList = DirectCast(sender, DataList)

            Select Case e.CommandName.ToLower
                Case "edit"
                    Response.Redirect(EditUrl("AggregatorModuleId", DirectCast(sender, DataList).DataKeys(e.Item.ItemIndex).ToString, "EditModule"))
                Case "editsettings"
                    Dim ReturnUrl As String = HttpContext.Current.Request.RawUrl
                    If ReturnUrl.IndexOf("?returnurl=") <> -1 Then
                        ReturnUrl = ReturnUrl.Substring(0, ReturnUrl.IndexOf("?returnurl="))
                    End If
                    ReturnUrl = HttpUtility.UrlEncode(ReturnUrl)

                    Dim ctrl As New AggregatorController
                    Dim ami As AggregatorModuleInfo = ctrl.GetAggregatorModule(Convert.ToInt32(Convert.ToString(DirectCast(sender, DataList).DataKeys(e.Item.ItemIndex))))
                    Response.Redirect(NavigateURL(TabId, "Module", "ModuleId=" & ami.ModuleId.ToString, "returnurl=" & ReturnUrl))
                Case "delete"
                    Dim ctrl As New AggregatorController
                    ctrl.DeleteAggregatorModule(Convert.ToInt32(Convert.ToString(DirectCast(sender, DataList).DataKeys(e.Item.ItemIndex))))
                    BindTabGrid()
                Case "moveup"
                    Dim ctrl As New AggregatorController
                    ctrl.UpdateModuleOrder(Integer.Parse(Convert.ToString(moduleList.DataKeys(e.Item.ItemIndex))), -1)
                    BindTabGrid()
                Case "movedown"
                    Dim ctrl As New AggregatorController
                    ctrl.UpdateModuleOrder(Integer.Parse(Convert.ToString(moduleList.DataKeys(e.Item.ItemIndex))), 1)
                    BindTabGrid()
            End Select
        End Sub

#End Region

#Region " Helpers"
        Public Function MaxTrim(ByVal s As String, ByVal f As String, ByVal l As Integer) As String
            If s.Length > 0 Then
                If s.Length > l Then Return String.Format(f, s.Substring(0, l)) Else Return String.Format(f, s)
            End If
            Return ""
        End Function
#End Region

#Region " Custom Settings"

        Public Function SettingsFilename() As String
            Dim skinFolder As String = ResolveUrl(String.Format("Skins/{0}/{1}", cboTabSkin.SelectedValue, cboTabTemplate.SelectedValue))
            Return IO.Path.Combine(MapPath(skinFolder), "settings.xml")
        End Function

        Public Sub LoadAggregatorCustomProperties(ByVal cpv As CustomPropertiesViewer)
            If IO.File.Exists(SettingsFilename) Then
                Dim cp As CustomProperties = CustomProperties.Load(SettingsFilename)
                ' load values from db
                Dim values As ArrayList = CBO.FillCollection(DataProvider.Instance().GetProperties(ModuleId, -1, cboTabSkin.SelectedValue, cboTabTemplate.SelectedValue), GetType(CustomSettingsInfo))
                For Each csi As CustomSettingsInfo In values
                    If cp.Properties.ContainsKey(csi.Name) Then
                        cp.Properties(csi.Name).Value = csi.Value
                    End If
                Next
                cpv.Description = cp.Description
                cpv.Properties = cp.Properties

                lblCustomPropertyHelp.Text = cp.Help
                Exit Sub
            End If
            cpv.Description = ""
            cpv.Properties = New Dictionary(Of String, CustomProperty)

            lblCustomPropertyHelp.Text = ""
        End Sub

#End Region

    End Class


End Namespace