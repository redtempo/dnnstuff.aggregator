Namespace DNNStuff.Aggregator

#If DNNVERSION = "DNN4" Then


    Module Compatibility
        ' this module will provide compatibility between DNN versions

        Public Sub SetProxy(ByVal wr As Net.HttpWebRequest)
            ' set proxy server
            If Convert.ToString(DotNetNuke.Common.HostSettings("ProxyServer")) <> "" Then
                wr.Proxy = New Net.WebProxy(Convert.ToString(DotNetNuke.Common.HostSettings("ProxyServer")), Convert.ToInt32(Convert.ToString(DotNetNuke.Common.HostSettings("ProxyPort"))))
                ' set the credentials for an authenticated proxy
                If Not Convert.ToString(DotNetNuke.Common.HostSettings("ProxyUsername")).Equals("") Then
                    wr.Proxy.Credentials = New Net.NetworkCredential(Convert.ToString(DotNetNuke.Common.HostSettings("ProxyUsername")), Convert.ToString(DotNetNuke.Common.HostSettings("ProxyPassword")))
                End If
            End If
        End Sub

        Public Function IncludeModule(ByVal mi As DotNetNuke.Entities.Modules.ModuleInfo, ByVal ps As DotNetNuke.Entities.Portals.PortalSettings) As Boolean
            ' should module be included inside tab?
            If DotNetNuke.Security.PortalSecurity.IsInRoles(mi.AuthorizedViewRoles) = True Then

                ' if current date is within module display schedule or user is admin
                If (mi.StartDate < Now And mi.EndDate > Now) Or DotNetNuke.Common.Globals.IsLayoutMode Then

                    ' modules which are displayed on all tabs should not be displayed on the Admin or Super tabs
                    If mi.AllTabs = False Or ps.ActiveTab.IsAdminTab = False Then
                        Return True
                    End If
                End If
            End If

            Return False
        End Function

        Public Function GetModuleInfo(ByVal InnerModuleId As Integer, ByVal InnerTabId As Integer, ByVal ps As DotNetNuke.Entities.Portals.PortalSettings) As DotNetNuke.Entities.Modules.ModuleInfo
            ' returns a full ModuleInfo object for a module and tab
            Dim moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo
            Dim moduleControl As DotNetNuke.Entities.Modules.ModuleControlInfo
            Dim moduleController As New DotNetNuke.Entities.Modules.ModuleController
            Dim moduleControlController As New DotNetNuke.Entities.Modules.ModuleControlController
            Dim skinController As New DotNetNuke.UI.Skins.SkinController
            Dim moduleControls As ArrayList

            moduleInfo = moduleController.GetModule(InnerModuleId, InnerTabId)

            ' read in the definition information because GetModule doesn't return this for some reason
            moduleControls = DotNetNuke.Entities.Modules.ModuleControlController.GetModuleControlsByKey("", moduleInfo.ModuleDefID)

            If moduleControls.Count > 0 Then
                moduleControl = DirectCast(moduleControls(0), DotNetNuke.Entities.Modules.ModuleControlInfo)
                ' update the moduleInfo object with missing ModuleDef info
                With moduleInfo
                    .ModuleControlId = moduleControl.ModuleControlID
                    .ControlSrc = moduleControl.ControlSrc
                    .ControlType = moduleControl.ControlType
                    .ControlTitle = moduleControl.ControlTitle
                    .HelpUrl = moduleControl.HelpURL
                    .SupportsPartialRendering = moduleControl.SupportsPartialRendering
                End With
            Else
                Return Nothing
            End If

            If DotNetNuke.Common.Utilities.Null.IsNull(moduleInfo.StartDate) Then
                moduleInfo.StartDate = Date.MinValue
            End If
            If DotNetNuke.Common.Utilities.Null.IsNull(moduleInfo.EndDate) Then
                moduleInfo.EndDate = Date.MaxValue
            End If
            ' container
            If moduleInfo.ContainerSrc = "" Then
                moduleInfo.ContainerSrc = ps.ActiveTab.ContainerSrc
            End If
            moduleInfo.ContainerSrc = DotNetNuke.UI.Skins.SkinController.FormatSkinSrc(moduleInfo.ContainerSrc, ps)
            moduleInfo.ContainerPath = DotNetNuke.UI.Skins.SkinController.FormatSkinPath(moduleInfo.ContainerSrc)

            Return moduleInfo
        End Function

        Public Sub InjectIntoTabs(ByVal agg As Aggregator, ByVal LoadEvent As LoadEventType, ByVal ai As AggregatorInfo, ByVal currentTabNumber As Integer)
            If ai.Tabs.Count = 0 Then Exit Sub ' fixes problem when injecting controls directly in page, i.e. demo mode

            Try

                ' inject our list of modules
                Dim baseSkin As DotNetNuke.UI.Skins.Skin = DotNetNuke.UI.Skins.Skin.GetParentSkin(agg)
                Dim paneCtrl As Web.UI.Control

                If baseSkin Is Nothing Then
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(agg, "Error: Could not find ParentSkin", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Else
                    Dim tabNumber As Integer = 1
                    For Each ati As AggregatorTabInfo In New ArrayList(ai.Tabs)
                        If Not (tabNumber <> currentTabNumber And ati.Postback) Then ' don't inject modules if tab is postback and it's not current tab
                            For Each ami As AggregatorModuleInfo In New ArrayList(ati.Modules)
                                If (ami.LoadEvent = LoadEventType.Default And ami.ModuleInfo.SupportsPartialRendering = True And LoadEvent = LoadEventType.PageInit) _
                                Or (ami.LoadEvent = LoadEventType.Default And ami.ModuleInfo.SupportsPartialRendering = False And LoadEvent = LoadEventType.PageLoad) _
                                Or (ami.LoadEvent = LoadEvent) Then
                                    ami.ModuleInfo.IsDeleted = False
                                    paneCtrl = agg.FindControl(ami.ModuleInfo.PaneName)
                                    If Not paneCtrl Is Nothing Then
#If DEBUG Then
                                        ami.ModuleInfo.Header = "Loaded in " & [Enum].GetName(GetType(LoadEventType), LoadEvent)
#End If
                                        baseSkin.InjectModule(paneCtrl, ami.ModuleInfo, agg.PortalSettings)
                                    End If
                                    ami.ModuleInfo.IsDeleted = True
                                End If
                            Next
                        End If
                        tabNumber += 1
                    Next
                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.ProcessModuleLoadException(agg, ex)
            End Try

        End Sub

        Public Function ReplaceGenericTokens(ByVal agg As DNNStuff.Aggregator.Aggregator, ByVal text As String) As String
            Dim ret As String

            Dim objTokenReplace As New DotNetNuke.Services.Tokens.TokenReplace()
            objTokenReplace.ModuleId = agg.ModuleId
            ret = objTokenReplace.ReplaceEnvironmentTokens(text)

            objTokenReplace.User = agg.UserInfo
            If agg.UserInfo.Profile.PreferredLocale IsNot Nothing Then ' will be nothing for anonymous users
                objTokenReplace.Language = agg.UserInfo.Profile.PreferredLocale
            End If
            ret = objTokenReplace.ReplaceEnvironmentTokens(ret)

            ' for backward compatibility to old replace function
            ret = agg.MakeReplacements_Backward(ret)
            Return ret
        End Function

#Region "Module Compatibility"
        Public Function ModuleIsPortable(ByVal m As DotNetNuke.Entities.Modules.ModuleInfo) As Boolean
            Return m.IsPortable
        End Function
        Public Function ModuleBusinessControllerClass(ByVal m As DotNetNuke.Entities.Modules.ModuleInfo) As String
            Return m.BusinessControllerClass
        End Function
        Public Function ModuleVersion(ByVal m As DotNetNuke.Entities.Modules.ModuleInfo) As String
            Return m.Version
        End Function

#End Region
    End Module

#End If
End Namespace
