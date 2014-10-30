'***************************************************************************/
'* ModuleSettings.vb
'*
'* Copyright (c) 2007 by DNNStuff.
'* All rights reserved.
'*
'* Date:        March 19,2007
'* Author:      Richard Edwards
'* Description: Class to organize module settings
'*************/
Imports DotNetNuke

Namespace DNNStuff.Aggregator

    Public Class ModuleSettings
        Private _ModuleId As Integer = 0

#Region " Properties"
        Private Const SETTING_HIDETITLES As String = "HideTitles"
        Private _HideTitles As Boolean = True
        Public Property HideTitles() As Boolean
            Get
                Return _HideTitles
            End Get
            Set(ByVal value As Boolean)
                _HideTitles = value
            End Set
        End Property

        Private Const SETTING_TABSKIN As String = "TabSkin"
        Private _TabSkin As String = ""
        Public Property TabSkin() As String
            Get
                Return _TabSkin
            End Get
            Set(ByVal value As String)
                _TabSkin = value
            End Set
        End Property

        Private Const SETTING_TABTEMPLATE As String = "TabTemplate"
        Private _TabTemplate As String = ""
        Public Property TabTemplate() As String
            Get
                Return _TabTemplate
            End Get
            Set(ByVal value As String)
                _TabTemplate = value
            End Set
        End Property

        Private Const SETTING_SHOWPREVNEXT As String = "ShowPrevNext"
        Private _ShowPrevNext As Boolean = False
        Public Property ShowPrevNext() As Boolean
            Get
                Return _ShowPrevNext
            End Get
            Set(ByVal value As Boolean)
                _ShowPrevNext = value
            End Set
        End Property

        Private Const SETTING_HIDESINGLETAB As String = "HideSingleTab"
        Private _HideSingleTab As Boolean = False
        Public Property HideSingleTab() As Boolean
            Get
                Return _HideSingleTab
            End Get
            Set(ByVal value As Boolean)
                _HideSingleTab = value
            End Set
        End Property

        Private Const SETTING_ACTIVEHOVER As String = "ActiveHover"
        Private _ActiveHover As Boolean = False
        Public Property ActiveHover() As Boolean
            Get
                Return _ActiveHover
            End Get
            Set(ByVal value As Boolean)
                _ActiveHover = value
            End Set
        End Property

        Private Const SETTING_ACTIVEHOVERDELAY As String = "ActiveHoverDelay"
        Private _ActiveHoverDelay As Integer = 0
        Public Property ActiveHoverDelay() As Integer
            Get
                Return _ActiveHoverDelay
            End Get
            Set(ByVal value As Integer)
                _ActiveHoverDelay = value
            End Set
        End Property

        Private Const SETTING_DEFAULTTAB As String = "DefaultTab"
        Private _DefaultTab As Integer = 1
        Public Property DefaultTab() As Integer
            Get
                Return _DefaultTab
            End Get
            Set(ByVal value As Integer)
                _DefaultTab = value
            End Set
        End Property

        Private Const SETTING_WIDTH As String = "Width"
        Private _Width As String = ""
        Public Property Width() As String
            Get
                Return _Width
            End Get
            Set(ByVal value As String)
                _Width = value
            End Set
        End Property

        Private Const SETTING_HEIGHT As String = "Height"
        Private _Height As String = ""
        Public Property Height() As String
            Get
                Return _Height
            End Get
            Set(ByVal value As String)
                _Height = value
            End Set
        End Property

        Private Const SETTING_HIDETABS As String = "HideTabs"
        Private _HideTabs As Boolean = False
        Public Property HideTabs() As Boolean
            Get
                Return _HideTabs
            End Get
            Set(ByVal value As Boolean)
                _HideTabs = value
            End Set
        End Property

        Private Const SETTING_REMEMBERLASTOPENTAB As String = "RememberLastOpenTab"
        Private _RememberLastOpenTab As Boolean = True
        Public Property RememberLastOpenTab() As Boolean
            Get
                Return _RememberLastOpenTab
            End Get
            Set(ByVal value As Boolean)
                _RememberLastOpenTab = value
            End Set
        End Property

        Private Const SETTING_RSSURL As String = "RSSUrl"
        Private _rssUrl As String = ""
        Public Property RSSUrl() As String
            Get
                Return _rssUrl
            End Get
            Set(ByVal value As String)
                _rssUrl = value
            End Set
        End Property

        Private Const SETTING_RSSMAXITEMS As String = "RSSMaxItems"
        Private _rssMaxItems As Integer = 0
        Public Property RSSMaxItems() As Integer
            Get
                Return _rssMaxItems
            End Get
            Set(ByVal value As Integer)
                _rssMaxItems = value
            End Set
        End Property

        Private Const SETTING_RSSCACHETIME As String = "RSSCacheTime"
        Private _rssCacheTime As Integer = 0
        Public Property RSSCacheTime() As Integer
            Get
                Return _rssCacheTime
            End Get
            Set(ByVal value As Integer)
                _rssCacheTime = value
            End Set
        End Property

        Private Const SETTING_RSSTABCAPTION As String = "RSSTabCaption"
        Private _rssTabCaption As String = ""
        Public Property RSSTabCaption() As String
            Get
                Return _RSSTabCaption
            End Get
            Set(ByVal value As String)
                _RSSTabCaption = value
            End Set
        End Property

        Private Const SETTING_RSSUSERNAME As String = "RSSUsername"
        Private _rssUsername As String = ""
        Public Property RSSUsername() As String
            Get
                Return _RSSUsername
            End Get
            Set(ByVal value As String)
                _RSSUsername = value
            End Set
        End Property

        Private Const SETTING_RSSPASSWORD As String = "RSSPassword"
        Private _rssPassword As String = ""
        Public Property RSSPassword() As String
            Get
                Dim value As String = _rssPassword
                If _rssPassword <> "" Then
                    Dim encKey As String = DNNUtilities.GetEncryptionKey
                    If Not String.IsNullOrEmpty(encKey) Then
                        Dim ps As New DotNetNuke.Security.PortalSecurity
                        value = ps.Decrypt(encKey, value)
                    End If
                End If
                Return value
            End Get
            Set(ByVal value As String)
                If Not String.IsNullOrEmpty(value) Then
                    Dim encKey As String = DNNUtilities.GetEncryptionKey
                    If Not String.IsNullOrEmpty(encKey) Then
                        Dim ps As New DotNetNuke.Security.PortalSecurity
                        value = ps.Encrypt(encKey, value)
                    End If
                End If
                _rssPassword = value
            End Set
        End Property

