'***************************************************************************/
'* Aggregator.ascx.vb
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

Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Net

Imports DotNetNuke
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions

Imports RssToolkit.Rss

Namespace DNNStuff.Aggregator

    Partial Class Aggregator
        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.Communications.IModuleListener
        Implements Entities.Modules.IActionable

        ' currently selected tab
        Private _selectedTabNumber As Integer = 1

        ' object graph for tabs and contained modules
        Private _aggregator As AggregatorInfo = New AggregatorInfo

        ' module settings
        Private _ms As ModuleSettings

        ' token settings
        Private _aggregatorTokens As Hashtable

#Region " Properties"
        Private ReadOnly Property SettingsFilename() As String
            Get
                Return IO.Path.Combine(MapPath(SkinFolder), "settings.xml")
            End Get
        End Property

        Private ReadOnly Property SkinFilename() As String
            Get
                Return ResolveUrl(SkinFolder & "/styles.css")
            End Get
        End Property

        Private ReadOnly Property SkinFolder() As String
            Get
                Return ResolveUrl(String.Format("Skins/{0}/{1}", _ms.TabSkin, _ms.TabTemplate))
            End Get
        End Property

        Private ReadOnly Property ResourceFolder() As String
            Get
                Return ResolveUrl(String.Format("Resources"))
            End Get
        End Property

        Private ReadOnly Property SkinBaseFolder() As String
            Get
                Return ResolveUrl(String.Format("Skins/{0}", _ms.TabSkin))
            End Get
        End Property

        Private ReadOnly Property Unique(ByVal s As String, Optional ByVal scope As String = "module") As String
            ' make string unique for this module instance
            Get
                Select Case scope.ToLower
                    Case "template"
                        Return "Agg" & _ms.TabSkin & "_" & _ms.TabTemplate & "_" & s
                    Case "skin"
                        Return "Agg" & _ms.TabSkin & "_" & s
                    Case "resource"
                        Return "Agg" & "_" & s
                    Case Else
                        Return "Agg" & Me.ModuleId.ToString & "_" & s
                End Select
            End Get
        End Property

#End Region

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        'NOTE: The following placeholder declaration is required by the Web Form Designer.
        'Do not delete or move it.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            ' settings
            _ms = New ModuleSettings(ModuleId)

            ' inject css
            DNNUtilities.InjectCSS(Page, SkinFilename)

            ' inject js libraries appearing on every page
            Page.ClientScript.RegisterClientScriptInclude("dnnstuff", ResolveUrl("Resources/Support/dnnstuff-min.js"))

            ' render tabs
            RenderTabs()

            ' inject partial rendering modules here so they work
            ' module menus are still slighly affected but are still usuable
            InjectIntoTabsPartialRendering()
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ' register the client api
            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(Me.Page, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn)

            ' load jQuery and UI
            DotNetNuke.Framework.jQuery.RequestRegistration()
            DotNetNuke.Framework.jQuery.RequestUIRegistration()

            ' inject non partial rendering modules here so that databinding works properly in the injected modules
            ' and the menus act normal
            InjectIntoTabsNonPartialRendering()

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ' inject inline javascript - moved here to ensure it comes after the DNN5 jquery include
            If Not (_aggregator.SingleTab And _ms.HideSingleTab) Then
                Dim t As New Template(_ms, MapPath(SkinFolder), MapPath(ResourceFolder()))
                InjectScript(t, "script", "body", "template", True)
                InjectScript(t, "head", "head", "resource", False)
                InjectScript(t, "head", "head", "skin", False)
                InjectScript(t, "head", "head", "template", False)
            End If
        End Sub
#End Region

