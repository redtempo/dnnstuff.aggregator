'***************************************************************************/
'* AggregatorDB.vb
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
Imports System.Data
Imports System.Xml
Imports DotNetNuke
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Search

Namespace DNNStuff.Aggregator

#Region " AggregatorInfo Class"

    Public Class AggregatorInfo

        ' local property declarations
        Private _ModuleId As Integer = -1

        ' collections
        Private _Tabs As ArrayList = Nothing
        Private _Targets As ArrayList = Nothing
        Private _Properties As ArrayList = Nothing

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property Tabs() As ArrayList
            Get
                Return _Tabs
            End Get
            Set(ByVal Value As ArrayList)
                _Tabs = Value
            End Set
        End Property

        Public Property Targets() As ArrayList
            Get
                Return _Targets
            End Get
            Set(ByVal Value As ArrayList)
                _Targets = Value
            End Set
        End Property

        Public Property Properties() As ArrayList
            Get
                Return _Properties
            End Get
            Set(ByVal Value As ArrayList)
                _Properties = Value
            End Set
        End Property

#Region " Derived Properties"
        Public ReadOnly Property SingleTab() As Boolean
            Get
                Return Tabs.Count = 1
            End Get
        End Property
        Public ReadOnly Property TabsToShow() As Boolean
            Get
                Return Tabs.Count > 0
            End Get
        End Property
        Public ReadOnly Property NextTabNumber(ByVal tabNumber As Integer) As Integer
            Get
                If tabNumber = _Tabs.Count Then
                    Return 1
                Else
                    Return tabNumber + 1
                End If
            End Get
        End Property
        Public ReadOnly Property NextTabCaption(ByVal tabNumber As Integer) As String
            Get
                Return CType(_Tabs(NextTabNumber(tabNumber) - 1), AggregatorTabInfo).Caption
            End Get
        End Property

#End Region
    End Class

#End Region

#Region " AggregatorTabInfo Class "

    Public Class AggregatorTabInfo

        ' local property declarations
        Private _AggregatorTabId As Integer = -1
        Private _ModuleId As Integer = -1
        Private _TabOrder As Integer = 0
        Private _Caption As String = ""
        Private _Locale As String = "All"
        Private _HtmlText As String = ""
        Private _Postback As Boolean = False

        ' collections
        Private _Modules As ArrayList = Nothing
        Private _Properties As ArrayList = Nothing
        Private _RSS As Hashtable = Nothing

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property AggregatorTabId() As Integer
            Get
                Return _AggregatorTabId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorTabId = Value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property TabOrder() As Integer
            Get
                Return _TabOrder
            End Get
            Set(ByVal Value As Integer)
                _TabOrder = Value
            End Set
        End Property

        Public Property Caption() As String
            Get
                Return _Caption
            End Get
            Set(ByVal Value As String)
                _Caption = Value
            End Set
        End Property

        Public Property Locale() As String
            Get
                Return _Locale
            End Get
            Set(ByVal Value As String)
                _Locale = Value
            End Set
        End Property

        Public Property Postback() As Boolean
            Get
                Return _Postback
            End Get
            Set(ByVal value As Boolean)
                _Postback = value
            End Set
        End Property

        Public Property HtmlText() As String
            Get
                Return _HtmlText
            End Get
            Set(ByVal Value As String)
                _HtmlText = Value
            End Set
        End Property

        Public Property Modules() As ArrayList
            Get
                Return _Modules
            End Get
            Set(ByVal Value As ArrayList)
                _Modules = Value
            End Set
        End Property

        Public Property Properties() As ArrayList
            Get
                Return _Properties
            End Get
            Set(ByVal Value As ArrayList)
                _Properties = Value
            End Set
        End Property

        Public Property RSS() As Hashtable
            Get
                Return _RSS
            End Get
            Set(ByVal Value As Hashtable)
                _RSS = Value
            End Set
        End Property
    End Class

#End Region

