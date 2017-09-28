Imports DotNetNuke
Imports System.Configuration
Imports System.IO
Imports Dotnetnuke.Services.Localization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common
Imports System.Collections.Generic

Namespace DNNStuff.Aggregator

    Partial Class EditTab
        Inherits Entities.Modules.PortalModuleBase

        Private _ms As ModuleSettings

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            DNNUtilities.InjectCSS(Me.Page, ResolveUrl("Resources/Support/edit.css"))
            Page.ClientScript.RegisterClientScriptInclude(Me.GetType, "yeti", ResolveUrl("resources/support/yetii-min.js"))

            ' initialize
            If Not Request.QueryString("AggregatorTabId") Is Nothing Then
                AggregatorTabId = Int32.Parse(Request.QueryString("AggregatorTabId").ToString)
            Else
                AggregatorTabId = -1
            End If
            If Not Request.QueryString("TabNumber") Is Nothing Then
                TabNumber = Int32.Parse(Request.QueryString("TabNumber").ToString)
            Else
                TabNumber = 0
            End If

            InitAggregator()

            _ms = New ModuleSettings(ModuleId)
        End Sub

#End Region

#Region " Aggregator"
        Private _TabNumber As Integer
        Public Property TabNumber() As Integer
            Get
                Return _TabNumber
            End Get
            Set(ByVal value As Integer)
                _TabNumber = value
            End Set
        End Property
        Private _AggregatorTabId As Integer
        Public Property AggregatorTabId() As Integer
            Get
                Return _AggregatorTabId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorTabId = Value
            End Set
        End Property
        Private _Aggregator As AggregatorTabInfo
        Public Property Aggregator() As AggregatorTabInfo
            Get
                Return _Aggregator
            End Get
            Set(ByVal Value As AggregatorTabInfo)
                _Aggregator = Value
            End Set
        End Property

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Not Page.IsPostBack Then
                ' drop down report type
                BindAggregator()
                DisplayHelp()
            End If

            ' Load custom settings (if any)
            cpvMain.Properties = AggregatorCustomProperties()
        End Sub

#End Region

#Region " Data"
        Private Sub InitAggregator()
            Dim objAggregatorController As AggregatorController = New AggregatorController
            Dim objAggregator As AggregatorTabInfo = objAggregatorController.GetAggregatorTab(AggregatorTabId)
            If objAggregator Is Nothing Then
                objAggregator = New AggregatorTabInfo
                With objAggregator
                    .ModuleId = ModuleId
                    .AggregatorTabId = -1
                End With
            End If
            ' load from database
            Aggregator = objAggregator
        End Sub

        Private Sub BindAggregator()
            If Not Aggregator Is Nothing Then
                With Aggregator
                    txtCaption.Text = .caption
                    BindLocaleList(cboLocale, .Locale)
                    txtContent.Text = .HtmlText
                    chkPostback.Checked = .Postback
                End With
            End If
        End Sub

        Private Sub SaveAggregator()
            With Aggregator
                .Caption = txtCaption.Text
                .Locale = cboLocale.SelectedItem.Value
                .HtmlText = txtContent.Text
                .Postback = chkPostback.Checked
            End With
            Dim objAggregatorController As AggregatorController = New AggregatorController
            Aggregator.AggregatorTabId = objAggregatorController.UpdateAggregatorTab(Aggregator)

            ' update properties
            cpvMain.SetProperties()
            For Each prop As CustomProperty In cpvMain.Properties.Values
                DataProvider.Instance().UpdateProperties(ModuleId, Aggregator.AggregatorTabId, _ms.TabSkin, _ms.TabTemplate, prop.Name, prop.Value)
            Next
        End Sub

        Private Sub BindLocaleList(ByVal o As DropDownList, ByVal selectedCulture As String)
            Localization.LoadCultureDropDownList(o, CultureDropDownTypes.DisplayName, selectedCulture)
            With o
                .Items.Insert(0, New ListItem(Localization.GetString("Locales.Fallback", LocalResourceFile), "Fallback"))
                .Items.Insert(0, New ListItem(Localization.GetString("Locales.All", LocalResourceFile), "All"))
            End With
            o.ClearSelection()
            Dim item As ListItem = o.Items.FindByValue(selectedCulture)
            If Not item Is Nothing Then item.Selected = True

        End Sub
#End Region

#Region " Navigation"
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            If Page.IsValid Then
                SaveAggregator()
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

#Region " Linking Help"
        Private Sub DisplayHelp()
            If TabNumber = 0 Then Return

            Dim linkToTab As String = String.Format("<a href=""{0}"" >Link To This Tab</a>", NavigateURL(TabId, "", "Agg" & Aggregator.ModuleId & "_SelectTab=" & TabNumber))
            Dim clientLinkToTab As String = String.Format("<a href='javascript:void(0)' onClick='javascript:Agg{0}_SelectTab({1},{0});'>Link To This Tab</a>", Aggregator.ModuleId, TabNumber)

            divHelp.Controls.Add(New LiteralControl("<strong>Code to create a link to the tab from another page</strong><br />"))
            divHelp.Controls.Add(New LiteralControl("<textarea cols=100 rows=4>" & linkToTab & "</textarea><br />"))
            divHelp.Controls.Add(New LiteralControl("<strong>Code to create a link to the tab from the same page</strong><br />"))
            divHelp.Controls.Add(New LiteralControl("<textarea cols=100 rows=1>" & clientLinkToTab & "</textarea>"))

        End Sub
#End Region

#Region " Custom Settings"

        Public Function SettingsFilename() As String
            Dim skinFolder As String = ResolveUrl(String.Format("Skins/{0}/{1}", _ms.TabSkin, _ms.TabTemplate))
            Return IO.Path.Combine(MapPath(skinFolder), "settings.xml")
        End Function

        Public Function AggregatorCustomProperties() As Dictionary(Of String, CustomProperty)
            If IO.File.Exists(SettingsFilename) Then
                Dim cp As CustomProperties = CustomProperties.Load(SettingsFilename)
                ' load values from db
                Dim values As ArrayList = CBO.FillCollection(DataProvider.Instance().GetProperties(ModuleId, AggregatorTabId, _ms.TabSkin, _ms.TabTemplate), GetType(CustomSettingsInfo))
                For Each csi As CustomSettingsInfo In values
                    If cp.TabProperties.ContainsKey(csi.Name) Then
                        cp.TabProperties(csi.Name).Value = csi.Value
                    End If
                Next
                Return cp.TabProperties
            End If
            Return New Dictionary(Of String, CustomProperty)
        End Function

#End Region
    End Class

End Namespace