#Region " Optional Interfaces"

        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
                Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.ContentOptions, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "", EditUrl(), False, SecurityAccessLevel.Edit, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("CopyAggregator", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ExportModule, "", "", EditUrl("CopyAggregator"), False, SecurityAccessLevel.Admin, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("ManageSkin", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ImportModule, "", "", EditUrl("ManageSkin"), False, SecurityAccessLevel.Admin, True, False)
                Actions.Add(GetNextActionID, Localization.GetString("UploadSkin", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ImportModule, "", "", EditUrl("UploadSkin"), False, SecurityAccessLevel.Host, True, False)
                Return Actions
            End Get
        End Property

#End Region

#Region " Main Rendering"

        Private Sub LoadAggregator()
            ' Load ModuleInfo objects for all tabs we will be Aggregating
            Dim ctrl As New AggregatorController
            _aggregator = ctrl.GetAggregatorObjectGraph(ModuleId, _ms.TabSkin, _ms.TabTemplate)

            ' process tabs hidden by querystring
            RemoveQueryStringModules()

            ' process locale settings
            RemoveLocaleSpecificModules()
            RemoveLocaleSpecificTabs()

            ' remove additional modules based on security
            RemoveInvisibleModules()

            ' update captions
            UpdateTabCaptions()

            ' remove tabs if no modules inside, or no text to show
            RemoveEmptyTabs()

            ' add tabs based on rss
            AddRSSTabs()

            ' add demo tab
#If Config = "Trial" Then
            AddDemoTab()
#End If
            ' get selected module
            _selectedTabNumber = GetSelectedTab()
        End Sub

        Private Sub AddRSSTabs()

            If _ms.RSSUrl.Length = 0 Then Exit Sub

            ' grab feed
            Dim feed As RssToolkit.Rss.RssDocument = DownloadFeed()
            If feed Is Nothing Then Exit Sub

            ' determine max items
            Dim maxItems As Integer = _ms.RSSMaxItems
            If maxItems = 0 Then maxItems = feed.Channel.Items.Count

            ' template
            Dim t As New Template(_ms, MapPath(SkinFolder), MapPath(ResourceFolder))

            Dim item As RssItem
            For itemIndex As Integer = 0 To Math.Min(maxItems - 1, feed.Channel.Items.Count - 1)
                item = feed.Channel.Items(itemIndex)

                Dim ati As New AggregatorTabInfo
                With ati
                    .Caption = GetRSSTabCaptionTemplate(_ms.RSSTabCaption, item)
                    .HtmlText = GetRSSContentTemplate(t, item)
                    .ModuleId = Me.ModuleId
                    .AggregatorTabId = -1
                    .Modules = New ArrayList
                    .Properties = New ArrayList
                    .RSS = RSSItemSettings(item)
                End With
                _aggregator.Tabs.Add(ati)
            Next

        End Sub

        Private Sub AddDemoTab()

            Dim sb As New StringBuilder
            sb.Append("<p>Thank you for evaluating the #1 tab control for DotNetNuke.</p>")
            sb.Append("<p>This evaluation version is fully functional in every way. The only difference from the licensed version of Aggregator is the addition of this informational tab.</p>")
            sb.Append("<p>If after your evaluation you wish to support great DotNetNuke software, please visit <a style=""text-decoration:underline"" target=""_blank"" title=""DNNStuff"" href=""http://www.dnnstuff.com?utm_source=dnnstuff&utm_medium=demo&utm_campaign=aggregator"">DNNStuff</a> to purchase a licensed version. Use discount code <strong>'TRIAL'</strong> at checkout for 10% off!</p>")
            sb.Append("<p>Here are a few additional resources for you to consider:</p>")
            sb.Append("<ul><li><a href=""http://www.dnnstuff.com/Modules/AggregatorTabbedModules/AggregatorSkins/tabid/304/Default.aspx?utm_source=dnnstuff&utm_medium=demo&utm_campaign=aggregator"">Aggregator Skins</a></li>")
            sb.Append("<li><a href=""http://www.dnnstuff.com/Modules/AggregatorTabbedModules/AggregatorFAQ/tabid/293/Default.aspx?utm_source=dnnstuff&utm_medium=demo&utm_campaign=aggregator"">Aggregator FAQ</a></li>")
            sb.Append("<li><a href=""http://www.dnnstuff.com/Modules/AggregatorTabbedModules/AggregatorDemos/tabid/322/Default.aspx?utm_source=dnnstuff&utm_medium=demo&utm_campaign=aggregator"">Aggregator Demos</a></li>")
            sb.Append("<li><a href=""http://www.dnnstuff.com/Modules/AggregatorTabbedModules/AggregatorTestimonials/tabid/334/Default.aspx?utm_source=dnnstuff&utm_medium=demo&utm_campaign=aggregator"">Aggregator Testimonials</a></li>")
            sb.Append("<li><a href=""http://wiki.dnnstuff.com/Aggregator.ashx?utm_source=dnnstuff&utm_medium=demo&utm_campaign=aggregator"">Aggregator Documentation</a></li>")
            sb.Append("</ul>")

            Dim ati As New AggregatorTabInfo
            With ati
                .Caption = "Unlicensed Version"
                .HtmlText = sb.ToString
                .ModuleId = Me.ModuleId
                .AggregatorTabId = -1
                .Modules = New ArrayList
                .Properties = New ArrayList
            End With
            'alternate between first and last tab
            If Now.Minute Mod 2 = 0 Then
                _aggregator.Tabs.Insert(0, ati)
            Else
                _aggregator.Tabs.Add(ati)
            End If

        End Sub

        Private Sub UpdateTabCaptions()
            ' process tabs
            Dim tabNumber As Integer = 1
            For Each ai As AggregatorTabInfo In _aggregator.Tabs
                ' update caption
                If ai.Caption.Contains("[") And ai.Caption.Contains("]") Then
                    ai.Caption = ReplaceAggregatorTabInfoTokens(ai.Caption, ai, tabNumber)
                    tabNumber += 1
                End If
            Next
        End Sub

        Private Sub RemoveInvisibleModules()
            Dim includeModule As Boolean

            ' process tabs
            For Each ai As AggregatorTabInfo In _aggregator.Tabs

                For Each ami As AggregatorModuleInfo In New ArrayList(ai.Modules)

                    includeModule = False

                    If Not ami.ModuleInfo Is Nothing Then
                        includeModule = Compatibility.IncludeModule(ami.ModuleInfo, PortalSettings)
                    End If

                    ' update or remove depending on outcome
                    If Not includeModule Then
                        ' remove it
                        ai.Modules.Remove(ami)
                    End If
                Next
            Next

        End Sub

        Private Sub RemoveLocaleSpecificTabs()
            ' remove any tabs from data that don't match the current locale
            Dim currentLocale As String = System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
            Dim fallbackCount As Integer = 0
            ' first pass, remove anything not matching current locale exactly
            For Each ai As AggregatorTabInfo In New ArrayList(_aggregator.Tabs)
                ' check for locale
                If ai.Locale <> "All" And ai.Locale <> "Fallback" Then
                    If ai.Locale.ToLower <> currentLocale Then
                        _aggregator.Tabs.Remove(ai)
                    End If
                End If
                If ai.Locale = "Fallback" Then fallbackCount += 1
            Next

            ' second pass, remove the fallback locale only if another tab is present that isn't a fallback also
            If _aggregator.Tabs.Count > fallbackCount Then
                For Each ai As AggregatorTabInfo In New ArrayList(_aggregator.Tabs)
                    ' check for locale
                    If ai.Locale = "Fallback" Then
                        If _aggregator.Tabs.Count > 1 Then
                            _aggregator.Tabs.Remove(ai)
                        End If
                    End If
                Next
            End If

        End Sub

        Private Sub RemoveEmptyTabs()
            ' remove any empty tabs

            Dim currentLocale As String = System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
            For Each ai As AggregatorTabInfo In New ArrayList(_aggregator.Tabs)
                ' remove if no modules inside, or no text to show
                If ai.Modules.Count = 0 And ai.HtmlText.Length = 0 Then
                    _aggregator.Tabs.Remove(ai)
                End If
            Next
        End Sub

        Private Sub RemoveLocaleSpecificModules()
            ' remove any modules from data that don't match the current locale

            Dim currentLocale As String = System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
            For Each ai As AggregatorTabInfo In New ArrayList(_aggregator.Tabs)
                For Each ami As AggregatorModuleInfo In New ArrayList(ai.Modules)
                    If ami.Locale <> "All" And ami.Locale <> "Fallback" Then
                        If ami.Locale.ToLower <> currentLocale Then
                            ai.Modules.Remove(ami)
                        End If
                    End If
                Next
            Next
            ' second pass, remove the default locale only if another module is present
            For Each ai As AggregatorTabInfo In New ArrayList(_aggregator.Tabs)
                For Each ami As AggregatorModuleInfo In New ArrayList(ai.Modules)
                    If ami.Locale = "Fallback" Then
                        If ai.Modules.Count > 1 Then
                            ai.Modules.Remove(ami)
                        End If
                    End If
                Next
            Next
        End Sub

        Private Sub RenderTabs()
            ' hide any aggregated modules on this page before DNN shows them
            HidePageLevelAggregations()

            LoadAggregator()

            If _aggregator.TabsToShow Then

                Dim t As New Template(_ms, MapPath(SkinFolder), MapPath(ResourceFolder()))

                ' render
                RenderLayout(t)

                HideAggregatedModules()
            End If
        End Sub

        Private Sub HideAggregatedModules()
            ' setup to hide modules so that DNN doesn't render them outside our tabs .. we'll render them later in page load
            For Each ai As AggregatorTabInfo In _aggregator.Tabs

                For Each ami As AggregatorModuleInfo In ai.Modules

                    If _ms.HideTitles Then
                        ami.ModuleInfo.ContainerSrc = ResolveUrl("no container.ascx")
                    End If
                    ami.ModuleInfo.IsDeleted = True

                Next
            Next

        End Sub

        Private Sub InjectIntoTabsPartialRendering()
            InjectIntoTabs(Me, LoadEventType.PageInit, _aggregator, _selectedTabNumber)
        End Sub

        Private Sub InjectIntoTabsNonPartialRendering()
            InjectIntoTabs(Me, LoadEventType.PageLoad, _aggregator, _selectedTabNumber)
        End Sub

        Private Sub HidePageLevelAggregations()
            ' hide any modules that will eventually be aggregated on this page, !necessary for nesting

            Dim dr As IDataReader = Nothing

            Try
                Dim ctrl As New AggregatorController
                dr = ctrl.GetPageModules(TabId)

                Dim innerModuleId As Integer
                While dr.Read
                    innerModuleId = Convert.ToInt32(dr("ModuleId"))
                    For Each mi As ModuleInfo In PortalSettings.ActiveTab.Modules

                        If mi.ModuleID = innerModuleId Then
                            ' it's aggregated somewhere, hide before DNN gets to show it
                            mi.IsDeleted = True
                        End If
                    Next
                End While

            Catch ex As Exception
            Finally
                If dr IsNot Nothing Then dr.Close()
            End Try


        End Sub

        Private Function FindControlRecursive(ByVal root As Control, ByVal id As String) As Control
            ' 10/Feb/2006 - Currently not used in the injection routine but left here in case it becomes necessary to use it at a later date
            If root.ID = id Then
                Return root
            End If
            For Each c As Control In root.Controls
                Dim t As Control = FindControlRecursive(c, id)
                If Not (t Is Nothing) Then
                    Return t
                End If
            Next
            Return Nothing
        End Function

#End Region

#Region " Template Rendering"
        Private Sub RenderLayout(ByVal t As Template)

            Dim tabStrip As String = ""
            Dim tabNumber As Integer = 1

            If Not (_aggregator.SingleTab And _ms.HideSingleTab) Then

                ' render tabs
                tabNumber = 1
                Dim tabs As New StringBuilder
                For Each ai As AggregatorTabInfo In _aggregator.Tabs
                    tabs.Append(GetTabTemplate(t, ai, tabNumber))
                    tabNumber += 1
                Next

                ' render tabstrip
                tabStrip = t.TabStripTemplate _
                    .Replace("[TABS]", tabs.ToString)

            End If

            ' tab pages - only if not already rendered within tab template
            Dim tabPages As New StringBuilder
            If Not t.TabTemplate.Contains("[TABPAGE]") Then
                ' render tab pages
                tabNumber = 1
                For Each ai As AggregatorTabInfo In _aggregator.Tabs
                    tabPages.Append(GetTabPageTemplate(t, ai, tabNumber))
                    tabNumber += 1
                Next
            End If

            ' make replacements in layout template
            Dim LayoutTemplate As String = t.LayoutTemplate _
                            .Replace("[TABSTRIP]", tabStrip) _
                            .Replace("[TABPAGES]", tabPages.ToString)

            ' paging
            Dim pagingTemplate As String = ""
            If Not (_aggregator.SingleTab And _ms.HideSingleTab) Then
                If _ms.ShowPrevNext Then
                    ' build paging controls
                    pagingTemplate = GetPagingTemplate(t, Nothing, 0)
                    If Not t.PagingInTemplates Then
                        ' add to layouttemplate
                        LayoutTemplate = LayoutTemplate & "[PAGING]"
                    End If
                End If
            End If

            ' make one last replacements for paging - either replaces with controls or clears if paging is not activated in settings
            ' and any other global replacements
            LayoutTemplate = LayoutTemplate _
                    .Replace("[PAGING]", pagingTemplate)

            ' aggregator tokens
            LayoutTemplate = ReplaceAggregatorInfoTokens(LayoutTemplate)


            ' parse and add to page
            Controls.Add(ParseControl(LayoutTemplate))

        End Sub

        Private Function GetRSSContentTemplate(ByVal t As Template, ByVal item As RssItem) As String
            ' returns all of the code that makes up an rss item
            Return ReplaceRSSItemTokens(t.RSSContentTemplate, item)
        End Function

        Private Function GetRSSTabCaptionTemplate(ByVal s As String, ByVal item As RssItem) As String
            ' returns all of the code that makes up an rss tab caption
            Return ReplaceRSSItemTokens(s, item)
        End Function

        Private Function GetTabTemplate(ByVal t As Template, ByVal ati As AggregatorTabInfo, ByVal tabNumber As Integer) As String
            ' returns all of the code that makes up a tab

            ' events
            Dim events As New StringBuilder
            If _ms.ActiveHover Then
                events.Append("onmouseover=""javascript:setTimeout('" & Unique("SelectTab") & "([TABNUMBER],[MODULEID])'," & _ms.ActiveHoverDelay.ToString & ");"" ")
            Else
                events.Append("onclick=""javascript:" & Unique("SelectTab") & "([TABNUMBER],[MODULEID]);"" ")
                events.Append("onmouseover=""javascript:" & Unique("MouseOverTab") & "(this);"" ")
                events.Append("onmouseout=""javascript:" & Unique("MouseOutTab") & "(this);"" ")
            End If

            ' tab
            Dim tab As String = t.TabTemplate _
                .Replace("[TABACTION]", events.ToString)

            ' check for tabpage token, used for inline tabs
            If tab.Contains("[TABPAGE]") Then
                tab = tab.Replace("[TABPAGE]", GetTabPageTemplate(t, ati, tabNumber))
            End If

            ' tab tokens
            If ati IsNot Nothing Then
                tab = ReplaceAggregatorTabInfoTokens(tab, ati, tabNumber)
            End If

            Return tab
        End Function

        Private Function GetTabPageTemplate(ByVal t As Template, ByVal ati As AggregatorTabInfo, ByVal tabNumber As Integer) As String

            ' build tab page content 
            Dim tabPage As New StringBuilder
            Dim wrappedModuleCount As Integer = 0

            ' see if there is html text first and add that
            If ati.HtmlText.Length > 0 Then
                Dim text As String = ati.HtmlText
                For Each ami As AggregatorModuleInfo In ati.Modules
                    Dim token As String = "[MOD" & ami.ModuleId & "]"
                    If ati.HtmlText.Contains(token) Then
                        ami.ModuleInfo.PaneName = "wrap" & tabNumber.ToString & "_" & ami.AggregatorModuleId.ToString
                        text = text.Replace(token, String.Format("<div runat=""server"" id=""{0}"" valign=""top""></div>", ami.ModuleInfo.PaneName))
                        wrappedModuleCount += 1
                    End If
                Next
                ' remove any remaining [MODxxxx] tokens for modules the user doesn't have rights to that were skipped
                text = RegularExpressions.Regex.Replace(text, "\[MOD\d+\]", "")
                ' append
                tabPage.Append(text)
            End If

            If wrappedModuleCount < ati.Modules.Count Then
                ' not all modules are using wrapping, we need to add injection spots
                If (ati.Modules.Count - wrappedModuleCount) > 1 Then
                    ' use a table so we can get the breaking correct
                    tabPage.Append("<table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">")
                    tabPage.Append("<tr>")

                    ' start table
                    Dim moduleNumber As Integer = 1
                    For Each ami As AggregatorModuleInfo In ati.Modules
                        If Not ami.ModuleInfo.PaneName.StartsWith("wrap") Then
                            ' inject below text area if not wrapped in html/text area above
                            ami.ModuleInfo.PaneName = "cell" & tabNumber.ToString & "_" & moduleNumber.ToString
                            tabPage.AppendFormat("<td runat=""server"" id=""{0}"" valign=""top""></td>", ami.ModuleInfo.PaneName)

                            If ami.InsertBreak And moduleNumber < ati.Modules.Count Then
                                ' finish table and start a new one
                                tabPage.Append("</tr>")
                                tabPage.Append("</table>")
                                tabPage.Append("<table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">")
                                tabPage.Append("<tr>")
                            End If

                            moduleNumber += 1

                        End If
                    Next
                    ' finish row and start a new one
                    tabPage.Append("</tr>")
                    tabPage.Append("</table>")
                Else
                    If (ati.Modules.Count - wrappedModuleCount) > 0 Then
                        ' if only one module then don't put inside table, unnecessary
                        Dim ami As AggregatorModuleInfo = DirectCast(ati.Modules(0), AggregatorModuleInfo)
                        If Not ami.ModuleInfo.PaneName.StartsWith("wrap") Then
                            ami.ModuleInfo.PaneName = "div" & tabNumber.ToString & "_1"
                            tabPage.AppendFormat("<div runat=""server"" id=""{0}"" />", ami.ModuleInfo.PaneName)
                        End If
                    End If
                End If
            End If

            ' grab empty tab page template
            Dim emptyTabPage As String = GetEmptyTabPageTemplate(t, ati, tabNumber)
            emptyTabPage = emptyTabPage.Replace("[TABPAGECONTENT]", tabPage.ToString)

            ' tab tokens
            If ati IsNot Nothing Then
                emptyTabPage = ReplaceAggregatorTabInfoTokens(emptyTabPage, ati, tabNumber)
            End If

            Return emptyTabPage

        End Function

        Private Function GetEmptyTabPageTemplate(ByVal t As Template, ByVal ati As AggregatorTabInfo, ByVal tabNumber As Integer) As String
            ' return a hidden div for injecting module into

            ' events
            Dim events As New StringBuilder
            If Not (_aggregator.SingleTab And _ms.HideSingleTab) Then
                events.Append("style=""display:none""")
            End If

            ' tabpage
            Dim tabpage As String = t.TabPageTemplate

            ' check for paging
            Dim pagingTemplate As String = ""
            If tabpage.Contains("[PAGING]") Then
                pagingTemplate = GetPagingTemplate(t, ati, tabNumber)
            End If

            ' do replacements
            tabpage = tabpage _
                .Replace("[PAGING]", pagingTemplate) _
                .Replace("[TABPAGEACTION]", events.ToString)

            Return tabpage
        End Function

        Private Function GetPagingTemplate(ByVal t As Template, ByVal ati As AggregatorTabInfo, ByVal tabNumber As Integer) As String

            Dim prevnext As String = t.PagingTemplate _
                .Replace("[PAGINGITEMLIST]", GetPagingItemListTemplate(t))

            ' tab tokens
            If ati IsNot Nothing Then
                prevnext = ReplaceAggregatorTabInfoTokens(prevnext, ati, tabNumber)
            End If

            Return prevnext
        End Function

        Private Function GetPagingItemListTemplate(ByVal t As Template) As String

            ' render tabs
            Dim tabNumber As Integer = 1
            Dim pagingItemList As New StringBuilder
            For Each ati As AggregatorTabInfo In _aggregator.Tabs
                pagingItemList.Append(GetPagingItemTemplate(t, ati, tabNumber))
                tabNumber += 1
            Next
            Return pagingItemList.ToString

        End Function

        Private Function GetPagingItemTemplate(ByVal t As Template, ByVal ai As AggregatorTabInfo, ByVal tabNumber As Integer) As String
            Return ReplaceAggregatorTabInfoTokens(t.PagingItemTemplate, ai, tabNumber)
        End Function

#End Region

#Region " Javascript Injection"
        Private Sub InjectScript(ByVal t As Template, ByVal ScriptName As String, ByVal Location As String, ByVal Scope As String, ByVal UseFallback As Boolean)

            Dim js As String = t.GetScript(ScriptName, Scope, UseFallback)

            ' do replacements
            js = ReplaceAggregatorInfoTokens(js)

            If Not String.IsNullOrEmpty(js) Then
                Select Case Location.ToLower
                    Case "head"
                        Dim scriptKey As String = Unique(ScriptName.ToLower, Scope) & "_" & Scope
                        If Not Page.ClientScript.IsClientScriptBlockRegistered(Me.GetType, scriptKey) Then
                            Page.Header.Controls.Add(New LiteralControl(js))
                            ' dummy script block so we can track if it's already in header
                            Page.ClientScript.RegisterClientScriptBlock(Me.GetType, scriptKey, "")
                        End If
                    Case Else
                        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), Unique(ScriptName.ToLower), js)
                End Select
            End If
        End Sub

#End Region

#Region " Programatic Tab Selected"
        Private Sub RemoveQueryStringModules()
            If Not Request.QueryString("Agg" & ModuleId.ToString & "_HideTabs") Is Nothing And Not Page.IsPostBack Then
                Dim delimTabNumbers As String = Request.QueryString("Agg" & ModuleId.ToString & "_HideTabs")
                Dim tabNumbers() As String = delimTabNumbers.Split(","c)
                Array.Reverse(tabNumbers)   ' start backwards
                For Each tabToken As String In tabNumbers
                    Dim tabNumber As Integer
                    If Int32.TryParse(tabToken, tabNumber) Then
                        If _aggregator.Tabs.Count >= tabNumber Then
                            ' remove it
                            _aggregator.Tabs.RemoveAt(tabNumber - 1)
                        End If
                    End If
                Next
            End If
        End Sub

        Private Function GetSelectedTab() As Integer
            ' determine selected tab
            Dim selectedTab As Integer = _ms.DefaultTab

            If Not Request.QueryString("Agg" & ModuleId.ToString & "_SelectTab") Is Nothing And Not Page.IsPostBack Then
                ' check for selection by tab number
                selectedTab = Convert.ToInt32(Request.QueryString("Agg" & ModuleId.ToString & "_SelectTab"))
            Else
                If Not Request.QueryString("Agg" & ModuleId.ToString & "_SelectByNum") Is Nothing And Not Page.IsPostBack Then
                    ' check for selection by tab number
                    selectedTab = Convert.ToInt32(Request.QueryString("Agg" & ModuleId.ToString & "_SelectByNum"))
                Else
                    If Not Request.QueryString("Agg" & ModuleId.ToString & "_SelectByTitle") Is Nothing And Not Page.IsPostBack Then
                        ' check for selection by title
                        Dim matchTitle As String = ReplaceGenericTokens(Me, Request.QueryString("Agg" & ModuleId.ToString & "_SelectByTitle"))
                        Dim tabNumber As Integer = 1
                        For Each ai As AggregatorTabInfo In New ArrayList(_aggregator.Tabs)
                            If ai.Caption.ToUpper = matchTitle.ToUpper Then
                                selectedTab = tabNumber
                                Exit For
                            End If
                            tabNumber += 1
                        Next
                    Else
                        ' get selected module by checking cookie, this allows us to maintain state
                        ' not only between post back but also calls to other pages such as edit pages
                        ' v5.6.3 - added the remember option, only applicable in view mode or non postback
                        If _ms.RememberLastOpenTab Or Page.IsPostBack Or Me.IsEditable Then
                            If Not Request.Cookies("DNNSTUFF_Aggregator") Is Nothing Then
                                ' new consolidated cookie handling
                                selectedTab = Convert.ToInt32(Request.Cookies("DNNSTUFF_Aggregator")(ModuleId.ToString))
                            Else
                                If Not Request.Cookies(Unique("SelectedElementId")) Is Nothing Then
                                    ' old single cookie handling for backward compatibility
                                    selectedTab = Convert.ToInt32(Replace(Request.Cookies(Unique("SelectedElementId")).Value, Unique("Tab"), ""))
                                Else
                                    ' lets check hidden field, just in case cookies disabled, then at least
                                    ' we get postback
                                    If Request.Form(Unique("SelectedElementId")) IsNot Nothing Then
                                        selectedTab = Convert.ToInt32(Replace(Request.Form(Unique("SelectedElementId")), Unique("Tab"), ""))
                                    End If
                                End If
                            End If
                        Else
                            ' lets check hidden field, just in case cookies disabled, then at least
                            ' we get postback
                            If Request.Form(Unique("SelectedElementId")) IsNot Nothing Then
                                selectedTab = Convert.ToInt32(Replace(Request.Form(Unique("SelectedElementId")), Unique("Tab"), ""))
                            End If
                        End If
                    End If
                End If
            End If
            If selectedTab < 1 Or selectedTab > _aggregator.Tabs.Count Then
                selectedTab = _ms.DefaultTab
                If selectedTab < 1 Or selectedTab > _aggregator.Tabs.Count Then
                    selectedTab = 1
                End If
            End If
            Return selectedTab
        End Function

        Public Sub OnModuleCommunication(ByVal s As Object, ByVal e As Entities.Modules.Communications.ModuleCommunicationEventArgs) Implements Entities.Modules.Communications.IModuleListener.OnModuleCommunication

            If e Is Nothing Then Exit Sub

            If e.Target = "Aggregator" Then
                Select Case e.Type
                    Case "SelectByNum", "SelectTab"
                        _selectedTabNumber = Convert.ToInt32(e.Value.ToString())
                    Case "SelectByTitle", "SelectTitle"
                        Dim tabNumber As Integer = 1
                        For Each ai As AggregatorTabInfo In New ArrayList(_aggregator.Tabs)
                            If ai.Caption.ToUpper = e.Value.ToString.ToUpper Then
                                _selectedTabNumber = tabNumber
                                Exit For
                            End If
                            tabNumber += 1
                        Next
                End Select
            End If
        End Sub

#End Region

#Region " Token Replacement"
        Public Function MakeReplacements_Backward(ByVal s As String) As String
            ' RDE - removed need for moduleSettings after the multiple modules per tab change
            ' replace system settings
            s = Regex.Replace(s, "\[DNN:PORTAL.PORTALID\]", PortalId.ToString, RegexOptions.IgnoreCase)
            s = Regex.Replace(s, "\[DNN:PORTAL.PORTALNAME\]", PortalSettings.PortalName, RegexOptions.IgnoreCase)
            s = Regex.Replace(s, "\[DNN:MODULE.MODULEID\]", ModuleId.ToString, RegexOptions.IgnoreCase)
            s = Regex.Replace(s, "\[DNN:TAB.TABID\]", TabId.ToString, RegexOptions.IgnoreCase)
            s = Regex.Replace(s, "\[DNN:TAB.TABNAME\]", PortalSettings.ActiveTab.TabName, RegexOptions.IgnoreCase)
            s = Regex.Replace(s, "\[DNN:USER.USERID\]", UserId.ToString, RegexOptions.IgnoreCase)
            If Not UserInfo.Username Is Nothing Then
                If Not UserInfo.DisplayName Is Nothing Then
                    s = Regex.Replace(s, "\[DNN:USER.FULLNAME\]", UserInfo.DisplayName, RegexOptions.IgnoreCase)
                End If
                s = Regex.Replace(s, "\[DNN:USER.USERNAME\]", UserInfo.Username, RegexOptions.IgnoreCase)
            Else
                s = Regex.Replace(s, "\[DNN:USER.FULLNAME\]", "Anonymous", RegexOptions.IgnoreCase)
                s = Regex.Replace(s, "\[DNN:USER.USERNAME\]", "Anonymous", RegexOptions.IgnoreCase)
            End If
            s = Regex.Replace(s, "\[DNN:IMAGEURL\]", ResolveUrl("~/images"), RegexOptions.IgnoreCase)

            ' now do query strings
            For Each key As String In Request.QueryString.Keys
                s = Regex.Replace(s, "\[QUERYSTRING:" & UCase(key) & "\]", Request.QueryString(key).ToString, RegexOptions.IgnoreCase)
            Next
            Return s
        End Function

        Private Function ReplaceAggregatorTabInfoTokens(ByVal text As String, ByVal ati As AggregatorTabInfo, ByVal tabNumber As Integer) As String
            ' do generic replacements
            text = Compatibility.ReplaceGenericTokens(Me, text)

            ' do aggregator tab specific replacements
            Dim logicReplacer As New DNNStuff.Utilities.RegularExpression.IfDefinedTokenReplacement(AggregatorTabSettings(ati, tabNumber, text))
            logicReplacer.ReplaceIfNotFound = False
            text = logicReplacer.Replace(text)

            Dim replacer As New DNNStuff.Utilities.RegularExpression.TokenReplacement(AggregatorTabSettings(ati, tabNumber, text))
            replacer.ReplaceIfNotFound = False
            Return replacer.Replace(text)

        End Function

        Private Function ReplaceAggregatorInfoTokens(ByVal text As String) As String

            ' do generic replacements
            text = ReplaceGenericTokens(Me, text)

            ' do aggregator tab specific replacements
            Dim logicReplacer As New DNNStuff.Utilities.RegularExpression.IfDefinedTokenReplacement(AggregatorSettings())
            text = logicReplacer.Replace(text)

            Dim replacer As New DNNStuff.Utilities.RegularExpression.TokenReplacement(AggregatorSettings())
            replacer.ReplaceIfNotFound = False

            Return replacer.Replace(text)

        End Function

        Private Function ReplaceRSSItemTokens(ByVal text As String, ByVal item As RssItem) As String
            ' do generic replacements
            text = ReplaceGenericTokens(Me, text)

            ' rss content
            Dim logicReplacer As New DNNStuff.Utilities.RegularExpression.IfDefinedTokenReplacement(RSSItemSettings(item))
            text = logicReplacer.Replace(text)

            Dim tokenReplacer As New DNNStuff.Utilities.RegularExpression.TokenReplacement(RSSItemSettings(item))
            tokenReplacer.ReplaceIfNotFound = False
            text = tokenReplacer.Replace(text)

            Return text
        End Function

        Private Function AggregatorSettings() As Hashtable

            If _aggregatorTokens IsNot Nothing Then Return _aggregatorTokens

            Dim tokens As New Hashtable
            tokens.Add("UNIQUE", Unique(""))
            tokens.Add("PARENTID", Me.Parent.ClientID)
            tokens.Add("MODULEID", Me.ModuleId.ToString)
            tokens.Add("MODULEFOLDER", Me.ControlPath.Remove(Me.ControlPath.Length - 1)) ' remove last / character to be consistent with resolveurl
            tokens.Add("TABMODULEID", Me.TabModuleId.ToString)

            tokens.Add("SKIN", _ms.TabTheme)
            tokens.Add("SKINFOLDER", ResolveUrl(SkinFolder))
            tokens.Add("SKINBASEFOLDER", ResolveUrl(SkinBaseFolder))
            tokens.Add("SELECTEDTABNUMBER", _selectedTabNumber.ToString)
            tokens.Add("TABCOUNT", _aggregator.Tabs.Count.ToString)

            tokens.Add("IMAGEURL", ResolveUrl("~/images"))

            ' SELECTTARGET
            Dim selectTarget As New StringBuilder
            If _aggregator.Targets.Count > 0 Then
                For Each at As AggregatorTargetInfo In _aggregator.Targets
                    selectTarget.AppendLine("if (typeof " & "Agg" & at.TargetModuleId.ToString & "_SelectTab!=""undefined"" && source!=" & at.TargetModuleId.ToString & ") {")
                    selectTarget.AppendLine("Agg" & at.TargetModuleId.ToString & "_SelectTab(tabNumber,source);}")
                Next
            End If
            tokens.Add("SELECTTARGET", selectTarget.ToString)

            tokens.Add("PAGEFIRSTACTION", "onClick=""javascript:" & Unique("SelectTab") & "(1);"" ")
            tokens.Add("PAGEPREVACTION", "onClick=""javascript:" & Unique("SelectPrevTab") & "();"" ")
            tokens.Add("PAGENEXTACTION", "onClick=""javascript:" & Unique("SelectNextTab") & "();"" ")
            tokens.Add("PAGELASTACTION", "onClick=""javascript:" & Unique("SelectTab") & "(" & _aggregator.Tabs.Count & ");"" ")

            tokens.Add("FIRSTCAPTION", Localization.GetString("First.Text", LocalResourceFile))
            tokens.Add("PREVCAPTION", Localization.GetString("Prev.Text", LocalResourceFile))
            tokens.Add("NEXTCAPTION", Localization.GetString("Next.Text", LocalResourceFile))
            tokens.Add("LASTCAPTION", Localization.GetString("Last.Text", LocalResourceFile))

            tokens.Add("HIDETABS", _ms.HideTabs.ToString)
            tokens.Add("ACTIVEHOVER", _ms.ActiveHover.ToString)
            tokens.Add("ACTIVEHOVERDELAY", _ms.ActiveHoverDelay.ToString)
            tokens.Add("HIDESINGLETAB", _ms.HideSingleTab.ToString)
            tokens.Add("HIDETITLES", _ms.HideTitles.ToString)
            tokens.Add("SHOWPAGER", _ms.ShowPrevNext.ToString)
            tokens.Add("DEFAULTTABNUMBER", _ms.DefaultTab.ToString)
            tokens.Add("REMEMBERLASTOPENTAB", _ms.RememberLastOpenTab.ToString)
            tokens.Add("HEIGHT", _ms.Height)
            tokens.Add("WIDTH", _ms.Width)

            tokens.Add("LOCALE", System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower)

            Dim overflow_style As String = ""
            Dim height_style As String = ""
            If _ms.Height <> "" Then
                Dim height As Integer
                If Int32.TryParse(_ms.Height, height) Then
                    height_style = "height:" & height.ToString & "px;"
                Else
                    height_style = "height:" & _ms.Height & ";"
                End If
                overflow_style = "overflow:auto;"
            End If

            Dim width_style As String = ""
            If _ms.Width <> "" Then
                Dim width As Integer
                If Int32.TryParse(_ms.Width, width) Then
                    width_style = "width:" & width.ToString & "px;"
                Else
                    width_style = "width:" & _ms.Width & ";"
                End If
                overflow_style = "overflow:auto;"
            End If
            tokens.Add("HEIGHT_STYLE", height_style)
            tokens.Add("WIDTH_STYLE", width_style)
            tokens.Add("OVERFLOW_STYLE", "position:relative;" & overflow_style)
            tokens.Add("SAVEACTIVETAB", String.Format("subcookiejar.bake('DNNSTUFF_Aggregator',{{{0}:tabNumber.toString()}});", ModuleId))

            ' replace our special [REQUIRES] tokens with something inoccuous like a comment
            tokens.Add("REQUIRESJQUERY", "<!-- requires jquery -->")
            tokens.Add("REQUIRESJQUERYUI", "<!-- requires jquery ui -->")

            tokens.Add("CDATASTART", "<![CDATA[")
            tokens.Add("CDATAEND", "]]>")

            ' grab settings if available
            If _aggregator.Properties IsNot Nothing Then
                For Each prop As CustomSettingsInfo In _aggregator.Properties
                    tokens.Add(prop.Name.ToUpper, prop.Value)
                Next
            End If

            ' add querystring values
            Dim qs As New Specialized.NameValueCollection(Request.QueryString) ' create a copy, some weird errors happening with url rewriters
            Dim keyval As Object
            For Each key As String In qs.Keys
                keyval = qs(key)
                If key IsNot Nothing And keyval IsNot Nothing Then
                    tokens.Add("QS_" & key.ToUpper(), keyval.ToString())
                End If
            Next

            _aggregatorTokens = tokens
            Return _aggregatorTokens
        End Function

        Private Function ParseLocalizedString(ByVal s As String) As String
            Dim currentLocale As String = System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower

            Dim reg As Regex = New Regex(String.Format("{0}:(.*?)(\||$)", currentLocale), RegexOptions.IgnoreCase)
            If reg.IsMatch(s) Then
                Dim match As RegularExpressions.Match = reg.Match(s)
                If match.Groups.Count > 1 Then
                    s = match.Groups(1).Value
                End If
            End If

            Return s

        End Function
        Private Function AggregatorTabSettings(ByVal ati As AggregatorTabInfo, ByVal tabNumber As Integer, ByVal text As String) As Hashtable
            ' build hashtable of tokens relative to an individual tab
            Dim tokens As New Hashtable

            Dim ami As AggregatorModuleInfo = Nothing
            If ati.Modules.Count > 0 Then
                ami = CType(ati.Modules(0), AggregatorModuleInfo)
            End If

            ' MODULETITLE
            If ami IsNot Nothing Then
                tokens.Add("MODULETITLE", ami.ModuleInfo.ModuleTitle)
            Else
                tokens.Add("MODULETITLE", ati.Caption)
            End If

            ' MMLINKS
            If text.Contains("[MMLINKSTITLE]") Then
                ' mmlinks causes a db lookup so don't include unless the token exists in the text
                If ami IsNot Nothing Then
                    Dim currentLocale As String = System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
                    Dim dr As IDataReader = Nothing
                    Try
                        Dim ctrl As New AggregatorController
                        dr = ctrl.GetMMLinks(ami.ModuleId, currentLocale)
                        If dr.Read Then
                            tokens.Add("MMLINKSTITLE", dr(0).ToString)
                        End If
                    Catch ex As Exception
                    Finally
                        If dr IsNot Nothing Then dr.Close()
                    End Try
                End If
            End If

            ' MHTML
            If text.Contains("[MLHTMLTITLE]") Then
                ' mhtml causes a db lookup so don't include unless the token exists in the text
                If ami IsNot Nothing Then
                    Dim currentLocale As String = System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
                    Dim dr As IDataReader = Nothing
                    Try
                        Dim ctrl As New AggregatorController
                        dr = ctrl.GetMLHTML(ami.ModuleId, currentLocale)
                        If dr.Read Then
                            tokens.Add("MLHTMLTITLE", dr(0).ToString)
                        End If
                    Catch ex As Exception
                    Finally
                        If dr IsNot Nothing Then dr.Close()
                    End Try
                End If
            End If

            ' NUNTIO
            If text.Contains("[NUNTIOTITLE]") Then
                ' mhtml causes a db lookup so don't include unless the token exists in the text
                If ami IsNot Nothing Then
                    Dim currentLocale As String = System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
                    Dim dr As IDataReader = Nothing
                    Try
                        Dim ctrl As New AggregatorController
                        dr = ctrl.GetNUNTIO(ami.ModuleId, currentLocale)
                        If dr.Read Then
                            tokens.Add("NUNTIOTITLE", dr(0).ToString)
                        End If
                    Catch ex As Exception
                    Finally
                        If dr IsNot Nothing Then dr.Close()
                    End Try
                End If
            End If

            tokens.Add("NEXTTABCAPTION", ParseLocalizedString(_aggregator.NextTabCaption(tabNumber).ToString))
            tokens.Add("NEXTTABNUMBER", _aggregator.NextTabNumber(tabNumber).ToString)
            tokens.Add("TABCAPTION", ParseLocalizedString(ati.Caption))
            tokens.Add("TABNUMBER", tabNumber.ToString)
            tokens.Add("PAGEITEMACTION", "onClick=""javascript:" & Unique("SelectTab") & "(" & tabNumber.ToString & ");""")
            tokens.Add("TABPAGEID", Unique("TabPage") & tabNumber)
            tokens.Add("TABID", Unique("Tab" & tabNumber.ToString))
            tokens.Add("CURRENTTAB", (_selectedTabNumber = tabNumber).ToString)
            tokens.Add("POSTBACK", ati.Postback.ToString)
            tokens.Add("LASTTAB", (tabNumber = _aggregator.Tabs.Count).ToString)
            Dim params() As String = DNNUtilities.GetParamsForNavigate("Agg" & ModuleId & "_SelectTab", tabNumber.ToString)
            tokens.Add("POSTBACKSELECTTAB", NavigateURL(TabId, "", params))

            ' grab settings if available
            For Each prop As CustomSettingsInfo In ati.Properties
                tokens.Add(prop.Name.ToUpper, prop.Value)
            Next

            ' append rss item tokens
            If ati.RSS IsNot Nothing Then
                tokens.Add("RSS", "True")
                For Each item As DictionaryEntry In ati.RSS
                    tokens.Add(item.Key.ToString.ToUpper, item.Value)
                Next
            End If

            Return tokens
        End Function

        Private Function RSSItemSettings(ByVal item As RssItem) As Hashtable
            ' build hashtable of tokens relative to an individual rss item

            Dim tokens As New Hashtable
            With tokens
                .Add("RSSTITLE", item.Title)
                .Add("RSSAUTHOR", item.Author)
                .Add("RSSDESCRIPTION", item.Description)
                .Add("RSSLINK", item.Link)
                .Add("RSSPUBDATE", item.PubDateParsed.ToString)
            End With
            If item.Enclosure IsNot Nothing Then
                tokens.Add("RSSENCLOSUREURL", item.Enclosure.Url)
                tokens.Add("RSSENCLOSURETYPE", item.Enclosure.Type)
            Else
                tokens.Add("RSSENCLOSUREURL", "")
                tokens.Add("RSSENCLOSURETYPE", "")
            End If

            Return tokens
        End Function

#End Region

#Region " RSS"
        Private Function DownloadFeed() As RssDocument

            ' grab cache if present and return
            If _ms.RSSCacheTime > 0 Then
                If DataCache.GetCache("RSS_" & ModuleId.ToString) IsNot Nothing Then
                    Return DirectCast(DataCache.GetCache("RSS_" & ModuleId.ToString), RssDocument)
                End If
            End If

            Try

                Dim Password As String = _ms.RSSPassword
                Dim UserAccount As String = Mid(_ms.RSSUsername, InStr(_ms.RSSUsername, "\") + 1)
                Dim DomainName As String = ""
                If InStr(_ms.RSSUsername, "\") <> 0 Then
                    DomainName = Left(_ms.RSSUsername, InStr(_ms.RSSUsername, "\") - 1)
                End If
                ' make remote request
                Dim wr As HttpWebRequest = CType(WebRequest.Create(_ms.RSSUrl), HttpWebRequest)
                If UserAccount <> "" Then
                    wr.Credentials = New NetworkCredential(UserAccount, Password, DomainName)
                End If

                ' set proxy
                Compatibility.SetProxy(wr)

                ' set the HTTP properties
                wr.Timeout = 10000       ' 10 seconds

                ' read the response
                Dim resp As WebResponse = wr.GetResponse()
                Dim stream As Stream = resp.GetResponseStream()

                ' load XML document
                Dim reader As XmlTextReader = New XmlTextReader(stream)
                reader.XmlResolver = Nothing

                'doc.Load(reader)
                Dim originalType As RssToolkit.Rss.DocumentType = RssToolkit.Rss.DocumentType.Unknown
                Dim rssdoc As RssToolkit.Rss.RssDocument = RssToolkit.Rss.RssDocument.Load(reader)

                If _ms.RSSCacheTime > 0 Then
                    DataCache.SetCache("RSS_" & ModuleId.ToString, rssdoc, Now.AddSeconds(_ms.RSSCacheTime))
                End If
                Return rssdoc

            Catch ex As Exception

                Return Nothing

            End Try

        End Function
#End Region

    End Class
End Namespace