#Region " AggregatorModuleInfo Class "
    Public Enum LoadEventType As Integer
        [Default] = 1
        PageInit = 2
        PageLoad = 3
    End Enum

    Public Class AggregatorModuleInfo

        ' local property declarations
        Private _AggregatorModuleId As Integer = -1
        Private _AggregatorTabId As Integer = -1
        Private _TabModuleId As Integer = -1
        Private _ModuleId As Integer = -1
        Private _TabId As Integer = -1
        Private _ModuleOrder As Integer = 0
        Private _Locale As String = ""
        Private _InsertBreak As Boolean = True
        Private _LoadEvent As LoadEventType = LoadEventType.Default
        Private _ModuleInfo As Entities.Modules.ModuleInfo

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property AggregatorTabId() As Integer
            Get
                Return _AggregatorTabId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorTabId = Value
            End Set
        End Property

        Public Property AggregatorModuleId() As Integer
            Get
                Return _AggregatorModuleId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorModuleId = Value
            End Set
        End Property

        Public Property TabModuleId() As Integer
            Get
                Return _TabModuleId
            End Get
            Set(ByVal Value As Integer)
                _TabModuleId = Value
            End Set
        End Property
        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property
        Public Property TabId() As Integer
            Get
                Return _TabId
            End Get
            Set(ByVal Value As Integer)
                _TabId = Value
            End Set
        End Property
        Public Property ModuleOrder() As Integer
            Get
                Return _ModuleOrder
            End Get
            Set(ByVal Value As Integer)
                _ModuleOrder = Value
            End Set
        End Property

        Public Property Locale() As String
            Get
                Return _Locale
            End Get
            Set(ByVal Value As String)
                _Locale = Value
            End Set
        End Property
        Public Property InsertBreak() As Boolean
            Get
                Return _InsertBreak
            End Get
            Set(ByVal Value As Boolean)
                _InsertBreak = Value
            End Set
        End Property

        Public Property LoadEvent() As LoadEventType
            Get
                Return _LoadEvent
            End Get
            Set(ByVal value As LoadEventType)
                _LoadEvent = value
            End Set
        End Property

        Public Property ModuleInfo() As Entities.Modules.ModuleInfo
            Get
                If _ModuleInfo Is Nothing Then
                    _ModuleInfo = Compatibility.GetModuleInfo(ModuleId, TabId, CType(HttpContext.Current.Items("PortalSettings"), Entities.Portals.PortalSettings))
                End If
                Return _ModuleInfo
            End Get
            Set(ByVal Value As Entities.Modules.ModuleInfo)
                _ModuleInfo = Value
            End Set
        End Property

#Region " Used for Copy Aggregator"
        Public ReadOnly Property ModuleTitle() As String
            Get
                Return ModuleInfo.ModuleTitle
            End Get
        End Property
        Public ReadOnly Property IsPortable() As Boolean
            Get
                Return Compatibility.ModuleIsPortable(ModuleInfo)
            End Get
        End Property

        Private _TagData As String = ""
        Public Property TagData() As String
            Get
                Return _TagData
            End Get
            Set(ByVal value As String)
                _TagData = value
            End Set
        End Property

#End Region
    End Class

#End Region

#Region " AggregatorTargetInfo Class "

    Public Class AggregatorTargetInfo

        ' local property declarations
        Private _AggregatorTargetId As Integer
        Private _ModuleId As Integer
        Private _TargetModuleId As Integer

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property AggregatorTargetId() As Integer
            Get
                Return _AggregatorTargetId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorTargetId = Value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property TargetModuleId() As Integer
            Get
                Return _TargetModuleId
            End Get
            Set(ByVal Value As Integer)
                _TargetModuleId = Value
            End Set
        End Property

    End Class

#End Region

