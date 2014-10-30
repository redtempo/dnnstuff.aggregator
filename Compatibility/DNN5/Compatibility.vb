Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Host

Namespace DNNStuff.Aggregator
#If DNNVERSION = "DNN5" Then

    Module Compatibility
        Public Function IsAllowedExtension(ByVal Extension As String) As Boolean
            Return InStr(1, ",ASCX,HTM,HTML,CSS,SWF,RESX," & Host.FileExtensions, "," & Extension.ToUpper) <> 0
        End Function

        Public Function AllowedExtensions() As String
            Return Replace(Host.FileExtensions, ",", ", *.")
        End Function

        Public Sub SetProxy(ByVal wr As Net.HttpWebRequest)
            ' set proxy server
            If Host.ProxyServer <> "" Then
                wr.Proxy = New Net.WebProxy(Host.ProxyServer, Host.ProxyPort)
                ' set the credentials for an authenticated proxy
                If Not Host.ProxyUsername.Equals("") Then
                    wr.Proxy.Credentials = New Net.NetworkCredential(Host.ProxyUsername, Host.ProxyPassword)
                End If
            End If
        End Sub

        Public Function IncludeModule(ByVal mi As DotNetNuke.Entities.Modules.ModuleInfo, ByVal ps As DotNetNuke.Entities.Portals.PortalSettings) As Boolean
            ' should module be included inside tab?

            If ModulePermissionController.CanViewModule(mi) Then
                ' if current date is within module display schedule or user is admin
                If (mi.StartDate < Now And mi.EndDate > Now) Or DotNetNuke.Common.Globals.IsLayoutMode Then
                    Return True
                End If
            End If

            Return False
        End Function

        Public Function GetModuleInfo(ByVal InnerModuleId As Integer, ByVal InnerTabId As Integer, ByVal ps As DotNetNuke.Entities.Portals.PortalSettings) As DotNetNuke.Entities.Modules.ModuleInfo
            ' returns a full ModuleInfo object for a module and tab
            Dim moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo
            Dim moduleControl As DotNetNuke.Entities.Modules.ModuleControlInfo
            Dim moduleController As New DotNetNuke.Entities.Modules.ModuleController
            Dim skinController As New DotNetNuke.UI.Skins.SkinController

            moduleInfo = moduleController.GetModule(InnerModuleId, InnerTabId)

            ' read in the definition information because GetModule doesn't return this for some reason
            moduleControl = DotNetNuke.Entities.Modules.ModuleControlController.GetModuleControlByControlKey("", moduleInfo.ModuleDefID)

            If moduleControl IsNot Nothing Then
                ' update the moduleInfo object with missing ModuleDef info
                With moduleInfo
                    .ModuleControlId = moduleControl.ModuleControlID
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
                Dim paneCtrl As DotNetNuke.UI.Skins.Pane

                If baseSkin Is Nothing Then
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(agg, "Error: Could not find ParentSkin", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Else
                    Dim tabNumber As Integer = 1
                    For Each ati As DNNStuff.Aggregator.AggregatorTabInfo In New ArrayList(ai.Tabs)
                        If Not (tabNumber <> currentTabNumber And ati.Postback) Then ' don't inject modules if tab is postback and it's not current tab
                            For Each ami As DNNStuff.Aggregator.AggregatorModuleInfo In New ArrayList(ati.Modules)
                                If (ami.LoadEvent = LoadEventType.Default And ami.ModuleInfo.ModuleControl.SupportsPartialRendering = True And LoadEvent = LoadEventType.PageInit) _
                                Or (ami.LoadEvent = LoadEventType.Default And ami.ModuleInfo.ModuleControl.SupportsPartialRendering = False And LoadEvent = LoadEventType.PageLoad) _
                                Or (ami.LoadEvent = LoadEvent) Then
                                    ami.ModuleInfo.IsDeleted = False
                                    Dim container As Control = agg.FindControl(ami.ModuleInfo.PaneName)
                                    If container IsNot Nothing Then
                                        paneCtrl = New DotNetNuke.UI.Skins.Pane(CType(container, HtmlContainerControl))
                                        If Not paneCtrl Is Nothing Then
                                            baseSkin.InjectModule(paneCtrl, ami.ModuleInfo)
                                        End If
                                        ami.ModuleInfo.IsDeleted = True
                                    Else
                                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(agg, String.Format("Error: Could not find Pane '{0}'", ami.ModuleInfo.PaneName), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                                    End If
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
            Return m.DesktopModule.IsPortable
        End Function
        Public Function ModuleBusinessControllerClass(ByVal m As DotNetNuke.Entities.Modules.ModuleInfo) As String
            Return m.DesktopModule.BusinessControllerClass
        End Function
        Public Function ModuleVersion(ByVal m As DotNetNuke.Entities.Modules.ModuleInfo) As String
            Return m.DesktopModule.Version
        End Function

#End Region

    End Module
#End If
End Namespace
