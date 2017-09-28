Imports DotNetNuke
Imports System.Configuration
Imports System.IO
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Common.Utilities

Namespace DNNStuff.Aggregator

    Partial Class EditModule
        Inherits Entities.Modules.PortalModuleBase


#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            ' initialize
            If Not Request.QueryString("AggregatorModuleId") Is Nothing Then
                AggregatorModuleId = Int32.Parse(Request.QueryString("AggregatorModuleId").ToString)
            Else
                AggregatorModuleId = -1
            End If

            If Not Request.QueryString("AggregatorTabId") Is Nothing Then
                AggregatorTabId = Int32.Parse(Request.QueryString("AggregatorTabId").ToString)
            Else
                AggregatorTabId = -1
            End If

            InitAggregatorModule()

        End Sub

#End Region

#Region " Aggregator Module"

        Private _AggregatorTabId As Integer
        Public Property AggregatorTabId() As Integer
            Get
                Return _AggregatorTabId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorTabId = Value
            End Set
        End Property

        Private _AggregatorModuleId As Integer
        Public Property AggregatorModuleId() As Integer
            Get
                Return _AggregatorModuleId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorModuleId = Value
            End Set
        End Property

        Private _AggregatorModule As AggregatorModuleInfo
        Public Property AggregatorModule() As AggregatorModuleInfo
            Get
                Return _AggregatorModule
            End Get
            Set(ByVal Value As AggregatorModuleInfo)
                _AggregatorModule = Value
            End Set
        End Property

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit.css"))
            Page.ClientScript.RegisterClientScriptInclude(Me.GetType, "yeti", ResolveUrl("resources/support/yetii-min.js"))

            If Not Page.IsPostBack Then
                ' drop down report type
                BindAggregatorModule()

            End If

        End Sub

#End Region

#Region " Data"
        Private Sub InitAggregatorModule()
            Dim objAggregatorController As AggregatorController = New AggregatorController
            Dim objAggregatorModule As AggregatorModuleInfo = objAggregatorController.GetAggregatorModule(AggregatorModuleId)
            If objAggregatorModule Is Nothing Then
                objAggregatorModule = New AggregatorModuleInfo
                With objAggregatorModule
                    .AggregatorTabId = AggregatorTabId
                    .AggregatorModuleId = -1
                End With
            End If
            ' load from database
            AggregatorModule = objAggregatorModule
        End Sub

        Private Sub BindAggregatorModule()
            If Not AggregatorModule Is Nothing Then
                With AggregatorModule
                    BindModuleList(cboTabModule, False, AggregatorModuleId)
                    BindLocaleList(cboLocale, .Locale)
                    BindTabList(cboTab)
                    BindLoadEventList(cboLoadEvent, .LoadEvent)
                    chkInsertBreak.Checked = .InsertBreak
                End With
            End If
        End Sub

        Private Sub SaveAggregatorModule()
            With AggregatorModule
                .TabModuleId = CType(cboTabModule.SelectedItem.Value, Integer)
                .Locale = cboLocale.SelectedItem.Value
                .InsertBreak = chkInsertBreak.Checked
                .AggregatorTabId = CType(cboTab.SelectedItem.Value, Integer)
                .LoadEvent = CType(cboLoadEvent.SelectedItem.Value, LoadEventType)
            End With

            ' determine appropriate tab caption if new tab
            Dim TabCaption As String = "New Tab"
            If AggregatorModule.AggregatorTabId = -1 Then
                If cboTabModule.SelectedItem.Text.Contains(">") Then
                    TabCaption = System.Text.RegularExpressions.Regex.Replace(cboTabModule.SelectedItem.Text, ".*>", "")
                End If
            End If

            Dim objAggregatorController As AggregatorController = New AggregatorController
            AggregatorModule.AggregatorModuleId = objAggregatorController.UpdateAggregatorModule(AggregatorModule, ModuleId, TabCaption)
        End Sub

        Private Sub BindLocaleList(ByVal o As DropDownList, ByVal selectedCulture As String)
            Localization.LoadCultureDropDownList(o, CultureDropDownTypes.DisplayName, selectedCulture)
            With o
                .Items.Insert(0, New ListItem(Localization.GetString("Locales.Fallback", LocalResourceFile), "Fallback"))
                .Items.Insert(0, New ListItem(Localization.GetString("Locales.All", LocalResourceFile), "All"))
                .ClearSelection()
            End With
            Dim item As ListItem = o.Items.FindByValue(selectedCulture)
            If Not item Is Nothing Then item.Selected = True

        End Sub

        Private Sub BindLoadEventList(ByVal o As DropDownList, ByVal selectedLoadEvent As Integer)

            DNNUtilities.EnumToListBox(GetType(LoadEventType), o)
            Dim item As ListItem = o.Items.FindByValue(CStr(selectedLoadEvent))
            If Not item Is Nothing Then item.Selected = True

        End Sub

        Private Sub BindModuleList(ByVal o As DropDownList, ByVal showAllModules As Boolean, ByVal AggregatorModuleId As Integer)
            Dim dr As IDataReader

            ' modules
            dr = DataProvider.Instance().GetTabModules(TabId, ModuleId, showAllModules, AggregatorModuleId, PortalId)
            With o
                .DataSource = dr
                .DataValueField = "TabModuleId"
                .DataTextField = "FullModuleTitle"
                .DataBind()
            End With

            o.ClearSelection()
            Dim item As ListItem = o.Items.FindByValue(CStr(AggregatorModule.TabModuleId))
            If Not item Is Nothing Then item.Selected = True

        End Sub

        Private Sub BindTabList(ByVal o As DropDownList)
            Dim dr As IDataReader

            ' modules
            dr = DataProvider.Instance().ListAggregator(ModuleId)
            With o
                .DataSource = dr
                .DataValueField = "AggregatorTabId"
                .DataTextField = "Caption"
                .DataBind()
                .Items.Insert(0, New ListItem("New Tab", "-1"))
            End With

            o.ClearSelection()
            Dim item As ListItem = o.Items.FindByValue(CStr(AggregatorModule.AggregatorTabId))
            If Not item Is Nothing Then item.Selected = True

        End Sub


#End Region

#Region " Navigation"
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            If Page.IsValid Then
                SaveAggregatorModule()
                Entities.Modules.ModuleController.SynchronizeModule(ModuleId)
                ' Redirect back to the Aggregator
                Response.Redirect(NavigateAggregator())
            End If

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateAggregator())
        End Sub

        Private Function NavigateAggregator() As String
            Return EditUrl("Edit")
        End Function
#End Region

#Region " Validation"
#End Region

        Private Sub chkShowAllModules_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkShowAllModules.CheckedChanged
            BindModuleList(cboTabModule, chkShowAllModules.Checked, AggregatorModuleId)
        End Sub
    End Class

End Namespace