#Region " AggregatorController Class "

    Public Class AggregatorController
        Implements Entities.Modules.ISearchable
        Implements Entities.Modules.IPortable

        Private Const MAX_DESCRIPTION_LENGTH As Integer = 100

        Public Function GetAggregatorObjectGraph(ByVal ModuleId As Integer, ByVal Skin As String, ByVal Template As String) As AggregatorInfo
            Dim ai As New AggregatorInfo
            ai.ModuleId = ModuleId

            ' properties
            ai.Properties = CBO.FillCollection(GetProperties(ModuleId, -1, Skin, Template), GetType(CustomSettingsInfo))

            ' tabs/modules
            Dim tabs As ArrayList = CBO.FillCollection(ListAggregator(ModuleId), GetType(AggregatorTabInfo))
            For Each o As AggregatorTabInfo In tabs
                Dim modules As ArrayList
                modules = CBO.FillCollection(ListAggregatorModule(o.AggregatorTabId), GetType(AggregatorModuleInfo))

                o.Modules = modules

                o.Properties = CBO.FillCollection(GetProperties(ModuleId, o.AggregatorTabId, Skin, Template), GetType(CustomSettingsInfo))
            Next
            ai.Tabs = tabs

            ' targets
            Dim targets As ArrayList = CBO.FillCollection(GetTargets(ModuleId), GetType(AggregatorTargetInfo))
            ai.Targets = targets

            Return ai
        End Function

        Public Function ListAggregator(ByVal ModuleId As Integer) As IDataReader
            Return DataProvider.Instance().ListAggregator(ModuleId)
        End Function
        Public Function ListAggregatorModule(ByVal AggregatorTabId As Integer) As IDataReader
            Return DataProvider.Instance().ListAggregatorModule(AggregatorTabId)
        End Function

        Public Function GetAggregatorModule(ByVal AggregatorTabId As Integer) As AggregatorModuleInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetAggregatorModule(AggregatorTabId), GetType(AggregatorModuleInfo)), AggregatorModuleInfo)
        End Function

        ' tabs
        Public Function GetAggregatorTab(ByVal AggregatorTabId As Integer) As AggregatorTabInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetAggregatorTab(AggregatorTabId), GetType(AggregatorTabInfo)), AggregatorTabInfo)
        End Function

        Public Function UpdateAggregatorTab(ByVal obj As AggregatorTabInfo) As Integer
            Return DataProvider.Instance().UpdateAggregatorTab(obj.AggregatorTabId, obj.ModuleId, obj.Caption, obj.Locale, obj.HtmlText, obj.Postback)
        End Function

        Public Sub DeleteAggregatorTab(ByVal AggregatorTabId As Integer)
            DataProvider.Instance().DeleteAggregatorTab(AggregatorTabId)
        End Sub

        Public Sub UpdateTabOrder(ByVal AggregatorTabId As Integer, ByVal Increment As Integer)
            DataProvider.Instance().UpdateTabOrder(AggregatorTabId, Increment)
        End Sub

        ' module
        Public Function UpdateAggregatorModule(ByVal obj As AggregatorModuleInfo, ByVal ModuleId As Integer, ByVal TabCaption As String) As Integer
            ' if new tab (-1), then create new tab and use that to add the module
            If obj.AggregatorTabId = -1 Then
                Dim objTab As New AggregatorTabInfo
                With objTab
                    .AggregatorTabId = -1
                    .Caption = TabCaption
                    .Locale = obj.Locale
                    .ModuleId = ModuleId
                End With
                obj.AggregatorTabId = UpdateAggregatorTab(objTab)
            End If
            Return DataProvider.Instance().UpdateAggregatorModule(obj.AggregatorModuleId, obj.AggregatorTabId, obj.TabModuleId, obj.Locale, obj.InsertBreak, obj.LoadEvent)
        End Function

        Public Sub DeleteAggregatorModule(ByVal AggregatorModuleId As Integer)
            DataProvider.Instance().DeleteAggregatorModule(AggregatorModuleId)
        End Sub

        Public Sub UpdateModuleOrder(ByVal AggregatorModuleId As Integer, ByVal Increment As Integer)
            DataProvider.Instance().UpdateModuleOrder(AggregatorModuleId, Increment)
        End Sub

        Public Function GetTabModules(ByVal TabId As Integer, ByVal ModuleId As Integer, ByVal ShowAllModules As Boolean, ByVal AggregatorModuleId As Integer, ByVal PortalId As Integer) As IDataReader
            Return DataProvider.Instance().GetTabModules(TabId, ModuleId, ShowAllModules, AggregatorModuleId, PortalId)
        End Function

        Public Function GetPageModules(ByVal TabId As Integer) As IDataReader
            Return DataProvider.Instance().GetPageModules(TabId)
        End Function

        ' targets
        Public Function GetTargets(ByVal ModuleId As Integer) As IDataReader
            Return DataProvider.Instance().GetTargets(ModuleId)
        End Function

        Public Sub UpdateTarget(ByVal obj As AggregatorTargetInfo)
            obj.AggregatorTargetId = DataProvider.Instance().UpdateTarget(obj.AggregatorTargetId, obj.ModuleId, obj.TargetModuleId)
        End Sub

        Public Sub DeleteTarget(ByVal AggregatorTargetId As Integer)
            DataProvider.Instance().DeleteTarget(AggregatorTargetId)
        End Sub

        Public Function GetAvailableTargets(ByVal TabId As Integer, ByVal ModuleId As Integer, ByVal AggregatorTargetId As Integer) As IDataReader
            Return DataProvider.Instance().GetAvailableTargets(TabId, ModuleId, AggregatorTargetId)
        End Function

        ' properties
        Public Function GetProperties(ByVal ModuleId As Integer, ByVal AggregatorTabId As Integer, ByVal Skin As String, ByVal Theme As String) As IDataReader
            Return DataProvider.Instance().GetProperties(ModuleId, AggregatorTabId, Skin, Theme)
        End Function
        Public Function UpdateProperties(ByVal ModuleId As Integer, ByVal AggregatorTabId As Integer, ByVal Skin As String, ByVal Theme As String, ByVal Name As String, ByVal Value As String) As Integer
            Return DataProvider.Instance().UpdateProperties(ModuleId, AggregatorTabId, Skin, Theme, Name, Value)
        End Function
        ' localization module support
        Public Function GetMMLinks(ByVal ModuleId As Integer, ByVal Locale As String) As IDataReader
            Return DataProvider.Instance().GetMMLinks(ModuleId, Locale)
        End Function
        Public Function GetMLHTML(ByVal ModuleId As Integer, ByVal Locale As String) As IDataReader
            Return DataProvider.Instance().GetMLHTML(ModuleId, Locale)
        End Function
        Public Function GetNUNTIO(ByVal ModuleId As Integer, ByVal Locale As String) As IDataReader
            Return DataProvider.Instance().GetNUNTIO(ModuleId, Locale)
        End Function

        ' scripts
        ''' <summary>
        ''' Adds a new Script record
        ''' </summary>
        ''' <returns>Identifier for the newly created object</returns>
        Public Function AddScript(ByVal scriptInfo As ScriptInfo) As Integer
            Return CType(DataProvider.Instance().AddScript(scriptInfo.PortalId, scriptInfo.ScriptName, scriptInfo.DontLoadScript, scriptInfo.InternalScriptPath, scriptInfo.LoadHosted, scriptInfo.HostedScriptPath), Integer)
        End Function


        ''' <summary>
        ''' Updates a specified Script
        ''' </summary>
        Public Sub UpdateScript(ByVal scriptInfo As ScriptInfo)
            DataProvider.Instance().UpdateScript(scriptInfo.PortalId, scriptInfo.ScriptName, scriptInfo.DontLoadScript, scriptInfo.InternalScriptPath, scriptInfo.LoadHosted, scriptInfo.HostedScriptPath)
        End Sub


        ''' <summary>
        ''' Deletes a specified Script
        ''' </summary>
        Public Sub DeleteScript(ByVal portalId As Integer, ByVal scriptName As String)
            DataProvider.Instance().DeleteScript(portalId, scriptName)
        End Sub


        ''' <summary>
        ''' Retrieves the details of a specified Script
        ''' </summary>
        ''' <returns>ScriptInfo object</returns>
        Public Function GetScript(ByVal portalId As Integer, ByVal scriptName As String) As ScriptInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetScript(portalId, scriptName), GetType(ScriptInfo)), ScriptInfo)
        End Function


        ''' <summary>
        ''' Retrieves the entire list of Scripts
        ''' </summary>
        ''' <returns>ArrayList of ScriptInfo objects</returns>
        Public Function GetScripts(ByVal PortalId As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetScripts(PortalId), GetType(ScriptInfo))
        End Function