#End Region

#Region " Derived Properties"
        Public ReadOnly Property TabTheme() As String
            Get
                Return TabSkin & "_" & TabTemplate
            End Get
        End Property

        Public ReadOnly Property TabUnselectedCssClass() As String
            Get
                Return TabTheme & "_TabUnselected"
            End Get
        End Property
        Public ReadOnly Property TabSelectedCssClass() As String
            Get
                Return TabTheme & "_TabSelected"
            End Get
        End Property
        Public ReadOnly Property TabPageCssClass() As String
            Get
                Return TabTheme & "_TabPage"
            End Get
        End Property
        Public ReadOnly Property TabMouseOverCssClass() As String
            Get
                Return TabTheme & "_TabMouseOver"
            End Get
        End Property
        Public ReadOnly Property TabStripCssClass() As String
            Get
                Return TabTheme & "_TabStrip"
            End Get
        End Property
        Public ReadOnly Property PrevNextCssClass() As String
            Get
                Return TabTheme & "_PrevNext"
            End Get
        End Property


#End Region

#Region "Methods"
        Public Sub New(ByVal moduleId As Integer)
            _ModuleId = moduleId

            LoadSettings()
        End Sub

        Private Sub LoadSettings()
            Dim ctrl As New DotNetNuke.Entities.Modules.ModuleController
            Dim settings As Hashtable = ctrl.GetModuleSettings(_ModuleId)

            _HideTitles = Convert.ToBoolean(DNNUtilities.GetSetting(settings, SETTING_HIDETITLES, "True"))
            _TabSkin = DNNUtilities.GetSetting(settings, SETTING_TABSKIN, "Default")
            _TabTemplate = DNNUtilities.GetSetting(settings, SETTING_TABTEMPLATE, "Top")
            _ShowPrevNext = Convert.ToBoolean(DNNUtilities.GetSetting(settings, SETTING_SHOWPREVNEXT, "False"))
            _HideSingleTab = Convert.ToBoolean(DNNUtilities.GetSetting(settings, SETTING_HIDESINGLETAB, "False"))
            _ActiveHover = Convert.ToBoolean(DNNUtilities.GetSetting(settings, SETTING_ACTIVEHOVER, "False"))
            _ActiveHoverDelay = Convert.ToInt32(DNNUtilities.GetSetting(settings, SETTING_ACTIVEHOVERDELAY, "0"))
            _HideTabs = Convert.ToBoolean(DNNUtilities.GetSetting(settings, SETTING_HIDETABS, "False"))
            _DefaultTab = Convert.ToInt32(DNNUtilities.GetSetting(settings, SETTING_DEFAULTTAB, "1"))
            _RememberLastOpenTab = Convert.ToBoolean(DNNUtilities.GetSetting(settings, SETTING_REMEMBERLASTOPENTAB, "True"))
            _Height = DNNUtilities.GetSetting(settings, SETTING_HEIGHT, "")
            _Width = DNNUtilities.GetSetting(settings, SETTING_WIDTH, "")

            ' rss
            _rssUrl = DNNUtilities.GetSetting(settings, SETTING_RSSURL, "")
            _rssMaxItems = Convert.ToInt32(DNNUtilities.GetSetting(settings, SETTING_RSSMAXITEMS, "0"))
            _rssCacheTime = Convert.ToInt32(DNNUtilities.GetSetting(settings, SETTING_RSSCACHETIME, "0"))
            _rssTabCaption = DNNUtilities.GetSetting(settings, SETTING_RSSTABCAPTION, "[RSSTITLE]")
            _RSSUsername = DNNUtilities.GetSetting(settings, SETTING_RSSUSERNAME, "")
            _RSSPassword = DNNUtilities.GetSetting(settings, SETTING_RSSPASSWORD, "")

        End Sub

        Public Sub UpdateSettings()
            Dim ctrl As New DotNetNuke.Entities.Modules.ModuleController
            With ctrl
                .UpdateModuleSetting(_ModuleId, SETTING_HIDETITLES, HideTitles.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_TABSKIN, TabSkin.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_TABTEMPLATE, TabTemplate.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_SHOWPREVNEXT, ShowPrevNext.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_HIDESINGLETAB, HideSingleTab.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_ACTIVEHOVER, ActiveHover.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_ACTIVEHOVERDELAY, ActiveHoverDelay.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_HIDETABS, HideTabs.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_DEFAULTTAB, DefaultTab.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_REMEMBERLASTOPENTAB, RememberLastOpenTab.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_HEIGHT, Height.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_WIDTH, Width.ToString)

                ' RSS
                .UpdateModuleSetting(_ModuleId, SETTING_RSSURL, RSSUrl)
                .UpdateModuleSetting(_ModuleId, SETTING_RSSMAXITEMS, RSSMaxItems.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_RSSCACHETIME, RSSCacheTime.ToString)
                .UpdateModuleSetting(_ModuleId, SETTING_RSSTABCAPTION, RSSTabCaption)
                .UpdateModuleSetting(_ModuleId, SETTING_RSSUSERNAME, RSSUsername)
                .UpdateModuleSetting(_ModuleId, SETTING_RSSPASSWORD, RSSPassword)

            End With

        End Sub

#End Region

    End Class
End Namespace