#Region " Search Interface"
        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems

            Dim SearchItemCollection As New SearchItemInfoCollection

            Dim ai As AggregatorInfo = GetAggregatorObjectGraph(ModInfo.ModuleID, "", "")

            Dim tabNumber As Integer = 1
            For Each ati As AggregatorTabInfo In ai.Tabs
                If ati.HtmlText.Length > 0 Then

                    Dim strDesktopHtml As String = HttpUtility.HtmlDecode(ati.HtmlText)

                    'Get the description string
                    Dim strDescription As String = HtmlUtils.Shorten(HtmlUtils.Clean(strDesktopHtml, False), MAX_DESCRIPTION_LENGTH, "...")
                    Dim strGuid As String = "Agg" & ModInfo.ModuleID.ToString & "_SelectTab=" & tabNumber.ToString

                    Dim SearchItem As SearchItemInfo = New SearchItemInfo(ModInfo.ModuleTitle, strDescription, 0, Date.Now, ModInfo.ModuleID, tabNumber.ToString, strDesktopHtml, strGuid)

                    SearchItemCollection.Add(SearchItem)

                End If
                tabNumber += 1
            Next
            Return SearchItemCollection

        End Function

#End Region

#Region " IPortable"
        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements DotNetNuke.Entities.Modules.IPortable.ExportModule
            Return ExportModule(ModuleID, Nothing)
        End Function

        Public Function ExportModule(ByVal ModuleID As Integer, ByVal ModuleMapping As System.Collections.Generic.Dictionary(Of Integer, Integer)) As String
            Dim strXML As New Text.StringBuilder()
            Dim settings As New XmlWriterSettings()
            settings.Indent = True
            settings.OmitXmlDeclaration = True

            Dim Writer As XmlWriter = XmlWriter.Create(strXML, settings)
            Writer.WriteStartElement("Aggregator")

            Dim ms As New ModuleSettings(ModuleID)
            Writer.WriteElementString("ActiveHover", ms.ActiveHover.ToString)
            Writer.WriteElementString("ActiveHoverDelay", ms.ActiveHoverDelay.ToString)
            Writer.WriteElementString("DefaultTab", ms.DefaultTab.ToString)
            Writer.WriteElementString("HideSingleTab", ms.HideSingleTab.ToString)
            Writer.WriteElementString("HideTabs", ms.HideTabs.ToString)
            Writer.WriteElementString("HideTitles", ms.HideTitles.ToString)
            Writer.WriteElementString("RSSCacheTime", ms.RSSCacheTime.ToString)
            Writer.WriteElementString("RSSMaxItems", ms.RSSMaxItems.ToString)
            Writer.WriteElementString("RSSPassword", ms.RSSPassword.ToString)
            Writer.WriteElementString("RSSTabCaption", ms.RSSTabCaption.ToString)
            Writer.WriteElementString("RSSUrl", ms.RSSUrl.ToString)
            Writer.WriteElementString("RSSUsername", ms.RSSUsername.ToString)
            Writer.WriteElementString("ShowPrevNext", ms.ShowPrevNext.ToString)
            Writer.WriteElementString("TabSkin", ms.TabSkin.ToString)
            Writer.WriteElementString("TabTemplate", ms.TabTemplate.ToString)

            Dim ai As AggregatorInfo = GetAggregatorObjectGraph(ModuleID, ms.TabSkin.ToString, ms.TabTemplate.ToString)

            ' start settings
            Writer.WriteStartElement("Settings")
            For Each prop As CustomSettingsInfo In ai.Properties
                Writer.WriteElementString(prop.Name, prop.Value)
            Next
            Writer.WriteEndElement()

            ' start tabs
            Writer.WriteStartElement("Tabs")
            For Each ati As AggregatorTabInfo In ai.Tabs
                ' start tab
                Writer.WriteStartElement("Tab")
                Writer.WriteElementString("Caption", ati.Caption.ToString)
                Writer.WriteElementString("HtmlText", ati.HtmlText.ToString)
                Writer.WriteElementString("Locale", ati.Locale.ToString)
                Writer.WriteElementString("Postback", ati.Postback.ToString)
                Writer.WriteElementString("TabOrder", ati.TabOrder.ToString)

                ' start settings
                Writer.WriteStartElement("Settings")
                For Each prop As CustomSettingsInfo In ati.Properties
                    Writer.WriteElementString(prop.Name, prop.Value)
                Next
                Writer.WriteEndElement()

                ' start modules
                Writer.WriteStartElement("Modules")
                For Each ami As AggregatorModuleInfo In ati.Modules
                    If ModuleMapping Is Nothing Then
                        ' start module
                        Writer.WriteStartElement("Module")
                        Writer.WriteElementString("InsertBreak", ami.InsertBreak.ToString)
                        Writer.WriteElementString("Locale", ami.Locale.ToString)
                        Writer.WriteElementString("TabModuleId", ami.TabModuleId.ToString)
                        Writer.WriteElementString("ModuleOrder", ami.ModuleOrder.ToString)
                        Writer.WriteElementString("LoadEvent", Convert.ToInt32(ami.LoadEvent).ToString)
                        Writer.WriteEndElement()
                    Else
                        If ModuleMapping.ContainsKey(ami.TabModuleId) Then
                            ' start module
                            Writer.WriteStartElement("Module")
                            Writer.WriteElementString("InsertBreak", ami.InsertBreak.ToString)
                            Writer.WriteElementString("Locale", ami.Locale.ToString)
                            Writer.WriteElementString("TabModuleId", ModuleMapping(ami.TabModuleId).ToString)
                            Writer.WriteElementString("ModuleOrder", ami.ModuleOrder.ToString)
                            Writer.WriteElementString("LoadEvent", Convert.ToInt32(ami.LoadEvent).ToString)
                            Writer.WriteEndElement()
                        End If
                    End If
                Next
                Writer.WriteEndElement()
                ' end modules

                Writer.WriteEndElement()
                ' end tab
            Next
            Writer.WriteEndElement()
            ' end tabs

            ' start targets
            Writer.WriteStartElement("Targets")
            For Each ati As AggregatorTargetInfo In ai.Targets
                ' start module
                Writer.WriteStartElement("Target")
                Writer.WriteElementString("TargetModuleId", ati.TargetModuleId.ToString)
                Writer.WriteEndElement()
            Next
            Writer.WriteEndElement()
            ' end targets

            Writer.WriteEndElement()

            Writer.Close()

            Return strXML.ToString()
        End Function

        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements DotNetNuke.Entities.Modules.IPortable.ImportModule

            Dim xml As XmlNode = DotNetNuke.Common.Globals.GetContent(Content, "Aggregator")

            ' module settings
            Dim ms As New ModuleSettings(ModuleID)
            With ms
                .ActiveHover = CBool(xml.SelectSingleNode("ActiveHover").InnerText)
                .ActiveHoverDelay = CInt(xml.SelectSingleNode("ActiveHoverDelay").InnerText)
                .DefaultTab = CInt(xml.SelectSingleNode("DefaultTab").InnerText)
                .HideSingleTab = CBool(xml.SelectSingleNode("HideSingleTab").InnerText)
                .HideTabs = CBool(xml.SelectSingleNode("HideTabs").InnerText)
                .HideTitles = CBool(xml.SelectSingleNode("HideTitles").InnerText)
                .RSSCacheTime = CInt(xml.SelectSingleNode("RSSCacheTime").InnerText)
                .RSSMaxItems = CInt(xml.SelectSingleNode("RSSMaxItems").InnerText)
                .RSSPassword = xml.SelectSingleNode("RSSPassword").InnerText
                .RSSTabCaption = xml.SelectSingleNode("RSSTabCaption").InnerText
                .RSSUrl = xml.SelectSingleNode("RSSUrl").InnerText
                .RSSUsername = xml.SelectSingleNode("RSSUsername").InnerText
                .ShowPrevNext = CBool(xml.SelectSingleNode("ShowPrevNext").InnerText)
                .TabSkin = xml.SelectSingleNode("TabSkin").InnerText
                .TabTemplate = xml.SelectSingleNode("TabTemplate").InnerText
            End With
            ms.UpdateSettings()

            ' properties
            Dim xmlSettings As XmlNodeList = xml.SelectSingleNode("Settings").ChildNodes
            For Each xmlSetting As XmlNode In xmlSettings
                UpdateProperties(ModuleID, -1, ms.TabSkin, ms.TabTemplate, xmlSetting.Name, xmlSetting.InnerText)
            Next

            ' tabs
            Dim xmlTabs As XmlNodeList = xml.SelectNodes("//Aggregator/Tabs/Tab")
            For Each xmlTab As XmlNode In xmlTabs
                Dim ati As AggregatorTabInfo = New AggregatorTabInfo
                With ati
                    .ModuleId = ModuleID
                    .Caption = xmlTab.SelectSingleNode("Caption").InnerText
                    .HtmlText = xmlTab.SelectSingleNode("HtmlText").InnerText
                    .Locale = xmlTab.SelectSingleNode("Locale").InnerText
                    .Postback = CBool(xmlTab.SelectSingleNode("Postback").InnerText)
                    .TabOrder = CInt(xmlTab.SelectSingleNode("TabOrder").InnerText)
                End With

                Dim AggregatorTabId As Integer = UpdateAggregatorTab(ati)

                ' modules
                Dim xmlModules As XmlNodeList = xmlTab.SelectNodes("Modules/Module")
                For Each xmlModule As XmlNode In xmlModules
                    Dim ami As AggregatorModuleInfo = New AggregatorModuleInfo
                    With ami
                        .TabModuleId = CInt(xmlModule.SelectSingleNode("TabModuleId").InnerText)
                        .InsertBreak = CBool(xmlModule.SelectSingleNode("InsertBreak").InnerText)
                        .Locale = xmlModule.SelectSingleNode("Locale").InnerText
                        .LoadEvent = CType(CInt(xmlModule.SelectSingleNode("LoadEvent").InnerText), LoadEventType)
                        .ModuleOrder = CInt(xmlModule.SelectSingleNode("ModuleOrder").InnerText)
                        .AggregatorTabId = AggregatorTabId
                    End With

                    Dim AggregatorModuleId As Integer = UpdateAggregatorModule(ami, ModuleID, "")
                Next

                ' properties
                Dim xmlTabSettings As XmlNodeList = xmlTab.SelectSingleNode("Settings").ChildNodes
                For Each xmlTabSetting As XmlNode In xmlTabSettings
                    UpdateProperties(ModuleID, AggregatorTabId, ms.TabSkin, ms.TabTemplate, xmlTabSetting.Name, xmlTabSetting.InnerText)
                Next

            Next
        End Sub
#End Region

    End Class
#End Region

#Region "Custom Settings"
    Public Class CustomSettingsInfo

        ' local property declarations
        Private _AggregatorTabId As Integer = -1
        Private _ModuleId As Integer = -1
        Private _Name As String = ""
        Private _Value As String = ""
        Private _Skin As String = ""
        Private _Template As String = ""

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property AggregatorTabId() As Integer
            Get
                Return _AggregatorTabId
            End Get
            Set(ByVal Value As Integer)
                _AggregatorTabId = Value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property Skin() As String
            Get
                Return _Skin
            End Get
            Set(ByVal Skin As String)
                _Skin = Skin
            End Set
        End Property

        Public Property Template() As String
            Get
                Return _Template
            End Get
            Set(ByVal Theme As String)
                _Template = Template
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return _Value
            End Get
            Set(ByVal Value As String)
                _Value = Value
            End Set
        End Property
    End Class
#End Region

#Region " Script"

    Public Class ScriptInfo

#Region " Private fields "

        Private _portalId As Integer
        Private _scriptName As String
        Private _dontLoadScript As Boolean
        Private _internalScriptPath As String
        Private _loadHosted As Boolean
        Private _hostedScriptPath As String

#End Region

#Region " Constructors "

        Public Sub New()
            ' default object initialization
        End Sub

        Public Sub New(ByVal portalId As Integer, ByVal scriptName As String, ByVal dontLoadScript As Boolean, ByVal internalScriptPath As String, ByVal loadHosted As Boolean, ByVal hostedScriptPath As String)
            _portalId = portalId
            _scriptName = scriptName
            _dontLoadScript = dontLoadScript
            _internalScriptPath = internalScriptPath
            _loadHosted = loadHosted
            _hostedScriptPath = hostedScriptPath
        End Sub

#End Region

#Region " Properties "

        Public Property PortalId() As Integer
            Get
                Return _portalId
            End Get
            Set(ByVal Value As Integer)
                _portalId = Value
            End Set
        End Property

        Public Property ScriptName() As String
            Get
                Return _scriptName
            End Get
            Set(ByVal Value As String)
                _scriptName = Value
            End Set
        End Property

        Public Property DontLoadScript() As Boolean
            Get
                Return _dontLoadScript
            End Get
            Set(ByVal Value As Boolean)
                _dontLoadScript = Value
            End Set
        End Property

        Public Property InternalScriptPath() As String
            Get
                Return _internalScriptPath
            End Get
            Set(ByVal Value As String)
                _internalScriptPath = Value
            End Set
        End Property

        Public Property LoadHosted() As Boolean
            Get
                Return _loadHosted
            End Get
            Set(ByVal Value As Boolean)
                _loadHosted = Value
            End Set
        End Property

        Public Property HostedScriptPath() As String
            Get
                Return _hostedScriptPath
            End Get
            Set(ByVal Value As String)
                _hostedScriptPath = Value
            End Set
        End Property

#End Region

    End Class


#End Region
End Namespace
